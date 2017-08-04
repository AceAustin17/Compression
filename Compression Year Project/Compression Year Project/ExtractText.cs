using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Compression_Year_Project
{
    class ExtractText :Extract<string>
    {
        private string fileContents;
        private List<KeyValuePair<string, string>> indexList = new List<KeyValuePair<string, string>>();

        public ExtractText(byte[] firstinput)
        {
            fileContents = SmazSharp.Smaz.Decompress(firstinput);
        }

        public override string extract()
        {
           string pattern = @"(\[\w*, \w*])|(\[\w*\n\w*, \w*])";

           Match m = Regex.Match(fileContents, pattern);
           string index = "";
           while(m.Success)
            {
                Group g = m.Groups[0];
                index += g.ToString();
                m = m.NextMatch();
            }

            fileContents = Regex.Replace(fileContents, pattern, "");

            char[] separatingChars = {']'};     
           
            index = index.Trim();
            string[] pairs = index.Split(separatingChars,System.StringSplitOptions.RemoveEmptyEntries);
            
            for(int i =0; i < pairs.Length; i++)
            {
               pairs[i] = pairs[i].Trim('[',']',' ');
               string[] chk = pairs[i].Split(',');

               indexList.Add(new KeyValuePair<string, string>(chk[0], chk[1]));               
            }

            foreach(KeyValuePair<string,string> kv in indexList)
            {
                fileContents = fileContents.Replace(kv.Value, " " +kv.Key);
            }

            return fileContents;
        }
    }
}
