using System.Collections.Generic;
using UnityEngine;

namespace Assets.Networking
{
    public class HostSession : MonoBehaviour
    {
        public Queue<Player> turns;
        public static HostSession Instance;

        public void Start()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(this);


            turns = new Queue<Player>();
        }
        public string CurrentSong;
    }
}
