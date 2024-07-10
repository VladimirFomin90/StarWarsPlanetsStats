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

        Console.WriteLine();
        Console.WriteLine("Print raiting of planets");
        Console.WriteLine("population");
        Console.WriteLine("diameter");
        Console.WriteLine("surface water");
        Console.WriteLine();

        var userChoice = Console.ReadLine();

        if (userChoice == "population")
        {
            var planetMaxPopulation = planets.MaxBy(planet => planet.Population);
            Console.WriteLine($"Max population : " + $"{planetMaxPopulation.Population} " +
                $"Planet : " + $"{ planetMaxPopulation.Name}");

            var planetMinPopulation = planets.MinBy(planet => planet.Population);
            Console.WriteLine($"Min population : " + $"{planetMinPopulation.Population} " +
                $"Planet : " + $"{planetMinPopulation.Name}");
        }
        else if (userChoice == "diameter")
        {
            var planetMaxDiameter = planets.MaxBy(planet => planet.Diameter);
            Console.WriteLine($"Max diameter : " + $"{planetMaxDiameter.Diameter} " +
                $"Planet : " + $"{planetMaxDiameter.Name}");

            var planetMinDiameter = planets.MinBy(planet => planet.Diameter);
            Console.WriteLine($"Min diameter : " + $"{planetMinDiameter.Diameter} " +
                $"Planet : " + $"{planetMinDiameter.Name}");
        }
        else if (userChoice == "surface water")
        {
            var planetMaxSurfaceWater = planets.MaxBy(planet => planet.SurfaceWater);
            Console.WriteLine($"Max surface water : " + $"{planetMaxSurfaceWater.SurfaceWater} " +
                $"Planet : " + $"{planetMaxSurfaceWater.Name}");

            var planetMinSurfaceWater = planets.MinBy(planet => planet.SurfaceWater);
            Console.WriteLine($"Min surface water : " + $"{planetMinSurfaceWater.SurfaceWater} " +
                $"Planet : " + $"{planetMinSurfaceWater.Name}");
        }
        else
        {
            Console.WriteLine("Invalid choice");
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