namespace ControlPlane.Api.Services;

public enum ConfigKeys
{
    DatabaseUrl,
    JwtKey,
    JwtIssuer,
    JwtClockSkewSeconds,
    TokenExpirationHours,
    CorsPolicyOrigin,
}

public static class ConfigService
{
    static ConfigService()
    {
        DotNetEnv.Env.Load();
    }

    public static string GetConfigValue(ConfigKeys key)
    {
        var value = Environment.GetEnvironmentVariable(ConvertConfigKeyToEnvVar(key));
        if (string.IsNullOrEmpty(value))
        {
            throw new InvalidOperationException($"Configuration value for '{key}' not found.");
        }
        return value;
    }

    public static T GetConfigValue<T>(ConfigKeys key)
    {
        var value = GetConfigValue(key);
        return (T)Convert.ChangeType(value, typeof(T));
    }

    private static string ConvertConfigKeyToEnvVar(ConfigKeys key)
    {
        return key switch
        {
            ConfigKeys.DatabaseUrl => "DATABASE_URL",
            ConfigKeys.JwtKey => "JWT_KEY",
            ConfigKeys.JwtIssuer => "JWT_ISSUER",
            ConfigKeys.JwtClockSkewSeconds => "JWT_CLOCK_SKEW_SECONDS",
            ConfigKeys.TokenExpirationHours => "TOKEN_EXPIRATION_HOURS",
            ConfigKeys.CorsPolicyOrigin => "CORS_POLICY_ORIGIN",
            _ => throw new ArgumentOutOfRangeException(nameof(key), key, null)
        };
    }
}