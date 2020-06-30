using UnityEngine.UI;
using UnityEngine;

public class MenuController : MonoBehaviour {
    public static MenuController instance = null;

    private string ip = "127.0.0.1";
    private int port = 8080;
    public GameObject startMenu;
    public InputField usernameInput;
    public GameObject connectCamera;
    public GameObject crosshair;

    private void Awake() {
        if (instance != null && instance != this){
            Destroy(this.gameObject);
            return;
        }

        instance = this;
    }

    public void connectToServer() {
        Debug.Log("Connecting......");

        Client.instance.setMyIp(ip);
        Client.instance.setMyPort(port);
        Client.instance.setMyUsername(usernameInput.text);

        Client.instance.connectToServer();
    }

    public void removeMenu() {
        startMenu.SetActive(false);
        usernameInput.interactable = false;
        crosshair.SetActive(true);
        Destroy(connectCamera);
    }

    private void OnApplicationQuit() {
        Client.instance.disconnect();
    }
}
