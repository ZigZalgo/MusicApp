using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.IO;
using System;
using Assets.Networking;
using System.Text.RegularExpressions;

public class Client : MonoBehaviour
{

    private bool SocketReady;

    private TcpClient socket;

    private NetworkStream stream;

    private StreamWriter writer;

    private StreamReader reader;

    string RegexPattern = "([0-9]+)OF([0-9]+)";
    MP3File[] ReceivedSongs;

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
            Debug.Log("Connected to server from client");
            SocketReady = true;
        }
        catch (Exception e)
        {
            //ALTER TO ALERT PLAYER THAT NO HOST EXISTS
            Debug.Log(e);
        }
    }

    private void Update()
    {
        if (SocketReady)
        {
            if (stream.DataAvailable)
            {
                string data = reader.ReadLine();
                if (data != null)
                {
                    OnIncomingData(data);
                }
            }
        }
    }

    void OnIncomingData(string data)
    {
        if (data.StartsWith("SONGS:"))
        {
            InterpretSongMessage(data);
        }


        if (data.StartsWith("TURN:"))
        {
            //FIRE OFF THE DELEGATE SIGNALLING ITS OUR TURN
        }

    }

    private void InterpretSongMessage(string data)
    {
        string receivedString = data.Substring(6, data.Length - 6);

        int indexOFColon = receivedString.IndexOf(':');
        string regexInput = receivedString.Substring(0, indexOFColon);
        string jsonString;

        jsonString = receivedString.Substring(indexOFColon + 1, receivedString.Length - (indexOFColon) - 2);


        Match m = Regex.Match(regexInput, RegexPattern);
        if (!m.Success)
            return;
        if (m.Groups.Count < 3)
            return;
        int xOf = int.Parse(m.Groups[1].Value);
        int ofY = int.Parse(m.Groups[2].Value);
        if (ReceivedSongs == null)
        {
            ReceivedSongs = new MP3File[ofY + 1];
        }
        if (data.EndsWith(":DONE"))
        {
            jsonString = jsonString.Substring(0, jsonString.Length - 5);
        }

        MP3File[] mp3s = JsonHelper.FromJson<MP3File>(jsonString + "}");

        for (int i = 0; i < mp3s.Length; i++)
        {
            try
            {
                ReceivedSongs[xOf + i] = mp3s[i];
            }
            catch (Exception e)
            {
                //Debug.Log("I: " + i + "\n\txOf: " + xOf + "\n\tReceived length: " + ReceivedSongs.Length + "\n\tmp3s Length: " + mp3s.Length);
                throw e;
            }
        }
        if (data.EndsWith(":DONE"))
        {
            Debug.Log("DONE");
        }
    }
}
