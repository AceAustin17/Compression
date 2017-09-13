using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compression_Year_Project
{
    class BinaryData
    {
        private int length;
        private int increment = 0;
        public BinaryData(int len)
        {
            length = len;
        }

        public double[] GetData()
        {
            int[] intarray = Enumerable
                .Range(1, length)
                .Select(i => increment / (1 << (length - i)) % 2)
                .ToArray();

            double[] datarray = new double[length];

            for(int i = 0; i < datarray.Length;i++)
            {
                datarray[i] = intarray[i];
            }

            increment++;
            return datarray;
        }
    }
}
