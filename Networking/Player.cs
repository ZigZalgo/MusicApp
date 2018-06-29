using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Networking
{
    public class Player
    {
        public ServerClient client;
        public bool isHost = false;

        public Player(bool server)
        {
            isHost = true;
        }

        public Player(ServerClient client)
        {
            this.client = client;
        }
    }
}
