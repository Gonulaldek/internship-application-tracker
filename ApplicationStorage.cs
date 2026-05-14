using System.Text.Json;

namespace InternshipApplicationTracker;

/// <summary>
/// Reads and writes the application list to a local JSON file
/// (applications.json next to the executable). No database.
/// </summary>
public static class ApplicationStorage
{
    public const string FileName = "applications.json";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    /// <summary>Full path to applications.json next to the .exe.</summary>
    public static string GetFilePath()
    {
        return Path.Combine(AppContext.BaseDirectory, FileName);
    }

    /// <summary>
    /// Loads the list from disk. Returns an empty list if the file is missing,
    /// empty, or unreadable (we don't want a bad file to crash the form).
    /// </summary>
    public static List<ApplicationEntry> Load(out string? errorMessage)
    {
        errorMessage = null;
        var path = GetFilePath();

        if (!File.Exists(path))
        {
            return new List<ApplicationEntry>();
        }

        try
        {
            var json = File.ReadAllText(path);
            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<ApplicationEntry>();
            }

            var entries = JsonSerializer.Deserialize<List<ApplicationEntry>>(json);
            return entries ?? new List<ApplicationEntry>();
        }
        catch (Exception ex)
        {
            errorMessage = $"Could not read {FileName}: {ex.Message}";
            return new List<ApplicationEntry>();
        }
    }

    /// <summary>
    /// Writes the list to disk. Returns true on success, false on failure
    /// (sets <paramref name="errorMessage"/> in the failure case).
    /// </summary>
    public static bool Save(List<ApplicationEntry> entries, out string? errorMessage)
    {
        errorMessage = null;
        try
        {
            var path = GetFilePath();
            var json = JsonSerializer.Serialize(entries, JsonOptions);
            File.WriteAllText(path, json);
            return true;
        }
        catch (Exception ex)
        {
            errorMessage = $"Could not write {FileName}: {ex.Message}";
            return false;
        }
    }
}
