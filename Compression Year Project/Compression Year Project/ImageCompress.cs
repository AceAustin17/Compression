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
            int[] layersizes = new int[5] {1,3,3,2,1};
            ActivationFunction[] activFunctions = new ActivationFunction[5]{ ActivationFunction.None,ActivationFunction.Sigmoid,ActivationFunction.Sigmoid, ActivationFunction.Sigmoid, ActivationFunction.Linear };


            XmlDocument xdoc = new XmlDocument();
            xdoc.Load("ann.xml");

            ds = new DataSet();
            ds.Load((XmlElement)xdoc.DocumentElement.ChildNodes[0]);

            bpnetwork = new BackPropNetwork(layersizes, activFunctions);
            nt = new NetworkTrainer(bpnetwork, ds);

            nt.maxError = 0.1;
            nt.maxiterations = 100000;
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
            double[] tmpOutput = new double[1];
            List<CImage.posCol> pcList = new List<CImage.posCol>();
            
            ci = new CImage(norm._image.Width, norm._image.Height);
           
            bool checkloopdone;
            for (int y = 0; y < norm._image.Height; y++)
            {           
                checkloopdone = true;
                int num = 1;
                CImage.posCol pc = new CImage.posCol();
                    for (int x = 0; x < norm._image.Width; x++)
                    { 
                        tmpInput[0] = norm._numArray[x, y];
                        bpnetwork.run(ref tmpInput, out tmpOutput);
                        double checkVal = Math.Round(tmpOutput[0], 2);
                        bool one = false;
                        Color col = new Color();
                        foreach (KeyValuePair<Color, double> kv in norm._ColourList)
                        {
                            if ((kv.Value - checkVal >= -0.02) && (kv.Value - checkVal <= 0.02))
                            {
                                one = true;
                                col = kv.Key;
                                num++;
                                break;
                            }
                        }
                        if (checkloopdone)
                        {
                            if (one)
                            {
                                pc.x = x;
                                pc.y = y;
                                pc.col = col;
                                pc.num = num;
                            }
                        }
                        else if (one)
                        {
                            pc.num = num;
                        }
                        if(!one)
                        {
                            foreach (KeyValuePair<Color, double> kv in norm._ColourList)
                            {
                                if(kv.Value == tmpInput[0])
                                {
                                pc.col= kv.Key;
                                break;
                                }
                            }
                            pc.x = x;
                            pc.y = y;
                            pc.num = 1;
                            pcList.Add(pc);
                        }
                        one = false;
                        checkloopdone = false;                   
                }                
                    pcList.Add(pc);           
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
