using NLog;
using Server_Lidar.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Threading;

namespace Server_Lidar
{
    class Program
    {
        /// <summary>
        /// Indica il logger utilizzato per avere traccia di quello che accade.
        /// </summary>
        private static Logger myLogger = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// Indica la porta del server.
        /// </summary>
        private static int serverPort;
        /// <summary>
        /// Indica la velocità dei dati in bit al secondo.
        /// </summary>
        private static int baudRate;
        /// <summary>
        /// Indica la porta seriale utilizzata per comunicare con arduino.
        /// </summary>
        private static string comPort;
        static void Main(string[] args)
        {
            Init();
            Console.Title = String.Format("Server_Lidar {0}:{1}", Dns.GetHostEntry(Dns.GetHostName()).HostName,serverPort);
            comPort = DetectArduino();
            Server server = null;
            if(!comPort.Equals("NO PORT FOUND"))
            {
                server = new Server(serverPort, comPort, baudRate);
                try
                {
                    server.Start();
                }
                catch (Exception e)
                {
                    myLogger.Error(e.Message);
                }
                finally
                {
                    server.Stop();
                }
            }
            Console.WriteLine("Premi un pulsante per finire...");
            Console.ReadLine();
        }
        /// <summary>
        /// Inizializza le componenti per l'avvio del server
        /// </summary>
        private static void Init()
        {
            baudRate = int.Parse(ConfigurationManager.AppSettings.Get("BaudRate"));
            serverPort = int.Parse(ConfigurationManager.AppSettings.Get("ServerPort"));
        }

        /// <summary>
        /// Il metodo si connette alle porte COM disponibili fino a cercare quello che è collegata ad arduino.
        /// Se sulla porta a cui si connette è collegato arduino, l'arduino comunica al pc di essere arduino e quindi di usare quella porta.
        /// </summary>
        /// <returns>La porta COM dove è collegato l'arduino</returns>
        /// <copyright>Matteo Lupica</copyright>
        public static string DetectArduino()
        {
            bool found = false;
            string[] availblePort = SerialPort.GetPortNames();
            List<string> ports = availblePort.OfType<string>().ToList();
            List<string> disponibili = new List<string>();
            SerialPort serial = new SerialPort();
            serial.BaudRate = baudRate;
            int cnt = 0;
            while (true && !found)
            {
                try
                {
                    serial.PortName = ports[cnt < ports.Count - 1 ? cnt++ : cnt];
                    myLogger.Debug("Porta attuale: " + serial.PortName);
                    serial.Open();
                    Thread.Sleep(500);
                    if (serial.BytesToRead > 0)
                    {
                        string reader = serial.ReadExisting();
                        if (reader.Contains("CIAO"))
                        {
                            disponibili.Add(serial.PortName);
                            myLogger.Debug("SONO ARDUINO");
                            found = true;
                        }
                    }
                    else
                    {
                        serial.Close();
                    }

                }
                catch (Exception ioe)
                {
                    myLogger.Error(ioe.Message);
                }
            }
            serial.Write("OK");
            serial.Close();
            foreach (var item in disponibili)
            {
                return item;
            }
            return "NO PORT FOUND";
        }
    }
}