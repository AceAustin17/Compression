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

            for (int x = 0; x < cimage._width; x++)
            {
                for (int y = 0; y < cimage._height; y++)
                {
                    if (cimage._numarray[x, y] != 0)
                    {
                        foreach (KeyValuePair<Color, double> kv in cimage._ColourList)
                        {
                            if (cimage._numarray[x, y] == kv.Value)
                            {
                                bmp.SetPixel(x, y, kv.Key);
                                break;
                            }
                        }
                    }
                }
            }            
                for(int t =0 ; t < cimage._PosList.Length;t++)
                {
                    foreach (KeyValuePair<Color, double> kv in cimage._ColourList)
                    {
                        if (cimage._numarray[cimage._PosList[t].x, cimage._PosList[t].y] == kv.Value)
                        {
                            for (int i = 1; i <= cimage._PosList[t].num; i++)
                            {
                                if (cimage._PosList[t].x + i < cimage._width)
                                {
                                    bmp.SetPixel(cimage._PosList[t].x + i, cimage._PosList[t].y, kv.Key);
                                  }
                            }
                        break;
                        }
                    }
                }            
            return bmp;
        }
    }
}
