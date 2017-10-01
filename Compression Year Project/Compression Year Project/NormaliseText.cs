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
    class NormaliseText : Normalise
    {
        private string[] linedata;
        private string[] worddata;
        private double[] inputData;
        private double[] outputData;
        private Random random = new Random();
        private List<KeyValuePair<string, double>> map = new List<KeyValuePair<string, double>>();
        public NormaliseText(string filePath)
        {
            using (StreamReader sr = new StreamReader(filePath))
            {
                char[] delimiterChars = { ' ', ',', '.', ';','\t',':'};
                while (sr.Peek() >= 0)
                {
                    string line = sr.ReadToEnd();
                    linedata = line.Split('\n');
                    worddata = line.Split(delimiterChars);
                }
            }
            popArray(worddata);
            outputData = new double[inputData.Length];
            for(int i = 0; i< outputData.Length;i++)
            {
                if(i == outputData.Length - 1)
                {
                    outputData[i] = 0.0;
                }
                {                     
                        outputData[i] = inputData[i + 1];
                }
                
            }
        }

        public override void saveToXML()
        {
            DataPoint[] d = new DataPoint[inputData.Length];
            DataSet DS = new DataSet();
            double[] tmpInput = new double[1];
            double[] tmpOutput = new double[1];

            for (int i = 0; i < outputData.Length; i++)
            {
                //if(i== outputData.Length-1)
                //{
                //    tmpInput[0] = inputData[i];
                //    tmpInput[1] = 0.0;
                //    tmpOutput[0] = outputData[i];
                //    d[i] = new DataPoint(tmpInput, tmpOutput);
                //    DS.data.Add(d[i]);
                //}
                //else
                //{
                if (inputData[i] != 0.0 && outputData[i] !=0.0)
                {
                    tmpInput[0] = inputData[i];
                    // tmpInput[1] = inputData[i + 1];
                    tmpOutput[0] = outputData[i];
                    d[i] = new DataPoint(tmpInput, tmpOutput);
                    DS.data.Add(d[i]);
                }
               // }
                
            }

            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<Root/>");

            doc.DocumentElement.AppendChild(DS.toXml(doc));

            doc.Save("./ann.xml");
        }
        private void popArray(string[] worData)
        {
            inputData = new double[worData.Length];
            List<double> randomlist = new List<double>();
            for(int i = 0; i < worData.Length;i++)
              {
                if (String.IsNullOrWhiteSpace(worData[i]))
                {
                    inputData[i] = 0.0;
                }
                else
                {
                    if (this.map.Any())
                    {
                        bool check = true;
                        foreach (KeyValuePair<string, double> kv in this.map)
                        {
                                if (worData[i].CompareTo(kv.Key) == 0)
                                {
                                    inputData[i] = kv.Value;
                                    check = false;
                                    break;
                                }                         
                        }
                        if (check)
                        {
                            double tmp = Math.Round(GetRandomNumber(0,1),3);
                            while (randomlist.Contains(tmp))
                            {
                                tmp = Math.Round(GetRandomNumber(0,1), 3);
                            }
                            randomlist.Add(tmp);
                            inputData[i] = tmp;
                            KeyValuePair<string, double> tmpKV = new KeyValuePair<string, double>(worData[i], inputData[i]);
                            map.Add(tmpKV);
                        }
                    }
                    else
                    {
                        double tmp = Math.Round(GetRandomNumber(0,1),3);
                        randomlist.Add(tmp);
                        inputData[i] = tmp;
                        KeyValuePair<string, double> tmpKV = new KeyValuePair<string, double>(worData[i], inputData[i]);
                        map.Add(tmpKV);
                    }
                }
            }
    }
    private double GetRandomNumber(double minimum, double maximum)
        {
            
            return random.NextDouble() * (maximum - minimum) + minimum;
        }

        public List<KeyValuePair<string,double>> _map
        {
            get
            {
                return this.map;                  
            }
        }
        public double[] _inputData
        {
            get
            {
                return this.inputData;
            }
            set
            {
                this.inputData = value;
            }
        }
        public string[] _worddata
        {
            get
            {
                return this.worddata;
            }
            set
            {
                this.worddata = value;
            }
        }
        public string[] _linedata
        {
            get
            {
                return this.linedata;
            }
            set
            {
                this.linedata = value;
            }
         }
        public double[] _outputdata
        {
            get
            {
                return this.outputData;
            }
            set
            {
                this.outputData = value;
            }
        }

    }
}
