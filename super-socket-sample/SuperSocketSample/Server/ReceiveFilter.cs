using SuperSocket.Facility.Protocol;
using System;
using System.Text;

namespace SuperSocketSample.Server
{
    class ReceiveFilter : FixedHeaderReceiveFilter<RequestInfo>
    {
        static int i = 0;

        public ReceiveFilter() : base(Config.HeaderSize) {}

        protected override int GetBodyLengthFromHeader(byte[] header, int offset, int length)
        {
            var i = BitConverter.ToInt32(header, offset);
            return i;
        }

        protected override RequestInfo ResolveRequestInfo(ArraySegment<byte> header, byte[] bodyBuffer, int offset, int length)
        {
            if(length <= 0)
            {
                return NullRequestInfo;
            }

            var msg = Encoding.UTF8.GetString(bodyBuffer, offset, length);

            return new RequestInfo(BitConverter.ToString(bodyBuffer, offset, Config.HeaderSize), msg);
        }
    }
}
