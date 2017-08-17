using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compression_Year_Project
{
    class ExtractImage : Extract<Bitmap>
    {
        private CImage cimage;
        public ExtractImage(CImage cimage)
        {
            this.cimage = cimage;
        }

        public override Bitmap extract()
        {
            Bitmap bmp = new Bitmap(cimage._width, cimage._height);
            Color tempColor;
            for (int x = 0; x < cimage._width; x++)
            {
                for (int y = 0; y < cimage._height; y++)
                {
                    foreach(KeyValuePair<Color,double> kv in cimage._ColourList)
                    {
                        if(kv.Value == cimage._numArray[x,y])
                        {
                            tempColor = kv.Key;
                            bmp.SetPixel(x, y, tempColor);
                            break;
                        }
                    }
                }
            }
            return bmp;
        }
    }
}
