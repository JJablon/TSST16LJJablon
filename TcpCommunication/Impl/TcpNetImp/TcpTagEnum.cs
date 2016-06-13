using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpCommunication.Impl.TcpNetImp.exceptions;

namespace TcpCommunication.Impl.TcpNetImp
{
    enum TcpTagEnum
    {
        START,END, FINALIZE
    }

    static class TcpTagEnumExtensions
    {
        private const string Start = "<!START!>";
        private const string End = "<!END!>";
        private const string Finalize = "<!FINALIZE!>";
        public static string GetToken(this TcpTagEnum tag)
        {
            switch (tag) {
                case TcpTagEnum.START:
                    {
                        return Start;
                    }
                case TcpTagEnum.END:
                    {
                        return End;
                    }
                case TcpTagEnum.FINALIZE:
                    {
                        return Finalize;
                    }
                default:
                    {
                        throw new TagUnknownException();
                    }
            }
            
        }

        public static TcpTagEnum GetTag(string type)
        {
            switch (type)
            {
                case Start:
                    {
                        return TcpTagEnum.START;
                    }
                case End:
                    {
                        return TcpTagEnum.END;
                    }
                case Finalize:
                    {
                        return TcpTagEnum.FINALIZE;
                    }
                default:
                    {
                        throw new TagUnknownException();
                    }
            }

        }
    }
}
