using Microsoft.Extensions.Configuration;

namespace DevTKSS.Uno.Samples.MvuxGallery.Models.CodeSamples;
public record CodeSampleService : ICodeSampleService
{
    public CodeSampleService(
        IOptions<CodeSampleOptionsConfiguration> options,
        ILogger<CodeSampleService> logger,
        IStorage storage)
    {
        _options = options.Value;
        _logger = logger;
        _storage = storage;
    }

    private readonly IStorage _storage;
    private readonly ILogger<CodeSampleService> _logger;
    private readonly CodeSampleOptionsConfiguration _options;

    /// <summary>
    /// Get a static Collection of Values for <see cref="CodeSampleOptions"/>
    /// </summary>
    /// <param name="ct">
    /// A CancellationToken to make it compileable
    /// <remarks>
    /// since `ListFeed.Async` requires a CancellationToken even if Uno Documentation remarks this parameter to be optional.<br/>
    /// <see href="https://learn.microsoft.com/en-us/dotnet/csharp/misc/cs0411?f1url=%3FappId%3Droslyn%26k%3Dk(CS0411)">CS0411</see><br/>
    /// <br/>
    /// adding then the type string or IImmutableList<string> to the ListFeed like `ListFeed<string>.Async(...)`,
    /// or to the Async Extension itself like `ListFeed.Async<IImutableList<string>` results in a type mismatch.<br/>
    /// <see href="https://learn.microsoft.com/en-us/dotnet/csharp/misc/cs1503?f1url=%3FappId%3Droslyn%26k%3Dk(CS1503)">CS1503</see>
    /// </remarks>
    /// <returns>An awaitable <see cref="ValueTask{TResult}"/> providing a <see cref="ImmutableList{T}"/> of <see langword="string"/> with the Sample Names to select from</returns>
    public async ValueTask<IImmutableList<string>> GetCodeSampleOptionsAsync(CancellationToken ct = default)
    {
        await Task.Delay(1);
        return _options.Samples.Keys.ToImmutableList(); 
    }

    public async ValueTask<string> GetCodeSampleAsync(string sampleID, CancellationToken ct = default)
    {
        if (_options.Samples.TryGetValue(sampleID, out CodeSampleOption? sampleOption))
        {
            return await _storage.ReadLinesFromPackageFile(sampleOption.FilePath, sampleOption.LineRanges);
        }

        _logger.LogWarning("Code sample with ID {sampleID} not found", sampleID);
        return string.Empty;
    }

}
