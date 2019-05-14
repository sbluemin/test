using SuperSocket.SocketBase.Protocol;

namespace SuperSocketSample.Server
{
    public class RequestInfo : IRequestInfo
    {
        string m_key;
        public string m_message = null;

        public RequestInfo(string key, string msg)
        {
            m_key = key;
            m_message = msg;
        }

        public string Key
        {
            get
            {
                return m_key;
            }
        }
    }
}
