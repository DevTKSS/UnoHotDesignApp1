using Uno.Extensions.Reactive;

namespace UnoHotDesignApp1.Presentation.Models;

public partial record DashboardModel
{
    #region Services
    private readonly INavigator _navigator;
    private readonly ILocalizationService _localizationService;
    private readonly IStringLocalizer _stringLocalizer;
    private readonly IGalleryImageService _galleryImageService;
    private readonly IStorage _storage;
    #endregion

    public DashboardModel(
        INavigator navigator,
        ILocalizationService localizationService,
        IStringLocalizer stringLocalizer,
        IGalleryImageService galleryImageService,
        IStorage storage)
    {
        this._navigator = navigator;
        this._localizationService = localizationService;
        this._stringLocalizer = stringLocalizer;
        this._galleryImageService = galleryImageService;
        this._storage = storage;
    }

    public IListFeed<GalleryImageModel> GalleryImages => ListFeed.Async(_galleryImageService.GetGalleryImagesWithoutReswAsync);
    public IListFeed<GalleryImageModel> GalleryImagesWithResw => ListFeed.Async(_galleryImageService.GetGalleryImagesWithReswAsync);
    public IState<string> DashboardTitle => State<string>.Value(this, () => _stringLocalizer["DashboardTitle"]);
       
    #region CodeSample import
    public IListFeed<string> CodeSampleOptions => ListFeed.Async(GetCodeSampleOptionsAsync);

    public async ValueTask<IImmutableList<string>> GetCodeSampleOptionsAsync(CancellationToken ctk)
    {
        await Task.Delay(TimeSpan.FromMilliseconds(1), ctk);
        return new List<string>
                {
                    "C# in Model",
                    "DI Service Resw",
                    "DI Service without Resw",
                    "C# Record",
                    "XAML DataTemplate",
                    "FeedView + GridView XAML"
                }.ToImmutableList();
    }
    public IState<string> CurrentCodeSample => State<string>.Value(this, () => string.Empty);
    public async Task SwitchCodeSampleAsync(object? parameter)
    {
        string? codeSample = parameter?.ToString() switch
        {
            "C# in Model" => await _storage.ReadPackageFileAsync("Assets/Samples/ModelBinding-Sample.cs.txt"),
            "DI Service Resw" => await _storage.ReadPackageFileAsync("Assets/Samples/GalleryImageService-resw.cs.txt"),
            "DI Service without Resw" => await _storage.ReadPackageFileAsync("Assets/Samples/GalleryImageService-noResw.cs.txt"),
            "C# Record" => await _storage.ReadPackageFileAsync("Assets/Samples/GalleryImageModel.cs.txt"),
            "XAML DataTemplate" => await _storage.ReadPackageFileAsync("Assets/Samples/Card-GalleryImage.DataTemplate.xaml.txt"),
            "FeedView + GridView XAML" => await _storage.ReadPackageFileAsync("Assets/Samples/FeedView-GridView-Sample.xaml.txt"),
            _ => string.Empty
        };

        await CurrentCodeSample.SetAsync(codeSample ?? string.Empty);
    }
    #endregion


    #region ViewHeaderContent
    public IFeed<HeaderContent> ViewHeaderContent => Feed.Async(GetGridViewHeaderAsync);
    public async ValueTask<HeaderContent> GetGridViewHeaderAsync(CancellationToken ctk)
    {
        await Task.Delay(TimeSpan.FromMilliseconds(1), ctk);
        var headerContent = new HeaderContent("Assets/Images/styled_logo.png", _stringLocalizer["GridViewTitle"]);
        return headerContent;
    }
    #endregion
}



