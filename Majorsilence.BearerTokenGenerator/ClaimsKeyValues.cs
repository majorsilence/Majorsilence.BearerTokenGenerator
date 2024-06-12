namespace Majorsilence.BearerTokenGenerator;

public class ClaimsKeyValues(string key, string value)
{
    public string Key { get; set; } = key;
    public string Value { get; set; } = value;
}