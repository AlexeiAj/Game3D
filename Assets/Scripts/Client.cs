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
//     private Udp udp;

    private void Awake() {
        if (instance != null && instance != this){
            Destroy(this.gameObject);
            return;
        }

        instance = this;
    }

    public void connectToServer() {
        Debug.Log("Connecting......");
        TcpClient socket = new TcpClient();
        tcp = new Tcp(socket, ip, port); 
        tcp.connect();
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

//     public void connectUdp() {
//         udp = new Udp();
//         udp.connect(ip, port, tcp.getLocalPort());
//         sendMsgUdp("teste");
//     }

//     public void sendMsgUdp(string msg) {
//         Packet packet = new Packet();
//         packet.Write(msg);
//         packet.Write(UIManager.instance.username.text);
//         packet.Write(id);

//         sendUdpData(packet);
//     }

//     private void sendUdpData(Packet packet) {
//         packet.WriteLength();
//         udp.sendData(packet);
//     }

    public void setId(int id) {
        this.id = id;
    }

    public int getId() {
        return id;
    }
}
