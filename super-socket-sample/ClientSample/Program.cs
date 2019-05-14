using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ClientSample
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                client.Connect("127.0.0.1", 30000);

                var stream = new MemoryStream();
                using (var bw = new BinaryWriter(stream))
                {
                    var body = Encoding.UTF8.GetBytes("Hello, SuperSocket!");

                    // 헤더가 4바이트
                    bw.Write(body.Length);

                    // 본문 스트링
                    bw.Write(body);
                }

                var buffer = stream.ToArray();

                while (true)
                {
                    // 블러킹 전송
                    var size = client.Send(buffer);

                    // 1초
                    Thread.Sleep(1000);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
