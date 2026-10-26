# WPFW Portfolio API

Deze repository bevat de backend voor een portfoliosite. De API biedt volledige CRUD voor projecten en blogposts en bewaart gegevens via Entity Framework Core in SQL Server.

Repository: [github.com/JustKurayy/wpfw-api](https://github.com/JustKurayy/wpfw-api)

## Techniek en keuzes

- .NET SDK 10.0.400 met target framework `net10.0`
- ASP.NET Core Web API met controllers
- Lagenstructuur met Controllers, Services en Data
- EF Core 10 met SQL Server en een migration
- Aparte request- en response-DTO's
- Swagger UI via Swashbuckle.AspNetCore
- xUnit-tests, inclusief een persistentietest op SQL Server Express

De controller bepaalt het HTTP-resultaat. De service bevat applicatielogica, EF Core-aanroepen en DTO-mapping. `PortfolioDbContext` vormt de datalaag. Controllers ontvangen services via constructorinjectie en gebruiken de DbContext nooit rechtstreeks.

## Vereisten

- .NET 10 SDK
- SQL Server 2019 of nieuwer, SQL Server Express werkt ook
- Een Windows-account dat toegang heeft tot de gekozen SQL Server-database

Controleer de SDK:

```powershell
dotnet --version
```

De verwachte uitvoer begint met `10.`.

## Database configureren

De standaard connectionstring in `appsettings.json` gebruikt de lokale instantie `localhost\SQLEXPRESS` en database `WpfwPortfolio` met Windows-authenticatie:

```text
Server=localhost\SQLEXPRESS;Database=WpfwPortfolio;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true
```

Pas alleen de servernaam of database aan als jouw SQL Server anders heet. Zet wachtwoorden niet in de repository. Gebruik bij SQL-authenticatie .NET user secrets of een lokale omgevingsvariabele.

Deze applicatie maakt de database aan met EF Core migrations. Herstel eerst de lokale tool en voer daarna de migration uit:

```powershell
dotnet tool restore
dotnet tool run dotnet-ef database update
```

De migration staat in `Migrations/` en maakt de tabellen `Projects` en `BlogPosts`.

## Applicatie starten

```powershell
dotnet restore
dotnet run --launch-profile https
```

Open daarna [https://localhost:7043/swagger](https://localhost:7043/swagger). Het HTTP-profiel gebruikt [http://localhost:5001/swagger](http://localhost:5001/swagger).

## Endpoints

| Resource | Methode | Route | Resultaat |
| --- | --- | --- | --- |
| Project | GET | `/api/projects` | Alle projecten |
| Project | GET | `/api/projects/{id}` | Eén project of 404 |
| Project | POST | `/api/projects` | Nieuw project met 201 |
| Project | PUT | `/api/projects/{id}` | Gewijzigd project of 404 |
| Project | DELETE | `/api/projects/{id}` | 204 of 404 |
| Blogpost | GET | `/api/blogposts` | Alle blogposts |
| Blogpost | GET | `/api/blogposts/{id}` | Eén blogpost of 404 |
| Blogpost | POST | `/api/blogposts` | Nieuwe blogpost met 201 |
| Blogpost | PUT | `/api/blogposts/{id}` | Gewijzigde blogpost of 404 |
| Blogpost | DELETE | `/api/blogposts/{id}` | 204 of 404 |

Swagger toont alleen de DTO-contracten. De create-DTO's bevatten geen `Id`, want SQL Server genereert de sleutel.

## Tests uitvoeren

```powershell
dotnet test WpfwApi.slnx
```

De tests gebruiken SQLite alleen voor geïsoleerde service-unittests. De applicatie zelf gebruikt uitsluitend SQL Server. De integratietest met categorie `SqlServer` maakt een tijdelijke SQL Server-database, voert migrations uit, doorloopt CRUD voor beide resources, start een nieuwe applicatiehost en controleert persistentie.

Alleen de SQL Server-integratietest uitvoeren:

```powershell
dotnet test tests/WpfwApi.Tests/WpfwApi.Tests.csproj --filter Category=SqlServer
```

## Bronnen

- [Microsoft Learn controllergebaseerde Web API](https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-10.0)
- [Microsoft Learn EF Core SQL Server-provider](https://learn.microsoft.com/en-us/ef/core/providers/sql-server/)
