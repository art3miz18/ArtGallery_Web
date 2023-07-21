using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float speed = 10.0f;
    public float rotationSpeed = 100.0f;

    void Update()
    {
        float translation = speed * Time.deltaTime;

        // Move the camera up or down, left or right based on input
        transform.Translate(new Vector3(Input.GetAxis("Horizontal") * translation, 0, Input.GetAxis("Vertical") * translation));

        // Rotate the camera based on mouse movement
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        float mouseY = -Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

        transform.Rotate(new Vector3(mouseY, mouseX, 0));

        // Ensure that we don't end up with strange camera rotations by 'zeroing out' the z-axis
        Vector3 currentRotation = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(currentRotation.x, currentRotation.y, 0);
    }
}

