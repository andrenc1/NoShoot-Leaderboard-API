namespace NoShoot.Api.Validation;

public static class PlayerNameValidator
{
    public const int MinLength = 4;
    public const int MaxLength = 20;

    public static Dictionary<string, string[]> Validate(string? name)
    {
        var errors = new Dictionary<string, string[]>();
        string trimmed = name?.Trim() ?? "";

        if (trimmed.Length == 0)
            errors["name"] = ["Name is required."];
        else if (trimmed.Length < MinLength || trimmed.Length > MaxLength)
            errors["name"] = [$"Name must have between {MinLength} and {MaxLength} characters."];
        else if (!trimmed.All(c => char.IsAsciiLetterOrDigit(c) || c == '_'))
            errors["name"] = ["Name can only contain letters (A-Z), numbers and '_'."];

        return errors;
    }

    public static string Normalize(string name) => name.Trim().ToUpperInvariant();
}