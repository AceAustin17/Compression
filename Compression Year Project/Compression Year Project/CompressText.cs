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
    class CompressText : Compress<NormaliseText>
    {
        private BackPropNetwork bpnetwork;
        private NetworkTrainer nt;
        private DataSet ds;
        private string compressedString;
        private string legend;
        private Random r = new Random();
        private List<string> ranVariable = new List<string>();
        private List <KeyValuePair<string, string>> legendlist = new List<KeyValuePair<string, string>>();
        public CompressText()
        {
            int[] layersizes = new int[10] { 1,10,9,8,7,5,4,3,2,1 };
            ActivationFunction[] activFunctions = new ActivationFunction[10]{ ActivationFunction.None,ActivationFunction.Gaussian,ActivationFunction.Sigmoid,ActivationFunction.Sigmoid,ActivationFunction.Sigmoid,ActivationFunction.Sigmoid,ActivationFunction.Sigmoid,ActivationFunction.Sigmoid,ActivationFunction.Sigmoid,
                ActivationFunction.Linear };
          

            XmlDocument xdoc = new XmlDocument();
            xdoc.Load("ann.xml");

            ds = new DataSet();
            ds.Load((XmlElement)xdoc.DocumentElement.ChildNodes[0]);


            bpnetwork = new BackPropNetwork(layersizes, activFunctions);
            nt = new NetworkTrainer(bpnetwork, ds);

            nt.maxError = 0.1;
            nt.maxiterations = 10000;
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
        public override void compressFile(NormaliseText norm)
        {
            List<int> indices = new List<int>();
            bool[] indexarray = new bool[norm._map.Count];
            for (int t = 0; t < indexarray.Length; t++)
            {
                indexarray[t] = false;
            }

            for (int i = 0; i < norm._inputData.Length; i++)
            {
                double[] tmpInput = new double[1];
                double[] tmpOutput = new double[1];

                tmpInput[0] = norm._inputData[i];
                bpnetwork.run(ref tmpInput, out tmpOutput);

                double checkVal = Math.Round(tmpOutput[0], 2);
                
                int check = 0;
                foreach (KeyValuePair<string, double> kv in norm._map)
                {
                    if ((kv.Value - checkVal >= -0.03) && (kv.Value - checkVal <= 0.03))
                    {
                        indexarray[check] = true;
                    }
                    check++;

                }
            }
            for(int b =0; b  < indexarray.Length; b++)
            {
                if (indexarray[b])
                {
                    double val = norm._map[b].Value;
                    string word = norm._map[b].Key;
                    string var = RandomString(2);
                    while (ranVariable.Contains(var))
                    {
                        var = RandomString(2);
                    }
                    legendlist.Add(new KeyValuePair<string, string>(word, var));
                    norm._map[b] = new KeyValuePair<string, double>(var, val);
                    ranVariable.Add(var);
                }
            }
            foreach (KeyValuePair<string, double> kv in norm._map)
            {
                for (int j = 0; j < norm._worddata.Length; j++)
                {
                    if (norm._inputData[j] == kv.Value)
                    {
                        norm._worddata[j] = kv.Key;
                    }
                }
            }     
            foreach(KeyValuePair<string,string> lgd in legendlist)
            {
                legend += lgd.ToString() + "\n";
            }
            for (int k = 0; k < norm._worddata.Length; k++)
            {
                compressedString += norm._worddata[k];
                compressedString += " ";
            }

            compressedString += "\n" + "\n";
            compressedString += legend;
        }
        public string _compressedString
        {
            get
            {
                return this.compressedString;
            }
        }
        public BackPropNetwork _bpnetwwork
        {
            get
            {
                return bpnetwork;
            }
        }

        // Code adapted from https://stackoverflow.com/questions/9995839/how-to-make-random-string-of-numbers-and-letters-with-a-length-of-5
        private string RandomString(int Size)
        {
            string input = "abcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder builder = new StringBuilder();
            char ch;
            for (int i = 0; i < Size; i++)
            {
                ch = input[r.Next(0, input.Length)];
                builder.Append(ch);
            }

           return  builder.ToString();
        }

    }
}
