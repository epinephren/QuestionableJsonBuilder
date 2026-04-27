using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace QuestionableJsonBuilder.Services;

public sealed class RemoteQuestIndexService : IDisposable
{
    private readonly HttpClient httpClient = new();
    private readonly string cachePath;
    private HashSet<ushort> implementedIds = new();
    private string _sourceStatus = "Questionable quest index not loaded.";
    
    public string SourceStatus {
        get => _sourceStatus;
        private set {
            _sourceStatus = value;
            Plugin.Log.Info("RemoteSourceStatus: {0}", _sourceStatus);
        }
    }

    public IReadOnlySet<ushort> ImplementedQuestIds => implementedIds;

    public RemoteQuestIndexService(string remoteUrl)
    {
        RemoteUrl = remoteUrl;

        var cacheDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "QuestionableJsonBuilder");

        Directory.CreateDirectory(cacheDirectory);
        cachePath = Path.Combine(cacheDirectory, "questionable-implemented-cache.json");

        httpClient.DefaultRequestHeaders.UserAgent.Add(
            new ProductInfoHeaderValue("QuestionableJsonBuilder", "1.0"));

        LoadCachedOrEmpty();
    }

    public string RemoteUrl { get; private set; }

    public void UpdateRemoteUrl(string remoteUrl)
    {
        RemoteUrl = remoteUrl;
    }

    public void RefreshFromRemote()
    {
        if (string.IsNullOrWhiteSpace(RemoteUrl))
        {
            SourceStatus = "Remote Questionable URL is empty. Using cached implemented quest data.";
            return;
        }

        try
        {
            var json = httpClient.GetStringAsync(RemoteUrl).GetAwaiter().GetResult();
            var parsed = ParseWorkerResponse(json);

            if (parsed.Count == 0)
            {
                SourceStatus = "Remote quest index loaded, but no quests were parsed. Using previous data.";
                return;
            }

            implementedIds = parsed;
            SaveCache(implementedIds);
            SourceStatus = $"Loaded {implementedIds.Count} implemented quests from Worker.";
        }
        catch (Exception ex)
        {
            SourceStatus = $"Remote refresh failed. Using cached implemented quest data. {ex.Message}";
            LoadCachedOrEmpty();
        }
    }

    private void LoadCachedOrEmpty()
    {
        try
        {
            if (File.Exists(cachePath))
            {
                var json = File.ReadAllText(cachePath);
                var cached = JsonSerializer.Deserialize<List<ushort>>(json);

                if (cached is { Count: > 0 })
                {
                    implementedIds = cached.ToHashSet();
                    SourceStatus = $"Using cached implemented quest list with {implementedIds.Count} quests.";
                    return;
                }
            }
        }
        catch
        {
        }

        implementedIds = new HashSet<ushort>();
        SourceStatus = "No cached implemented quest list available yet.";
    }

    private void SaveCache(HashSet<ushort> ids)
    {
        var json = JsonSerializer.Serialize(
            ids.OrderBy(x => x).ToList(),
            new JsonSerializerOptions { WriteIndented = true });

        File.WriteAllText(cachePath, json);
    }

    private static HashSet<ushort> ParseWorkerResponse(string json)
    {
        var result = new HashSet<ushort>();

        using var document = JsonDocument.Parse(json);

        if (!document.RootElement.TryGetProperty("quests", out var questsElement) ||
            questsElement.ValueKind != JsonValueKind.Array)
            return result;

        foreach (var quest in questsElement.EnumerateArray())
        {
            if (!quest.TryGetProperty("questId", out var questIdElement))
                continue;

            if (!questIdElement.TryGetUInt16(out var questId))
                continue;

            if (questId == 0)
                continue;

            result.Add(questId);
        }

        return result;
    }

    public void Dispose()
    {
        httpClient.Dispose();
    }
}