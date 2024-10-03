using UnityEngine;

public class SkyboxRotater : MonoBehaviour
{
    public float rotationSpeed = 1.0f;  // Speed of rotation

    void Update()
    {
        // Get the current skybox material
        Material skyboxMaterial = RenderSettings.skybox;

        // Check if skybox material is assigned
        if (skyboxMaterial != null)
        {
            // Get the current rotation value
            float rotationY = skyboxMaterial.GetFloat("_Rotation");

            // Increment the rotation value based on the speed and time
            rotationY += rotationSpeed * Time.deltaTime;

            // Set the new rotation value
            skyboxMaterial.SetFloat("_Rotation", rotationY);

            // Apply the changes to the skybox
            RenderSettings.skybox = skyboxMaterial;
        }
    }
}
