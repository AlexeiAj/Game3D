// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using System.Net;
// using System.Net.Sockets;
// using System;

// public class Udp {
//     private UdpClient socket;
//     private IPEndPoint endPoint;

//     public void connect(String ip, int port, int localport) {
//         endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
//         socket = new UdpClient(localport);

//         socket.Connect(endPoint);
//         socket.BeginReceive(receiveCallback, null);

//         //iniciar conexao
//         Packet packet = new Packet();
//         sendData(packet);
//     }

//     public void sendData(Packet packet) {
//         Debug.Log("ENVIANDO UDP");
//         packet.Write(Client.instance.getId());
//         try {
//             if(socket == null) return;
//             socket.BeginSend(packet.ToArray(), packet.Length(), null, null);
//         } catch {
//             Debug.Log("Erro ao enviar msg para o server udp!");
//         }
//     }

//     private void receiveCallback(System.IAsyncResult result) {
//         try {
//             byte[] data = socket.EndReceive(result, ref endPoint);
//             socket.BeginReceive(receiveCallback, null);

//             if(data.Length < 4) {
//                 Debug.Log("Desconectar do server udp");
//                 return;
//             }

//             handleData(data);
//         } catch (System.Exception e) {
//             Debug.Log(e);
//             Debug.Log("Desconectar do server udp");
//         }
//     }

//     private void handleData(byte[] data) {
//         Packet packet = new Packet(data);
//         int packetLenght =  packet.ReadInt();
//         data = packet.ReadBytes(packetLenght);

//         packet = new Packet(data);

//         string msg = packet.ReadString();
//         int id = packet.ReadInt();
//         Debug.Log("msgUDP: " + msg + " id: " + id);
        
//         Client.instance.setId(id);
//         Client.instance.sendMsgUdp("Estou conectado ao servidor UDP!");
//     }

// }
