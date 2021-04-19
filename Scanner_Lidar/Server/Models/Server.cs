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
        #endregion

        #region =================== membri statici =============
        private static Logger myLogger = LogManager.GetCurrentClassLogger();
        private static bool canSend = true;
        private static bool isSending = false;
        private static TcpListener tcpListener;
        private static List<Vector3> vector3s = new List<Vector3>();
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
        public Server(int serverPort,string serialPort,int baudRate, int max_points)
        {
            ServerPort = ServerPort;
            arduino = new SerialPort(serialPort, baudRate);
            ts = new CancellationTokenSource();
            end = ts.Token;
        }
        #endregion

        #region =================== metodi aiuto ===============
        /// <summary>
        /// Configura il servizio per abilitare il DB
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="services"></param>
        private static void ConfigureServices(IConfiguration configuration, IServiceCollection services)
        {
            string connection = "Data Source =" + Path.Combine(AppContext.BaseDirectory, "c:\\temp\\Lidar.3AA.db");
            services.AddDbContext<AppDbContext>(options => options.UseSqlite(connection));
            services.AddSingleton<IVector3Repository, VectorDbDataRepository>();
        }
        /// <summary>
        /// Avvia l'ascolto del server per l'accettazione dei client.
        /// </summary>
        private void ServerListener()
        {
            while (true)
            {
                myLogger.Info("Waiting for a connection...");
                TcpClient connectedClient = tcpListener.AcceptTcpClient();
                Thread t = new Thread(new ParameterizedThreadStart(ServerHandler));
                t.Start(connectedClient);
                myLogger.Info(String.Format("The client {0} has connected", connectedClient.Client.RemoteEndPoint));
            }
        }
        /// <summary>
        /// Legge i dati provenienti dalla seriale dove vi è connesso l'arduino.
        /// </summary>
        private void ReadArduino()
        {
            int cnt = 0;
            if (arduino != null)
            {
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        // Avvio la connessione seriale tramite la porta COM
                        arduino.Open();
                        while (true)
                        {
                            if (end.IsCancellationRequested)
                            {
                                myLogger.Debug("task canceled");
                                break;
                            }
                            try
                            {
                                if (arduino.BytesToRead > 5)
                                {
                                    // faccio resettare lo stream di arduino per evitare caratteri ambigui
                                    // leggo i dati disponibili sulla seriale
                                    myLogger.Debug("BytesToRead: " + arduino.BytesToRead);
                                    byte[] buffer = new byte[arduino.BytesToRead];
                                    string reader = "";
                                    for (int i = 0; i < buffer.Length; i++)
                                    {
                                        reader += (char)arduino.ReadByte();
                                    }
                                    myLogger.Debug(reader);
                                    myLogger.Debug(String.Format("Index of vector: {0}", cnt));
                                    Vector3 v3 = CreateVector(reader, cnt);
                                    if (v3 != null)
                                    {
                                        vector3s.Add(v3);
                                        myLogger.Info(string.Format("Scanned point " + cnt + " in => (x:{0}, y:{1}, z:{2})", v3.X, v3.Y, v3.Z));
                                        cnt++;
                                    }
                                    else
                                    {
                                        if (reader.Contains("CIAO"))
                                        {
                                            arduino.Write("OK");
                                            myLogger.Fatal("RESET DATA REQUEST TO ARDUINO");
                                        }else if (reader.Contains("<EOF>"))
                                        {
                                            myLogger.Info("Arduino send all data");
                                            ts.Cancel();
                                        }
                                    }     
                                }
                                arduino.BaseStream.Flush();
                            }
                            catch (InvalidOperationException ioe)
                            {
                                myLogger.Error(ioe.StackTrace);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        myLogger.Error(e.StackTrace);
                        return;
                    }
                }, end);
            }
        }
        /// <summary> 	
        /// Invia un messaggio al client usando la connessione tramite socket.
        /// </summary> 	
        private static void SendMessage(string serverMessage, TcpClient connectedClient)
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
        /// <summary>
        /// Imposta una nuova connessione 
        /// </summary>
        private void newConnection()
        {
            canSend = true;
            isSending = false;
        }
        /// <summary>
        /// Crea un vettore tramite 2 angoli e l'intensità
        /// </summary>
        /// <param name="data">Stringa contenente i dati (hz°,vt°,|i|) .</param>
        /// <returns></returns>
        private Vector3 CreateVector(string data, int id)
        {
            string[] coords = data.Split(',');
            float horizontal;
            float vertical;
            float intensity;
            if(float.TryParse(coords[0], out horizontal))
            {
                horizontal = float.Parse(coords[0]);
            }
            else
            {
                return null;
            }
            if (float.TryParse(coords[1], out vertical))
            {
                vertical = float.Parse(coords[1]);
            }
            else
            {
                return null;
            }
            if (float.TryParse(coords[2], out intensity))
            {
                intensity = float.Parse(coords[2]);
            }
            else
            {
                return null;
            }
            myLogger.Debug(String.Format("Horizontal {0}, Vertical {1}, Intensity {2}", horizontal, vertical, intensity));
            return new Vector3
            {
                X = (float)(intensity * Math.Round(Math.Cos(Math.PI * vertical / 180.0), 6) * Math.Round(Math.Cos(Math.PI * horizontal / 180.0),6)),
                Y = (float)(intensity * Math.Round(Math.Cos(Math.PI * horizontal / 180.0),6) * Math.Round(Math.Sin(Math.PI * vertical / 180.0),6)),
                Z = (float)(intensity * Math.Round(Math.Sin(Math.PI * vertical / 180.0), 6)), 
                Id = id
            };

        }
        /// <summary>
        /// Invia i dati al client
        /// </summary>
        /// <param name="value">Indica il valore iniziale e quanti dati inviare</param>
        private void SendToUnity(TcpClient client, params int[] value)
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
                    SendMessage(message, client);
                    myLogger.Debug(message);
                }
                fake = message;
            }
            // Invia il cancelletto solo quando il client richiede più dati di quelli disponibili
            if (isSending)
            {
                message = String.Format("#,#,#");
                SendMessage(message, client);
                myLogger.Debug(message);
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
            // Inizio struttura con database sqlite non del tutto implementata
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
            ReadArduino();
            ServerListener();
        }
        /// <summary>
        /// Interrompe tutte le interazioni del server
        /// </summary>
        public void Stop()
        {
            ts.Cancel();
            tcpListener.Stop();
            myLogger.Fatal("The server is shutting down");
        }
        /// <summary>
        /// Gestisce il multi-threading del server, 
        /// per poter fornire il servizio a più client contemporaneamente
        /// </summary>
        /// <param name="obj">Indica il client accettato dalla connessione</param>
        public void ServerHandler(object obj)
        {
            TcpClient client = (TcpClient)obj;
            Byte[] bytes = new Byte[1024];
            string clientMessage;
            newConnection();
            var stream = client.GetStream();
            int length;
            MyThread mT = new MyThread();
            // leggo i dati che arrivano sullo stream e gli inserisco in un array di byte.
            while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                var incomingData = new byte[length];
                Array.Copy(bytes, 0, incomingData, 0, length);
                // converto l'array di byte nel messaggio.
                clientMessage = Encoding.ASCII.GetString(incomingData);
                myLogger.Info(String.Format("The client {0} sent {1}", client.Client.LocalEndPoint, clientMessage));
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
                    if (!mT.IsSending)
                    {
                        mT.IsSending = true;
                    }
                    //0,1
                    string[] content = message[1].Split(',');
                    int min = int.Parse(content[0]);
                    int max;
                    if (content[1].Equals("*"))
                    {
                        max = vector3s.Count - 1;
                    }
                    else
                    {
                        max = int.Parse(content[1]);
                    }
                    if (canSend)
                    {
                        object args = new object[5] { myLogger, client, vector3s, min, max };
                        (new Thread(new ParameterizedThreadStart(mT.SendToUnity))).Start(args);
                        myLogger.Info(String.Format("The server sends data to client {0}", client.Client.RemoteEndPoint));
                    }
                }
                else if (header.Equals("Msg"))
                {
                    string command = message[1];
                    if (command.Equals("Stop"))
                    {
                        mT.Stop();
                        myLogger.Info(String.Format("The server stops sending data to client {0}", client.Client.RemoteEndPoint));
                    }
                }
                else if (header.Contains("GET / HTTP/1.1"))
                {
                    TcpClient web = client;
                    string path = Path.Combine(AppContext.BaseDirectory, @"..\..\..\Request\web.html");
                    string page = File.ReadAllText(path);
                    var utf8 = new UTF8Encoding();
                    web.GetStream().Write(utf8.GetBytes("HTTP/1.1 \r\n 200 OK"));
                    web.GetStream().Write(utf8.GetBytes("ContentType: text/html\r\n"));
                    web.GetStream().Write(utf8.GetBytes("\r\n"));
                    web.GetStream().Write(utf8.GetBytes(page));
                    web.GetStream().Write(utf8.GetBytes("\r\n\r\n"));
                    web.GetStream().Flush();
                }
                else
                {
                    myLogger.Warn("No header element passed");
                }
            }
            myLogger.Warn(String.Format("The client {0} disconnected", client.Client.RemoteEndPoint));
            stream.Close();
        }
        #endregion
    }
}