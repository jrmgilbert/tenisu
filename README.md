# Tenisu

Application d'exploration de données et de statistiques sur le Tennis et ses meilleurs joueurs.

## Comment lancer ?

### Prérequis

- [SDK .NET 10](https://dotnet.microsoft.com/download/dotnet/10.0) (version figée dans `global.json`)
- Optionnel : Docker, pour lancer l'image en local
- Optionnel : un Visual Studio récent (support de Microsoft.Testing.Platform dans l'explorateur de tests)

### En local

- Depuis `backend/` : `dotnet run --project src/Tenisu.Api`
- L'API répond sur http://localhost:5287, la documentation interactive est sur http://localhost:5287/scalar
- Dans Visual Studio, F5 ouvre directement la page Scalar
- `src/Tenisu.Api/Tenisu.Api.http` contient des requêtes prêtes à l'emploi (Visual Studio, Rider ou VS Code + REST Client)

### Tests

- Toute la suite : `dotnet test` depuis `backend/`
- Un seul groupe de tests : `dotnet test --filter-query "/*/*/GetPlayer*" --ignore-exit-code 8`

### Avec Docker

- Construire l'image (sans Dockerfile, c'est le SDK qui s'en charge) :
  `dotnet publish src/Tenisu.Api -c Release --os linux --arch x64 -t:PublishContainer -p ContainerRegistry= -p ContainerImageTag=local`
- La lancer : `docker run --rm -p 8080:8080 jrmgilbert/tenisu-api:local`, puis http://localhost:8080/scalar

### Endpoints

| Méthode | Route                         | Rôle                                                                   |
| ------- | ----------------------------- | ---------------------------------------------------------------------- |
| GET     | `/api/players`                | Tous les joueurs, du meilleur au moins bon (`?sort=BestFirst`, défaut) |
| GET     | `/api/players/{id}`           | Un joueur (404 si inconnu)                                             |
| GET     | `/api/players/statistics`     | Pays au meilleur ratio de victoires, IMC moyen, taille médiane         |
| POST    | `/api/players`                | Ajoute un joueur (201, 400 si invalide, 409 si nom court déjà pris)    |
| GET     | `/healthcheck`                | `Healthy` (200) ou `Unhealthy` (503)                                   |
| GET     | `/scalar`, `/openapi/v1.json` | Documentation de l'API                                                 |

### Déploiement

- Chaque push sur `main` qui touche le backend lance la CI, publie l'image publique `ghcr.io/jrmgilbert/tenisu-api:1.0.<n>`
  et la déploie sur Azure Container Apps (France Central)
- `dev` et les pull requests ne lancent que la CI (format, build sans warning, tests)
- Mise en place Azure, une seule fois : `IMAGE=ghcr.io/jrmgilbert/tenisu-api:1.0.1 bash deploy/provision.sh`
  (nécessite `az` et `gh` connectés ; le script affiche l'URL publique à la fin)

## Décisions et notes

### Stack

- .NET 10 (LTS la plus récente), ASP.NET Core en Minimal APIs, EF Core 10 sur SQLite en mémoire
- OpenAPI natif + Scalar pour la documentation
- Tests : xUnit v3 sur Microsoft.Testing.Platform, `WebApplicationFactory` pour les tests HTTP
- Observabilité : OpenTelemetry vers Azure Monitor Application Insights
- CI/CD : GitHub Actions, images sur GitHub Container Registry, hébergement sur Azure Container Apps

### Architecture

- Clean Architecture « pragmatique » en 4 projets, dépendances vers l'intérieur :
  `Tenisu.Core` (domaine) ← `Tenisu.Application` (use cases, DTOs) ← `Tenisu.Infrastructure` (EF Core, seed) ← `Tenisu.Api`
- Une seule abstraction : `ITenisuDbContext`. Pas de repository générique, ni MediatR, ni AutoMapper : le `DbContext` suffit
- Dossiers organisés par fonctionnalité (`Players/`), pas par type technique
- Les règles du domaine n'arrivent que lorsqu'un endpoint en a besoin

### Données

- `Data/headtohead.json` est lu au démarrage et injecté dans une base SQLite en mémoire (`UseAsyncSeeding`)
- Une connexion « keep-alive » garde la base en vie ; chaque requête utilise son propre `DbContext`
- Le seed passe par les factories du domaine : un fichier invalide fait échouer le démarrage, plutôt que de servir des données fausses

### Domaine et règles métier

- Tri des joueurs « du meilleur au moins bon » : rang croissant, puis points décroissants, puis id (ordre stable). Le tri tourne en SQL
  (`PlayerOrderings`) ; un `IComparer` équivalent (`PlayerRanking`) reste disponible pour trier en mémoire
- Statistiques : le ratio de victoires additionne tous les matchs des joueurs d'un pays ; en cas d'égalité, le premier code
  par ordre alphabétique gagne. IMC = kg / m². Taille médiane classique (moyenne des deux valeurs centrales si nombre pair)

### API

- REST : `POST` sur la collection pour créer (201 + en-tête `Location`), 405 automatique sur les autres méthodes
- Toutes les erreurs sont des ProblemDetails (`application/problem+json`)
- Validation en couches : sur la requête (validation native .NET 10), puis le domaine, puis l'unicité du nom court (409),
  et contraintes en base de données
- Les champs JSON suivent le nommage des entités du domaine (`firstName`, `shortName`) ; le `POST` accepte aussi la casse du fichier (`firstname`)
- La documentation Scalar est publique en Production (pour la démo et les tests manuels)

### Qualité

- Code et commentaires en Anglais, pour équipe potentiellement internationale, mais README en Français pour les utilisateurs
- Développement piloté par les tests, sur 3 niveaux : domaine (pur), application (vraie base SQLite, sans mocks), HTTP
- Les tests utilisent une copie des vraies données (`TestData/headtohead.json`)
- Analyseurs .NET en mode `10.0-recommended` et règles de style vérifiées au build ; la CI refuse le moindre warning

## Limitations

### Données

- Base de données en mémoire : chaque redémarrage ou déploiement repart du fichier JSON, les joueurs ajoutés par `POST` sont perdus
- Une seule instance possible (sinon chaque réplique aurait sa propre copie des données)
- Mise à l'échelle du service à zéro sur Azure : le premier appel froid après une inactivité prend quelques secondes
- Pas de migrations (`EnsureCreated`) : il faudrait en ajouter pour passer à une vraie base
- Hypothèses sur le fichier : `last` = derniers résultats (1 = victoire, 0 = défaite)
- Les nouveaux joueurs reçoivent l'id suivant le plus grand id du fichier ; un id envoyé par le client est ignoré

### Performances

- Pas de pagination sur la liste des joueurs : à envisager en fonction de la taille prévue
- Les statistiques chargent toute la table des joueurs en mémoire à chaque appel
- Pas de cache, ni de limitation de débit (rate limiting) pour l'instant

### API

- Les valeurs de `?sort=` sont sensibles à la casse (`BestFirst`)
- Les clés des erreurs de validation suivent les noms C# (`Data.Rank`), pas les noms JSON (`data.rank`)
- Le nom court doit déjà respecter le format `N.DJO` dans la requête
- La règle « chaque résultat vaut 0 ou 1 » n'est vérifiée que si les autres champs de `data` sont valides
