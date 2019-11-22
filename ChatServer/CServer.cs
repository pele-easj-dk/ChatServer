using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ChatServer
{
    public class CServer
    {
        private static readonly BlockingCollection<String> commondata = new BlockingCollection<String>();
        private static readonly LinkedList<TcpClient> liste = new LinkedList<TcpClient>();
        private static readonly ConcurrentBag<TcpClient> removeList = new ConcurrentBag<TcpClient>();
        public void Start()
        {
            TcpListener server = new TcpListener(IPAddress.Any, 7777);
            server.Start();

            Task.Run(() => DoServer(commondata, liste, removeList));

            while (true)
            {
                TcpClient sock = server.AcceptTcpClient();
                liste.AddLast(sock);
                Task.Run(
                    () =>
                    {
                        TcpClient tmp = sock;
                        DoClient(tmp, commondata, liste, removeList);
                    }
                );

            }
            

        }




        // internal
        void DoClient(TcpClient socket, BlockingCollection<String> comm, LinkedList<TcpClient> allClients, ConcurrentBag<TcpClient> removeList)
        {
            StreamReader sr = new StreamReader(socket.GetStream());
            try
            {
                // indsat kommentar
                while (true) // ToDo  stop at STOP string
                {
                    String str = sr.ReadLine();
                    comm.Add(str);
                }
            }
            catch (Exception ex)
            {
                // socket close - remove from list
                removeList.Add(socket);

            }
        }

        void DoServer(BlockingCollection<String> comm, LinkedList<TcpClient> allClients, ConcurrentBag<TcpClient> removeList)
        {
            while (true)
            {
                String str = comm.Take();
                foreach (TcpClient client in allClients)
                {
                    try
                    {
                        StreamWriter sw = new StreamWriter(client.GetStream());
                        sw.WriteLine(str);
                        sw.Flush();
                    }
                    catch (Exception) { }
                }

                foreach (TcpClient client in removeList)
                {
                    allClients.Remove(client);
                }
            }
        }
    }
}
