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
    class ImageCompress : Compress<NormaliseImage>
    {
        private BackPropNetwork bpnetwork;
        private NetworkTrainer nt;
        private DataSet ds;
        public ImageCompress()
        {
            int[] layersizes = new int[4] { 3,3,2,1};
            ActivationFunction[] activFunctions = new ActivationFunction[4]{ ActivationFunction.None,ActivationFunction.Sigmoid, ActivationFunction.Sigmoid, ActivationFunction.Linear };


            XmlDocument xdoc = new XmlDocument();
            xdoc.Load("ann.xml");

            ds = new DataSet();
            ds.Load((XmlElement)xdoc.DocumentElement.ChildNodes[0]);


            bpnetwork = new BackPropNetwork(layersizes, activFunctions);
            nt = new NetworkTrainer(bpnetwork, ds);

            nt.maxError = 0.1;
            nt.maxiterations = 10000;
            nt.nudgewindow = 500;
            nt.traininrate = 0.1;
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
        public override void compressFile(NormaliseImage norm)
        {
        }
    }
}
