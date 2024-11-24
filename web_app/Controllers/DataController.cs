using Cassini.Managers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;
using static System.Runtime.InteropServices.JavaScript.JSType;

[ApiController]
[Route("api/[controller]")]
public class DataController : ControllerBase
{
    private readonly HttpClient _httpClient;

    public DataController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
    }

    [HttpGet]
    public async Task<IActionResult> GetData()
    {
        // URL of the Azure Storage where the JSON file is stored
        string storageUrl = "https://cassinidev.blob.core.windows.net/data/sample_turkey.json";

        // Fetch the JSON from Azure Storage
        HttpResponseMessage response = await _httpClient.GetAsync(storageUrl);

        if (!response.IsSuccessStatusCode)
        {
            return BadRequest("Failed to fetch the JSON data.");
        }

        // Read the JSON content
        string jsonString = await response.Content.ReadAsStringAsync();

        // Deserialize JSON into a list of objects
        var dataBefore = JsonConvert.DeserializeObject<List<Point>>(jsonString)
            .Where(x => x.date == "2023/01/28");

        var dataAfter = JsonConvert.DeserializeObject<List<Point>>(jsonString)
            .Where(x => x.date == "2023/02/09");

        var data = dataBefore.GroupJoin(dataAfter,
            before => new { before.x, before.y },
            after => new { after.x, after.y },
            (before, after) => new
            {
                before,
                after = after.First()
            }).Where(x => Math.Abs(x.before.z - x.after.z) > 8)
            .Select(x => new Point()
            {
                x = x.before.x,
                y = x.before.y,
                z = x.before.z - x.after.z,
                date = x.before.date
            }).ToList();

        float maxDelta = data.Max(x => x.z);
        float minDelta = data.Min(x => x.z);

        data = data.Select(x => new Point()
        {
            x = x.x,
            y = x.y,
            z = x.z > 0 ? ((int) (5 * x.z / maxDelta + 0.5)) : -((int) (5 * x.z / minDelta + 0.5)),
            date = x.date
        }).ToList();


        // Top-left corner of Antakya (latitude and longitude)
        double topLeftLat = 36.20851804388457;
        double topLeftLon = 36.148692878491327;

        double bottomRightLat = 36.202254495488305;
        double bottomRightLon = 36.15976831740696;

        int verticalResolution = 73;
        int horizontalResolution = 103;

        // Cell size (distance in degrees for each x and y step)
        double cellSizeLat = (bottomRightLat - topLeftLat) / verticalResolution; // Change in latitude for each step (north-south)
        double cellSizeLon = (bottomRightLon - topLeftLon) / horizontalResolution;  // Change in longitude for each step (east-west)

        // Aggregation factor (process every 4-th point in x and y)
        int aggregationFactor = 4;

        // Aggregate data
        var aggregatedData = data
            .Where((_, index) => index % aggregationFactor == 0)
            .ToList();

        // Convert aggregated data to GeoJSON with proper coordinates
        var geoJson = new
        {
            type = "FeatureCollection", 
            features = aggregatedData.ConvertAll(point =>
            {
                double latitude = topLeftLat + (point.y * cellSizeLat);
                double longitude = topLeftLon + (point.x * cellSizeLon);

                return new
                {
                    type = "Feature",
                    geometry = new
                    {
                        type = "Point",
                        coordinates = new[] { longitude, latitude } // Only longitude and latitude here
                    },
                    properties = new
                    {
                        z = point.z // Include z as a property
                    }
                };
            })
        };

        // Serialize GeoJSON and return
        return Ok(geoJson);

    }

    // Point class for deserializing JSON data
    private class Point
    {
        public int x { get; set; }
        public int y { get; set; }
        public float z { get; set; }
        public string date { get; set; } 
    }
}