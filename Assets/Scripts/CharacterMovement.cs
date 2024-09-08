using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public float speed = 6f;

    public Transform cameraTransform; // Assign this to the main Cinemachine Camera

    void Update()
    {
        // Get input for movement
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            // Get the camera's forward direction
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;

            // Smoothly rotate the character to face the same direction as the camera
            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);

            // Convert the movement direction to be relative to the camera's direction
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            // Move the character
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }
    }
}
