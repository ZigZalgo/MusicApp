using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using Assets.Networking;
using Assets.Extensions;

public class Server : MonoBehaviour
{

    public Canvas success;
    public Canvas failed;

    public static Server Instance;

    public List<ServerClient> connectedClients;
    private List<ServerClient> disconnectClients;

    public int port = 1337;

    private TcpListener serverListener;

    private bool Started;

    private void Start()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            if(!ReferenceEquals(Instance, this))
            {
                Destroy(this);
            }
        }

        connectedClients = new List<ServerClient>();
        disconnectClients = new List<ServerClient>();

        try
        {
            serverListener = new TcpListener(IPAddress.Any, port);
            Debug.Log(serverListener.LocalEndpoint.ToString());
            serverListener.Start();

            StartListening();
            Started = true;
            success.enabled = true;
            failed.enabled = false;
        }
        catch(Exception e)
        {
            failed.enabled = true;
            success.enabled = false;
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
        //If a client has sent that they are ready, we signify so
        if(data == "READY:")
        {
            c.Ready = true;
        }
        //If a client has sent their choice
        if(data.StartsWith("CHOICE:") && ReferenceEquals(HostSession.Instance.current, c))
        {

        }
    }

    public void Broadcast(string data, List<ServerClient> clientsToSend)
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

    private void AcceptTcpClient(IAsyncResult result)
    {
        TcpListener listener = (TcpListener)result.AsyncState;
        ServerClient client = new ServerClient(listener.EndAcceptTcpClient(result));
        connectedClients.Add(client);
        SendAllSongs(client);
        StartListening();
    }

    private void SendAllSongs(ServerClient client)
    {
        for (int i = 0; i < MusicPlayer.Instance.asMP3.Length; i += 100)
        {
            string outText = "SONGS:";
            outText += i + "OF" + MusicPlayer.Instance.asMP3.Length + ":";
            string fromJson = JsonHelper.ToJson<MP3File>(MusicPlayer.Instance.asMP3.Slice(i, 100));
            //Debug.Log("I: "+i+"\n\tSENDING THIS: "+fromJson);
            outText += fromJson;
            if (i+100 > MusicPlayer.Instance.asMP3.Length)
            {
                outText += ":DONE";
            }
            Broadcast(outText, new List<ServerClient>() { client });
        }
    }
}

public class ServerClient
{
    public TcpClient tcp;
    public string clientName;
    public bool Ready;

    public ServerClient(TcpClient clientSocket)
    {
        clientName = "Guest";
        tcp = clientSocket;
    }
}
