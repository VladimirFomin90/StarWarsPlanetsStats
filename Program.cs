using System.Text.Json;
using StarWarsPlanetsStats.ApiDataAccess;
using StarWarsPlanetsStats.DTO;

try
{
    await new StarWarsPlanetsStatsApp(new ApiDataReader()).Run();
}
catch (Exception e)
{
    Console.WriteLine("An error has occurred" + e.Message);
}

Console.WriteLine("Press any key to close");
Console.ReadKey();

public class StarWarsPlanetsStatsApp
{
    public readonly IApiDataReader _apiDataReader;

    public StarWarsPlanetsStatsApp(IApiDataReader apiDataReader)
    {
        _apiDataReader = apiDataReader;
    }

    public async Task Run()
    {
        string? json = null;

        try
        {
            // var json = await apiDataReader.Read("https://swapi.dev/", "api/planets");
            json = await _apiDataReader.Read("https://swapi.dev/", "api/planets");
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine("API request was unsuccessful" + e.Message);
            throw;
        }

        var root = JsonSerializer.Deserialize<Root>(json);
        var planets = ToPlanets(root);
    }

    private IEnumerable<Planet> ToPlanets(Root? root)
    {
        if (root is null)
        {
            throw new ArgumentException(nameof(root));
        }

        throw new NotImplementedException();
    }
}

public readonly record struct Planet
{
    public string Name { get; }
    public int Diameter { get; }
    public int? SurfaceWater { get; }
    public int? Population { get; }

    public Planet(
        string name,
        int diameter,
        int? surfacewater,
        int? population
    )
    {
        if (name is null)
        {
            throw new ArgumentException(nameof(name));
        }
        Name = name;
        Diameter = diameter;
        SurfaceWater = surfacewater;
        Population = population;
    }
}

// Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);