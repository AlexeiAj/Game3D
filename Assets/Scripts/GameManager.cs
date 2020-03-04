using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GameManager : MonoBehaviour {
    Dictionary<int, GameObject> players = new Dictionary<int, GameObject>();
    public static GameManager instance = null;
    public GameObject startMenu;
    public InputField username;
    public GameObject playerPrefab;
    public GameObject playerEnemyPrefab;
    public GameObject connectCamera;
    private int id;

    private static Stack actions = new Stack();

    private void Awake() {
        if (instance != null && instance != this){
            Destroy(this.gameObject);
            return;
        }

        instance = this;
    }

    private void Update() {
        if (actions.Count == 0) return;

        Action action = (Action) actions.Pop();
        action.Invoke();
    }

    public void connectToServer() {
        Client.instance.connectToServer();
    }

    public void instantiatePlayer(int id, string myUsername, Vector3 position, Quaternion rotation) {
        startMenu.SetActive(false);
        username.interactable = false;
        Destroy(connectCamera);
        GameObject playerGO = Instantiate(playerPrefab, position, rotation);
        players.Add(id, playerGO);
        this.id = id;
    }

    public void instantiatePlayerEnemy(int id, string username, Vector3 position, Quaternion rotation) {
        GameObject playerGO = Instantiate(playerEnemyPrefab, position, rotation);
        players.Add(id, playerGO);
    }

    public void playerPosition(int id, Vector3 position, Quaternion rotation) {
        players[id].transform.position = position;
        if(this.id != id) players[id].transform.rotation = rotation;
    }

    internal void addAction(Action action) {
        actions.Push(action);
    }

}
