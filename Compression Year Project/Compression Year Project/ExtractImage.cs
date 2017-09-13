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

            for (int i = 0; i < cimage._PosList.Length; i++)
            {
                for (int t = 0; t < cimage._PosList[i].num; t++)
                {                     
                    if ((cimage._PosList[i].x + t < cimage._width) && (cimage._PosList[i].y < cimage._height))
                    {
                        bmp.SetPixel(cimage._PosList[i].x +t, cimage._PosList[i].y, cimage._PosList[i].col);
                    }                        
                    
                }
            }
            return bmp;
        }
    }
}
