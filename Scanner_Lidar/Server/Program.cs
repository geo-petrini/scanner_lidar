using NLog;
using Server_Lidar.Models;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading;

namespace Server_Lidar
{
    class Program
    {
        private static int SERVER_PORT;
        private static Logger myLogger = LogManager.GetCurrentClassLogger();
        private const int BAUD_RATE = 115200;
        static void Main(string[] args)
        {
            Console.Title = "Server";
            SERVER_PORT = 12345;
            //115200
            string comPort = DetectArduino();
            Server server = new Server(SERVER_PORT, comPort, BAUD_RATE);
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
            Console.WriteLine("Premi un pulsante per finire...");
            Console.ReadLine();
        }
        /// <summary>
        /// Il metodo si connette alle porte COM disponibili fino a cercare quello che è collegata ad arduino.
        /// Se sulla porta a cui si connette è collegato arduino, l'arduino comunica al pc di essere arduino e quindi di usare quella porta.
        /// </summary>
        /// <returns>La porta COM dove è collegato l'arduino</returns>
        public static string DetectArduino()
        {
            string[] availblePort = SerialPort.GetPortNames();
            List<string> ports = availblePort.OfType<string>().ToList();
            List<string> disponibili = new List<string>();
            SerialPort serial = new SerialPort();
            serial.BaudRate = BAUD_RATE;
            int cnt = 0;
        StartLoop:
            try
            {
                serial.PortName = ports[cnt < ports.Count - 1 ? cnt++ : cnt];
                myLogger.Debug("Porta attuale: " + serial.PortName);
                serial.Open();
                while (true)
                {
                    try
                    {
                        Thread.Sleep(500);
                        if (serial.BytesToRead > 0)
                        {
                            string reader = serial.ReadExisting();
                            if (reader.Contains("CIAO"))
                            {
                                disponibili.Add(serial.PortName);
                                myLogger.Debug("SONO ARDUINO");
                                goto EndLoop;
                            }
                        }
                        else
                        {
                            serial.Close();
                            goto StartLoop;
                        }

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
                goto StartLoop;
            }
        EndLoop:
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

