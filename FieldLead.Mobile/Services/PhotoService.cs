using System.IO;
using System.Threading.Tasks;
using FieldLead.Mobile.Models;
using Microsoft.Maui.Media;
using Microsoft.Maui.Storage;

namespace FieldLead.Mobile.Services;

public class PhotoService
{
    private readonly IFileSystem _fileSystem;

    public PhotoService(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }

    public async Task<LeadPhoto?> CaptureLeadPhotoAsync(string leadId, string? caption = null)
    {
        if (!MediaPicker.Default.IsCaptureSupported)
        {
            return null;
        }

        var photo = await MediaPicker.Default.CapturePhotoAsync(new MediaPickerOptions
        {
            Title = "Capture site photo"
        });

        if (photo is null)
        {
            return null;
        }

        var fileName = $"lead_{leadId}_{DateTimeOffset.UtcNow:yyyyMMddHHmmss}.jpg";
        var destination = Path.Combine(_fileSystem.AppDataDirectory, fileName);

        await using var sourceStream = await photo.OpenReadAsync();
        await using var destinationStream = File.OpenWrite(destination);
        await sourceStream.CopyToAsync(destinationStream);

        return new LeadPhoto
        {
            LeadId = leadId,
            LocalPath = destination,
            Caption = caption ?? string.Empty,
            CapturedAt = DateTimeOffset.UtcNow
        };
    }
}
