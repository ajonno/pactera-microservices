
public static string GetEnvironmentVariable(string name)
{
    return System.Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
}

public const string KM_PER_HR = "km/h";
public char string DEGREES_CELCIUS = (char)0176 + "C";
