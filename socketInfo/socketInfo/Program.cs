using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;
namespace socketInfo
{
    class Program
    {
        private static byte[] result = new byte[1024];
        private static int myProt = 9100;   //端口  
        static Socket serverSocket;
        static void Main(string[] args)
        {
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(ip, myProt));  //绑定IP地址：端口  
            serverSocket.Listen(10);    //设定最多10个排队连接请求  
            Console.WriteLine("启动监听{0}成功", serverSocket.LocalEndPoint.ToString());
            //通过Clientsoket发送数据  
            Thread myThread = new Thread(ListenClientConnect);
            myThread.Start();
            Console.ReadLine();
        }
        /// <summary>  
        /// 监听客户端连接  
        /// </summary>  
        private static void ListenClientConnect()
        {
            while (true)
            {
                Socket clientSocket = serverSocket.Accept();
                clientSocket.Send(Encoding.ASCII.GetBytes("Server Say Hello"));
                Thread receiveThread = new Thread(ReceiveMessage);
                receiveThread.Start(clientSocket);
            }
        }
        
        /// <summary>  
        /// 接收消息  
        /// </summary>  
        /// <param name="clientSocket"></param>  
        private static void ReceiveMessage(object clientSocket)
        {//markem
            Console.WriteLine("{0}客户端连接上", System.DateTime.Now);
            byte[] finishflag = new byte[18] { 0x02, 0x00, 0x7e, 0x00, 0x44, 0x00, 0x56, 0x00, 0x30, 0x00, 0x7c, 0x00, 0x30, 0x00, 0x7c, 0x00, 0x03, 0x00 };
            Socket myClientSocket = (Socket)clientSocket;
            string data = "sjc";
            int i = 0;
            int receiveNumber = 0;
            receiveNumber = myClientSocket.Receive(result);
            myClientSocket.Send(System.Text.Encoding.ASCII.GetBytes(data)); ;
            Console.WriteLine("接收客户端{0}初始消息{1}", myClientSocket.RemoteEndPoint.ToString(), Encoding.ASCII.GetString(result, 0, receiveNumber));
            while (true)
            {
                try
                {
                    //通过clientSocket接收数据  
                     receiveNumber = myClientSocket.Receive(result);
                    if (receiveNumber == 0) {
                        myClientSocket.Shutdown(SocketShutdown.Both);
                        myClientSocket.Close();
                        Console.WriteLine("客户端关闭{0},总共溯源码个数:{1}", System.DateTime.Now, i);
                        i = 0;
                        break;
                    }
                    i++;

                    myClientSocket.Send(System.Text.Encoding.ASCII.GetBytes(data) ); ;
                    Console.WriteLine("{0},第{1}次",System.DateTime.Now,i);
                    Console.WriteLine("接收客户端{0}消息{1}", myClientSocket.RemoteEndPoint.ToString(), Encoding.UTF8.GetString(result, 0, receiveNumber));
                    receiveNumber = myClientSocket.Receive(result);
                    if (receiveNumber == 0)
                    {
                        myClientSocket.Shutdown(SocketShutdown.Both);
                        myClientSocket.Close();
                        
                        Console.WriteLine("客户端关闭{0},总共溯源码个数:{1}", System.DateTime.Now, i);
                        i = 0;
                        break;
                    }
                    myClientSocket.Send(finishflag); 
                    Console.WriteLine("{0},第{1}次", System.DateTime.Now, i);
                    Console.WriteLine("接收客户端{0}消息{1}", myClientSocket.RemoteEndPoint.ToString(), Encoding.UTF8.GetString(result, 0, receiveNumber));
                    Thread.Sleep(500);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    myClientSocket.Shutdown(SocketShutdown.Both);
                    myClientSocket.Close();
                    break;
                }
            }
        }
    }
}
