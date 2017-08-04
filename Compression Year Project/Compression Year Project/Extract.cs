using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compression_Year_Project
{
    abstract class Extract<T>
    {
        public virtual T extract()
        {
            return default(T);
        }
    }
}
