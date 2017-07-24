/* Code adapted from
 * https://www.youtube.com/watch?v=aVId8KMsdUU&list=PL29C61214F2146796 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ANeuralNetwork
{
    public enum ActivationFunction
    {
        None,
        Sigmoid,
        HyperTan,
        Linear,
        Gaussian,
        RationalSig
    }

    static class ActivationFunctions
    {
        public static double Evaluate(ActivationFunction aFun, double x)
        {
            switch (aFun)
            {
                case ActivationFunction.HyperTan:
                    return hyperTan(x);
                case ActivationFunction.Sigmoid:
                    return sigmoid(x);
                case ActivationFunction.Linear:
                    return linear(x);
                case ActivationFunction.Gaussian:
                    return gaussian(x);
                case ActivationFunction.RationalSig:
                    return rationalSig(x);
                case ActivationFunction.None:
                default:
                    return 0.0;
            }

        }
        public static double EvaluateDerivative(ActivationFunction aFun, double x)
        {
            switch (aFun)
            {
                case ActivationFunction.HyperTan:
                    return hyperTanDeriv(x);
                case ActivationFunction.Sigmoid:
                    return sigmoidDeriv(x);
                case ActivationFunction.Linear:
                    return linearDeriv(x);
                case ActivationFunction.Gaussian:
                    return gaussianderiv(x);
                case ActivationFunction.RationalSig:
                    return rationalsigDeriv(x);
                case ActivationFunction.None:
                default:
                    return 0.0;
            }

        }

        private static double sigmoid(double x)
        {
            return 1.0 / (1.0 + Math.Exp(-x));
        }
        private static double sigmoidDeriv(double x)
        {
            return sigmoid(x) * (1 - sigmoid(x));
        }

        private static double hyperTan(double x)
        {
            return (Math.Exp(x) - Math.Exp(-x)) / (Math.Exp(x) + Math.Exp(-x));
        }

        private static double hyperTanDeriv(double x)
        {
            return 1.0 / Math.Pow(Math.Cosh(x), 2);
        }

        private static double linear( double x)
        {
            return x;
        }
        private static double linearDeriv(double x)
        {
            return 1.0;
        }

        private static double gaussian(double x)
        {
            return Math.Exp(-Math.Pow(x, 2));
        }
        private static double gaussianderiv(double x)
        {
            return -2.0 * x * gaussian(x);
        }

        private static double rationalSig (double x)
        {
            return x / (1.0 + Math.Sqrt(1.0 + x * x));
        }
        private static double rationalsigDeriv(double x)
        {
            double a = Math.Sqrt(1.0 + x * x);
            return 1.0 / (a * (1 + a));
        }
    }
    public class BackPropNetwork
    {
        public string name = "default";

        private int numLayers;
        private int inputSize;
        private int[] layerSize;
        private ActivationFunction[] activFunctions;

        private double[][] layerOutput; //x = layer index , y = node index
        private double[][] inputLayer; // same
        private double[][] bias;
        private double[][] delta;
        private double[][] previousBiasDelta;

        private double[][][] weights;
        private double[][][] prevWeights;

        private XmlDocument xdoc = null;
        private bool loaded = true;
        
        public BackPropNetwork(int[] layerSizes, ActivationFunction[] aFunctions)
        {
            if (aFunctions.Length != layerSizes.Length || aFunctions[0] != ActivationFunction.None)
            {
                throw new ArgumentException("Cannot create nework");
            }

            numLayers = layerSizes.Length - 1;

            inputSize = layerSizes[0];

            layerSize = new int[numLayers];

            for (int i = 0; i < numLayers; i++)
            {
                layerSize[i] = layerSizes[i + 1];
            }

            activFunctions = new ActivationFunction[numLayers];

            for (int i = 0; i < numLayers; i++)
            {
                activFunctions[i] = aFunctions[i + 1];
            }

            bias = new double[numLayers][];
            delta = new double[numLayers][];
            previousBiasDelta = new double[numLayers][];

            layerOutput = new double[numLayers][];
            inputLayer = new double[numLayers][];

            weights = new double[numLayers][][];
            prevWeights = new double[numLayers][][];

            for (int l = 0; l < numLayers; l++)
            {
                bias[l] = new double[layerSize[l]];
                previousBiasDelta[l] = new double[layerSize[l]];
                delta[l] = new double[layerSize[l]];
                layerOutput[l] = new double[layerSize[l]];
                inputLayer[l] = new double[layerSize[l]];

                weights[l] = new double[l == 0 ? inputSize : layerSize[l - 1]][];
                prevWeights[l] = new double[l == 0 ? inputSize : layerSize[l - 1]][];

                for (int i = 0; i < (l == 0 ? inputSize : layerSize[l - 1]); i++)
                {
                    weights[l][i] = new double[layerSize[l]];
                    prevWeights[l][i] = new double[layerSize[l]];
                }
            }
            //initialise weights

            for (int l = 0; l < numLayers; l++)
            {
                for (int j = 0; j < layerSize[l]; j++)
                {
                    bias[l][j] = Gaussian.getRandomGaussian();
                    previousBiasDelta[l][j] = 0.0;
                    layerOutput[l][j] = 0.0;
                    inputLayer[l][j] = 0.0;
                    delta[l][j] = 0.0;
                }
                for (int i = 0; i < (l == 0 ? inputSize : layerSize[l - 1]); i++)
                {
                    for (int j = 0; j < layerSize[l]; j++)
                    {
                        weights[l][i][j] = Gaussian.getRandomGaussian();
                        prevWeights[l][i][j] = 0.0;
                    }
                }


            }
        }

        //another constructor

        public BackPropNetwork(string filepath)
        {
            loaded = false;

            Load(filepath);

            loaded = true;
        }
        //Methods
        public void run(ref double[] input, out double[] output)
        {
            if (input.Length != inputSize)
            {
                throw new ArgumentException("Not enough data");
            }

            output = new double[layerSize[numLayers - 1]];

            //running the network
            for (int l = 0; l < numLayers; l++)
            {
                for (int j = 0; j < layerSize[l]; j++)
                {
                    double sum = 0.0;
                    for (int i = 0; i < (l == 0 ? inputSize : layerSize[l - 1]); i++)
                    {
                        sum += weights[l][i][j] * (l == 0 ? input[i] : layerOutput[l - 1][i]);

                    }
                    sum += bias[l][j];

                    inputLayer[l][j] = sum;

                    layerOutput[l][j] = ActivationFunctions.Evaluate(activFunctions[l], sum);

                    
                }
            }

            for (int i = 0; i < layerSize[numLayers - 1]; i++)
            {
                output[i] = layerOutput[numLayers - 1][i];

            }
        }

        public double train(ref double[] input, ref double[] desired, double trainingRate, double momentum)
        {
            if (input.Length != inputSize)
            {
                throw new ArgumentException("Ivalid input");
            }

            if (desired.Length != layerSize[numLayers - 1])
            {
                throw new ArgumentException("Invalid parameter", "desired");

            }
            double error = 0.0;
            double sum = 0.0;
            double biasDelta = 0.0;
            double weightdelta = 0.0;

            double[] output = new double[layerSize[numLayers - 1]];

            // running the network
            run(ref input, out output);

            //back propagating
            for (int l = numLayers - 1; l >= 0; l--)
            {
                if (l == numLayers - 1)
                {
                    for (int k = 0; k < layerSize[l]; k++)
                    {
                        delta[l][k] = output[k] - desired[k];

                        error += Math.Pow(delta[l][k], 2);

                        delta[l][k] *= ActivationFunctions.EvaluateDerivative(activFunctions[l], inputLayer[l][k]);
                    }
                }
                else
                {
                    for (int i = 0; i < layerSize[l]; i++)
                    {
                        sum = 0.0;
                        for (int j = 0; j < layerSize[l + 1]; j++)
                        {
                            sum += weights[l + 1][i][j] * delta[l + 1][j];
                        }
                        sum *= ActivationFunctions.EvaluateDerivative(activFunctions[l], inputLayer[l][i]);

                        delta[l][i] = sum;
                    }
                }
            }
            //update weights
            for (int l = 0; l < numLayers; l++)
            {
                for (int i = 0; i < (l == 0 ? inputSize : layerSize[l - 1]); i++)
                {
                    for (int j = 0; j < layerSize[l]; j++)
                    {
                        weightdelta = trainingRate * delta[l][j] * (l == 0 ? input[i] : layerOutput[l - 1][i]) + momentum * prevWeights[l][i][j];
                        weights[l][i][j] -= weightdelta ;

                        prevWeights[l][i][j] = weightdelta;
                    }
                }
            }
            //update biases
            for (int l = 0; l < numLayers; l++)
            {
                for (int i = 0; i < layerSize[l]; i++)
                {
                    biasDelta = trainingRate * delta[l][i];
                    bias[l][i] -= biasDelta + momentum * previousBiasDelta[l][i];

                    previousBiasDelta[l][i] = biasDelta;
                }
            }
            return error;
        }

        public void Save(string filePath)
        {
            if(filePath == null)
            {
                return;
            }
            XmlWriter xw =  XmlWriter.Create(filePath);

            xw.WriteStartElement("ANeuralNetwork");
            xw.WriteAttributeString("Type", "BackPropagation");

            xw.WriteStartElement("Parameters");

            xw.WriteElementString("Name", name);
            xw.WriteElementString("InputSize", inputSize.ToString());
            xw.WriteElementString("NumLayers", numLayers.ToString());

            xw.WriteStartElement("Layers");

            for(int l =0; l < numLayers; l++)
            {
                xw.WriteStartElement("Layer");
                xw.WriteAttributeString("Index", l.ToString());

                xw.WriteAttributeString("Size", layerSize[l].ToString());
                xw.WriteAttributeString("Type", activFunctions[l].ToString());
                xw.WriteEndElement();
            }

            xw.WriteEndElement();
            xw.WriteEndElement(); // parameters

            //weights and bias

            xw.WriteStartElement("Weights");

            for(int l = 0; l < numLayers; l++)
            {
                xw.WriteStartElement("Layer");
                xw.WriteAttributeString("Index", l.ToString());

                for(int j = 0; j < layerSize[l];j++)
                {
                    xw.WriteStartElement("Node");
                    xw.WriteAttributeString("Index", j.ToString());

                    xw.WriteAttributeString("Bias",bias[l][j].ToString());

                    for(int i = 0; i < (l == 0 ? inputSize : layerSize[l - 1]); i++)
                    {
                        xw.WriteStartElement("Axon");
                        xw.WriteAttributeString("Index", i.ToString());

                        xw.WriteString(weights[l][i][j].ToString());

                        xw.WriteEndElement();//ax
                    }

                    xw.WriteEndElement();//nod
                }

                xw.WriteEndElement();//layer
            }

            xw.WriteEndElement();//weights
            
            xw.WriteEndElement();

            xw.Flush();
            xw.Close();
        }

        public void Load(string filepath)
        {
            if(filepath == null)
            {
                return;
            }

            xdoc = new XmlDocument();
            xdoc.Load(filepath);

            string basePath = "";
            string nodePath = "";
            double val;

            if (xPathVal("ANeuralNetwork/@Type") != "BackPropagation")
            {
                return;
            }

            basePath = "ANeuralNetwork/Parameters/";
            name = xPathVal(basePath +"Name");

            int.TryParse(xPathVal(basePath + "InputSize"),out inputSize);

            int.TryParse(xPathVal(basePath + "NumLayers"), out numLayers);

            layerSize = new int[numLayers];

            activFunctions = new ActivationFunction[numLayers];

            basePath = "ANeuralNetwork/Parameters/Layers/Layer";
            for (int l =0; l < numLayers; l++)
            {
                int.TryParse(xPathVal(basePath + "[@Index='" + l.ToString() + "']/@Size"), out layerSize[l]);
                Enum.TryParse<ActivationFunction>(xPathVal(basePath + "[@Index='" + l.ToString() + "']/@Type"), out activFunctions[l]);
            }

            //parse weights

            bias = new double[numLayers][];
            delta = new double[numLayers][];
            previousBiasDelta = new double[numLayers][];

            layerOutput = new double[numLayers][];
            inputLayer = new double[numLayers][];

            weights = new double[numLayers][][];
            prevWeights = new double[numLayers][][];

            for (int l = 0; l < numLayers; l++)
            {
                bias[l] = new double[layerSize[l]];
                previousBiasDelta[l] = new double[layerSize[l]];
                delta[l] = new double[layerSize[l]];
                layerOutput[l] = new double[layerSize[l]];
                inputLayer[l] = new double[layerSize[l]];

                weights[l] = new double[l == 0 ? inputSize : layerSize[l - 1]][];
                prevWeights[l] = new double[l == 0 ? inputSize : layerSize[l - 1]][];

                for (int i = 0; i < (l == 0 ? inputSize : layerSize[l - 1]); i++)
                {
                    weights[l][i] = new double[layerSize[l]];
                    prevWeights[l][i] = new double[layerSize[l]];
                }
            }
            //initialise weights

            for (int l = 0; l < numLayers; l++)
            {
                basePath = "ANeuralNetwork/Weights/Layer[@Index='" + l.ToString() + "']/";
                for (int j = 0; j < layerSize[l]; j++)
                {
                    nodePath = "Node[@Index='" + j.ToString() + "']/@Bias";
                    double.TryParse(xPathVal(basePath + nodePath), out val);

                    bias[l][j] = val;
                    previousBiasDelta[l][j] = 0.0;

                    layerOutput[l][j] = 0.0;
                    inputLayer[l][j] = 0.0;
                }
                for (int i = 0; i < (l == 0 ? inputSize : layerSize[l - 1]); i++)
                {
                    for (int j = 0; j < layerSize[l]; j++)
                    {
                        nodePath = "Node[@Index='" + j.ToString() + "']/Axon[@Index='" + i.ToString() + "']";

                        double.TryParse(xPathVal(basePath + nodePath), out val);

                        weights[l][i][j] = val;
                        prevWeights[l][i][j] = 0.0;
                    }
                }


            }
            xdoc = null;
        }

        private string xPathVal(string xpath)
        {
            XmlNode node = xdoc.SelectSingleNode(xpath);

            if(node == null)
            {
                throw new ArgumentException("Cannot find node", xpath);
            }

            return node.InnerText;
        }

        public void Nudge(double scalar)
        {
              // helps stop network getting stuck
              for(int l=0; l < numLayers; l++)
            {
                for (int j =0; j < layerSize[l]; j++)
                {
                    //nudging weights
                    for(int i = 0; i < (l==0 ? inputSize : layerSize[l-1]);i++)
                    {
                        double w = weights[l][i][j];
                        double u = Gaussian.getRandomGaussian(0f, w * scalar);
                        weights[l][i][j] += u;
                        prevWeights[l][i][j] = 0f;

                    }

                    //nudging bias
                    double b = bias[l][j];
                    double v = Gaussian.getRandomGaussian(0f, b * scalar);
                    bias[l][j] += v;
                    previousBiasDelta[l][j] = 0f;
                }
            }
        }
        public static class Gaussian
        {
            private static Random ran = new Random();
            public static void getRandomGaussian(double mean, double stddev, out double val1, out double val2)
            {
                double u, s, v, t;

                do
                {
                    u = 2 * ran.NextDouble() - 1;
                    v = 2 * ran.NextDouble() - 1;

                } while (u * u + v * v > 1 || u == 0 && v == 0);

                s = u * u + v * v;
                t = Math.Sqrt((-2.0 * Math.Log(s) / s));

                val1 = stddev * u * t + mean;
                val2 = stddev * v * t + mean;
            }

            public static double getRandomGaussian(double mean, double stddev)
            {
                double val1, val2;
                getRandomGaussian(mean, stddev, out val1, out val2);

                return val1;
            }

            public static double getRandomGaussian()
            {
                return getRandomGaussian(0.0, 1.0);
            }

        }
    }
}
