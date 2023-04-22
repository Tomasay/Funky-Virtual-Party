using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LinesParticleEffect : MonoBehaviour
{
    [SerializeField]
    Camera particleCam, displayCam;

    public void OnParticleSystemStopped()
    {
        if (displayCam)
        {
            displayCam.GetUniversalAdditionalCameraData().cameraStack.Remove(particleCam);
        }
        else
        {
            Debug.Log("Missing reference to display camera");
        }

        Destroy(gameObject);
    }
}