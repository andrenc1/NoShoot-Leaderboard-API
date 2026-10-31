using NoShoot.Api.Models;

namespace NoShoot.Api.Validation;

public static class ScoreValidator
{
    public const int MaxNameLength = 20;
    public const int MinNameLength = 4;
    public const int MaxMatchSeconds = 600;
    public const double MaxKillsPerSecond = 5.0;

    public static Dictionary<string, string[]> Validate(SubmitScoreRequest request)
    {
        var errors = new Dictionary<string, string[]>();

        string name = request.PlayerName?.Trim() ?? "";

        if (name.Length == 0)
            errors["playerName"] = ["Name is required."];
        else if (name.Length < MinNameLength || name.Length > MaxNameLength)
            errors["playerName"] = [$"Name must have between {MinNameLength} and {MaxNameLength} characters."];
        else if (!name.All(c => char.IsAsciiLetterOrDigit(c) || c == '_'))
            errors["playerName"] = ["Name can only contain letters (A-Z), numbers and '_'."];

        if (request.Kills < 0)
            errors["kills"] = ["Kills can't be negative."];

        if (request.SurvivedSeconds < 0 || request.SurvivedSeconds > MaxMatchSeconds)
            errors["survivedSeconds"] = [$"Survived seconds must be between 0 and {MaxMatchSeconds}."];
        else if (request.Kills > Math.Max(request.SurvivedSeconds, 1) * MaxKillsPerSecond)
            errors["kills"] = ["Too many kills for the time survived."];

        return errors;
    }
}