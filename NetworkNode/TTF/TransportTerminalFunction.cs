using NetworkNode.SDHFrame;
using NetworkNode.HPC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkNode.TTF
{
    public enum NodeMode
    {
        REGENERATOR,
        MULTIPLEXER,
        CLIENT
    }
    public class FrameArgs : InputDataArgs
    {
        public IFrame Frame { get; set; }
        public FrameArgs(int inputPort, IFrame frame)
            : base(inputPort)
        {
            Frame = frame;
        }
    }

    public delegate void HandleInputFrame(object sender, FrameArgs args);
    public delegate void HandleOutputFrame(object sender, FrameArgs args);

    public delegate void LogInputFrame(FrameArgs args);
    public delegate void LogOutputFrame(FrameArgs args);

    public class TransportTerminalFunction
    {
        private SynchronousPhysicalInterface spi;        
        private RegeneratorSectionTermination rst;
        private MultiplexSectionTermination mst;

        private IFrameBuilder builder;
        private NodeMode nodeMode;

        public event HandleInputFrame HandleInputFrame;
        public event HandleOutputFrame HandleOutputFrame;

        public event LogInputFrame LogInputFrame;
        public event LogOutputFrame LogOutputFrame;
        public TransportTerminalFunction(SynchronousPhysicalInterface spi, NodeMode mode)
        {
            this.spi = spi;
            rst = new RegeneratorSectionTermination();
            this.nodeMode = mode;
            if (nodeMode == NodeMode.MULTIPLEXER)
            {
                mst = new MultiplexSectionTermination();
            }
            this.spi.HandleInputData += new HandleInputData(getInputData);
            builder = new FrameBuilder();
        }

        private void getInputData(object sender, InputDataArgs args)
        {
            string bufferedData = spi.GetBufferedData(args.PortNumber);

            IFrame result = beginFrameEvaluation(bufferedData, args.PortNumber);
            if (HandleInputFrame != null)
            {
                HandleInputFrame(this, new FrameArgs(args.PortNumber, result));
            }
        }

        private IFrame beginFrameEvaluation(string bufferedData, int port)
        {
            IFrame frame = builder.BuildFrame(bufferedData);

            if (LogInputFrame != null)
            {
                LogInputFrame(new FrameArgs(port, frame));
            }

            rst.evaluateHeader(frame);
            
            if (nodeMode == NodeMode.MULTIPLEXER)
            {
                mst.evaluateHeader(frame);
            }
            

            return frame;
        }

        private void raportFrame(IFrame frame, string raportHeader)
        {
            Console.WriteLine(raportHeader);
            Console.WriteLine(((Frame) frame).ToString());
            if(((Frame)frame).Rsoh != null) 
            {
                Console.WriteLine("RSOH = " + ((Frame)frame).Rsoh.ToString());
            }

            if (((Frame)frame).Msoh != null)
            {
                Console.WriteLine("MSOH = " + ((Frame)frame).Msoh.ToString());
            }
        }

        public Dictionary<int, StmLevel>  GetPorts()
        {
            return spi.GetPorts();
        }
        public void PassDataToInterfaces(Dictionary<int,IFrame> outputFrames)
        {
            foreach (int outputPort in outputFrames.Keys)
            {
                IFrame frame = outputFrames[outputPort];
                rst.generateHeader(frame);
                
                if (nodeMode == NodeMode.MULTIPLEXER)
                {
                    mst.generateHeader(frame);
                }

                FrameArgs eventArgs = new FrameArgs(outputPort, frame);
            
                if (LogOutputFrame != null)
                {
                    LogOutputFrame(eventArgs);
                }

                if (HandleOutputFrame != null)
                {
                    HandleOutputFrame(this, eventArgs);
                }

                String textForm = builder.BuildLiteral(frame);
                spi.SendFrame(textForm, outputPort);
            }
        }

        public bool ShudownInterface(int number)
        {
            return spi.ShudownInterface(number);
        }

        internal void AddRsohContent(string dccContent)
        {
            rst.SetNextData(dccContent);
        }

        internal void AddMsohContent(string dccContent)
        {
            mst.SetNextData(dccContent);
        }
    }
}
