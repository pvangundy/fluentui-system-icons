using System.Collections.Generic;
using System.Linq;
using FieldLead.Mobile.ViewModels;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Devices.Sensors;

namespace FieldLead.Mobile.Pages;

public partial class MapPage : ContentPage
{
    private readonly MapViewModel _viewModel;

    public MapPage(MapViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
        _viewModel.PropertyChanged += ViewModelOnPropertyChanged;
        _viewModel.Sites.CollectionChanged += SitesOnCollectionChanged;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (!_viewModel.Sites.Any())
        {
            await _viewModel.LoadAsync();
        }
    }

    private void SitesOnCollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        RedrawPins();
    }

    private void ViewModelOnPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MapViewModel.CurrentLocation))
        {
            UpdateMapLocation();
        }
    }

    private void RedrawPins()
    {
        Map.Pins.Clear();
        foreach (var site in _viewModel.Sites)
        {
            var pin = new Pin
            {
                Type = PinType.Place,
                Label = site.Name,
                Address = site.Address,
                Location = new Location(site.Latitude, site.Longitude)
            };
            pin.MarkerClicked += async (_, args) =>
            {
                args.HideInfoWindow = true;
                await Shell.Current.GoToAsync("siteDetail", new Dictionary<string, object>
                {
                    ["site"] = site
                });
            };
            Map.Pins.Add(pin);
        }
    }

    private void UpdateMapLocation()
    {
        if (_viewModel.CurrentLocation is not { } location)
        {
            return;
        }

        var span = MapSpan.FromCenterAndRadius(new Location(location.Latitude, location.Longitude), Distance.FromMeters(500));
        Map.MoveToRegion(span);
    }
}
