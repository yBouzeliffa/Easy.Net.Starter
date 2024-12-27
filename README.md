# Easy.Net.Starter

**Easy.Net.Starter** est une bibliothèque .NET conçue pour accélérer le démarrage de projets **Console** ou **API**. Elle propose une configuration prête à l'emploi avec :

- **Logs** intégrés via **Serilog**.
- Gestion centralisée des **settings** (`appsettings.json`) pour différents environnements.
- Mise en place automatique du **Dependency Injection (DI)**.
- Support de la **base de données** (PostgreSQL avec conventions SnakeCase).
- Configuration rapide des **CORS**, middlewares, et capacités API (Swagger, santé, etc.).

---

## Fonctionnalités principales

1. **Initialisation des logs avec Serilog** :
   Enregistre automatiquement les logs dans la console ou des fichiers.

2. **Compilation et enregistrement des settings** :
   Charge les paramètres depuis `appsettings.json` et les expose via le DI.

3. **Support PostgreSQL** :
   Ajout facile d'un contexte Entity Framework avec une configuration par défaut.

4. **API Ready** :
   Active les capacités d'API (Swagger, contrôleurs, endpoints de santé) en une ligne.

5. **CORS** :
   Configuration simplifiée pour les origines, méthodes, et headers autorisés.

6. **Gestion des middlewares** :
   Ajout dynamique de middlewares personnalisés via réflexion.

---

## Installation

Ajoutez **Easy.Net.Starter** à votre projet via NuGet :

```bash
dotnet add package Easy.Net.Starter
```

---

## Exemple d'utilisation

### Initialisation dans une API

```csharp
using Easy.Net.Starter.Api;
using Easy.Net.Starter.App;
using Easy.Net.Starter.ProgramsEntries;

var startupOptions = new StartupBuilder()
    .AddScoped<ITestServiceWithInterface, TestServiceWithInterface>()
    .AddHttpLoggerMiddleware()
    .Build();

var app = WebApiStarter.Start<CustomAppSettings>(args, startupOptions);

app.Run();

```

### Initialisation avec une base de données

```csharp
using Easy.Net.Starter.Api;
using Easy.Net.Starter.App;
using Easy.Net.Starter.ProgramsEntries;

var startupOptions = new StartupBuilder()
    .AddDatabase<MyDatabaseContext>()
    .AddScoped<ITestServiceWithInterface, TestServiceWithInterface>()
    .AddHttpLoggerMiddleware()
    .Build();

var app = WebApiStarter.Start<CustomAppSettings>(args, startupOptions);

app.Run();

```

---

## Contribution

Les contributions sont bienvenues ! Si vous souhaitez proposer une amélioration ou signaler un bug, ouvrez une issue ou soumettez une Pull Request.

---

## Licence

Ce projet est sous licence [MIT](LICENSE).

---