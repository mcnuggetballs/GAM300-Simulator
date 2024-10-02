using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public enum ShakeSpace
    {
        Screen,
        World
    }

    [System.Serializable]
    public class ShakeSettings
    {
        public float duration = 1.0f;
        public ShakeSpace shakeSpace = ShakeSpace.Screen;
        public Vector3 shakeStrength = new Vector3(0.1f, 0.1f, 0.1f);
        public AnimationCurve shakeCurve = AnimationCurve.Linear(0, 1, 1, 0);
        [Range(0, 0.1f)] public float shakesDelay = 0;
    }

    private bool isShaking = false;
    private Vector3 originalPosition;
    private Vector3 shakeVector;
    private float elapsedTime;
    private Transform cameraParent; // Reference to the parent of the camera
    private ShakeSettings settings;

    public void StartShake(ShakeSettings shakeSettings, Transform cameraParentTransform)
    {
        if (isShaking)
        {
            StopShake();
        }

        cameraParent = cameraParentTransform;
        settings = shakeSettings;
        isShaking = true;
        elapsedTime = 0f;
        originalPosition = cameraParent.localPosition;

        StartCoroutine(ShakeRoutine());
    }

    private IEnumerator ShakeRoutine()
    {
        while (elapsedTime < settings.duration)
        {
            if (Time.timeScale == 0)
            {
                yield break;
            }

            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / settings.duration);

            Vector3 randomVec = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            shakeVector = Vector3.Scale(randomVec, settings.shakeStrength) * settings.shakeCurve.Evaluate(progress);

            if (settings.shakeSpace == ShakeSpace.Screen)
            {
                cameraParent.localPosition = originalPosition + cameraParent.transform.rotation * shakeVector;
            }
            else
            {
                cameraParent.localPosition = originalPosition + shakeVector;
            }

            yield return null;
        }

        StopShake();
    }

    public void StopShake()
    {
        isShaking = false;
        if (cameraParent != null)
        {
            cameraParent.localPosition = originalPosition;
        }
    }
}
