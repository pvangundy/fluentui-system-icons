# FieldLead Mobile Starter

This project is a .NET MAUI starter for a field lead capture app featuring Entra ID authentication, offline SQLite storage, maps, photo capture, and a background sync scaffold.

## Configure authentication
Update `Services/AuthService.cs` with your Entra ID information:

- `TenantId`
- `ClientId`
- `RedirectUri` (use the `msal{CLIENT_ID}://auth` format)
- `DefaultScopes` (for example, `api://YOUR_API_APP_ID/Leads.ReadWrite`)

Replace the redirect URI placeholders in the platform manifests:

- `Platforms/Android/AndroidManifest.xml`
- `Platforms/iOS/Info.plist`

## API base address
Set the real API base URL for the named `api` `HttpClient` in `MauiProgram.cs`.

## Fonts & images
Replace the placeholder files in `Resources/Fonts/` and `Resources/Images/` with your production assets.

## Building
```
# Android
dotnet build FieldLead.Mobile.csproj -t:Run -f net8.0-android

# iOS (requires macOS + Xcode)
dotnet build FieldLead.Mobile.csproj -t:Run -f net8.0-ios
```

The app registers shell routes for the map, login, and site detail pages. The `SyncService` contains the upload loop placeholder—extend it with your API integration.
