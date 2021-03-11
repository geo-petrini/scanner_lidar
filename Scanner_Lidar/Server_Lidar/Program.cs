using NLog;
using Server_Lidar.Classes;
using System;

namespace Server_Lidar
{
    class Program
    {
        private static int SERVER_PORT;
        private static Logger myLogger = LogManager.GetCurrentClassLogger();
        static void Main(string[] args)
        {
            Console.Title = "Server";
            Console.Write("Inserire il valore della porta: ");
            SERVER_PORT = int.Parse(Console.ReadLine());
            Server server = new Server(SERVER_PORT);
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
            Console.WriteLine("\n Press any key to continue...");
            Console.ReadKey();
        }
    }
}
