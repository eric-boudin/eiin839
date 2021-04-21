using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using WebProxy;

namespace Routing
{
    public class RoutingServiceImpl : IRoutingService, IRoutingServiceRest
    {
        List<JCDecauxItem> items = new List<JCDecauxItem>();

        public List<string> getDirectionRest(string start, string destination, string city)
        {       
            string path = calculDestination(start, destination, city, false);

            return new List<string>(path.Split('-'));
        }

        public string getDirection(string start, string destination, string city)
        {
            return calculDestination(start, destination, city, true);
        }

        private string calculDestination(string start, string destination, string city, bool parse) 
        {
            WebOperationContext.Current.OutgoingResponse.Headers.Add("Access-Control-Allow-Origin", "*");
            try
            {
                items.Clear();
                if (start.Equals(destination))
                    return "You are already at your destination";

                Position startPosition = getPositionFromAddress(start, city);
                Position destinationPosition = getPositionFromAddress(destination, city);

                items = getStationByCityName(city);

                if (items.Count == 0)
                    return "JCDecaux stations are not available in your city.";

                JCDecauxItem nearestStationFromStart = findNearestStation(startPosition, items);
                JCDecauxItem nearestStationFromDestination = findNearestStation(destinationPosition, items);

                if (nearestStationFromStart == null || nearestStationFromDestination == null)
                    return "No bikes is available in your city's stations.";

                if (getDistance(nearestStationFromStart.position, startPosition) == getDistance(nearestStationFromDestination.position, startPosition)
                    || nearestStationFromDestination.Equals(nearestStationFromStart))
                    return "The departure and arrival stations are at the same distance of your actual postion,so you'd better walk to your destination";


                if (parse)
                {
                    double totalDuration = 0;
                    string path = createPath(startPosition, nearestStationFromStart.position, "foot-walking", ref totalDuration);
                    path += createPath(nearestStationFromStart.position, nearestStationFromDestination.position, "cycling-road", ref totalDuration);
                    path += createPath(nearestStationFromDestination.position, destinationPosition, "foot-walking", ref totalDuration);

                    double totalDurationWithoutBikes = 0;
                    string pathWithoutBikes = createPath(startPosition, destinationPosition, "foot-walking", ref totalDurationWithoutBikes);

                    if (totalDurationWithoutBikes <= totalDuration)
                        return "It's equivalent or faster to walk to your destination.";

                    return path;
                }
                else
                {
                    string json = getJSONPath("foot-walking", startPosition, nearestStationFromStart.position);
                    List<Position> tabList = new List<Position>(extractPosition(json));
                    json = getJSONPath("driving-car", nearestStationFromStart.position, nearestStationFromDestination.position);
                    tabList.AddRange(extractPosition(json));
                    json = getJSONPath("driving-car", nearestStationFromDestination.position, destinationPosition);
                    tabList.AddRange(extractPosition(json));

                    return createPostionStringTab(tabList);
                }
            }catch(Exception e)
            {
                return "A problem occured with one of the addresses. Please verify that you correctly spell it.";
            }
        }

        private string createPostionStringTab(List<Position> tab)
        {
            string tabString = "";
            for(int i=0; i<tab.Count; i++)
            {
                tabString += "[" + tab[i].latitude + "],[" + tab[i].longitude + "]-";
            }
            tabString.Remove(tabString.LastIndexOf("-"));
            return tabString;
        } 

        private List<Position> extractPosition(string json)
        {
            dynamic obj = JsonConvert.DeserializeObject(json);

            List<Position> positions = new List<Position>();

            foreach(var position in obj.features[0].geometry.coordinates)
            {
                float longi = position[1];
                float lati = position[0];
                positions.Add(new Position(longi, lati));
            }

            return positions;
        }

        private string createPath(Position start, Position destination, string profile, ref double totalDistance)
        {
            string path = "";
         
            dynamic obj = JsonConvert.DeserializeObject(getJSONPath(profile, start, destination));

            foreach (var step in obj.features[0].properties.segments[0].steps)
            {
                path += step.instruction + " for " + step.distance + "m.\n";
                totalDistance += double.Parse(step.duration.ToString());
            }

            return path;
        }

        private string getJSONPath(string profile, Position start, Position destination)
        {
            string startLocation = start.longitude.ToString().Replace(",", ".") + "," + start.latitude.ToString().Replace(",", ".");
            string destinationLocation = destination.longitude.ToString().Replace(",", ".") + "," + destination.latitude.ToString().Replace(",", ".");

            string request = "https://api.openrouteservice.org/v2/directions/" + profile + "?api_key=5b3ce3597851110001cf62482b30ae4a0e954c29a751402182c44503&start=" + startLocation + "&end=" + destinationLocation;

            Task<string> requestHandler = askAPI(request);
            requestHandler.Wait();
            return requestHandler.Result;
        }

        //A vol d'oiseau pour limiter l'appel à l'api
        private JCDecauxItem findNearestStation(Position position, List<JCDecauxItem> stations)
        {
            if (stations.Count == 0)
                return null;

            JCDecauxItem nearestStation = null;
            double distance = -1;

            foreach (JCDecauxItem item in stations)
            {
                double temp = getDistance(position, item.position);
                if (distance == -1 || temp < distance)
                {
                    nearestStation = item;
                    distance = temp;
                }
            }

            stations.Remove(nearestStation);
            items.Remove(nearestStation);
            nearestStation = refreshItem(nearestStation);
            stations.Add(nearestStation);
            items.Add(nearestStation);

            if(nearestStation.mainStands.availabilities.Bikes==0)
            {
                stations.Remove(nearestStation);
                nearestStation = findNearestStation(position, stations);
            }                

            return nearestStation;
        }

        private JCDecauxItem refreshItem(JCDecauxItem station)
        {

            Task<string> requestHandler = askAPI("http://localhost:8733/Design_Time_Addresses/Biking/GoBikingService/refreshItem?id="+station.number+"&name="+station.name+"&contractName="+station.contractName);
            requestHandler.Wait();
            JCDecauxItem refreshedStation = JsonConvert.DeserializeObject<JCDecauxItem>(requestHandler.Result);
            return refreshedStation;
        }

        private double getDistance(Position a, Position b)
        {
            return Math.Sqrt(Math.Pow((b.latitude - a.latitude), 2) + Math.Pow((b.longitude - a.longitude), 2));
        }

        private Position getPositionFromAddress(string address, string city)
        {
            Task<string> requestHandler = askAPI("https://api.openrouteservice.org/geocode/search/structured?api_key=5b3ce3597851110001cf62482b30ae4a0e954c29a751402182c44503&address=" + address + "&locality=" + city);
            requestHandler.Wait();
            dynamic obj = JsonConvert.DeserializeObject(requestHandler.Result);

            float latitude = obj.features[0].geometry.coordinates[1];
            float longitude = obj.features[0].geometry.coordinates[0];

            Position p = new Position(latitude, longitude);
            return p;
        }

        private List<JCDecauxItem> getStationByCityName(string name)
        {
            Task<string> task = askAPI("http://localhost:8733/Design_Time_Addresses/Biking/GoBikingService/getStationByCityName?name=" + name.ToLower());
            task.Wait();
            List<JCDecauxItem> result = JsonConvert.DeserializeObject<List<JCDecauxItem>>(task.Result);
            List<JCDecauxItem> stations = new List<JCDecauxItem>();

            foreach (JCDecauxItem item in result)
            {
                if (item.contractName.Equals(name.ToLower()))
                    stations.Add(item);
                ;
            }
            return stations;
        }

        private async Task<string> askAPI(string request)
        {
            return await new HttpClient().GetStringAsync(request);
        }

}
}
