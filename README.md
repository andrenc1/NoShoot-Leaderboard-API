# NoShoot Leaderboard API

REST API that registers players, stores scores and serves the online leaderboard for [NoShoot](https://github.com/andrenc1/NoShoot), a 2D survivor-like game made in Unity.

## How it works

```
1. Register once
   POST /api/players  { "name": "andrenc1" }
   → 201 { "playerId": 1, "name": "andrenc1", "token": "9F3A...C21B" }
     The token is returned only once. The game stores it locally.

2. Submit each run
   POST /api/scores
   Authorization: Bearer 9F3A...C21B
   { "kills": 62, "survivedSeconds": 50 }
   → 201  The API finds the player by the token and calculates the score.

3. Show the leaderboard
   GET /api/scores/top
   → 200 [ { "playerName": "andrenc1", "bestScore": 900 }, ... ]
```

## Design decisions

**The server calculates the score.** The game sends only the raw results of the run (kills and seconds survived). The API applies the formula, so a modified client can't simply send `"score": 999999`:

```
score = kills × 10 + seconds survived × 1
```

**Players are identified by a token, not by a name.** Each player gets a random 256-bit token at registration and sends it in the `Authorization: Bearer` header. Nobody can submit a score under someone else's name without their token.

**Tokens are never stored in plain text.** The database keeps only a SHA-256 hash of each token. A leaked database doesn't expose usable tokens. SHA-256 is enough here because the tokens are long and fully random (unlike passwords chosen by people, which need a slow hash such as bcrypt).

**Unique names are enforced by the database.** Names are compared case-insensitively (`andrenc1` and `ANDRENC1` are the same name) through a normalized column with a unique index. The endpoint also checks before saving, and a database conflict caused by two simultaneous registrations is still returned as `409 Conflict`.

**Entities and responses are separate.** Endpoints return DTOs (`ScoreResponse`, `LeaderboardEntry`...) instead of database entities, so internal fields such as the token hash are never serialized.

**One entry per player in the leaderboard.** The leaderboard shows each player's best score, so a single player can't fill the top 10.

## Endpoints

| Method | Route | Auth | Description | Responses |
|---|---|---|---|---|
| `POST` | `/api/players` | — | Register a player and get a token | `201`, `400`, `409` |
| `POST` | `/api/scores` | Bearer token | Submit the result of a run | `201`, `400`, `401` |
| `GET` | `/api/scores/top?limit=10` | — | Best score of each player, highest first (`limit` 1–100, default 10) | `200` |
| `GET` | `/api/scores/{id}` | — | A single run by id | `200`, `404` |

### Examples

Register a player:

```http
POST /api/players
Content-Type: application/json

{ "name": "andrenc1" }
```

```json
{ "playerId": 1, "name": "andrenc1", "token": "9F3A0B...C21B" }
```

Submit a run:

```http
POST /api/scores
Authorization: Bearer 9F3A0B...C21B
Content-Type: application/json

{ "kills": 62, "survivedSeconds": 50 }
```

```json
{
  "id": 1,
  "playerName": "andrenc1",
  "kills": 62,
  "survivedSeconds": 50,
  "score": 670,
  "createdAt": "2026-11-13T17:50:12.4471234Z"
}
```

Leaderboard:

```http
GET /api/scores/top
```

```json
[
  { "playerName": "andrenc1", "bestScore": 900 },
  { "playerName": "maria", "bestScore": 850 }
]
```

## Validation

Invalid requests get `400 Bad Request` with all the errors at once, in the standard Problem Details format.

| Field | Rule |
|---|---|
| `name` (registration) | 4 to 20 characters; only letters `A-Z`, numbers and `_` (no accents or spaces). Leading and trailing spaces are trimmed |
| `kills` | Can't be negative |
| `survivedSeconds` | Between 0 and 600 (a match lasts at most 10 minutes) |
| Kills per second | At most 5 kills per second survived, to reject impossible runs |

```json
{
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "kills": ["Kills can't be negative."],
    "survivedSeconds": ["Survived seconds must be between 0 and 600."]
  }
}
```

Requests to `POST /api/scores` without a token, or with a token that doesn't belong to any player, get `401 Unauthorized`.

## Tech

- **C#** with **.NET 10** and **ASP.NET Core** Minimal APIs
- **Entity Framework Core** with **SQLite** (one-to-many relationship between players and scores, unique indexes)
- Migrations applied automatically on startup
- **xUnit** for automated tests

## How to run

Requires the [.NET 10 SDK](https://dotnet.microsoft.com/download).

```bash
git clone https://github.com/andrenc1/NoShoot-Leaderboard-API.git
cd NoShoot-Leaderboard-API
dotnet run --project NoShoot.Api
```

The API starts at `http://localhost:5031`. The SQLite database (`leaderboard.db`) is created automatically in the `NoShoot.Api` folder on the first run.

To try the endpoints, open `NoShoot.Api/scores.http` in Visual Studio or VS Code (with the REST Client extension). Send the registration request first, then paste the returned token into the `@token` variable at the top of the file.

## Tests

```bash
dotnet test
```

42 tests cover:

- the score formula;
- every validation rule, including boundary values (4 and 20 characters are accepted, 3 and 21 are rejected; 50 kills in 10 s is accepted, 51 is rejected);
- name normalization (`andrenc1`, `AndreNC1` and `  andrenc1  ` are the same name);
- token generation and hashing (length, format, randomness, consistency);
- reading the `Authorization: Bearer` header, including missing, empty and malformed headers.

## Project structure

```
NoShoot.Leaderboard.slnx             # Solution with both projects
├── NoShoot.Api/                     # The API
│   ├── Program.cs                   # Configuration and endpoints
│   ├── Models/                      # Database entities
│   │   ├── Player.cs
│   │   └── ScoreEntry.cs
│   ├── Contracts/                   # Request and response DTOs
│   │   ├── PlayerContracts.cs
│   │   └── ScoreContracts.cs
│   ├── Data/
│   │   └── LeaderboardDbContext.cs  # Tables, relationship and unique indexes
│   ├── Scoring/
│   │   └── ScoreCalculator.cs       # Score formula
│   ├── Security/
│   │   ├── PlayerToken.cs           # Token generation and hashing
│   │   └── BearerToken.cs           # Reads the Authorization header
│   ├── Validation/
│   │   ├── PlayerNameValidator.cs   # Name rules and normalization
│   │   └── ScoreValidation.cs       # Run rules (kills, time, kills per second)
│   ├── Migrations/                  # Database schema history (EF Core)
│   └── scores.http                  # Requests for manual testing
└── NoShoot.Api.Tests/               # xUnit tests
```

## Roadmap

- [x] Submit scores and get the leaderboard
- [x] Persistent storage with EF Core and SQLite
- [x] Input validation and anti-cheat checks (reject impossible kills per second)
- [x] Automated tests with xUnit
- [x] Player registration with unique names and a per-player token
- [x] Leaderboard with each player's best score
- [ ] Integration with the Unity game (`UnityWebRequest`)
- [ ] PostgreSQL with Docker Compose
- [ ] Deploy to a cloud host

## Author

[André Noro](https://github.com/andrenc1)
