using System;
using SQLite;

namespace FieldLead.Mobile.Models;

public class Lead
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public string LeadId { get; set; } = string.Empty;

    [Indexed]
    public string SiteId { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string ContactName { get; set; } = string.Empty;

    public string ContactPhone { get; set; } = string.Empty;

    public string Notes { get; set; } = string.Empty;

    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}
