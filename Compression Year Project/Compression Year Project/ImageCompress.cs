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
        private int iter = 1;
       
        public ImageCompress()
        { 
            int[] layersizes = new int[4] { 1,3,2,2};
            ActivationFunction[] activFunctions = new ActivationFunction[4]{ ActivationFunction.None,ActivationFunction.Sigmoid, ActivationFunction.Sigmoid, ActivationFunction.Linear };


            XmlDocument xdoc = new XmlDocument();
            xdoc.Load("ann.xml");

            ds = new DataSet();
            ds.Load((XmlElement)xdoc.DocumentElement.ChildNodes[0]);


            bpnetwork = new BackPropNetwork(layersizes, activFunctions);
            nt = new NetworkTrainer(bpnetwork, ds);

            nt.maxError = 0.1;
            nt.maxiterations = 100;
            nt.nudgewindow = 50;
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
            double[] tmpOutput = new double[2];
           List<CImage.posCol> pcList = new List<CImage.posCol>();
            CImage.posCol pc = new CImage.posCol();
            ci = new CImage(norm._image.Width, norm._image.Height);
            ci._ColourList = norm._ColourList;
            bool first = true;
            bool second = true;
            bool third = false;
            for (int x = 0; x < norm._image.Width; x++)
            {
                for (int y = 0; y < norm._image.Height; y++)
                  {
                    if (x != norm._image.Width - 1 && y != norm._image.Height - 1)
                    {
                        tmpInput[0] = norm._numArray[x, y];
                        bpnetwork.run(ref tmpInput, out tmpOutput);

                        double checkVal = Math.Round(tmpOutput[0], 2);
                        if(first)
                        {
                          ci._numarray[x, y] = norm._numArray[x, y];
                        }                        
                        //foreach (KeyValuePair<Color, double> kv in norm._ColourList)
                        //{
                        //    if ((kv.Value - checkVal >= -0.03) && (kv.Value - checkVal <= 0.03))
                        //    {
                        //        if (second)
                        //        {
                        //            pc.x = x;
                        //            pc.y = y;
                        //            pc.num = iter;
                        //            iter++;
                        //            first = false;
                        //            second = false;
                        //            third = true;
                        //            break;
                        //        }
                        //        else
                        //        {
                        //            pc.num = iter;
                        //            iter++;
                        //            first = false;
                        //            break;
                        //        }
                        //    }
                            
                        //}
                        //if (third)
                        //{
                        //    pcList.Add(pc);
                        //    second = true;
                        //    first = true;
                        //    third = false;
                        //}
                    }
                    else
                    {
                        ci._numarray[x, y] = norm._numArray[x, y];
                    }
                }
            }
            ci._PosList = pcList.ToArray();
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
