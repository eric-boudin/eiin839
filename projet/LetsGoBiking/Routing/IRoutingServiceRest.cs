using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Routing
{
    [ServiceContract]
    public interface IRoutingServiceRest
    {
        [OperationContract]
        [WebGet(UriTemplate = "/direction/{start}/{destination}/{name}", RequestFormat = WebMessageFormat.Json,ResponseFormat = WebMessageFormat.Json)]
        List<string> getDirectionRest(string start, string destination, string name);
    }
}
