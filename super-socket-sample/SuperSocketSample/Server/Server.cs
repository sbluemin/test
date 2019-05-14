using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using System;

namespace SuperSocketSample.Server
{
    public class Server : AppServer<Session, RequestInfo>
    {
        public Server() : base(new DefaultReceiveFilterFactory<ReceiveFilter, RequestInfo>()) {}

        protected override void OnNewSessionConnected(Session session)
        {
            // 클라이언트 접속시 이곳 콜백

            Console.WriteLine("클라이언트 접속 " + session.SessionID);
        }

        protected override void OnSessionClosed(Session session, CloseReason value)
        {
            // 클라이언트 종료시 이곳 콜백

            Console.WriteLine("클라이언트 종료 " + session.SessionID);
        }

        protected override void ExecuteCommand(Session session, RequestInfo requestInfo)
        {
            // 각 클라이언트로부터 받은 메시지가 있을때 이곳 콜백
            // (ReceiveFilter를 거치고 온다.)

            Console.WriteLine(session.SessionID + ": " + requestInfo.m_message);
        }
    }
}
