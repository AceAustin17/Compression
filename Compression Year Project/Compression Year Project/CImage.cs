using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compression_Year_Project
{
    [Serializable]
    class CImage
    {
        private int height;
        private int width;

        private List<KeyValuePair<Color, double>> ColourList;
        private double[,] numArray;

        public CImage(int height, int width)
        {
            this.height = height;
            this.width = width;
        }

        public List<KeyValuePair<Color, double>> _ColourList
        {
            get
            {
                return this.ColourList;
            }
            set
            {
                this.ColourList = value;
            }
        }

        public double[,] _numArray
        { 

            get
            {
                return this.numArray;
            }
            set
            {
                this.numArray = value;
            }
        }

        public int _width
        {
            get
            {
                return this.width;
            }
        }
        public int _height
        {
            get
            {
                return this.height;
            }
        }
    }
}
