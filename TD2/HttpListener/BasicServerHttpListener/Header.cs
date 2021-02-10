using System.Collections.Specialized;

namespace BasicServerHTTPlistener
{
    class Header
    {
        private NameValueCollection headers;

        public Header(NameValueCollection headers)
        {
            this.headers = headers;
        }

        public string values()
        {
            string rep = "";

            for(int i=0; i<headers.Count; i++)
            {
                rep += headers.GetKey(i) + " : " + headers.Get(i) + "<br />";
            }
            return rep;
        }

    }
}
