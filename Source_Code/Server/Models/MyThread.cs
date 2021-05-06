using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using NLog;

namespace Server_Lidar.Models
{
    public class MyThread
    {
        private bool isSending;

        public bool IsSending
        {
            get { return isSending; }
            set { isSending = value; }
        }
        public MyThread()
        {
            IsSending = true;
        }
        /// <summary>
        /// Invia i dati al client.
        /// param name="args[0]" Indica il riferimento al Logger</param>
        /// param name="args[1]" Indica il riferimento al client connesso
        /// param name="args[2]" Indica il riferimento alla lista di punti
        /// param name="args[3]" Indica il riferimento all'indice del client
        /// param name="args[4]" Indica il riferimento al numero di pallini da ottenere
        /// </summary>
        /// <param name="args">Array di oggetti</param>
        public void SendToUnity(object args)
        {
            Array argsArray = new Object[5];
            argsArray = (Array)args;
            Logger myLogger = (Logger)argsArray.GetValue(0);
            TcpClient client = (TcpClient)argsArray.GetValue(1);
            List<Vector3> vector3s = (List<Vector3>)argsArray.GetValue(2);
            int clientIndex = (int)argsArray.GetValue(3);
            int n_pallini = (int)argsArray.GetValue(4);

            string message = String.Empty;
            string fake = String.Empty;
            for (int i = clientIndex; i < clientIndex + n_pallini && i < vector3s.Count - 1; i++)
            {
                if (!IsSending)
                {
                    break;
                }
                message = String.Format("{0},{1},{2}", vector3s[i].X, vector3s[i].Y, vector3s[i].Z);
                if (!message.Equals(fake))
                {
                    SendMessage(myLogger, message, client);
                    myLogger.Debug(message);
                }
                fake = message;
                Thread.Sleep(63);
            }
            // Invia il cancelletto solo quando il client richiede più dati di quelli disponibili
            if (IsSending)
            {
                message = String.Format("#,#,#");
                SendMessage(myLogger, message, client);
                myLogger.Debug(message);
            }
        }
        /// <summary> 	
        /// Invia un messaggio al client usando la connessione tramite socket.
        /// </summary> 	
        private void SendMessage(Logger myLogger, string serverMessage, TcpClient connectedClient)
        {
            if (connectedClient == null)
            {
                return;
            }
            if (connectedClient.Connected)
            {
                try
                {
                    // ottengo l'oggetto stream per scrivere.		
                    NetworkStream stream = connectedClient.GetStream();
                    if (stream.CanWrite)
                    {
                        // Converto la stringa in un array di byte.                 
                        byte[] serverMessageAsByteArray = Encoding.ASCII.GetBytes(serverMessage);
                        // Scrivo l'array di byte sul socketConnection stream.               
                        stream.Write(serverMessageAsByteArray, 0, serverMessageAsByteArray.Length);
                        myLogger.Info(String.Format("The server send a message to client {0}", connectedClient.Client.RemoteEndPoint));
                    }
                    else
                    {
                        stream.Close();
                    }
                }
                catch (Exception e)
                {
                    IsSending = false;
                    myLogger.Error(e.Message);
                }
            }

        }

        public void Stop()
        {
            isSending = false;
        }
    }
}
