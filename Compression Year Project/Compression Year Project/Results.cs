/* Code adapted from
 * https://en.wikipedia.org/wiki/Data_compression_ratio
 *
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compression_Year_Project
{
    class Results
    {
        private long original;
        private long compressed;
        public Results(long original, long compressed)
        {
            this.original = original;
            this.compressed = compressed;
        }

        public string ShowRatio()
        {
            double ratio = original / (double)compressed;

            return "The compression ratio is " + Math.Round(ratio,1) + ":1";
        }
        public string ShowSavedData()
        {
            double saved = (1 - (compressed / (double)original))* 100;

            return "The amount of data saved is " + Math.Round(saved) + "%";
        }


    }
}
