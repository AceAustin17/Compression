using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Compression_Year_Project
{
    class Extract
    {
        private string fileContents;

        public Extract(byte[] firstinput)
        {
            fileContents = SmazSharp.Smaz.Decompress(firstinput);
        }

        public string extract()
        {
            string pattern = @"(\[\w*, \w*])|(\[\w*\n\w*, \w*])";

           Match m = Regex.Match(fileContents, pattern);
            string t = "";
           while(m.Success)
            {
                Group g = m.Groups[0];
                t += g.ToString();
                m = m.NextMatch();
            }
            return t;
        }
    }
}
