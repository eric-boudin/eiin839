using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebProxy
{
    public class Position
    {
        public float latitude { get; set; }
        public float longitude { get; set; }

        public Position() { }

        public Position(float latitude, float longitude)
        {
            this.latitude = latitude;
            this.longitude = longitude;
        }

        public override bool Equals(Object o)
        {
            if (o is Position)
            {
                Position temp = (Position)o;
                return temp.latitude == this.latitude && temp.longitude == this.longitude;
            }
            else
                return false;
        }

    }
}
