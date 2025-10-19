using System.Collections.ObjectModel;
using System.Text.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FieldLead.Mobile.Data;
using FieldLead.Mobile.Models;
using FieldLead.Mobile.Services;

namespace FieldLead.Mobile.ViewModels;

public partial class SiteDetailViewModel : ObservableObject
{
    private readonly LeadRepository _repository;
    private readonly PhotoService _photoService;

    [ObservableProperty]
    private Site? _site;

    public ObservableCollection<LeadPhoto> Photos { get; } = new();

    public SiteDetailViewModel(LeadRepository repository, PhotoService photoService)
    {
        _repository = repository;
        _photoService = photoService;
    }

    public async Task InitializeAsync(Site site)
    {
        Site = site;
        await _repository.InitializeAsync();

        var existingPhotos = await _repository.GetLeadPhotosAsync(site.SiteId);

        Photos.Clear();
        foreach (var photo in existingPhotos)
        {
            Photos.Add(photo);
        }
    }

    [RelayCommand]
    private async Task CapturePhotoAsync()
    {
        if (Site is null)
        {
            return;
        }

        var photo = await _photoService.CaptureLeadPhotoAsync(Site.SiteId);
        if (photo is null)
        {
            return;
        }

        await _repository.InsertLeadPhotoAsync(photo);
        await _repository.EnqueueOutboxItemAsync(new OutboxItem
        {
            EntityId = Site.SiteId,
            EntityType = nameof(LeadPhoto),
            AttachmentPath = photo.LocalPath,
            PayloadJson = System.Text.Json.JsonSerializer.Serialize(new
            {
                SiteId = Site.SiteId,
                PhotoPath = photo.LocalPath
            })
        });

        Photos.Add(photo);
    }
}
