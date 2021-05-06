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
        /// <summary>
        /// Indica il logger utilizzato per avere traccia di quello che accade.
        /// </summary>
        private static Logger myLogger = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// Indica se il server è abilitato a mandare i dati ai client.
        /// </summary>
        private static bool canSend = true;
        /// <summary>
        /// Indica il socket utilizzato per effetuare il binding della porta da parte del server.
        /// </summary>
        private static TcpListener tcpListener;
        /// <summary>
        /// Indica la lista dei punti che vengono scansionati
        /// </summary>
        private static volatile List<Vector3> vector3s = new List<Vector3>();
        /// <summary>
        /// Indica la sorgente del token.
        /// </summary>
        private static CancellationTokenSource ts = new CancellationTokenSource();
        /// <summary>
        /// Indica il token per poter cancellare una determinata task.
        /// </summary>
        private static CancellationToken end = ts.Token;
        /// <summary>
        /// Indica il tempo del primo gennaio 1970 per poter calcolare il currenttime
        /// </summary>
        private static DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        #endregion

        #region =================== membri & proprietà =========
        /// <summary>
        /// Indica la porta seriale utilizzata dal computer per comunicare con arduino.
        /// </summary>
        private SerialPort arduino;
        /// <summary>
        /// Indica la porta del server.
        /// </summary>
        private int serverPort;
        
        public int ServerPort
        {
            get 
            { 
                return serverPort; 
            }
            set 
            {
                if(value >= 1024 && value <= 65535)
                {
                    serverPort = value;
                }
                else
                {
                    myLogger.Warn("The port value in: Server_Lidar.dll.config is not in the range of [1024,65535], default settings loaded -> port = 12345");
                    serverPort = 12345;
                }
            }
        }

        #endregion

        #region =================== costruttori ================
        /// <summary>
        /// Costruttore per inizializzare il server.
        /// </summary>
        /// <param name="serverPort">Indica la porta del server</param>
        /// <param name="serialPort">Indica la porta seriale (COM)</param>
        /// <param name="baudRate">Indica la velocità dei dati in bit al secondo.</param>
        public Server(int serverPort,string serialPort,int baudRate)
        {
            ServerPort = serverPort;
            arduino = new SerialPort(serialPort, baudRate);
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
                // Avvio un'attività in parallelo al processo principale
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        // Avvio la connessione seriale tramite la porta COM
                        arduino.Open();
                        long start = (long)(DateTime.UtcNow - dateTime).Seconds;
                        while (true)
                        {
                            if (end.IsCancellationRequested)
                            {
                                long end = (long)(DateTime.UtcNow - dateTime).Seconds;
                                myLogger.Debug("Task cancelled");
                                myLogger.Info(String.Format("Execution time of data scan: ~{0} seconds", end - start));
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
                                            vector3s.Clear();
                                            cnt = 0;
                                        }
                                        else if (reader.Contains("<EOF>"))
                                        {
                                            myLogger.Info("Arduino sent all data");
                                            ts.Cancel();
                                        }
                                    }     
                                }
                                arduino.BaseStream.Flush();
                            }
                            catch (InvalidOperationException ioe)
                            {
                                myLogger.Error(ioe.Message);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        myLogger.Error(e.Message);
                        return;
                    }
                }, end);
            }
        }
        /// <summary>
        /// Imposta una nuova connessione 
        /// </summary>
        private void newConnection()
        {
            canSend = true;
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
                Y = (float)(intensity * Math.Round(Math.Cos(Math.PI * vertical / 180.0),6) * Math.Round(Math.Sin(Math.PI * horizontal / 180.0),6)),
                Z = (float)(intensity * Math.Round(Math.Sin(Math.PI * vertical / 180.0), 6)), 
                Id = id
            };

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
            myLogger.Info(String.Format("The server {0} is online listening on {1} ", host.HostName, tcpListener.LocalEndpoint));
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
                        myLogger.Info(String.Format("The server sent data to client {0}", client.Client.RemoteEndPoint));
                    }
                }
                else if (header.Equals("Msg"))
                {
                    string command = message[1];
                    if (command.Equals("Stop"))
                    {
                        mT.Stop();
                        myLogger.Info(String.Format("The server stopped sending data to client {0}", client.Client.RemoteEndPoint));
                    }
                }
                else if (header.Contains("GET / HTTP/1.1"))
                {
                    TcpClient web = client;
                    web.Close();
                    return;
                }
                else
                {
                    myLogger.Warn("No protocol supported");
                }
            }
            myLogger.Warn(String.Format("The client {0} disconnected", client.Client.RemoteEndPoint));
            stream.Close();
        }
        #endregion
    }
}
