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

        private List<double> randDoubles = new List<double>();
        private List<KeyValuePair<Color, double>> ColourList = new List<KeyValuePair<Color, double>>();
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
                    if (ColourList.Any())
                    {
                        double temp = Math.Round(GetRandomNumber(0, 1), 3);
                        ColourList.Add(new KeyValuePair<Color, double>(c, temp));
                        randDoubles.Add(temp);
                        numArray[x, y] = temp;
                    }
                    else
                    {
                        bool check = true;
                        double temp;
                        foreach (KeyValuePair<Color, double> kv in this.ColourList)
                        {
                            if (kv.Key.Equals(c))
                            {
                                numArray[x,y] = kv.Value;

                                check = false;
                                break;
                            }
                        }
                        if(check)
                        {
                            temp = Math.Round(GetRandomNumber(0, 1), 3);
                            while (randDoubles.Contains(temp))
                            {
                                temp = Math.Round(GetRandomNumber(0, 1), 3);
                            }
                            randDoubles.Add(temp);
                            numArray[x,y] = temp;

                            ColourList.Add(new KeyValuePair<Color, double>(c, temp));
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
            double[] tmpInput = new double[3];
            double[] tmpOutput = new double[1];
            int count = 0;
            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    if(x !=image.Width-1 && y != image.Height-1 )
                    {
                        tmpInput[0] = numArray[x + 1, y];
                        tmpInput[1] = numArray[x, y +1];
                        tmpInput[2] = numArray[x +1, y + 1];
                        tmpOutput[0] = numArray[x, y];
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
        private double GetRandomNumber(double minimum, double maximum)
        {

            return random.NextDouble() * (maximum - minimum) + minimum;
        }
    }
}
