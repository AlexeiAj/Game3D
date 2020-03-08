using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;

public class Client : MonoBehaviour {
    public static Client instance = null;
    private string ip = "127.0.0.1";
    private int port = 8080;
    private int id;
    private string username;

    private Tcp tcp;
    private bool tcpConnected = false;

    private Udp udp;
    private bool udpConnected = false;

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

    public void setUdpConnected() {
        udpConnected = true;
        Debug.Log("Connected by tcp and udp!");
    }

    public void sendPlayerKeys(float x, float y, float mouseX, float mouseY, bool mouseLeft, bool mouseRight, bool jumping, bool shift, bool e) {
        Packet packet = new Packet();
        packet.Write("playerKeys");
        packet.Write(id);
        packet.Write(x);
        packet.Write(y);
        packet.Write(mouseX);
        packet.Write(mouseY);
        packet.Write(mouseLeft);
        packet.Write(mouseRight);
        packet.Write(jumping);
        packet.Write(shift);
        packet.Write(e);
        sendUdpData(packet);
    }

    public void spawnPlayer(int id, Vector3 position, Quaternion rotation) {
        tcpConnected = true;
        this.id = id;
        username = GameManager.instance.username.text;

        ThreadManager.ExecuteOnMainThread(() => {
            GameManager.instance.instantiatePlayer(id, username, position, rotation);
        });

        Packet packet = new Packet();
        packet.Write("newConnection");
        packet.Write(id);
        packet.Write(username);
        sendTcpData(packet);
    }

    public void newConnection(int id, string username, Vector3 position, Quaternion rotation) {
        ThreadManager.ExecuteOnMainThread(() => {
            GameManager.instance.instantiatePlayerEnemy(id, username, position, rotation);
        });
    }

    public void setId(int id) {
        this.id = id;
    }

    public int getId() {
        return id;
    }
}
