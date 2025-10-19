using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Identity.Client;

namespace FieldLead.Mobile.Services;

public class AuthService
{
    private const string TenantId = "YOUR_TENANT_ID";
    private const string ClientId = "YOUR_CLIENT_ID";
    private const string RedirectUri = "msalYOUR_CLIENT_ID://auth";
    private static readonly string[] DefaultScopes =
    {
        "api://YOUR_API_APP_ID/Leads.ReadWrite"
    };

    private readonly IPublicClientApplication _pca;
    private IAccount? _currentAccount;
    private AuthenticationResult? _lastResult;

    public AuthService()
    {
        _pca = PublicClientApplicationBuilder.Create(ClientId)
            .WithTenantId(TenantId)
            .WithRedirectUri(RedirectUri)
            .WithIosKeychainSecurityGroup("com.microsoft.adalcache")
            .Build();
    }

    public bool IsAuthenticated => _lastResult is not null;

    public async Task<IAccount?> SignInAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _currentAccount ??= (await _pca.GetAccountsAsync()).FirstOrDefault();
            if (_currentAccount is not null)
            {
                _lastResult = await _pca.AcquireTokenSilent(DefaultScopes, _currentAccount)
                    .ExecuteAsync(cancellationToken);
            }
            else
            {
                _lastResult = await _pca.AcquireTokenInteractive(DefaultScopes)
                    .WithPrompt(Prompt.SelectAccount)
                    .ExecuteAsync(cancellationToken);
            }

            _currentAccount = _lastResult.Account;
            return _currentAccount;
        }
        catch (MsalUiRequiredException)
        {
            _lastResult = await _pca.AcquireTokenInteractive(DefaultScopes)
                .WithPrompt(Prompt.SelectAccount)
                .ExecuteAsync(cancellationToken);
            _currentAccount = _lastResult.Account;
            return _currentAccount;
        }
    }

    public Task SignOutAsync()
    {
        return ClearAccountsAsync();
    }

    public async Task<string?> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        if (_currentAccount is null)
        {
            await SignInAsync(cancellationToken);
        }

        if (_currentAccount is null)
        {
            return null;
        }

        try
        {
            _lastResult = await _pca.AcquireTokenSilent(DefaultScopes, _currentAccount)
                .ExecuteAsync(cancellationToken);
        }
        catch (MsalUiRequiredException)
        {
            _lastResult = await _pca.AcquireTokenInteractive(DefaultScopes)
                .WithPrompt(Prompt.SelectAccount)
                .ExecuteAsync(cancellationToken);
        }

        _currentAccount = _lastResult.Account;
        return _lastResult.AccessToken;
    }

    private async Task ClearAccountsAsync()
    {
        var accounts = await _pca.GetAccountsAsync();
        foreach (var account in accounts)
        {
            await _pca.RemoveAsync(account);
        }

        _currentAccount = null;
        _lastResult = null;
    }
}
