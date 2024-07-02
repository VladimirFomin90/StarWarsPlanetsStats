using StarWarsPlanetsStats.ApiDataAccess;

IApiDataReader apiDataReader = new ApiDataReader();
var json = await apiDataReader.Read("https://swapi.dev/", "api/planets");

Console.WriteLine("Press any key to close");
Console.ReadKey();