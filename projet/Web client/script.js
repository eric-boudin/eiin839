var map;

class Position {
    constructor(a, b) {
        this.latitude = a;
        this.longitude = b;
    }

    set latitude(value) {
        this._latitude = value;
    }

    set longitude(value) {
        this._longitude = value;
    }

    get latitude() {
        return this._latitude;
    }

    get longitude() {
        return this._longitude;
    }
}

function getDirection() {

	var direction;

	/*const start = "487 Rue Jean Queillau";
    const destination = "140 avenue Viton";
    const city = "Marseille";*/

    const start = document.getElementById("start").value
    const destination = document.getElementById("destination").value
    const city = document.getElementById("city").value

    const url = 'http://localhost:8733/Design_Time_Addresses/Routing/Service1/rest/direction/'+start+'/'+destination+"/"+city;
    var request = new XMLHttpRequest();
    request.open("GET", url, true);
    request.setRequestHeader("Accept", "application/json")
    request.onload = displayDirection;
    request.send();
}

function createMap(element) {
    document.getElementById("map").innerHTML = "";
    map = new ol.Map({
        target: 'map', // <-- This is the id of the div in which the map will be built.
        layers: [
            new ol.layer.Tile({
                source: new ol.source.OSM()
            })
        ],

        view: new ol.View({
            center: ol.proj.fromLonLat([element.longitude, element.latitude]), // <-- Those are the GPS coordinates to center the map to.
            zoom: 14 // You can adjust the default zoom.
        })
    });
}

function addLine(posA, posB) {
    if(map == null)
        createMap();

    // Create an array containing the GPS positions you want to draw
    var coords = [[posA.longitude, posA.latitude], [posB.longitude, posB.latitude]];
    var lineString = new ol.geom.LineString(coords);

    // Transform to EPSG:3857
    lineString.transform('EPSG:4326', 'EPSG:3857');

    // Create the feature
    var feature = new ol.Feature({
        geometry: lineString,
        name: 'Line'
    });

    // Configure the style of the line
    var lineStyle = new ol.style.Style({
        stroke: new ol.style.Stroke({
            color: '#ffcc33',
            width: 5
        })
    });

    var source = new ol.source.Vector({
        features: [feature]
    });

    var vector = new ol.layer.Vector({
        source: source,
        style: [lineStyle]
    });

    map.addLayer(vector);
} 

function displayDirection() {

    var instructions = JSON.parse(this.responseText);
    var positions = new Array();
    var map = "";

    if(instructions.length > 1) {
        instructions.forEach(element => {
            var latitude = parseFloat(element.replace("[","").substring(0,element.indexOf("]")-1).replace(",","."));
            var longitude = parseFloat(element.substring(element.lastIndexOf("[")+1, element.lastIndexOf("]")).replace(element.lastIndexOf("]"),"").replace(",","."));
            positions.push(new Position(latitude,longitude));
        });
        createMap(positions[0]);

        for (var i = 1; i < positions.length; i++) {
            addLine(positions[i-1], positions[i]);
        }
    }
    else {
        document.getElementById("map").innerHTML = "<br><br><strong>"+instructions+"</strong>";
    }
}