using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ANeuralNetwork
{
    public class DataPoint
    {
        public DataPoint() { }
        public DataPoint(double[] input, double[] output) { Load(input, output); }

        public DataPoint(XmlElement xelem) { Load(xelem); }

        public double[] input;
        public double[] output;

        public int inputSize { get { return input.Length; } }
        public int outputSize { get { return output.Length; } }   
        public void Load(double[] input, double[] output)
        {
            this.input = new double[input.Length];
            this.output = new double[output.Length];
            Array.Copy(input, this.input, input.Length);
            Array.Copy(output, this.output, output.Length);
        }

        public void Load(XmlElement xElem)
        {
            XmlNode nType;
            int lIn, lOut, i;

            nType = xElem.SelectSingleNode("Input");

            lIn = nType.ChildNodes.Count;

            input = new double[lIn];

            foreach (XmlNode node in nType.ChildNodes)
            {
                XmlElement enode = (XmlElement)node;
                double val;

                int.TryParse(enode.GetAttribute("Index"), out i);
                double.TryParse(enode.InnerText, out val);

                input[i] = val;
            }

            nType = xElem.SelectSingleNode("Output");

            lOut = nType.ChildNodes.Count;

            output = new double[lOut];

            foreach (XmlNode node in nType.ChildNodes)
            {
                XmlElement enode = (XmlElement)node;
                double val;

                int.TryParse(enode.GetAttribute("Index"), out i);
                double.TryParse(enode.InnerText, out val);

                output[i] = val;
            }
        }

        public XmlElement toXml(XmlDocument xdoc)
        {
            XmlElement nDatapoint, ntype, node;

            int lIn = input.Length;
            int lOut = output.Length;

            nDatapoint = xdoc.CreateElement("Datapoint");
            ntype = xdoc.CreateElement("Input");

            for (int i = 0; i < lIn; i++)
            {
                node = xdoc.CreateElement("Data");
                node.SetAttribute("Index", i.ToString());
                node.AppendChild(xdoc.CreateTextNode(input[i].ToString()));
                ntype.AppendChild(node);
            }

            nDatapoint.AppendChild(ntype);

            ntype = xdoc.CreateElement("Output");

            for (int i = 0; i < lOut; i++)
            {
                node = xdoc.CreateElement("Data");
                node.SetAttribute("Index", i.ToString());
                node.AppendChild(xdoc.CreateTextNode(output[i].ToString()));
                ntype.AppendChild(node);
            }

            nDatapoint.AppendChild(ntype);

            return nDatapoint;
        }
    }

    public class DataSet
    {
        public List<DataPoint> data;
        public DataSet() { data = new List<DataPoint>(); }

        public XmlElement toXml(XmlDocument doc)
        {
            XmlElement nDataset;

            nDataset = doc.CreateElement("DataSet");

            foreach(DataPoint d in data)
            {
                nDataset.AppendChild(d.toXml(doc));

            }

            return nDataset;    
        }

        public void Load(XmlElement dataset)
        {
            foreach(XmlNode node in dataset.ChildNodes)
            {
                DataPoint dp = new DataPoint((XmlElement)node);
                data.Add(dp);
            }
        }

        public int Size
        {
            get
            {
               return  data.Count;
            }
        }
    }

    public class Permutator
    {
        public Permutator(int size)
        {
            indices = new int[size];

            for(int i = 0; i < size; i++)
            {
                indices[i] = i;
            }
            permute(size);
        }
        private int[] indices;
        private Random ran = new Random();

        public void permute(int times)
        {
            int i, j, t;
            for(int n= 0;n < times;n++)
            {
                i = ran.Next(indices.Length);
                j = ran.Next(indices.Length);

                if(i != j)
                {
                    t = indices[i];
                    indices[i] = indices[j];
                    indices[j] = t;
                }
            }
        }

        public  int this[int i]
        {
            get
            {
                return indices[i];
            }
        }
    }

    public class SimpNetworkTrainer
    {
        private Permutator idx;
        private int iterations;
        private double error;

        public double maxError = 0.1, maxiterations = 1000, traininrate = 0.1 , momentum = 0.5;

        public int nudgewindow = 500;
        public double nudscale = 0.25, nudtolerance = 0.0001;

        public BackPropNetwork bpnetwork;
        public DataSet dataset;

        private List<double> errorhistory;

        public SimpNetworkTrainer( BackPropNetwork BPN, DataSet ds)
        {
            bpnetwork = BPN;
            dataset = ds;
            idx = new Permutator(dataset.Size);
            iterations = 0;

            errorhistory = new List<double>();
        }

        public void trainDataSet()
        {
            do
            {
                iterations++;
                error = 0.0;
                idx.permute(dataset.Size);

                for(int i = 0; i < dataset.Size; i++)
                {
                    error += bpnetwork.train(ref dataset.data[idx[i]].input, ref dataset.data[idx[i]].output, traininrate, momentum);

                }

                errorhistory.Add(error);

                if( iterations % nudgewindow == 0)
                {
                  CheckNudge();
                }
            } while (error > maxError && iterations < maxiterations);

        }

        public double[] geteHistory()
        {
            return errorhistory.ToArray();
        }

        private void CheckNudge()
        {
            double oldAvg = 0f, newAvg=0f ;
            int errLen = errorhistory.Count;

            if (iterations < 2 * nudgewindow)
            {
                return;
            }

            for(int i =0; i < nudgewindow; i++)
            {
                oldAvg += errorhistory[errLen - 2 * nudgewindow + i];
                newAvg += errorhistory[errLen - nudgewindow + i];
            }

            oldAvg /= nudgewindow;
            newAvg /= nudgewindow;

            if(((double)Math.Abs(newAvg - oldAvg)) / nudgewindow < nudtolerance)
            {
                bpnetwork.Nudge(nudscale);
            }
        }
    }

    public class NetworkTrainer
    {
        private Permutator idx;
        private int iterations;
        private double error;

        public double maxError = 0.1, maxiterations = 1000, traininrate = 0.1, momentum = 0.5;

        public bool nudge = true;
        public int nudgewindow = 500;
        public double nudscale = 0.25, nudtolerance = 0.0001;

        public BackPropNetwork bpnetwork;
        public DataSet dataset;

        private List<double> errorhistory;

        public NetworkTrainer(BackPropNetwork BPN, DataSet ds)
        {
            bpnetwork = BPN;
            dataset = ds;
            init();
        }

        public void init()
        {
           
            iterations = 0;
            if(idx == null)
            {
                idx = new Permutator(dataset.Size);
            }
            else
            {
                idx.permute(dataset.Size);
            }
            if (errorhistory == null)
            {
                errorhistory = new List<double>();
            }
            else
            {
                errorhistory.Clear();
            }
        }

        public bool TrainDataset()
        {
            bool success = true;

            if (success)
            {
                success = _beforetraindataset();
            }
            if (success)
            {
                success = _trainDatasetAction();
            }
            if (success)
            {
                success = _Aftertraindataset();
            }

            return success;
        }

        protected virtual bool beforetraindataset() { return true; }
        protected virtual bool Aftertraindataset() { return true; }
        protected virtual bool beforetrainEpoch() { return true; }
        protected virtual bool AftertrainEpoch() { return true; }

        protected virtual bool BeforetrainDatapoint(ref double[] Input, ref double[] Output) { return true; }

        protected virtual bool AftertrainDatapoint(ref double[] Input, ref double[] Output) { return true; }

        private bool _beforetraindataset()
        {
            init();
            return beforetraindataset();
        }
        private bool _trainDatasetAction()
        {
            bool success = true;
            do
            {
             
                if(success)
                {
                    success = _BeforetrainEpoch();
                }
                if(success)
                {
                    success = _TrainepochAction();
                }
                if(success)
                {
                    success = _AfterTrainepoch();
                }
            } while (error > maxError && iterations < maxiterations && success);


            return success;
        }
        private bool _Aftertraindataset()
        {
            return Aftertraindataset();
        }

        private bool _BeforetrainEpoch()
        {
            iterations++;
            error = 0.0;
            idx.permute(dataset.Size);
            return beforetrainEpoch();
        }

        private bool _TrainepochAction()
        {

            bool success = true;
            for (int i = 0; i < dataset.Size && success; i++)
            {

                double[] input = new double[dataset.data[idx[i]].inputSize];
                double[] output = new double[dataset.data[idx[i]].outputSize];

                Array.Copy(dataset.data[idx[i]].input, input, input.Length);
                Array.Copy(dataset.data[idx[i]].output, output, output.Length);

                if (success)
                {
                 success = BeforetrainDatapoint(ref input, ref output);
                }

                error += bpnetwork.train(ref input, ref output, traininrate, momentum);

                if (success)
                {
                    AftertrainDatapoint(ref input, ref output);
                }

            }
            return success;
        }

        private bool _AfterTrainepoch()
        {
            errorhistory.Add(error);

            if (iterations % nudgewindow == 0 && nudge)
            {
                CheckNudge();
            }
            return AftertrainEpoch();
        }
        public double[] geteHistory()
        {
            return errorhistory.ToArray();
        }

        private void CheckNudge()
        {
            double oldAvg = 0f, newAvg = 0f;
            int errLen = errorhistory.Count;

            if (iterations < 2 * nudgewindow)
            {
                return;
            }

            for (int i = 0; i < nudgewindow; i++)
            {
                oldAvg += errorhistory[errLen - 2 * nudgewindow + i];
                newAvg += errorhistory[errLen - nudgewindow + i];
            }

            oldAvg /= nudgewindow;
            newAvg /= nudgewindow;

            if (((double)Math.Abs(newAvg - oldAvg)) / nudgewindow < nudtolerance)
            {
                bpnetwork.Nudge(nudscale);
            }
        }
    }
}
