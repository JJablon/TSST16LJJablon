using NetworkNode.MenagmentModule;
using NetworkNode.SDHFrame;
using NetworkNode.TTF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkNode.ConsoleLogger
{
    public class FrameLogger
    {
        private static string reportBorder = "===============================================================";
        private static string reportSeparator = "---------------------------------------------------------------";
        private string Name;
        public FrameLogger (ManagementCenter menagment, TransportTerminalFunction ttf, string name) 
        {
            ttf.LogInputFrame += new LogInputFrame(LogInputFrame);
            ttf.LogOutputFrame += new LogOutputFrame(LogOutputFrame);
            Name = name;
        }

        private void LogOutputFrame(FrameArgs args)
        {
            LogBorder();
            Console.WriteLine("Node : " + Name);
            Console.WriteLine("Output frame at: " + args.PortNumber);
            LogSeparator();
            LogFrame(args.Frame);
            LogBorder();
        }

        private void LogInputFrame(FrameArgs args)
        {
            LogBorder();
            Console.WriteLine("Node : "+Name);
            Console.WriteLine("Input frame at: " + args.PortNumber);
            LogSeparator();
            LogFrame(args.Frame);
            LogBorder();
        }

        private void LogFrame(IFrame iframe)
        {
            Frame frame = (Frame)iframe;

            LogMsoh(frame.Msoh);
            LogSeparator();
            
            LogRsoh(frame.Rsoh);
            LogSeparator();

            LogContent(frame.Content);
        }

        private void LogContent(List<IContent> content)
        {
            foreach (IContent item in content)
            {
                if (item == null)
                {
                    Console.WriteLine("[AU4] = P + Higher Order Path: [VC4]");
                }
                else if (VirtualContainer.isVirtualContainer(item))
                {
                    LogHigherOrder((VirtualContainer)item);
                }
                
            }
        }

        private void LogHigherOrder(VirtualContainer item)
        {
            Console.WriteLine("[AU4] = P + Higher Order Path: [VC4]");
            if (item.Type.Equals(ContentType.VICONTAINER))
            {
                foreach (IContent lowerContainer in item.Content)
                {
                    if (lowerContainer != null)
                    {
                        int index = item.Content.IndexOf(lowerContainer);
                        LogLowerOrder(lowerContainer, index);
                    }
                }
            }
        }

        private void LogLowerOrder(IContent content, int index)
        {
            VirtualContainer item = content as VirtualContainer;
            
            if (item == null)
            {
                Console.WriteLine("\t POH + C4 - CONTENT");
                return;
            }

            Console.WriteLine("\tLower Order Path: ");
            string container = item.Level.ToString();
            container = container.Substring(1, container.Length - 1);
            Console.Write("\t\t[" + item.Level + "] = POH + " + container + " at index : ");
            int sdhIndex = VirtualContainer.GetLowLevelSdhIndex(index, item.Level);
            Console.WriteLine(sdhIndex);
        }

        private void LogMsoh(Header msoh)
        {
            Console.WriteLine("MSOH:");
            LogHeader(msoh);
        }

        private void LogRsoh(Header rsoh)
        {
            Console.WriteLine("RSOH:");
            LogHeader(rsoh);
        }

        private void LogHeader(Header header) {
            Console.WriteLine("\t Data access channel:");
            Console.Write(" " + header.DCC);
            Console.WriteLine("\t Engineering Orderwire Serivce:");
            Console.Write(" " + header.EOW);
            Console.WriteLine("\t Parity:");
            Console.Write(" " + header.Checksum);
        }

        private void LogSeparator()
        {
            Console.WriteLine();
            Console.WriteLine(reportSeparator);
        }
        private void LogBorder()
        {
            Console.WriteLine();
            Console.WriteLine(reportBorder);
        }
    }
}
