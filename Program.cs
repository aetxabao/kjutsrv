using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace kjutsrv
{
    public class KjutServer
    {

        private static int TAM = 1024;
        private static int PORT = 11000;
        private static string data = null;
        private static int[] contadores = new int[3];

        public static void StartListening()
        {
            byte[] bytes = new Byte[TAM];

            IPAddress ipAddress = GetLocalIpAddress();
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, PORT);

            Socket listener = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(10);

                while (true)
                {
                    Console.WriteLine("Esperando conexiones en {0}:{1}...",
                                      ipAddress, PORT);
                    Socket handler = listener.Accept();
                    data = null;
                    int bytesRec = handler.Receive(bytes);
                    if (bytesRec > 0)
                    {
                        data += Encoding.ASCII.GetString(bytes, 0, bytesRec);

                        Console.WriteLine("{0} ----> {1}",
                            handler.RemoteEndPoint.ToString(), data);
                        Statistics(data);

                        byte[] msg = Encoding.ASCII.GetBytes(data); // Echo
                        handler.Send(msg);
                    }
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\nPulsa ENTER para terminar");
            Console.Read();

        }

        private static void Statistics(string data)
        {
            try
            {
                int idx = int.Parse(data);
                if (idx >= 0 && idx < 3)
                {
                    contadores[idx] += 1;
                }
                else
                {
                    Console.WriteLine("Otro NUM {0}", data);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Otro valor {0}", data);
            }
            for (int j = 0; j < 3; j++)
            {
                Console.Write($"[{j}]:{contadores[j]} ");
            }
            Console.WriteLine();
        }

        private static IPAddress GetLocalIpAddress()
        {
            List<IPAddress> ipAddressList = new List<IPAddress>();
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            int t = ipHostInfo.AddressList.Length;
            string ip;
            for (int i = 0; i < t; i++)
            {
                ip = ipHostInfo.AddressList[i].ToString();
                if (ip.Contains(".") && !ip.Equals("127.0.0.1"))
                {
                    ipAddressList.Add(ipHostInfo.AddressList[i]);
                    //Console.WriteLine($"+{ip}");
                }
                else
                {
                    //Console.WriteLine($"-{ip}");
                }
            }
            if (ipAddressList.Count == 1)
            {
                return ipAddressList[0];
            }
            else
            {
                int i = 0;
                foreach (IPAddress ipa in ipAddressList)
                {
                    Console.WriteLine($"[{i++}]: {ipa}");
                }
                t = ipAddressList.Count - 1;
                System.Console.Write($"Opción [0-{t}]: ");
                string s = Console.ReadLine();
                if (Int32.TryParse(s, out int j))
                {
                    if ((j >= 0) && (j <= t))
                    {
                        return ipAddressList[j];
                    }
                }
                return null;
            }
        }

        public static int Main(String[] args)
        {
            StartListening();
            return 0;
        }

    }
}