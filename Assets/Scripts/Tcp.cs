using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;

public class Tcp {
    private int dataBufferSize = 4096;
    private TcpClient socket;
    private NetworkStream stream;
    private byte[] recieveBuffer;
    private Packet receiveData;
    private string ip;
    private int port;
    private bool firstConnection = true;

    public Tcp(TcpClient socket, string ip, int port) {
        this.socket = socket;
        this.port = port;
        this.ip = ip;
    }

    public void connect() {
        socket.ReceiveBufferSize = dataBufferSize;
        socket.SendBufferSize = dataBufferSize;

        receiveData = new Packet();
        recieveBuffer = new byte[dataBufferSize];

        socket.BeginConnect(ip, port, connectCallback, socket);
    }
    
    private void connectCallback(System.IAsyncResult result) {
        socket.EndConnect(result);
        if (!socket.Connected) return;

        stream = socket.GetStream();
        stream.BeginRead(recieveBuffer, 0, dataBufferSize, receiveCallback, null);
    }

    private void receiveCallback(System.IAsyncResult result) {
        try {
            int byteLenght = stream.EndRead(result);

            if (byteLenght <= 0) {
                Debug.Log("Disconnecting client tcp...");
                return;
            }

            byte[] data = new byte[byteLenght];
            System.Array.Copy(recieveBuffer, data, byteLenght);
            receiveData.Reset(handleData(data));

            stream.BeginRead(recieveBuffer, 0, dataBufferSize, receiveCallback, null);
        } catch (System.Exception e) {
            Debug.Log(e);
            Debug.Log("Disconnecting client tcp...");
        }
    }

    private bool handleData(byte[] data) {
        int packetLenght = 0;

        receiveData.SetBytes(data);

        if (receiveData.UnreadLength() >= 4) {
            packetLenght = receiveData.ReadInt();
            if (packetLenght <= 0) return true;
        }

        while(packetLenght > 0 && packetLenght <= receiveData.UnreadLength()) {
            byte[] packetBytes = receiveData.ReadBytes(packetLenght);
            Packet packet = new Packet(packetBytes);

            string msg = packet.ReadString();
            int id = packet.ReadInt();
            Debug.Log("Server tcp message: " + msg + " id: " + id);
            
            if (firstConnection) {
                Client.instance.setId(id);
                Client.instance.setTcpConnected(true);
                Client.instance.connectToUdp(((IPEndPoint) socket.Client.LocalEndPoint).Port);
                firstConnection = false;
            }

            packetLenght = 0;

            if (receiveData.UnreadLength() >= 4) {
                packetLenght = receiveData.ReadInt();
                if (packetLenght <= 0) return true;
            }
        }

        return packetLenght <= 1;
    }

    public void sendData(Packet packet) {
        try {
            if(socket == null) return;
            stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
        } catch {
            Debug.Log("Err. sending tcp to server!");
        }
    }
}
