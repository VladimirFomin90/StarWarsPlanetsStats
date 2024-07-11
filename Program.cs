﻿using StarWarsPlanetsStats.ApiDataAccess;
using StarWarsPlanetsStats.DTO;
using System.Text.Json;

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
            json ??= await _apiDataReader.Read("https://swapi.dev/", "api/planets");
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
            ShowStatistic(planets, "population", planet => planet.Population);

        }
        else if (userChoice == "diameter")
        {
            ShowStatistic(planets, "diameter", planet => planet.Diameter);

        }
        else if (userChoice == "surface water")
        {
            ShowStatistic(planets, "surface water", planet => planet.SurfaceWater);

        }
        else
        {
            Console.WriteLine("Invalid choice");
        }
    }

    private static void ShowStatistic(IEnumerable<Planet> planets, string propertyName, Func<Planet, int?> propertySelector)
    {
        var planetMaxProperty = planets.MaxBy(propertySelector);
        Console.WriteLine($"Max {propertyName} : " + $"{propertySelector(planetMaxProperty)} " +
            $"Planet : " + $"{planetMaxProperty.Name}");

        var planetMinProperty = planets.MinBy(propertySelector);
        Console.WriteLine($"Min {propertyName} : " + $"{propertySelector(planetMinProperty)} " +
            $"Planet : " + $"{planetMinProperty.Name}");
    }

    private static IEnumerable<Planet> ToPlanets(Root? root)
    {
        if (root is null)
        {
            throw new ArgumentException(nameof(root));
        }

        return root.results.Select(planetDto => (Planet)planetDto);
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
        return int.TryParse(input, out int resultParsed) ? resultParsed : null;
    }
}