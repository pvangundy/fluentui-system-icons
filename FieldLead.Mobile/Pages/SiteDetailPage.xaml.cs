using System.Collections.Generic;
using FieldLead.Mobile.Models;
using FieldLead.Mobile.ViewModels;

namespace FieldLead.Mobile.Pages;

public partial class SiteDetailPage : ContentPage, IQueryAttributable
{
    private readonly SiteDetailViewModel _viewModel;

    public SiteDetailPage(SiteDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("site", out var siteObj) && siteObj is Site site)
        {
            await _viewModel.InitializeAsync(site);
        }
    }
}
