using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using Server_Lidar.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server_Lidar.Models
{
    class Server
    {
        #region =================== costanti ===================
        private const string PORT_NAME = "COM4";
        #endregion

        #region =================== membri statici =============
        private static Logger myLogger = LogManager.GetCurrentClassLogger();
        private static Thread send;
        private static bool canSend = true;
        private static bool isSending = false;
        private static TcpListener tcpListener;
        private static TcpClient connectedClient;
        private static List<Vector3> vector3s =new List<Vector3>();
        private static CancellationTokenSource ts;
        private static CancellationToken end;
        #endregion

        #region =================== membri & proprietà =========
        private SerialPort arduino;
        private int serverPort;
        
        public int ServerPort
        {
            get 
            { 
                return serverPort; 
            }
            set 
            {
                if(value > 1024)
                {
                    serverPort = value;
                }
                else
                {
                    serverPort = 12345;
                }
            }
        }

        #endregion

        #region =================== costruttori ================
        public Server(int port)
        {
            ServerPort = port;
            ts = new CancellationTokenSource();
            end = ts.Token;
        }
        public Server(int serverPort,string serialPort,int baudRate)
        {
            ServerPort = ServerPort;
            arduino = new SerialPort(serialPort, baudRate);
        }
        #endregion

        #region =================== metodi aiuto ===============
        private static void ConfigureServices(IConfiguration configuration, IServiceCollection services)
        {
            string connection = "Data Source =" + Path.Combine(AppContext.BaseDirectory, "c:\\temp\\Lidar.3AA.db");
            services.AddDbContext<AppDbContext>(options => options.UseSqlite(connection));
            services.AddSingleton<IVector3Repository, VectorDbDataRepository>();
        }
        /// <summary> 	
        /// Invia un messaggio al client usando la connessione tramite socket.
        /// </summary> 	
        private static void SendMessage(string serverMessage)
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
                    isSending = false;
                    myLogger.Error(e.Message);
                }
            }
            
        }
        #endregion

        #region =================== metodi generali ============
        public void Start()
        {
            // Istanzio un Ascoltatore TCP.
            tcpListener = new TcpListener(IPAddress.Parse("0.0.0.0"), ServerPort);
            // Conosco l'host del server.
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            tcpListener.Start();
            myLogger.Info(String.Format("The server {0} is online at {1} listening ", host.HostName, tcpListener.LocalEndpoint));
            Byte[] bytes = new Byte[1024];
            int cnt = 0;
            var database = Host.CreateDefaultBuilder()
                  .ConfigureServices((context, services) =>
                  {
                      ConfigureServices(context.Configuration, services);
                  })
                  .ConfigureLogging((context, logging) =>
                  {
                      logging.AddConfiguration(context.Configuration.GetSection("Logging"));
                      logging.AddConsole();
                      logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", Microsoft.Extensions.Logging.LogLevel.Warning);
                      logging.AddFilter("Microsoft.EntityFrameworkCore.Infrastructure", Microsoft.Extensions.Logging.LogLevel.Warning);
                  })
                  .Build();
            var db = database.Services.GetRequiredService<IVector3Repository>();
            Task.Factory.StartNew(() =>
            {
                try
                {
                    // Avvio la connessione seriale tramite la porta COM
                    arduino.Open();
                    while (vector3s.Count < 901)
                    {
                        if (end.IsCancellationRequested)
                        {
                            myLogger.Debug("task canceled");
                            break;
                        }
                        try
                        {
                            if (arduino.BytesToRead > 0)
                            {
                                // faccio resettare lo stream di arduino per evitare caratteri ambigui
                                arduino.BaseStream.Flush();
                                // leggo i dati disponibili sulla seriale
                                string reader = arduino.ReadExisting();
                                myLogger.Debug(reader);
                                myLogger.Debug(String.Format("Index of vector: {0}",cnt));
                                Vector3 v3 = CreateVector(reader, cnt);
                                vector3s.Add(v3);
                                //db.Insert(v3);
                                myLogger.Info(string.Format("Scanned point in => (x:{0}, y:{1}, z:{2})", v3.X, v3.Y, v3.Z));
                                cnt++;
                            }
                        }
                        catch (InvalidOperationException ioe)
                        {
                            myLogger.Error(ioe.StackTrace);
                            return;
                        }
                    }
                }
                catch (Exception e)
                {
                    myLogger.Error(e.Message);
                }
            }, end);
            while (true)
            {
                string clientMessage;
                using (connectedClient = tcpListener.AcceptTcpClient())
                {
                    newConnection();
                    myLogger.Info(String.Format("The client {0} has connected", connectedClient.Client.RemoteEndPoint));
                    // ottengo l'oggetto stream per leggere.
                    using (NetworkStream stream = connectedClient.GetStream())
                    {
                        int length;
                        // leggo i dati che arrivano sullo stream e gli inserisco in un array di byte.
                        while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            var incomingData = new byte[length];
                            Array.Copy(bytes, 0, incomingData, 0, length);
                            // converto l'array di byte nel messaggio.
                            clientMessage = Encoding.ASCII.GetString(incomingData);
                            myLogger.Info(String.Format("The client {0} sent {1}", connectedClient.Client.LocalEndPoint, clientMessage));
                            /* 
                            * FORMATO --> Int:0,10
                            * Int:{0},{1}
                            * 0 -> dati di partenza
                            * 1 -> quanti dati ricevere
                            */
                            string[] message = clientMessage.Split(':');
                            string header = message[0];
                            if (header.Equals("Int"))
                            {
                                //0,1
                                string[] content = message[1].Split(',');
                                int min = int.Parse(content[0]);
                                int max = int.Parse(content[1]);
                                send = new Thread(() => SendToUnity(min, max));
                                if (canSend)
                                {
                                    isSending = true;
                                    send.Start();
                                    myLogger.Info(String.Format("The server sends data to client {0}", connectedClient.Client.RemoteEndPoint));
                                }
                            }
                            else if (header.Equals("Msg"))
                            {
                                string command = message[1];
                                if (command.Equals("Stop"))
                                {
                                    isSending = false;
                                    myLogger.Info(String.Format("The server stops sending data to client {0}", connectedClient.Client.RemoteEndPoint));
                                }
                            }
                            else
                            {
                                myLogger.Warn("No header element passed");
                            }
                        }
                        myLogger.Warn(String.Format("The client {0} disconnected", connectedClient.Client.RemoteEndPoint));
                        stream.Close();
                    }
                }
            }
        }
        /// <summary>
        /// Crea un vettore tramite 2 angoli e l'intensità
        /// </summary>
        /// <param name="data">Stringa contenente i dati (hz°,vt°,|i|) .</param>
        /// <returns></returns>
        private Vector3 CreateVector(string data, int id)
        {
            string[] read = data.Split("\n");
            string[] coords = data.Split(',');
            //6,0,18
            float horizontal = float.Parse(coords[0]);
            float vertical = float.Parse(coords[1]);
            float intensity = float.Parse(coords[2]);
            myLogger.Debug(String.Format("Horizontal {0}, Vertical {1}, Intensity {2}", horizontal,vertical,intensity));
            return new Vector3
            {
                X = (float)(intensity * Math.Sin(horizontal) * Math.Cos(vertical)),
                Y = (float)(intensity * Math.Sin(horizontal) * Math.Sin(vertical)),
                Z = (float)(intensity * Math.Cos(horizontal)),
                /*X = (float)Math.Cos(horizontal),
                Y = (float)Math.Sin(horizontal),
                Z = (float)Math.Sin(vertical),*/
                Id = id
            };
        }
        /// <summary>
        /// Imposta una nuova connessione 
        /// </summary>
        private void newConnection()
        {
            canSend = true;
            isSending = false;
        }
        /// <summary>
        /// Invia i dati 
        /// </summary>
        /// <param name="value">Indica il valore iniziale e quanti dati inviare</param>
        private void SendToUnity(params int[] value )
        {
            string message = String.Empty;
            string fake = String.Empty;
            int clientIndex = value[0];
            int n_pallini = value[1];
            for (int i = clientIndex; i < clientIndex + n_pallini && i < vector3s.Count - 1; i++)
            {
                if (!isSending)
                {
                    break;
                }
				message = String.Format("{0},{1},{2}", vector3s[i].X, vector3s[i].Y, vector3s[i].Z);
                if (!message.Equals(fake))
                {
                    SendMessage(message);
                    myLogger.Debug(message);
                }
                fake = message;
                Thread.Sleep(125);
            }
            // Invia il cancelletto solo quando il client richiede più dati di quelli disponibili
            if (isSending)
            {
                message = String.Format("#,#,#");
                SendMessage(message);
                myLogger.Debug(message);
            }
        }
        /// <summary>
        /// Interrompe tutte le interazioni del server
        /// </summary>
        public void Stop()
        {
            ts.Cancel();
            tcpListener.Stop();
            myLogger.Warn("The server is shutting down");
        }
        #endregion
    }
}