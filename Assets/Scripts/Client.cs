using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;

public class Client : MonoBehaviour {
    public static Client instance = null;
    private string ip = "127.0.0.1";
    private int port = 8080;
    private int id = 0;

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
        sendUdpData("Hello from client UDP.");
    }

    private void sendTcpData(string msg) {
        Packet packet = new Packet();
        packet.Write(msg);
        packet.Write(UIManager.instance.username.text);
        packet.Write(id);
        packet.WriteLength();
        Debug.Log("Sending message to server.. " + msg);
        tcp.sendData(packet);
    }

    public void setTcpConnected(bool tcpConnected) {
        this.tcpConnected = tcpConnected;
        sendTcpData("I'm connected in the server by TCP!");
    }

    private void sendUdpData(string msg) {
        Packet packet = new Packet();
        packet.Write(msg);
        packet.Write(UIManager.instance.username.text);
        packet.Write(id);
        packet.WriteLength();
        Debug.Log("Sending message to server.. " + msg);
        udp.sendData(packet);
    }

    public void setUdpConnected(bool udpConnected) {
        this.udpConnected = udpConnected;
        sendUdpData("I'm connected in the server by UDP!");
    }

    public void setId(int id) {
        this.id = id;
    }

    public int getId() {
        return id;
    }
}
