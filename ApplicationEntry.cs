namespace InternshipApplicationTracker;

/// <summary>
/// One internship application. Plain data model — no logic, just fields.
/// Serialized to/from JSON via System.Text.Json.
/// </summary>
public class ApplicationEntry
{
    public string CompanyName { get; set; } = string.Empty;

    public string Position { get; set; } = string.Empty;

    public DateTime ApplicationDate { get; set; } = DateTime.Today;

    /// <summary>One of: Applied / Interview / Rejected / Accepted.</summary>
    public string Status { get; set; } = ApplicationStatus.Applied;

    public string Notes { get; set; } = string.Empty;
}
