using SuperSocket.SocketBase.Config;
using System;
using System.Threading;

namespace SuperSocketSample
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new Server.Server();

            var rootConfig = new RootConfig();
            var serverConfig = new ServerConfig();
            serverConfig.Port = 30000;
            serverConfig.Ip = "0.0.0.0";

            // #NEED_CHECK
            // 제일 중요함!
            // 이 값이 RequestInfo의 총 데이터 사이즈보다 작으면 데이터 전송 안됨
            serverConfig.MaxRequestLength = 1024 * 4;
            serverConfig.ReceiveBufferSize = 1024 * 4;
            serverConfig.SendBufferSize = 1024 * 4;
            
            if (server.Setup(rootConfig, serverConfig) == false)
            {
                throw new Exception("");
            }

            if (server.Start() == false)
            {
                throw new Exception("");
            }

            while(true)
            {
                Thread.Sleep(1);
            }
        }
    }
}
