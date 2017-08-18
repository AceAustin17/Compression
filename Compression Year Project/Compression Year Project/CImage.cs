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
        [Serializable]
        public struct posCol
        {
            public int x { get; set; }
            public int y { get; set; }
            public int num { get; set; }
        }
        private List<KeyValuePair<Color, double>> ColourList = new List<KeyValuePair<Color, double>>();
        private posCol[] PosList;
        private double[,] numarray;

        public CImage(int width, int height)
        {
            this.height = height;
            this.width = width;
            numarray = new double[width, height];
        }          
        public posCol[] _PosList
        {
            get
            {
                return PosList;
            }
            set
            {
                PosList = value;
            }
        }

        public double[,] _numarray
        {
            get
            {
                return numarray;
            }            
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
