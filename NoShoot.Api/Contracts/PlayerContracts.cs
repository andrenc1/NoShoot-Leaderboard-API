namespace NoShoot.Api.Contracts;

public record RegisterPlayerRequest(string Name);

public record RegisterPlayerResponse(int PlayerId, string Name, string Token);