using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compression_Year_Project
{
    class Normalise
    {
        private string[] linedata;
        private string[] worddata;
        private double[] inputData;
        public Normalise(string filePath)
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

        }

        private void saveToXML()
        {

        }
        private void popArray(string[] wordata)
        {
            inputData = new double[worddata.Length];
            List<double> randomList = new List<double>();

            for(int i = 0; i < worddata.Length;i++)
            {
                if (worddata[i].Equals(" "))
                {
                    inputData[i] = 0.0;
                }
                else
                {

                }
            }
        }
        private double GetRandomNumber(double minimum, double maximum)
        {
            Random random = new Random();
            return random.NextDouble() * (maximum - minimum) + minimum;
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

    }
}
