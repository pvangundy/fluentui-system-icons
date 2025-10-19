using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FieldLead.Mobile.Services;
using Microsoft.Identity.Client;
using System.Threading.Tasks;

namespace FieldLead.Mobile.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly AuthService _authService;

    [ObservableProperty]
    private bool _isSignedIn;

    [ObservableProperty]
    private string _accountDisplayName = string.Empty;

    public LoginViewModel(AuthService authService)
    {
        _authService = authService;
    }

    [RelayCommand]
    private async Task SignInAsync()
    {
        var account = await _authService.SignInAsync();
        if (account is not null)
        {
            AccountDisplayName = account.Username ?? account.HomeAccountId?.Identifier ?? string.Empty;
            IsSignedIn = true;
        }
    }
}
