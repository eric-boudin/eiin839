using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebProxy
{
    public class Availability
    {
        int bikes;
        int stands;

        public int Bikes
        {
            get => bikes;
            set
            {
                bikes = value;
            }
        }

        public int Stands
        {
            get => stands;
            set
            {
                stands = value;
            }
        }

    }
}
