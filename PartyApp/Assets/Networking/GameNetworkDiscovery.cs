using Assets.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameNetworkDiscovery : NetworkDiscovery {

	// Use this for initialization
	void Awake () {
        if (Init())
            Debug.Log("Started");
        else
            Debug.Log("Error initializing host");
	}

    /// <summary>
    /// Stops any brodcasts if exists, and reinitializes this network discovery
    /// </summary>
    /// <returns></returns>
    public bool Init()
    {
        try
        {
            StopBroadcast();
        }
        catch (Exception e)
        {
            Debug.Log("No brodcasting to begin with");
        }

        return Initialize();
    }

    /// <summary>
    /// Begins the network discovery in server mode
    /// </summary>
    /// <returns></returns>
    public bool CreateGame()
    {   
        Init();
        if (StartAsServer())
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Begins the network discovery in client mode
    /// </summary>
    /// <returns></returns>
    public bool JoinGame()
    {
        Init();
        return StartAsClient();
    }

    /// <summary>
    /// If we have received a broadcast, we are a client.
    /// <para>Thus we now setup the network variables with the proper information, then destroy this object</para>
    /// </summary>
    /// <param name="fromAddress"></param>
    /// <param name="data"></param>
    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        base.OnReceivedBroadcast(fromAddress, data);
        Debug.Log("Address: "+fromAddress);
        Destroy(this);
    }
    


}
