# NoShoot Leaderboard API

REST API that stores scores and serves the online leaderboard for [NoShoot](https://github.com/andrenc1/NoShoot), a 2D survivor-like game made in Unity.

The game sends the result of each run, and the API calculates the score, saves it and returns the top players.

## How the score works

The game does **not** send a score. It sends the raw results of the run (kills and seconds survived), and the API calculates the score on the server:

```
score = kills × 10 + seconds survived × 1
```

This way, a modified client can't simply send `"score": 999999`.

## Validation

Every submitted run is validated before it is saved. Invalid requests get `400 Bad Request` with all the errors at once, in the standard Problem Details format.

| Field | Rule |
|---|---|
| `playerName` | 4 to 20 characters; only letters `A-Z`, numbers and `_` (no accents or spaces). Leading and trailing spaces are trimmed |
| `kills` | Can't be negative |
| `survivedSeconds` | Between 0 and 600 (a match lasts at most 10 minutes) |
| Kills per second | At most 5 kills per second survived, to reject impossible runs |

Example of a rejected request:

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

## Endpoints

| Method | Route | Description | Responses |
|---|---|---|---|
| `POST` | `/api/scores` | Submit the result of a run | `201 Created`, `400 Bad Request` |
| `GET` | `/api/scores/top?limit=10` | Top scores, highest first (`limit` defaults to 10, max 100) | `200 OK` |
| `GET` | `/api/scores/{id}` | A single score by id | `200 OK`, `404 Not Found` |

### Example

Request:

```http
POST /api/scores
Content-Type: application/json

{ "playerName": "Andre", "kills": 62, "survivedSeconds": 50 }
```

Response (`201 Created`):

```json
{
  "id": 1,
  "playerName": "Andre",
  "kills": 62,
  "survivedSeconds": 50,
  "score": 670,
  "createdAt": "2026-10-30T18:08:33.9569987Z"
}
```

## Tech

- **C#** with **.NET 10** and **ASP.NET Core** Minimal APIs
- **Entity Framework Core** with **SQLite**
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

To try the endpoints, open `NoShoot.Api/scores.http` in Visual Studio or VS Code (with the REST Client extension) and send the requests. It includes valid requests and examples that should be rejected.

## Tests

```bash
dotnet test
```

The tests cover the score formula and every validation rule, including the boundary values (for example, 4 and 20 characters are accepted while 3 and 21 are rejected).

## Project structure

```
NoShoot.Leaderboard.slnx          # Solution with both projects
├── NoShoot.Api/                  # The API
│   ├── Program.cs                # Configuration and endpoints
│   ├── Models/
│   │   ├── ScoreEntry.cs         # Score entity (a row in the Scores table)
│   │   └── SubmitScoreRequest.cs # What the game sends
│   ├── Data/
│   │   └── LeaderboardDbContext.cs
│   ├── Scoring/
│   │   └── ScoreCalculator.cs    # Score formula
│   ├── Validation/
│   │   └── ScoreValidation.cs    # Validation rules
│   ├── Migrations/               # Database schema history (EF Core)
│   └── scores.http               # Requests for manual testing
└── NoShoot.Api.Tests/            # xUnit tests
    ├── ScoreCalculatorTests.cs
    └── ScoreValidationTests.cs
```

## Roadmap

- [x] Submit scores and get the leaderboard
- [x] Persistent storage with EF Core and SQLite
- [x] Input validation and anti-cheat checks (reject impossible kills per second)
- [x] Automated tests with xUnit
- [ ] Player registration with unique names and a per-player token
- [ ] Integration with the Unity game (`UnityWebRequest`)
- [ ] PostgreSQL with Docker Compose
- [ ] Deploy to a cloud host

## Author

[André Noro](https://github.com/andrenc1)
