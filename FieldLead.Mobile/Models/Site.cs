using SQLite;

namespace FieldLead.Mobile.Models;

public class Site
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string SiteId { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public string Address { get; set; } = string.Empty;
}
