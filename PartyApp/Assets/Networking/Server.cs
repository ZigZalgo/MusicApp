using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using Assets.Networking;

public class Server : MonoBehaviour
{

    private List<ServerClient> connectedClients;
    private List<ServerClient> disconnectClients;

    public int port = 1337;

    private TcpListener serverListener;

    private bool Started;

    private void Start()
    {
        connectedClients = new List<ServerClient>();
        disconnectClients = new List<ServerClient>();

        try
        {
            serverListener = new TcpListener(IPAddress.Any, port);
            Debug.Log(serverListener.LocalEndpoint.ToString());
            serverListener.Start();

            StartListening();
            Started = true;
            Debug.Log("Server started on port " + port);
        }
        catch(Exception e)
        {
            Debug.Log(e);
        }
    }

    private void Update()
    {
        if (!Started)
            return;
        foreach(ServerClient c in connectedClients)
        {
            if (!IsClientConnected(c.tcp))
            {
                c.tcp.Close();
                disconnectClients.Add(c);
                continue;
            }
            else
            {
                NetworkStream stream = c.tcp.GetStream();
                if (stream.DataAvailable)
                {
                    StreamReader reader = new StreamReader(stream, true);
                    string data = reader.ReadLine();

                    if (data != null)
                        OnIncomingData(c, data);
                }
            }
        }
    }

    private void OnIncomingData(ServerClient c, string data)
    {
        Debug.Log(c.clientName + " has sent " + data);
    }

    private void Broadcast(string data, List<ServerClient> clientsToSend)
    {
        foreach (ServerClient client in clientsToSend)
        {
            try
            {
                StreamWriter writer = new StreamWriter(client.tcp.GetStream());
                writer.WriteLine(data);
                writer.Flush();
            }
            catch(Exception e)
            {
                Debug.Log(e);
            }
        }
    }

    private bool IsClientConnected(TcpClient client)
    {
        try
        {
            if(client != null && client.Client != null && client.Client.Connected)
            {
                if(client.Client.Poll(0, SelectMode.SelectRead))
                {
                    return !(client.Client.Receive(new byte[1], SocketFlags.Peek) == 0);
                }

                return true;
            }
            else
                return false;
        }
        catch
        {
            return false;
        }
    }

    private void StartListening()
    {
        serverListener.BeginAcceptTcpClient(AcceptTcpClient, serverListener);
    }

    /// <summary>
    /// TODO Brodcast all MP3 Headers
    /// </summary>
    /// <param name="result"></param>
    private void AcceptTcpClient(IAsyncResult result)
    {
        TcpListener listener = (TcpListener)result.AsyncState;
        ServerClient client = new ServerClient(listener.EndAcceptTcpClient(result));
        connectedClients.Add(client);

        MP3File[] mp3s = MusicPlayer.Instance.asMP3;
        string outText = "SONGS\n";
        outText+=JsonHelper.arrayToJson<MP3File>(mp3s);
        Debug.Log(outText);
        Broadcast(outText, new List<ServerClient>(){client});


        StartListening();
    }
}

public class ServerClient
{
    public TcpClient tcp;
    public string clientName;

    public ServerClient(TcpClient clientSocket)
    {
        clientName = "Guest";
        tcp = clientSocket;
    }
}
