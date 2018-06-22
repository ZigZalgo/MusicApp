using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.IO;
using System;

public class Client : MonoBehaviour {

    private bool SocketReady = false;

    private TcpClient socket;

    private NetworkStream stream;

    private StreamWriter writer;

    private StreamReader reader;

    public void ConnectToServer()
    {
        //If we have already connected
        if (SocketReady)
            return;

        string host = "127.0.0.1";
        int port = 1337;


        try
        {
            socket = new TcpClient(host, port);
            stream = socket.GetStream();
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);
        }
        catch (Exception e)
        {
            //ALTER TO ALERT PLAYER THAT NO HOST EXISTS
            Debug.Log(e);
        }
    }

    private void update()
    {
        if (SocketReady)
        {
            if(stream.DataAvailable)
            {
                string data = reader.ReadLine();
                if(data != null)
                {
                    OnIncomingData(data);
                }
            }
        }
    }

    void OnIncomingData(string data)
    {
        Debug.Log(data);
    }
}
