using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public Camera playerCam;
    private float x;
    private float y;
    private float mouseX;
    private float mouseY;
    private bool jumping;
    private bool mouseLeft;
    private bool mouseRight;
    private bool shift;
    private bool e;

    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void FixedUpdate() {
        keys();
    }

    private void keys(){
        x = Input.GetAxisRaw("Horizontal");
        y = Input.GetAxisRaw("Vertical");
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");
        mouseLeft = Input.GetMouseButton(0);
        mouseRight = Input.GetMouseButton(1);
        jumping = Input.GetButton("Jump");
        shift = Input.GetKey(KeyCode.LeftShift);
        e = Input.GetKey(KeyCode.E);

        Client.instance.sendPlayerKeys(x, y, mouseX, mouseY, mouseLeft, mouseRight, jumping, shift, e);
    }
}
