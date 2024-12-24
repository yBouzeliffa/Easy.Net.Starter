# ConfigurationLibrary

**ConfigurationLibrary** est une bibliothèque .NET conçue pour simplifier la configuration des applications Console et API. Elle offre une solution facile et intuitive pour gérer :

- La configuration via des fichiers `appsettings.json`.
- La gestion centralisée des logs.
- L'initialisation rapide de la configuration pour différents environnements.

---

## Fonctionnalités

- **Support de `appsettings.json`** : Chargement et gestion des configurations via JSON avec support des sections hiérarchiques.
- **Gestion des environnements** : Configure automatiquement les environnements (`Development`, `Production`, etc.).
- **Intégration des logs** : Prise en charge de la journalisation via `ILogger` avec une configuration simplifiée pour `Console`, `File` ou tout autre fournisseur.
- **Extension facile** : Ajoutez vos propres paramètres personnalisés grâce à des mécanismes d’injection de dépendances.

---

## Installation

Ajoutez **ConfigurationLibrary** à votre projet via NuGet :

```bash
Install-Package ConfigurationLibrary
```

Ou via la CLI .NET :

```bash
dotnet add package ConfigurationLibrary
```

---

## Utilisation

### 1. Initialisation dans une Application Console

Ajoutez et configurez `appsettings.json` :

```json
{
  "AppSettings": {
    "Setting1": "Valeur1",
    "Setting2": "Valeur2"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning"
    }
  }
}
```

Exemple de configuration dans `Program.cs` :

```csharp
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

class Program
{
    static void Main(string[] args)
    {
        // ConfigurationBuilder pour charger appsettings.json
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        // Configuration des services et des logs
        var serviceCollection = new ServiceCollection()
            .AddLogging(builder =>
            {
                builder.AddConsole();
                builder.AddConfiguration(configuration.GetSection("Logging"));
            })
            .AddSingleton<IConfiguration>(configuration);

        var serviceProvider = serviceCollection.BuildServiceProvider();

        // Exemple d'utilisation de la configuration et des logs
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        var setting1 = configuration["AppSettings:Setting1"];

        logger.LogInformation($"Setting1: {setting1}");
    }
}
```

### 2. Initialisation dans une API ASP.NET Core

Ajoutez `ConfigurationLibrary` pour configurer les logs et fichiers de configuration dans `Program.cs` :

```csharp
var builder = WebApplication.CreateBuilder(args);

// Configuration des logs et de appsettings.json
builder.Host.ConfigureLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
})
.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.Run();
```

---

## Contribution

Les contributions sont les bienvenues ! Si vous souhaitez signaler un bug ou proposer une nouvelle fonctionnalité :

1. Forkez le repository.
2. Créez une branche pour votre fonctionnalité ou correctif (`git checkout -b feature/nouvelle-fonctionnalite`).
3. Effectuez vos modifications.
4. Créez une Pull Request.

---

## Licence

Ce projet est sous licence [MIT](LICENSE).

---

## Avenir du Projet

Nous prévoyons d’ajouter prochainement :

- Support des configurations via des sources distantes (par exemple, Azure App Configuration).
- Intégration avec d'autres bibliothèques populaires comme Serilog et NLog.
- Plus d'exemples d'utilisation dans divers scénarios.

---

### Auteur
**VotreNom**  
Pour toute question ou demande, n'hésitez pas à me contacter.

