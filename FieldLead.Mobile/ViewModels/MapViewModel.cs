using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FieldLead.Mobile.Data;
using FieldLead.Mobile.Models;
using FieldLead.Mobile.Services;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Controls;

namespace FieldLead.Mobile.ViewModels;

public class MapViewModel : ObservableObject
{
    private readonly IGeolocation _geolocation;
    private readonly LeadRepository _repository;
    private readonly AuthService _authService;

    private Location? _currentLocation;
    private bool _isBusy;

    public MapViewModel(IGeolocation geolocation, LeadRepository repository, AuthService authService)
    {
        _geolocation = geolocation;
        _repository = repository;
        _authService = authService;
        Sites = new ObservableCollection<Site>();

        RefreshCommand = new AsyncRelayCommand(LoadAsync);
    }

    public ObservableCollection<Site> Sites { get; }

    public Location? CurrentLocation
    {
        get => _currentLocation;
        set => SetProperty(ref _currentLocation, value);
    }

    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value);
    }

    public ICommand RefreshCommand { get; }

    public async Task LoadAsync()
    {
        if (IsBusy)
        {
            return;
        }

        try
        {
            IsBusy = true;

            if (!_authService.IsAuthenticated)
            {
                await _authService.SignInAsync();
            }

            var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
            var location = await _geolocation.GetLocationAsync(request) ?? await _geolocation.GetLastKnownLocationAsync();

            if (location is null)
            {
                return;
            }

            CurrentLocation = location;

            var sites = await _repository.GetNearbySitesAsync(location);
            Sites.Clear();
            foreach (var site in sites)
            {
                Sites.Add(site);
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
