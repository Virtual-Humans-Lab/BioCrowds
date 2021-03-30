using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;                                                       //defines the mouse sensitivity

    public Transform playerBody;

    float xRotation = 0f;                                                                       //defines the default xrotation

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;                                               // locks the cursor
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;            //defines the input o the x axis
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;            //defines the input o the y axis

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -60f, 90f);                                          //locks the roatation so that you cant break the player's neck

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);

    }
}