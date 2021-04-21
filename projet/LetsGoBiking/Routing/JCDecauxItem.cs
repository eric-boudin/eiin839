using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebProxy
{
    class JCDecauxItem
    {
        public int number { get; set; }
        public string contractName { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public Position position { get; set; }
        public bool banking { get; set; }
        public bool bonus { get; set; }
        public bool overflow { get; set; }
        public string shape { get; set; }
        public string status { get; set; }
        public bool connected { get; set; }
        public Stand totalStands { get; set; }
        public Stand mainStands { get; set; }
        public Stand overflowStands { get; set; }

        public JCDecauxItem() {}

        public override bool Equals(object obj)
        {
            if (obj is JCDecauxItem)
            {
                JCDecauxItem temp = (JCDecauxItem)obj;
                return temp.position.Equals(this.position);
            }
            else
                return false;
        }

    }
}
