using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

namespace Assets.Networking
{
    public class HostSession : MonoBehaviour
    {
        public Queue<Player> turns;
        public static HostSession Instance;
        public Player current;

        public void Start()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(this);


            turns = new Queue<Player>();
        }

        public void SignalNextTurn()
        {
            current = turns.Dequeue();
            turns.Enqueue(current);
            if (current.isHost)
            {
                //Signify Host Turn
            }
            else
            {
                Server.Instance.Broadcast("TURN:", new List<ServerClient>() { current.client });
            }
            
        }

        public void LoadSession()
        {
            foreach(ServerClient c in Server.Instance.connectedClients)
            {
                turns.Enqueue(new Player(c));
            }
            
        }
    }


}
