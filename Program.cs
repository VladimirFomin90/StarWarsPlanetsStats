using System.Text.Json;
using StarWarsPlanetsStats.ApiDataAccess;
using StarWarsPlanetsStats.DTO;

// string json = null;

try
{
IApiDataReader apiDataReader = new ApiDataReader();
// var json = await apiDataReader.Read("https://swapi.dev/", "api/planets");
var json = await apiDataReader.Read("https://swapi.dev/", "api/planets");
}
catch (HttpRequestException e)
{
    Console.WriteLine("API request was unsuccessful" + e.Message);
    throw;
}

var root = JsonSerializer.Deserialize<Root>(json);

Console.WriteLine("Press any key to close");
Console.ReadKey();

// Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);