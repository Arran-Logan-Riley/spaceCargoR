using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

public class SpaceStation
{
    public string Name { get; set; }
    public string Location { get; set; }
    public List<DockBay> DockBays { get; set; }
    public List<SpaceCraft> DockedSpaceCraft { get; set; }
    public List<string> Services { get; set; }
    public List<string> Personnel { get; set; }
    public List<string> SecurityMeasures { get; set; }
    public string MaintenanceSchedule { get; set; }
    public bool IsAvailable { get; set; }
    public List<DockingHistory> DockingHistory { get; set; }
}

public class DockingHistory
{
    public string SpaceCraftName { get; set; }
    public DateTime DockingTime { get; set; }
    public DateTime DepartureTime { get; set; }
}

public class SpaceCraft
{
    public string Name { get; set; }
    public string FutureDestination { get; set; }
    public string PastDestination { get; set; }
    public string Cargo { get; set; }
    public string Crew { get; set; }
    public int Age { get; set; }
    public string CurrentLocation { get; set; }
    public double Tonnage { get; set; }
    public string DockBayReq { get; set; }
}

public class DockBay
{
    public string Name { get; set; }
    public int MaxTonnage { get; set; }
    public int MinTonnage { get; set; }
    public bool IsAvailable { get; set; }
    public bool AirLockIsOpen { get; set; }
    public bool SpaceCraftIsDocked { get; set; }
    public List<SpaceCraft> DockedSpaceCraft { get; set; }
}

public class SpaceCraftAPI
{
    private static HttpListener listener;
    public static SpaceStation spaceStation;

    public static void Main(string[] args)
    {
        // Init Dock Bays

        // Init a space station
        spaceStation = new SpaceStation()
        {
            Name = "Space Station Alpha",
            Location = "Orbit around Earth",
            DockBays = new List<DockBay> {
                new DockBay{Name = "A", MaxTonnage=100, MinTonnage= 10, IsAvailable=false, AirLockIsOpen=false, SpaceCraftIsDocked=true},
                new DockBay{Name = "B", MaxTonnage=1000, MinTonnage= 100, IsAvailable=false, AirLockIsOpen=false, SpaceCraftIsDocked=true},
                new DockBay{Name = "C", MaxTonnage=10000, MinTonnage= 1000, IsAvailable=false, AirLockIsOpen=false, SpaceCraftIsDocked=true},
                new DockBay{Name = "D", MaxTonnage=150, MinTonnage= 50, IsAvailable=true, AirLockIsOpen=true, SpaceCraftIsDocked=false}
            },
            DockedSpaceCraft = new List<SpaceCraft>
            {
                new SpaceCraft
                {
                    Name = "Cosmic Dreamer",
                    FutureDestination = "Nebula NGC 6357",
                    PastDestination = "Black Hole Gargantua",
                    Cargo = "Quantum Energy Crystals",
                    Crew = "6",
                    Age = 3,
                    CurrentLocation = "Space Station Alpha",
                    Tonnage = 180.3,
                    DockBayReq = "A"
                },
                new SpaceCraft
                {
                    Name = "Stellar Voyager",
                    FutureDestination = "Alpha Centauri",
                    PastDestination = "Andromeda Galaxy",
                    Cargo = "Exotic Matter Samples",
                    Crew = "12",
                    Age = 5,
                    CurrentLocation = "Space Station Alpha",
                    Tonnage = 250.7,
                    DockBayReq = "B"
                },
                new SpaceCraft
                {
                    Name = "Star Hopper",
                    FutureDestination = "Galactic Core",
                    PastDestination = "Supernova Remnant",
                    Cargo = "Alien Artifacts",
                    Crew = "8",
                    Age = 2,
                    CurrentLocation = "Space Station Alpha",
                    Tonnage = 210.1,
                    DockBayReq = "C"
                }
            },
            Services = new List<string> { "Fueling", "Repairs", "Cargo Handling" },
            Personnel = new List<string> { "Engineers", "Technicians", "Operators" },
            SecurityMeasures = new List<string> { "Biometric access control", "Surveillance systems" },
            MaintenanceSchedule = "Regular maintenance scheduled every month",
            IsAvailable = true,
            DockingHistory = new List<DockingHistory>
            {
                new DockingHistory { SpaceCraftName = "SpaceShip001", DockingTime = DateTime.Now.AddDays(-2), DepartureTime = DateTime.Now.AddDays(-1) },
                new DockingHistory { SpaceCraftName = "SpaceShip002", DockingTime = DateTime.Now.AddDays(-5), DepartureTime = DateTime.Now.AddDays(-3) }
            }
        };

        // Add the initiated spacecraft to the respective dock bays
        foreach (var spacecraft in spaceStation.DockedSpaceCraft)
        {
            // Find the dock bay where the spacecraft should be placed
            DockBay dockBay = spaceStation.DockBays.FirstOrDefault(b => b.Name == spacecraft.DockBayReq);

            if (dockBay != null && dockBay.SpaceCraftIsDocked)
            {
                dockBay.DockedSpaceCraft = new List<SpaceCraft> { spacecraft };
                dockBay.SpaceCraftIsDocked = true;
                Console.WriteLine(spacecraft.Name);
            }
        }

        // Create an HttpListener and start listening for requests
        listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:8080/");
        listener.Start();

        Console.WriteLine("Space Dock is listening for requests...");

        // Handle the request asynchronously
        ThreadPool.QueueUserWorkItem(HandleRequests);

        Console.WriteLine("Press any key to stop the server");
        Console.ReadKey();

        // Stop listening for requests
        listener.Stop();
        listener.Close();
    }

    private static void HandleRequests(object state)
    {
        while (listener.IsListening)
        {
            // Get the incoming request
            HttpListenerContext context = listener.GetContext();

            // Create a response object
            HttpListenerResponse response = context.Response;

            // Set the response headers
            response.ContentType = "application/json";
            response.StatusCode = 200;

            // Process the request based on the HTTP method
            if (context.Request.HttpMethod == "POST")
            {
                // Read the request body
                string requestBody;
                using (var reader = new System.IO.StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
                {
                    requestBody = reader.ReadToEnd();
                }

                // Deserialize the ship data from the request body
                SpaceCraft spaceCraft = Newtonsoft.Json.JsonConvert.DeserializeObject<SpaceCraft>(requestBody);
                // This is where the data processing is done

                // Add the spacecraft to the list
                spaceStation.DockedSpaceCraft.Add(spaceCraft);

                //Add to its requested dock bay
                DockBay dockBay = spaceStation.DockBays.FirstOrDefault(b => b.Name == spaceCraft.DockBayReq);
                //Make sure the dock bay exists and that it is not currently occupied
                if (dockBay != null && dockBay.SpaceCraftIsDocked == false)
                {
                    dockBay.DockedSpaceCraft = new List<SpaceCraft> { spaceCraft };
                    dockBay.SpaceCraftIsDocked = true;
                }
                else
                {
                    Console.WriteLine("Cannot find bay or bay does not exist");
                }

                // Prepare the response
                string responseBody = "Spacecraft successfully docked!";
                byte[] buffer = Encoding.UTF8.GetBytes(responseBody);

                // Write the response body
                response.ContentLength64 = buffer.Length;
                response.OutputStream.Write(buffer, 0, buffer.Length);
            }
            else if (context.Request.HttpMethod == "DELETE")
            {
                // Get the identifier of the spacecraft to undock
                string spaceCraftName = context.Request.QueryString["name"];

                // Find the spacecraft on the list
                SpaceCraft spacecraftToRemove = spaceStation.DockedSpaceCraft.FirstOrDefault(s => s.Name == spaceCraftName);

                if (spacecraftToRemove != null)
                {
                    spaceStation.DockedSpaceCraft.Remove(spacecraftToRemove);

                    // Prepare the response
                    string responseBody = $"Spacecraft [{spaceCraftName}] has undocked from the space station";
                    byte[] buffer = Encoding.UTF8.GetBytes(responseBody);
                    // Write the response
                    response.ContentLength64 = buffer.Length;
                    response.OutputStream.Write(buffer, 0, buffer.Length);
                }
                else
                {
                    string responseBody = $"Spacecraft [{spaceCraftName}] cannot be found";
                    byte[] buffer = Encoding.UTF8.GetBytes(responseBody);
                    response.StatusCode = 404;
                    response.ContentLength64 = buffer.Length;
                    response.OutputStream.Write(buffer, 0, buffer.Length);
                }
            }
            else if (context.Request.HttpMethod == "GET")
            {
                // Serialize the dock bay list to JSON
                string responseBody = Newtonsoft.Json.JsonConvert.SerializeObject(spaceStation.DockedSpaceCraft);

                // Convert the response to a byte array
                byte[] buffer = Encoding.UTF8.GetBytes(responseBody);

                // Set the response content length and write the response
                response.ContentLength64 = buffer.Length;
                response.OutputStream.Write(buffer, 0, buffer.Length);
            }
            else
            {
                // Invalid HTTP method, return 405 Method Not Allowed
                response.StatusCode = 405;
            }

            // Close the response
            response.Close();
        }
    }
}
