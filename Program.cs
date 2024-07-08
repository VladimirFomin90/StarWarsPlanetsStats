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

        foreach (var planet in planets)
        {
            Console.WriteLine(planet);
        }
    }

    private IEnumerable<Planet> ToPlanets(Root? root)
    {
        if (root is null)
        {
            throw new ArgumentException(nameof(root));
        }

        var planets = new List<Planet>();

        foreach (var planetDto in root.results)
        {
            Planet planet = (Planet)planetDto;
            planets.Add(planet);
        }
        return planets;
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

    public static explicit operator Planet(Result planetDto)
    {
        var name = planetDto.name;
        var diameter = int.Parse(planetDto.diameter);

        int? population = planetDto.population.ToIntOrNull();

        int? surfaceWater = planetDto.surface_water.ToIntOrNull();

        return new Planet(name, diameter, population, surfaceWater);
    }
}

public static class StringExtension
{
    public static int? ToIntOrNull(this string? input)
    {
        int? result = null;
        if (int.TryParse(input, out int resultParsed))
        {
            result = resultParsed;
        }

        return result;
    }
}