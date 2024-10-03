using UnityEngine;

public class CameraShakeManager : MonoBehaviour
{
    private static CameraShakeManager _instance;
    private CameraShake cameraShake;

    public static CameraShakeManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("CameraShakeManager");
                _instance = obj.AddComponent<CameraShakeManager>();
                DontDestroyOnLoad(obj); // Persist across scenes
            }
            return _instance;
        }
    }

    private void Awake()
    {
        cameraShake = gameObject.AddComponent<CameraShake>();
    }

    // Start camera shake on the camera's parent object
    public void StartShake(CameraShake.ShakeSettings settings, Camera camera)
    {
        if (cameraShake != null)
        {
            Transform cameraParent = camera.transform.parent; // Apply shake to the parent of the camera
            if (cameraParent != null)
            {
                cameraShake.StartShake(settings, cameraParent);
            }
            else
            {
                Debug.LogWarning("Camera does not have a parent object! Shake not applied.");
            }
        }
    }
}
