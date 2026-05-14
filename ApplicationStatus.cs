namespace InternshipApplicationTracker;

/// <summary>
/// Status values used by the application. Kept here as constants so the
/// form combo boxes and the JSON payload use the same strings.
/// </summary>
public static class ApplicationStatus
{
    public const string All       = "All";
    public const string Applied   = "Applied";
    public const string Interview = "Interview";
    public const string Rejected  = "Rejected";
    public const string Accepted  = "Accepted";

    /// <summary>Values that can be assigned to an entry (no "All").</summary>
    public static readonly string[] Selectable =
    {
        Applied,
        Interview,
        Rejected,
        Accepted
    };

    /// <summary>Values shown in the filter combo box (includes "All").</summary>
    public static readonly string[] FilterChoices =
    {
        All,
        Applied,
        Interview,
        Rejected,
        Accepted
    };
}
