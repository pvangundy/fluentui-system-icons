using System;
using SQLite;

namespace FieldLead.Mobile.Models;

public class LeadPhoto
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public string LeadId { get; set; } = string.Empty;

    public string LocalPath { get; set; } = string.Empty;

    public string Caption { get; set; } = string.Empty;

    public DateTimeOffset CapturedAt { get; set; } = DateTimeOffset.UtcNow;
}
