function retrieveAllContracts() {
    var url = "https://api.jcdecaux.com/vls/v3/contracts?apiKey=" + document.getElementById("key").value;
    var request = new XMLHttpRequest(url);
    request.open("GET",url);
    request.setRequestHeader("Accept", "application/json");
    request.onload = contractsRetrieved;
    request.send();
}

function contractsRetrieved() {
    var label = document.createElement("label");
    label.innerHTML = "Choose a contract";
    label.setAttribute("for", "choice");
    var input = document.createElement("input");
    input.setAttribute("list", "choice");
    input.setAttribute("id", "contracts");
    document.body.appendChild(label);
    document.body.appendChild(input);

    var datalist = document.createElement("datalist");
    datalist.setAttribute("id", "choice");
    var oui = JSON.parse(this.responseText);
    oui.forEach(contract => {
        var option = document.createElement("option");
        option.setAttribute("value", contract.name);
        datalist.appendChild(option);
    });
    document.body.appendChild(datalist);

    var boutonStation = document.createElement("button");
    boutonStation.setAttribute("onclick", "retrieveContractStations()");
    boutonStation.innerHTML += "retrive stations"
    document.body.appendChild(boutonStation);
}

function retrieveContractStations() {
    var url = "https://api.jcdecaux.com/vls/v3/stations?contract=" + document.getElementById("contracts").value + "&apiKey=" + document.getElementById("key").value;
    var request = new XMLHttpRequest(url);
    request.open("GET", url);
    request.setRequestHeader("Accept", "application/json");
    request.onload = stationsRetrieved;
    request.send();
}

function stationsRetrieved() {
    console.log(this.responseText);
}

function getDistanceFrom2GpsCoordinates(lat1, lon1, lat2, lon2) {
    // Radius of the earth in km
    var earthRadius = 6371;
    var dLat = deg2rad(lat2 - lat1);
    var dLon = deg2rad(lon2 - lon1);
    var a =
        Math.sin(dLat / 2) * Math.sin(dLat / 2) +
        Math.cos(deg2rad(lat1)) * Math.cos(deg2rad(lat2)) *
        Math.sin(dLon / 2) * Math.sin(dLon / 2)
        ;
    var c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
    var d = earthRadius * c; // Distance in km
    return d;
}

function deg2rad(deg) {
    return deg * (Math.PI / 180)
}

function getClosestStation() {
    var lattitude = document.getElementById("lattitude");
    var longitude = document.getElementById("longitude");

}

//496571058607f1ccae3899d63be166f0058bca6e