using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Video;

public class video : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public UnityEngine.UI.RawImage image;

    [DllImport("__Internal")]
    private static extern void HTMLButtonPlugin();

    void Start(){
        playVideoWithoutSound();
        //HTMLButtonPlugin();
    }
    public void playVideoWithoutSound()
    {
        //Debug.Log("Play without Sound");
        videoPlayer.audioOutputMode = VideoAudioOutputMode.None;
        videoPlayer.EnableAudioTrack(0, false);
        videoPlayer.SetDirectAudioMute(0, true);
        videoPlayer.Play();
        //StartCoroutine(ChangeVideoSize());
    }


    public void playVideoFromJS()
    {
        videoPlayer.audioOutputMode = VideoAudioOutputMode.Direct;
        videoPlayer.EnableAudioTrack(0, true);
        videoPlayer.SetDirectAudioMute(0, false);
        videoPlayer.SetDirectAudioVolume(0, 1);
        //Debug.Log("Play from JS");
        videoPlayer.targetTexture = RenderTexture.GetTemporary(128, 128, 0);
        image.texture = videoPlayer.targetTexture;
        videoPlayer.Play();
        StartCoroutine(ChangeVideoSize());

    }

    IEnumerator ChangeVideoSize()
    {
        while(videoPlayer.width == 0){
            yield return null;
        }
        //Debug.Log("Play");
        videoPlayer.targetTexture.Release();
        videoPlayer.targetTexture = RenderTexture.GetTemporary((int)videoPlayer.width, (int)videoPlayer.height);
        image.texture = videoPlayer.targetTexture;
    }

}
