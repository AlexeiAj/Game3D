using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
using System.Reflection;

public class Udp {
    private IPEndPoint endPoint;
    private UdpClient socket;
    private string ip;
    private int port;

    public Udp(UdpClient socket, string ip, int port) {
        this.socket = socket;
        this.port = port;
        this.ip = ip;
        this.endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
    }

    public void connect() {
        socket.Connect(endPoint);
        socket.BeginReceive(receiveCallback, null);
    }

    private void receiveCallback(System.IAsyncResult result) {
        try {
            byte[] data = socket.EndReceive(result, ref endPoint);
            socket.BeginReceive(receiveCallback, null);
            if(data.Length < 4) {
                Debug.Log("Disconnecting client udp...");
                return;
            }

            handleData(data);
        } catch (System.Exception e) {
            Debug.Log(e);
            Debug.Log("Disconnecting client udp...");
        }
    }

    private void handleData(byte[] data) {
        Packet packet = new Packet(data);

        int i = packet.ReadInt(); //só para remover o id do pacote
        string method = packet.ReadString();

        MethodInfo theMethod = Client.instance.GetType().GetMethod(method);
        theMethod.Invoke(Client.instance, new object[]{packet});
    }

    public void sendData(Packet packet) {
        try {
            if(socket == null) return;
            socket.BeginSend(packet.ToArray(), packet.Length(), null, null);
        } catch {
            Debug.Log("Err. sending udp to server!");
        }
    }
}
