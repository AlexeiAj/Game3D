using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public Camera playerCam;
    public Keys keys { get; set; }

    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        keys = new Keys();
    }

    void FixedUpdate() {
        sendKeys();
    }

    private void sendKeys(){
        keys.updateKeys();
        Client.instance.sendPlayerKeys(keys);
    }
}
