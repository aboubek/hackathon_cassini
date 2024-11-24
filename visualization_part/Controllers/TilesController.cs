using Cassini.Managers;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class TileController : ControllerBase
{
    private readonly HttpClient _httpClient;

    public TileController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
    }

    [HttpPost]
    public async Task<IActionResult> GetTiles([FromBody] TileRequest request)
    {
        // Validate the request
        if (request == null || request.Zoom < 0)
        {
            return BadRequest("Invalid request.");
        }

        try
        {

            // Layer IDs for 2018 and 2024
            var layer2018 = "13161"; // Replace with the actual 2018 layer ID
            var layer2024 = "49849"; // Replace with the actual 2024 layer ID

            // Construct URLs for 2018 and 2024 tiles
            var tile2018Url = $"https://wayback.maptiles.arcgis.com/arcgis/rest/services/World_Imagery/WMTS/1.0.0/default028mm/MapServer/tile/{layer2018}/{request.Zoom}/{request.TileY}/{request.TileX}";
            var tile2024Url = $"https://wayback.maptiles.arcgis.com/arcgis/rest/services/World_Imagery/WMTS/1.0.0/default028mm/MapServer/tile/{layer2024}/{request.Zoom}/{request.TileY}/{request.TileX}";

            tile2018Url = "https://wayback.maptiles.arcgis.com/arcgis/rest/services/world_imagery/wmts/1.0.0/default028mm/mapserver/tile/13161/16/23022/39613";
            tile2024Url = "https://wayback.maptiles.arcgis.com/arcgis/rest/services/world_imagery/wmts/1.0.0/default028mm/mapserver/tile/49849/16/23022/39613";

            // Fetch tiles
            var tile2018 = await _httpClient.GetByteArrayAsync(tile2018Url);
            var tile2024 = await _httpClient.GetByteArrayAsync(tile2024Url);


            // Convert images to Base64
            var base64Image1 = Convert.ToBase64String(tile2018);
            var base64Image2 = Convert.ToBase64String(tile2024);

            AIImageAnalyzer _openAiHelper = new AIImageAnalyzer(_httpClient, "keys-removed");

            // Analyze images using OpenAI
            var analysisResult = await _openAiHelper.AnalyzeImages(base64Image1, base64Image2);


            // Return fetched tiles (as base64 strings or process them further)
            return Ok(new
            {
                Message = "Tiles fetched successfully.",
                Tile2018 = Convert.ToBase64String(tile2018), // Base64 encode the tile for easy transfer
                Tile2024 = Convert.ToBase64String(tile2024)
            });
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(500, $"Error fetching tiles: {ex.Message}");
        }
    }
}

// Define the request model
public class TileRequest
{
    public double Lat { get; set; } // Latitude of the click
    public double Lng { get; set; } // Longitude of the click
    public int Zoom { get; set; } // Current zoom level
    public int TileX { get; set; } // Tile X coordinate
    public int TileY { get; set; } // Tile Y coordinate
}