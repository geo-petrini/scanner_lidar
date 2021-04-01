using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Server_Lidar.Models
{
    public class ServerHandler
    {
        private TcpClient connectedClient;

        public ServerHandler(TcpClient connectedClient)
        {
            this.connectedClient = connectedClient;
        }

        internal void start()
        {
            throw new NotImplementedException();
        }
    }
}
