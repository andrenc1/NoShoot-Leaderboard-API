using NoShoot.Api.Contracts;

namespace NoShoot.Api.Validation;

public static class ScoreValidator
{
    public const int MaxMatchSeconds = 600;
    public const double MaxKillsPerSecond = 5.0;

    public static Dictionary<string, string[]> Validate(SubmitScoreRequest request)
    {
        var errors = new Dictionary<string, string[]>();

        if (request.Kills < 0)
            errors["kills"] = ["Kills can't be negative."];

        if (request.SurvivedSeconds < 0 || request.SurvivedSeconds > MaxMatchSeconds)
            errors["survivedSeconds"] = [$"Survived seconds must be between 0 and {MaxMatchSeconds}."];
        else if (request.Kills > Math.Max(request.SurvivedSeconds, 1) * MaxKillsPerSecond)
            errors["kills"] = ["Too many kills for the time survived."];

        return errors;
    }
}