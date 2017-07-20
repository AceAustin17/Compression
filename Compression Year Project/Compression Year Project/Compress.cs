using ANeuralNetwork;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Compression_Year_Project
{
    class Compress
    {
        private BackPropNetwork bpnetwork;
        private NetworkTrainer nt;
        private DataSet ds;
        public Compress()
        {
            int[] layersizes = new int[6] { 1,5,4,3,2,1 };
            ActivationFunction[] activFunctions = new ActivationFunction[6]{ ActivationFunction.None,ActivationFunction.Gaussian,ActivationFunction.Sigmoid,ActivationFunction.Sigmoid,ActivationFunction.Sigmoid,
                ActivationFunction.Linear };
          

            XmlDocument xdoc = new XmlDocument();
            xdoc.Load("ann.xml");

            ds = new DataSet();
            ds.Load((XmlElement)xdoc.DocumentElement.ChildNodes[0]);


            bpnetwork = new BackPropNetwork(layersizes, activFunctions);
            nt = new NetworkTrainer(bpnetwork, ds);

            nt.maxError = 0.1;
            nt.maxiterations = 1000000;
            nt.traininrate = 0.3;
            nt.TrainDataset();

            // save error
            double[] err = nt.geteHistory();
            string[] filedata = new string[err.Length];

            for (int i = 0; i < err.Length; i++)
            {
                filedata[i] = i.ToString() + " " + err[i].ToString();
            }

            File.WriteAllLines("../xornetwrk.txt", filedata);
        }
        public void compressFile(Normalise norm)
        {
            for (int i = 0; i < norm._inputData.Length; i++)
            {
                double[] tmpInput = new double[1];
                double[] tmpOutput = new double[1];

                tmpInput[0] = norm._inputData[i];
                bpnetwork.run(ref tmpInput, out tmpOutput);

                double checkVal = Math.Round(tmpOutput[0], 3);
                
                foreach (KeyValuePair<string, double> kv in norm._map)
                {
                    if ((kv.Value - checkVal >= -0.5) && (kv.Value - checkVal <= 0.5))
                    {
                      
                    }
                }
                
            }
        }
        public BackPropNetwork _bpnetwwork
        {
            get
            {
                return bpnetwork;
            }
        }

    }
}
