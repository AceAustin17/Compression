using ANeuralNetwork;
using System;
using System.Collections.Generic;
using System.Drawing;
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
        private CImage ci;
        public ImageCompress()
        {
            int[] layersizes = new int[4] { 1,3,2,3};
            ActivationFunction[] activFunctions = new ActivationFunction[4]{ ActivationFunction.None,ActivationFunction.Sigmoid, ActivationFunction.Sigmoid, ActivationFunction.Linear };


            XmlDocument xdoc = new XmlDocument();
            xdoc.Load("ann.xml");

            ds = new DataSet();
            ds.Load((XmlElement)xdoc.DocumentElement.ChildNodes[0]);


            bpnetwork = new BackPropNetwork(layersizes, activFunctions);
            nt = new NetworkTrainer(bpnetwork, ds);

            nt.maxError = 0.1;
            nt.maxiterations = 100;
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
        double[] tmpInput = new double[1];
        double[] tmpOutput = new double[3];

            ci = new CImage(norm._image.Height, norm._image.Width);
            ci._ColourList = norm._ColourList;
            ci._numArray = norm._numArray;
            //for (int x = 0; x < norm._image.Width; x++)
            //{
            //    for (int y = 0; y < norm._image.Height; y++)
            //    {
            //        if (x != norm._image.Width - 1 && y != norm._image.Height - 1)
            //        {
            //            tmpOutput[0] = norm._numArray[x + 1, y];
            //            tmpOutput[1] = norm._numArray[x, y + 1];
            //            tmpOutput[2] = norm._numArray[x + 1, y + 1];
            //            tmpInput[0] = norm._numArray[x, y];
            //            bpnetwork.run(ref tmpInput, out tmpOutput);

            //            double checkVal = Math.Round(tmpOutput[0], 3);

            //        }
            //    }
            //}
        }
        public BackPropNetwork _bpnetwwork
        {
            get
            {
                return bpnetwork;
            }
        }
        public CImage _ci
        {
            get
            {
                return this.ci;
            }
        }
    }
}
