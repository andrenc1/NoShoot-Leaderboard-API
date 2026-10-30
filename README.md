# NoShoot Leaderboard API

REST API that stores scores and serves the online leaderboard for [NoShoot](https://github.com/andrenc1/NoShoot), a 2D survivor-like game made in Unity.

The game sends the result of each run, and the API calculates the score, saves it and returns the top players.

## How the score works

The game does **not** send a score. It sends the raw results of the run (kills and seconds survived), and the API calculates the score on the server:

```
score = kills × 10 + seconds survived × 1
```

This way, a modified client can't simply send `"score": 999999`. Server-side validation of impossible results is on the roadmap.

## Endpoints

| Method | Route | Description | Responses |
|---|---|---|---|
| `POST` | `/api/scores` | Submit the result of a run | `201 Created` |
| `GET` | `/api/scores/top?limit=10` | Top scores, highest first (`limit` defaults to 10) | `200 OK` |
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

## How to run

Requires the [.NET 10 SDK](https://dotnet.microsoft.com/download).

```bash
git clone https://github.com/andrenc1/NoShoot-Leaderboard-API.git
cd NoShoot-Leaderboard-API
dotnet run --project NoShoot.Api
```

The API starts at `http://localhost:5031`. The SQLite database (`leaderboard.db`) is created automatically in the `NoShoot.Api` folder on the first run.

To try the endpoints, open `NoShoot.Api/scores.http` in Visual Studio or VS Code (with the REST Client extension) and send the requests.

## Project structure

```
NoShoot.Api/
├── Program.cs          # Configuration and endpoints
├── Models/
│   └── ScoreEntry.cs   # Score entity (a row in the Scores table)
├── Data/
│   └── LeaderboardDbContext.cs
├── Migrations/         # Database schema history (EF Core)
└── scores.http         # Requests for manual testing
```

## Roadmap

- [x] Submit scores and get the leaderboard
- [x] Persistent storage with EF Core and SQLite
- [ ] Input validation and anti-cheat checks (reject impossible kills per second)
- [ ] Automated tests with xUnit
- [ ] Integration with the Unity game (`UnityWebRequest`)
- [ ] PostgreSQL with Docker Compose
- [ ] Deploy to a cloud host

## Author

[André Noro Crivellenti](https://github.com/andrenc1)
