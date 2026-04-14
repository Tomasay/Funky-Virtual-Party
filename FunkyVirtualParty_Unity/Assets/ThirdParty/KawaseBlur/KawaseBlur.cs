using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;

public class KawaseBlur : ScriptableRendererFeature
{
    [System.Serializable]
    public class KawaseBlurSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        public Material blurMaterial = null;

        [Range(2,15)]
        public int blurPasses = 1;

        [Range(1,4)]
        public int downsample = 1;
        public bool copyToFramebuffer;
        public string targetName = "_blurTexture";
    }

    public KawaseBlurSettings settings = new KawaseBlurSettings();

    class CustomRenderPass : ScriptableRenderPass
    {
        public Material blurMaterial;
        public int passes;
        public int downsample;
        public bool copyToFramebuffer;
        public string targetName;
        string profilerTag;

        public CustomRenderPass(string profilerTag)
        {
            this.profilerTag = profilerTag;
        }

        private class PassData
        {
            public Material material;
            public float offset;
            public TextureHandle source;
        }

        private void AddBlurPass(RenderGraph renderGraph, TextureHandle src, TextureHandle dst, float offset, string passName)
        {
            using var builder = renderGraph.AddRasterRenderPass<PassData>(passName, out var data);
            data.material = blurMaterial;
            data.offset = offset;
            data.source = src;
            builder.UseTexture(src, AccessFlags.Read);
            builder.SetRenderAttachment(dst, 0, AccessFlags.Write);
            builder.AllowGlobalStateModification(true);
            builder.SetRenderFunc((PassData d, RasterGraphContext ctx) =>
            {
                ctx.cmd.SetGlobalFloat("_offset", d.offset);
                Blitter.BlitTexture(ctx.cmd, d.source, new Vector4(1, 1, 0, 0), d.material, 0);
            });
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            var resourceData = frameData.Get<UniversalResourceData>();
            var cameraData = frameData.Get<UniversalCameraData>();

            RenderTextureDescriptor desc = cameraData.cameraTargetDescriptor;
            desc.depthBufferBits = 0;
            desc.width /= downsample;
            desc.height /= downsample;

            TextureHandle cameraColor = resourceData.activeColorTexture;
            TextureHandle tmpRT1 = UniversalRenderer.CreateRenderGraphTexture(renderGraph, desc, "tmpBlurRT1", false, FilterMode.Bilinear);
            TextureHandle tmpRT2 = UniversalRenderer.CreateRenderGraphTexture(renderGraph, desc, "tmpBlurRT2", false, FilterMode.Bilinear);

            // First pass: camera color -> tmpRT1
            AddBlurPass(renderGraph, cameraColor, tmpRT1, 1.5f, profilerTag + " Pass 0");

            TextureHandle currentSrc = tmpRT1;
            TextureHandle currentDst = tmpRT2;

            for (int i = 1; i < passes - 1; i++)
            {
                AddBlurPass(renderGraph, currentSrc, currentDst, 0.5f + i, profilerTag + $" Pass {i}");
                (currentSrc, currentDst) = (currentDst, currentSrc);
            }

            // Final pass
            float finalOffset = 0.5f + passes - 1f;
            if (copyToFramebuffer)
            {
                AddBlurPass(renderGraph, currentSrc, cameraColor, finalOffset, profilerTag + " Final");
            }
            else
            {
                using var builder = renderGraph.AddRasterRenderPass<PassData>(profilerTag + " Final", out var data);
                data.material = blurMaterial;
                data.offset = finalOffset;
                data.source = currentSrc;
                builder.UseTexture(currentSrc, AccessFlags.Read);
                builder.SetRenderAttachment(currentDst, 0, AccessFlags.Write);
                builder.SetGlobalTextureAfterPass(currentDst, Shader.PropertyToID(targetName));
                builder.AllowGlobalStateModification(true);
                builder.SetRenderFunc((PassData d, RasterGraphContext ctx) =>
                {
                    ctx.cmd.SetGlobalFloat("_offset", d.offset);
                    Blitter.BlitTexture(ctx.cmd, d.source, new Vector4(1, 1, 0, 0), d.material, 0);
                });
            }
        }

        public override void FrameCleanup(CommandBuffer cmd) { }
    }

    CustomRenderPass scriptablePass;

    public override void Create()
    {
        scriptablePass = new CustomRenderPass("KawaseBlur");
        scriptablePass.blurMaterial = settings.blurMaterial;
        scriptablePass.passes = settings.blurPasses;
        scriptablePass.downsample = settings.downsample;
        scriptablePass.copyToFramebuffer = settings.copyToFramebuffer;
        scriptablePass.targetName = settings.targetName;
        scriptablePass.renderPassEvent = settings.renderPassEvent;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(scriptablePass);
    }
}
