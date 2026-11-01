# WPFW Portfolio API

Backend voor een portfoliosite, gebouwd met .NET 10, ASP.NET Core controllers en een lagenstructuur met Controllers, Services en Data. De API gebruikt EF Core en SQL Server voor projecten en blogposts.

## Starten

Vereisten: .NET 10 SDK en SQL Server Express op `localhost\SQLEXPRESS`.

```powershell
dotnet tool restore
dotnet tool run dotnet-ef database update
dotnet run --launch-profile http
```

Swagger UI staat daarna op `http://localhost:5001/swagger`.

De connectionstring staat in `appsettings.json`. De migration `20261026093506_InitialCreate` maakt de tabellen `Projects` en `BlogPosts` aan.
