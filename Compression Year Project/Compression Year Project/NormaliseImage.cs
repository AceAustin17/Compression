using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using ANeuralNetwork;
using System.Xml;

namespace Compression_Year_Project
{
    class NormaliseImage : Normalise
    {
        private double[,] numArray;
        private BinaryData binaryD = new BinaryData(8);
        private List<double> randDoubles = new List<double>();
        private List<KeyValuePair<Color, double>> ColourList = new List<KeyValuePair<Color, double>>();
        private List<KeyValuePair<double, double[]>> dataList = new List<KeyValuePair<double, double[]>>();
        Random random = new Random();
        private Bitmap image;

        public NormaliseImage(string path)
        {
            image = (Bitmap)Image.FromFile(path);
            numArray = new double[image.Width,image.Height];
            createArray();

        }
        private void createArray()
        {
            for (int x = 0; x < image.Width; x++)
            {
                for(int y = 0; y < image.Height;y++)
                {
                    Color c = image.GetPixel(x, y);
                    if (!this.ColourList.Any())
                    {
                        double temp = GetRandomNumber(0,1);
                        ColourList.Add(new KeyValuePair<Color, double>(c, temp));
                        dataList.Add(new KeyValuePair<double, double[]>(temp, binaryD.GetData()));
                        randDoubles.Add(temp);
                        numArray[x, y] = temp;
                    }
                    else
                    {
                        bool check = true;
                        double temp;
                        foreach (KeyValuePair<Color, double> kv in this.ColourList)
                        {
                            if (kv.Key == c)
                            {
                                numArray[x,y] = kv.Value;
                                check = false;
                                break;
                            }
                        }
                        if(check)
                        {
                            temp = GetRandomNumber(0,1);
                            while (randDoubles.Contains(temp))
                            {
                                temp = GetRandomNumber(0,1);
                            }
                            randDoubles.Add(temp);
                            numArray[x,y] = temp;
                            ColourList.Add(new KeyValuePair<Color, double>(c, temp));
                            dataList.Add(new KeyValuePair<double, double[]>(temp, binaryD.GetData()));
                        }                        
                    }
                   
                }
            }
        }

        public override void saveToXML()
        {
            int len = (image.Width - 1) * (image.Height - 1);
            DataPoint[] d = new DataPoint[len];
            DataSet DS = new DataSet();
            double[] tmpInput = new double[8];
            double[] tmpOutput = new double[8];
            int count = 0;
            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    if(x !=image.Width-1 && y != image.Height-1 )
                    {

                        foreach(KeyValuePair<double,double[]> kv in dataList)
                        {
                            if(kv.Key == numArray[x,y])
                            {
                                tmpInput = kv.Value;
                                break;
                            }
                        }
                        foreach (KeyValuePair<double, double[]> kv in dataList)
                        {
                            if (kv.Key == numArray[x + 1, y])
                            {
                                tmpOutput = kv.Value;
                                break;
                            }
                        }
                        d[count] = new DataPoint(tmpInput, tmpOutput);
                        DS.data.Add(d[count]);
                        count++;
                    }
                }
            }
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<Root/>");

            doc.DocumentElement.AppendChild(DS.toXml(doc));

            doc.Save("./ann.xml");
        }

        private double GetNumber(double x)
        {
            return x += 0.03;
        }
        private double GetRandomNumber(double minimum, double maximum)
        {
            return Math.Round(random.NextDouble() * (maximum - minimum) + minimum,3);
        }
        public Bitmap _image
        {
            get
            {
                return image;
            }
        }
        public double[,] _numArray
        {
            get
            {
                return numArray;
            }
        }
        public List<KeyValuePair<Color,double>> _ColourList
        {
            get
            {
                return ColourList;
            }
        }
        public List<KeyValuePair<double, double[]>> _DataList
        {
            get
            {
                return dataList;
            }
        }
    }
}
