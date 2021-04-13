using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WebProxy;

namespace Routing
{
    // REMARQUE : vous pouvez utiliser la commande Renommer du menu Refactoriser pour changer le nom de classe "Service1" à la fois dans le code et le fichier de configuration.
    public class RoutingServiceImpl : IRoutingService
    {
        List<JCDecauxItem> items;

        public string getDirection(string start, string destination, string city)
        {
            if (start.Equals(destination))
                return "You are already at your destination";
     
            Position startPosition = getPositionFromAddress(start);
            Position destinationPosition = getPositionFromAddress(destination);

            items = getStationByCityName(city);

            if (items.Count == 0)
                return "JCDecaux stations are not available in your city.";

            JCDecauxItem nearestStationFromStart = findNearestStation(startPosition);
            JCDecauxItem nearestStationFromDestination = findNearestStation(destinationPosition);

            if (getDistance(nearestStationFromStart.position, startPosition) == getDistance(nearestStationFromDestination.position, startPosition)
                || nearestStationFromDestination.Equals(nearestStationFromStart))
                return "The departure and arrival stations are at the same distance of your actual postion,so you'd better walk to your destination";


            
            string path = createPath(startPosition, nearestStationFromStart.position);
            path += createPath(nearestStationFromStart.position, nearestStationFromDestination.position);
            path += createPath(nearestStationFromDestination.position, destinationPosition);

            return path;
        }

        private string createPath(Position start, Position destination)
        {
            string path = "";

            string startLocation = start.longitude.ToString().Replace(",",".") + "," + start.latitude.ToString().Replace(",", ".");
            string destinationLocation = destination.longitude.ToString().Replace(",", ".") + "," + destination.latitude.ToString().Replace(",", ".");

            string request = "https://api.openrouteservice.org/v2/directions/driving-car?api_key=5b3ce3597851110001cf62482b30ae4a0e954c29a751402182c44503&start=" + startLocation + "&end=" + destinationLocation;

            Task<string> requestHandler = askOpenRoute(request);
            requestHandler.Wait();
            dynamic obj = JsonConvert.DeserializeObject(requestHandler.Result);

            foreach(var step in obj.features[0].properties.segments[0].steps)
            {
                path += step.instruction + " for " + step.distance + "m.\n";
            }

            return path;
        }

        //A vol d'oiseau pour limiter l'appel à l'api
        private JCDecauxItem findNearestStation(Position position) 
        {
            JCDecauxItem nearestStation = null;
            double distance = -1;

            foreach(JCDecauxItem item in items)
            {
                double temp = getDistance(position, item.position);
                if (distance == -1 ||  temp < distance)
                {
                    nearestStation = item;
                    distance = temp;
                }                   
            }
            return nearestStation;
        }

        private double getDistance(Position a, Position b)
        {
            return Math.Sqrt(Math.Pow((b.latitude - a.latitude), 2) + Math.Pow((b.longitude - a.longitude), 2));
        }

        private async Task<string> askOpenRoute(string request)
        {
            return await new HttpClient().GetStringAsync(request);
        }

        private Position getPositionFromAddress(string address)
        {
            Task<string> requestHandler = askOpenRoute("https://api.openrouteservice.org/geocode/search/structured?api_key=5b3ce3597851110001cf62482b30ae4a0e954c29a751402182c44503&address=" + address);
            requestHandler.Wait();
            dynamic obj = JsonConvert.DeserializeObject(requestHandler.Result);

            float latitude = obj.features[0].geometry.coordinates[1];
            float longitude = obj.features[0].geometry.coordinates[0];

            Position p = new Position(latitude, longitude);
            return p;
        }

        private List<JCDecauxItem> getStationByCityName(string name)
        {
            Task<string> task = askCache("http://localhost:8733/Design_Time_Addresses/Biking/GoBikingService/getStationByCityName?name="+name.ToLower());
            task.Wait();
            List<JCDecauxItem> result = JsonConvert.DeserializeObject<List<JCDecauxItem>>(task.Result);
            List<JCDecauxItem> stations = new List<JCDecauxItem>();

            foreach (JCDecauxItem item in result)
            {
                if (item.contractName.Equals(name.ToLower()))
                    stations.Add(item);
;            }
            return stations;
        }

        private async Task<string> askCache(string request)
        {
            return await new HttpClient().GetStringAsync(request);
        } 

    }
}
