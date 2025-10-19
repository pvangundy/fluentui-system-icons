using FieldLead.Mobile.Pages;

namespace FieldLead.Mobile;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute("map", typeof(MapPage));
        Routing.RegisterRoute("siteDetail", typeof(SiteDetailPage));
        Routing.RegisterRoute("login", typeof(LoginPage));
    }
}
