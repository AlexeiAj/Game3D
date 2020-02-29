using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    public static UIManager instance = null;
    public GameObject startMenu;
    public InputField username;

    private void Awake() {
        if (instance != null && instance != this){
            Destroy(this.gameObject);
            return;
        }

        instance = this;
    }

    public void connectToServer() {
        startMenu.SetActive(false);
        username.interactable = false;
        Client.instance.connectToServer();
    }
}
