using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace QuestionableJsonBuilder.Services;

public sealed class RemoteQuestIndexService : IDisposable
{
    private static readonly Regex QuestFileRegex = new(
        @"^QuestPaths/.+/(?<id>\d+)_(?<name>.+)\.json$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private readonly HttpClient httpClient = new();
    private readonly string cachePath;
    private HashSet<ushort> implementedIds = new();

    public string SourceStatus { get; private set; } = "Questionable quest repo not loaded.";

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
            var parsed = ParseGitHubTreeResponse(json);

            if (parsed.Count == 0)
            {
                SourceStatus = "Remote Questionable quest list loaded, but no quest files were parsed. Using previous data.";
                return;
            }

            implementedIds = parsed;
            SaveCache(implementedIds);
            SourceStatus = $"Loaded {implementedIds.Count} implemented quests from the remote Questionable source.";
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

    private static HashSet<ushort> ParseGitHubTreeResponse(string json)
    {
        var result = new HashSet<ushort>();

        using var document = JsonDocument.Parse(json);

        if (!document.RootElement.TryGetProperty("tree", out var treeElement) ||
            treeElement.ValueKind != JsonValueKind.Array)
            return result;

        foreach (var item in treeElement.EnumerateArray())
        {
            if (!item.TryGetProperty("path", out var pathElement))
                continue;

            var path = pathElement.GetString();
            if (string.IsNullOrWhiteSpace(path))
                continue;

            var match = QuestFileRegex.Match(path);
            if (!match.Success)
                continue;

            if (!ushort.TryParse(match.Groups["id"].Value, out var questId))
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