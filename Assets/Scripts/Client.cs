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

    private void Awake() {
        if (instance != null && instance != this){
            Destroy(this.gameObject);
            return;
        }

        instance = this;
    }

    public void connectToServer() {
        tcp = new Tcp(); 
        tcp.connect(ip, port);
    }

    public void sendMsg(string msg) {
        Packet packet = new Packet();
        packet.Write(msg);
        packet.Write(UIManager.instance.username.text);
        packet.Write(id);

        sendTcpData(packet);
    }

    private void sendTcpData(Packet packet) {
        packet.WriteLength();
        tcp.sendData(packet);
    }

    public void setId(int id) {
        this.id = id;
    }
}
