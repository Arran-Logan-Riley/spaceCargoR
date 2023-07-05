Open Postman
Set to Post
Set link: http://localhost:8080/
Add spacectaft to body
{
  "Name": "ShipName",
  "FutureDestination": "Destination1",
  "PastDestination": "Destination2",
  "Cargo": "CargoName",
  "Crew": "CrewName",
  "Age": 5,
  "CurrentLocation": "CurrentLocationName",
  "Tonnage": 100.5,
  DockBayReq: "D"
}
click send

To delete use url
Set to Delete
http://localhost:8080/spacecraft?name=ShipName