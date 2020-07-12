﻿using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Events;

public class MenuController : MonoBehaviour {
    public static MenuController instance = null;

    private int port = 8800;

    //Intro menu UI
    public GameObject startMenuUI;
    public InputField usernameInput;
    public InputField ipInput;
    public GameObject connectCamera;

    //Game UI
    public GameObject gameMenuUI;
    public Text health;
    public Text block;

    //Log UI
    public Text log;

    //Mobile UI
    public GameObject mobileInputUI;
    public Joystick joystickLeft;
    public Joystick joystickRight;
    private bool jump;
    private bool fire;

    private void Awake() {
        if (instance != null && instance != this){
            Destroy(this.gameObject);
            return;
        }

        instance = this;
    }

    public void connectToServer() {
        setLog("Connecting......");

        if (Application.isEditor) ipInput.text = "127.0.0.1";

        Client.instance.setMyIp(ipInput.text);
        Client.instance.setMyPort(port);
        Client.instance.setMyUsername(usernameInput.text);

        Client.instance.connectToServer();
    }

    public void removeMenu() {
        hideStartMenuUI();
        showGameUI();
        showMobileInputUI();
    }

    public void showMobileInputUI() {
        if (isAndroid()) mobileInputUI.SetActive(true);
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

    public bool isAndroid() {
        return Application.platform == RuntimePlatform.Android;
    }

    public Joystick getJoystickLeft() {
        return joystickLeft;
    }

    public Joystick getJoystickRight() {
        return joystickRight;
    }

    public void setJump(bool jump) {
        this.jump = jump;
    }

    public bool getJump() {
        return jump;
    }

    public void setFire(bool fire) {
        this.fire = fire;
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

    public bool getFire() {
        return fire;
    }

    public void quitGame(){
        Application.Quit();
    }

    private void OnApplicationQuit() {
        Client.instance.disconnect();
    }
}
