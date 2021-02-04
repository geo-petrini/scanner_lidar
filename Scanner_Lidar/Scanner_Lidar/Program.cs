using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using NLog;

namespace Scanner_Lidar
{
    class Program
    {
        private static string PORT_NAME = "COM4";
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        private static SerialPort _serialPort;
        private static CancellationTokenSource ts;
        private static CancellationToken ct;
        private static List<Vector3> points;
        static void Main(string[] args)
        {
            Init();
            ReadSerial(ct);
            WriteSerial(ct);
            
            Console.ReadLine();
        }
        /// <summary>
        /// Inizializza le componenti che servono per effettuare la connessione sulla seriale
        /// </summary>
        private static void Init()
        {
            points = new List<Vector3>();
            ///token per la cancellazione delle task
            ts = new CancellationTokenSource();
            ct = ts.Token;
            /// Istanziamento della porta seriale
            /// e configurazione di essa.
            _serialPort = new SerialPort();
            _serialPort.PortName = PORT_NAME;
            _serialPort.BaudRate = 9600;
            try
            {
                _serialPort.Open();
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
            }
        }
        /// <summary>
        /// Legge i valori provenienti da arduino dalla porta COM.
        /// </summary>
        /// <param name="ct">Token che serve a interrompere la lettura dalla seriale</param>
        private static void ReadSerial(CancellationToken ct)
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    if (ct.IsCancellationRequested)
                    {
                        _logger.Debug("task canceled");
                        break;
                    }
                    try
                    {
                        if (_serialPort.BytesToRead > 0)
                        {
                            string reader = _serialPort.ReadExisting();
                            _logger.Debug(reader);
                            string[] coords = new string[3];
                            coords =  reader.Split(';');
                            Vector3 v3 = new Vector3(float.Parse(coords[0]), float.Parse(coords[1]), float.Parse(coords[2]));
                            points.Add(v3);
                            _logger.Info(String.Format("Punto scansionato in => (x:{0}; y:{1}; z:{2})",v3.X,v3.Y,v3.Z));
                        }
                            
                    }
                    catch (InvalidOperationException ioe)
                    {
                        _logger.Error(ioe.Message);
                        return;
                    }

                }
            }, ct);
        }
        /// <summary>
        /// Scrive sulla seriale il valore di interruzione per l'invio di dati.
        /// </summary>
        private static void WriteSerial(CancellationToken ct)
        {
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(5000);
                try
                {
                    _serialPort.Write("-1");
                }
                catch (InvalidOperationException ioe)
                {
                    _logger.Error(ioe.Message);
                }
                Thread.Sleep(100);
                _logger.Debug("Sending stop");
                ts.Cancel();
            }, ct);
        }
    }
}
