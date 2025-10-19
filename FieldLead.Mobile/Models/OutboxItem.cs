using System;
using SQLite;

namespace FieldLead.Mobile.Models;

public class OutboxItem
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public string EntityId { get; set; } = string.Empty;

    public string EntityType { get; set; } = string.Empty;

    public string PayloadJson { get; set; } = string.Empty;

    public string? AttachmentPath { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
