using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Events;
using System.Text.RegularExpressions;

public class MenuController : MonoBehaviour {
    public static MenuController instance = null;

    private int port = 8800;

    //Intro menu UI
    public GameObject startMenuUI;
    public InputField usernameInput;
    public InputField ipInput;
    public GameObject connectCamera;
    public Button startButton;
    public GameObject connectingText;

    //Game UI
    public GameObject gameMenuUI;
    public GameObject damage;
    public Text health;
    public Text block;

    //Log UI
    public Text log;

    private void Awake() {
        if (instance != null && instance != this){
            Destroy(this.gameObject);
            return;
        }

        instance = this;
    }

    public void connectToServer() {
        if (Application.isEditor) ipInput.text = "127.0.0.1";

        if (!Regex.Match(ipInput.text, @"^\d{3}\.\d{1,3}\.\d{1,3}\.\d{1,3}").Success) {
            setLog("Ip doesn't match the format!");
            setInteractableStart(true);
            ipInput.text = "";
            return;
        }

        setInteractableStart(false);

        Client.instance.connectToServer(ipInput.text, port, usernameInput.text);
    }

    public void removeMenu() {
        hideStartMenuUI();
        showGameUI();
    }

    public void showGameUI() {
        gameMenuUI.SetActive(true);
    }

    public void hideStartMenuUI() {
        startMenuUI.SetActive(false);
        ipInput.interactable = false;
        usernameInput.interactable = false;
        Destroy(connectCamera);
    }

    public void setInteractableStart(bool interactable){
        if (interactable){
            ThreadManager.ExecuteOnMainThread(() => {
                Invoke("enableInteractable", 1);
            });
        } else {
            startButton.interactable = false;
            connectingText.SetActive(true);
        } 
    }

    public void enableInteractable() {
        startButton.interactable = true;
        connectingText.SetActive(false);
    }

    public void setLog(string text) {
        setLog(text, 1);
    }

    public void setLog(string text, int duration) {
        ThreadManager.ExecuteOnMainThread(() => {
            log.text = text;
            Invoke("clearLog", duration);
        });
    }

    public void setHealth(float health) {
        this.health.text = "Health " + health;
    }

    public void setBlock(float block) {
        this.block.text = "Block " + block;
    }

    public void clearLog() {
        log.text = "";
    }

    public void quitGame(){
        Application.Quit();
    }

    private void OnApplicationQuit() {
        Client.instance.disconnect();
    }

    public void enableDamage() {
        damage.SetActive(true);
        Invoke("disableDamage", 0.3f);
    }

    private void disableDamage() {
        damage.SetActive(false);
    }
}
