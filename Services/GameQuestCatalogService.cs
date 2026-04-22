using Dalamud.Plugin.Services;
using Lumina.Excel.Sheets;
using QuestionableJsonBuilder.Models;

namespace QuestionableJsonBuilder.Services;

public sealed class GameQuestCatalogService
{
    private readonly IDataManager dataManager;
    private List<QuestSearchEntry>? cachedEntries;

    public string LoadStatus { get; private set; } = "Game quest catalog not loaded.";
    public QuestCatalogDebugSnapshot LastDebugSnapshot { get; private set; } = new();
    public bool HasLoadedUsableQuests => cachedEntries is { Count: > 0 };

    public GameQuestCatalogService(IDataManager dataManager)
    {
        this.dataManager = dataManager;
    }

    public IReadOnlyList<QuestSearchEntry> GetAllQuests()
    {
        if (cachedEntries is not null)
            return cachedEntries;

        var snapshot = BuildDebugSnapshot();
        LastDebugSnapshot = snapshot;

        var preferred = snapshot.TestA.UsableEntries.Count > 0
            ? snapshot.TestA.UsableEntries
            : snapshot.TestB.UsableEntries;

        cachedEntries = preferred
            .GroupBy(x => x.QuestId)
            .Select(g => g.First())
            .OrderBy(x => x.QuestId)
            .ToList();

        var sourceUsed = snapshot.TestA.UsableEntries.Count > 0
            ? "IDataManager"
            : snapshot.TestB.UsableEntries.Count > 0
                ? "GameData"
                : "none";

        LoadStatus = $"Loaded {cachedEntries.Count} quests from game data via {sourceUsed}. " +
                     $"A(raw={snapshot.TestA.RawRowCount}, usable={snapshot.TestA.UsableEntries.Count}, q2694={snapshot.TestA.HasQuest2694}); " +
                     $"B(raw={snapshot.TestB.RawRowCount}, usable={snapshot.TestB.UsableEntries.Count}, q2694={snapshot.TestB.HasQuest2694})";

        return cachedEntries;
    }

    public void Reload()
    {
        cachedEntries = null;
        GetAllQuests();
    }

    public QuestCatalogDebugSnapshot RefreshDebugSnapshot()
    {
        cachedEntries = null;
        GetAllQuests();
        return LastDebugSnapshot;
    }

    private QuestCatalogDebugSnapshot BuildDebugSnapshot()
    {
        var snapshot = new QuestCatalogDebugSnapshot
        {
            DataManagerLanguage = dataManager.Language.ToString(),
        };

        snapshot.TestA = ProbeSheet(
            "A: IDataManager.GetExcelSheet<Quest>(dataManager.Language, \"Quest\")",
            () => dataManager.GetExcelSheet<Quest>(dataManager.Language, "Quest"));

        snapshot.TestB = ProbeSheet(
            "B: dataManager.GameData.GetExcelSheet<Quest>(name: \"Quest\")",
            () => dataManager.GameData.GetExcelSheet<Quest>(name: "Quest"));

        return snapshot;
    }

    private static QuestSheetProbe ProbeSheet(string label, Func<IEnumerable<Quest>> loadRows)
    {
        var probe = new QuestSheetProbe { Label = label };

        try
        {
            var rows = loadRows();
            foreach (var row in rows)
            {
                probe.RawRowCount++;

                var rawName = row.Name.ToString() ?? string.Empty;
                if (probe.FirstRows.Count < 10)
                {
                    probe.FirstRows.Add(new QuestRowPreview
                    {
                        RowId = row.RowId,
                        Name = rawName,
                    });
                }

                if (TryConvertRow(row.RowId, rawName, out var entry))
                {
                    probe.UsableEntries.Add(entry!);
                    if (entry!.QuestId == 2694)
                    {
                        probe.HasQuest2694 = true;
                        probe.Quest2694Name = entry.QuestName;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            probe.Exception = ex.ToString();
        }

        return probe;
    }

    private static bool TryConvertRow(uint rowId, string? name, out QuestSearchEntry? entry)
    {
        entry = null;

        if (rowId == 0)
            return false;

        var questId = unchecked((ushort)rowId);
        if (questId == 0)
            return false;
        name = name?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(name))
            name = $"Quest {questId}";

        entry = new QuestSearchEntry(questId, name, $"{questId} - {name}", false);
        return true;
    }
}

public sealed class QuestCatalogDebugSnapshot
{
    public string DataManagerLanguage { get; init; } = string.Empty;
    public QuestSheetProbe TestA { get; set; } = new();
    public QuestSheetProbe TestB { get; set; } = new();
}

public sealed class QuestSheetProbe
{
    public string Label { get; init; } = string.Empty;
    public int RawRowCount { get; set; }
    public bool HasQuest2694 { get; set; }
    public string Quest2694Name { get; set; } = string.Empty;
    public string Exception { get; set; } = string.Empty;
    public List<QuestRowPreview> FirstRows { get; } = new();
    public List<QuestSearchEntry> UsableEntries { get; } = new();
}

public sealed class QuestRowPreview
{
    public uint RowId { get; init; }
    public string Name { get; init; } = string.Empty;
}
