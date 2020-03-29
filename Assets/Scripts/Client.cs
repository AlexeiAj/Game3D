using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
using UnityEngine.UI;

public class Client : MonoBehaviour {
    public static Client instance = null;
    private string ip = "127.0.0.1";
    private int port = 8080;
    private int id;
    private string username;

    private Tcp tcp;
    private Udp udp;

    Dictionary<int, GameObject> players = new Dictionary<int, GameObject>();
    public GameObject startMenu;
    public InputField usernameInput;
    public GameObject playerPrefab;
    public GameObject playerEnemyPrefab;
    public GameObject connectCamera;

    private void Awake() {
        if (instance != null && instance != this){
            Destroy(this.gameObject);
            return;
        }

        instance = this;
    }

    public void connectToServer() {
        Debug.Log("Connecting......");

        TcpClient socketTcp = new TcpClient();
        tcp = new Tcp(socketTcp, ip, port); 
        tcp.connect();
    }

    public void connectToUdp(int localEndPointPort) {
        UdpClient socketUdp = new UdpClient(localEndPointPort);
        udp = new Udp(socketUdp, ip, port);
        udp.connect();

        Packet packet = new Packet();
        packet.Write("newConnectionUDP");
        packet.Write(id);
        sendUdpData(packet);
    }

    private void sendTcpData(Packet packet) {
        packet.WriteLength();
        tcp.sendData(packet);
    }

    private void sendUdpData(Packet packet) {
        packet.WriteLength();
        udp.sendData(packet);
    }

    public void newConnection(Packet packet) {
        int id = packet.ReadInt();
        string username = packet.ReadString();
        Vector3 position = packet.ReadVector3();
        Quaternion rotation = packet.ReadQuaternion();

        ThreadManager.ExecuteOnMainThread(() => {
            Client.instance.instantiatePlayerEnemy(id, username, position, rotation);
        });
    }

    public void newConnectionUDP(Packet packet) {
        int id = packet.ReadInt();
        Debug.Log("Connected by tcp and udp!");
    }

    public void instantiatePlayer(int id, string myUsername, Vector3 position, Quaternion rotation) {
        startMenu.SetActive(false);
        usernameInput.interactable = false;
        Destroy(connectCamera);
        GameObject playerGO = Instantiate(playerPrefab, position, rotation);
        players.Add(id, playerGO);
        this.id = id;
    }

    public void instantiatePlayerEnemy(int id, string username, Vector3 position, Quaternion rotation) {
        GameObject playerGO = Instantiate(playerEnemyPrefab, position, rotation);
        players.Add(id, playerGO);
    }

    public void spawnPlayer(Packet packet) {
        int id = packet.ReadInt();
        Vector3 position = packet.ReadVector3();
        Quaternion rotation = packet.ReadQuaternion();

        this.id = id;
        username = usernameInput.text;

        ThreadManager.ExecuteOnMainThread(() => {
            Client.instance.instantiatePlayer(id, username, position, rotation);
        });

        Packet packetSend = new Packet();
        packetSend.Write("newConnection");
        packetSend.Write(id);
        packetSend.Write(username);
        sendTcpData(packetSend);

        connectToUdp(((IPEndPoint) tcp.getSocket().Client.LocalEndPoint).Port);
    }

    public void playerPosition(Packet packet) {
        int id = packet.ReadInt();
        Vector3 position = packet.ReadVector3();
        Quaternion rotation = packet.ReadQuaternion();
        Quaternion camRotation = packet.ReadQuaternion();

        ThreadManager.ExecuteOnMainThread(() => {
            Client.instance.playerPositionUpdate(id, position, rotation, camRotation);
        });
    }

    public void playerPositionUpdate(int id, Vector3 position, Quaternion rotation, Quaternion camRotation) {
        players[id].transform.position = position;
        players[id].transform.rotation = rotation;
        if(this.id == id) players[id].GetComponentInChildren<Camera>().transform.localRotation = camRotation;
    }

    public void sendPlayerKeys(Keys keys) {
        Packet packet = new Packet();
        packet.Write("playerKeys");
        packet.Write(id);
        packet.Write(keys);
        sendUdpData(packet);
    }
}
