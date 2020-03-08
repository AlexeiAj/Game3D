using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    private float mouseSensitivity = 150f;
    private float xRotation = 0f;
    public Camera playerCam;
    private float x;
    private float y;
    private bool jumping;

    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void FixedUpdate() {
        keys();
        look();
    }

    private void keys(){
        x = Input.GetAxisRaw("Horizontal");
        y = Input.GetAxisRaw("Vertical");
        jumping = Input.GetButton("Jump");

        // Client.instance.sendPlayerKeys(x, y, jumping, transform.rotation);
    }

    private void look(){
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCam.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        transform.Rotate(Vector3.up * mouseX);
    }
}
