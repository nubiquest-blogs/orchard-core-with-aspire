using OrchardCore.ContentFields.Fields;
using OrchardCore.ContentManagement;

namespace OrchardApp.Pages;

public class HomeData : ContentPart
{
    public required TextField SubTitle { get; set; }
}