using Dalamud.Plugin.Services;
using QuestionableJsonBuilder.Models;

namespace QuestionableJsonBuilder.Services;

public sealed class QuestWizardController : IDisposable
{
    private readonly RemoteQuestIndexService implementedQuestService;
    private readonly GameQuestCatalogService gameQuestCatalogService;
    private readonly ExportService exportService = new();
    private readonly FileNameService fileNameService = new();
    private readonly Configuration configuration;

    private readonly IObjectTable objectTable;
    private readonly IClientState clientState;

    private bool gameDataLoaded;

    public QuestWizardState State { get; } = new();
    public string OutputText { get; private set; } = string.Empty;
    public string StatusText { get; private set; } = "Waiting for game data...";

    public string QuestSourceStatus
        => $"{implementedQuestService.SourceStatus} | {gameQuestCatalogService.LoadStatus}";

    public QuestCatalogDebugSnapshot DebugSnapshot => gameQuestCatalogService.LastDebugSnapshot;
    public int ImplementedQuestCount => implementedQuestService.ImplementedQuestIds.Count;
    public bool RepoContainsQuest(ushort questId) => implementedQuestService.ImplementedQuestIds.Contains(questId);

    public QuestWizardController(Configuration configuration, IObjectTable objectTable, IClientState clientState, IDataManager dataManager)
    {
        this.configuration = configuration;
        this.objectTable = objectTable;
        this.clientState = clientState;

        implementedQuestService = new RemoteQuestIndexService(configuration.RemoteQuestIndexUrl);
        gameQuestCatalogService = new GameQuestCatalogService(dataManager);

        State.Author = configuration.DefaultAuthor;

        implementedQuestService.RefreshFromRemote();
        StatusText = $"{implementedQuestService.SourceStatus} | Waiting for player login to load game quests.";
    }

    public bool EnsureGameDataLoaded()
    {
        if (gameDataLoaded && gameQuestCatalogService.HasLoadedUsableQuests)
            return true;

        if (!clientState.IsLoggedIn)
        {
            StatusText = $"{implementedQuestService.SourceStatus} | Waiting for player login to load game quests.";
            return false;
        }

        gameQuestCatalogService.Reload();
        gameDataLoaded = gameQuestCatalogService.HasLoadedUsableQuests;
        StatusText = gameDataLoaded
            ? QuestSourceStatus
            : $"{implementedQuestService.SourceStatus} | Quest sheet opened, but no usable quest rows were produced. Open Quest Debug for details.";
        return gameDataLoaded;
    }

    public QuestCatalogDebugSnapshot RefreshDebugSnapshot()
    {
        gameQuestCatalogService.RefreshDebugSnapshot();
        gameDataLoaded = gameQuestCatalogService.HasLoadedUsableQuests;
        StatusText = QuestSourceStatus;
        return gameQuestCatalogService.LastDebugSnapshot;
    }

    public IReadOnlyList<QuestSearchEntry> SearchQuests(string searchText)
    {
        if (!EnsureGameDataLoaded())
            return Array.Empty<QuestSearchEntry>();

        var implementedIds = implementedQuestService.ImplementedQuestIds;
        IEnumerable<QuestSearchEntry> query = gameQuestCatalogService.GetAllQuests()
            .Select(x => x with { Implemented = implementedIds.Contains(x.QuestId) });

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            var term = searchText.Trim();
            query = query.Where(x =>
                x.QuestName.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                x.DisplayText.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                x.QuestId.ToString().Contains(term, StringComparison.OrdinalIgnoreCase));
        }

        return query
            .OrderBy(x => x.Implemented)
            .ThenBy(x => x.QuestId)
            .ToList();
    }

    public void RefreshQuestIndex()
    {
        implementedQuestService.UpdateRemoteUrl(configuration.RemoteQuestIndexUrl);
        implementedQuestService.RefreshFromRemote();
        gameDataLoaded = false;
        EnsureGameDataLoaded();
    }

    public void SetQuestDraft(ushort questId)
    {
        if (!EnsureGameDataLoaded())
        {
            StatusText = "Game quest data is not loaded yet.";
            return;
        }

        if (State.QuestLocked)
        {
            StatusText = "Quest details are locked. Unlock first if you really need to change them.";
            return;
        }

        if (implementedQuestService.ImplementedQuestIds.Contains(questId))
        {
            StatusText = $"Quest {questId} already exists in Questionable and is blocked to avoid overwriting.";
            return;
        }

        var quest = gameQuestCatalogService.GetAllQuests().FirstOrDefault(x => x.QuestId == questId);
        if (quest is null)
        {
            StatusText = "Could not find the selected quest in the game quest list.";
            return;
        }

        State.Quest = new QuestIdentity(quest.QuestId, quest.QuestName);
        StatusText = $"Quest draft set: {quest.QuestName} ({quest.QuestId}).";
    }

    public void ConfirmQuestLock()
    {
        if (State.Quest is null)
        {
            StatusText = "Pick a quest before confirming it.";
            return;
        }

        State.QuestLocked = true;
        StatusText = "Quest details confirmed and locked.";
    }

    public void UnlockQuest()
    {
        State.QuestLocked = false;
        StatusText = "Quest details unlocked.";
    }

    public void AddMiddleSequence()
    {
        var nextSequence = State.MiddleSequences.Count == 0
            ? 1
            : Math.Clamp(State.MiddleSequences.Max(x => x.Sequence) + 1, 1, 254);

        State.MiddleSequences.Add(new SequenceState { Sequence = (byte)nextSequence });
        StatusText = $"Added middle sequence {nextSequence}.";
    }

    public void RemoveMiddleSequenceAt(int index)
    {
        if (index < 0 || index >= State.MiddleSequences.Count)
            return;

        var removed = State.MiddleSequences[index].Sequence;
        State.MiddleSequences.RemoveAt(index);
        StatusText = $"Removed sequence {removed}.";
    }

    public void AddStep(int sequenceIndex, UserStepKind kind)
    {
        if (sequenceIndex < 0 || sequenceIndex >= State.MiddleSequences.Count)
            return;

        var step = new StepState { UserStepKind = kind };
        ApplyDefaultsForKind(step);
        State.MiddleSequences[sequenceIndex].Steps.Add(step);
        StatusText = $"Added {UserStepKindInfo.GetLabel(kind)} step to sequence {State.MiddleSequences[sequenceIndex].Sequence}.";
    }

    public void RemoveStep(int sequenceIndex, int stepIndex)
    {
        if (sequenceIndex < 0 || sequenceIndex >= State.MiddleSequences.Count)
            return;

        var steps = State.MiddleSequences[sequenceIndex].Steps;
        if (stepIndex < 0 || stepIndex >= steps.Count)
            return;

        steps.RemoveAt(stepIndex);
        StatusText = $"Removed step from sequence {State.MiddleSequences[sequenceIndex].Sequence}.";
    }

    public void ApplyStepKindDefaults(StepState step)
    {
        ApplyDefaultsForKind(step);
        StatusText = $"Updated step defaults for {UserStepKindInfo.GetLabel(step.UserStepKind)}.";
    }

    public void MarkQuestDone(bool isDone)
    {
        State.QuestCompletedConfirmed = isDone;
        StatusText = isDone ? "Quest marked as done." : "Quest done confirmation removed.";
    }

    public void UseCurrentPosition(StepState step)
    {
        var localPlayer = objectTable.LocalPlayer;
        if (localPlayer is null)
        {
            StatusText = "Current position capture is unavailable because the local player is not ready.";
            return;
        }

        step.Position = localPlayer.Position;
        step.TerritoryId = clientState.TerritoryType;
        StatusText = "Filled step position from the current player position.";
    }

    public void Validate()
    {
        var errors = ValidateState(State);
        OutputText = errors.Count == 0 ? "Validation passed." : string.Join(Environment.NewLine, errors);
        StatusText = errors.Count == 0 ? "Document is valid." : "Document has validation errors.";
    }

    public void BuildJsonPreview()
    {
        var errors = ValidateState(State);
        if (errors.Count > 0)
        {
            OutputText = string.Join(Environment.NewLine, errors);
            StatusText = "Cannot export until validation errors are fixed.";
            return;
        }

        OutputText = exportService.Export(State, configuration.PrettyPrintJson);
        StatusText = "JSON preview updated.";
    }

    public string? SaveToDisk()
    {
        var errors = ValidateState(State);
        if (errors.Count > 0)
        {
            OutputText = string.Join(Environment.NewLine, errors);
            StatusText = "Cannot save until validation errors are fixed.";
            return null;
        }

        if (State.Quest is null)
        {
            StatusText = "No quest selected.";
            return null;
        }

        if (implementedQuestService.ImplementedQuestIds.Contains(State.Quest.QuestId))
        {
            StatusText = $"Quest {State.Quest.QuestId} already exists in Questionable and export was blocked.";
            return null;
        }

        var exportDirectory = string.IsNullOrWhiteSpace(configuration.DefaultExportDirectory)
            ? Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)
            : configuration.DefaultExportDirectory;

        Directory.CreateDirectory(exportDirectory);
        var fileName = fileNameService.BuildQuestJsonFileName(State.Quest);
        var fullPath = Path.Combine(exportDirectory, fileName);
        var json = exportService.Export(State, configuration.PrettyPrintJson);
        File.WriteAllText(fullPath, json);

        OutputText = json;
        StatusText = $"Saved: {fullPath}";
        return fullPath;
    }



private void ApplyDefaultsForKind(StepState step)
{
    step.Comment ??= null;

    switch (step.UserStepKind)
    {
        case UserStepKind.AcceptQuest:
            step.InteractionType = "AcceptQuest";
            step.StopDistance ??= 3f;
            break;
        case UserStepKind.TalkToNpc:
            step.InteractionType = "Interact";
            step.StopDistance ??= 3f;
            break;
        case UserStepKind.HandOverToNpc:
            step.InteractionType = "Interact";
            step.StopDistance ??= 3f;
            break;
        case UserStepKind.UseItem:
            step.InteractionType = "UseItem";
            step.StopDistance ??= 3f;
            break;
        case UserStepKind.HaveItem:
            step.InteractionType = "Instruction";
            step.StopDistance = null;
            step.Comment ??= "Player must already have the required item.";
            break;
        case UserStepKind.SayInChat:
            step.InteractionType = "Say";
            step.StopDistance = null;
            step.ChatMessageExcelSheet ??= "Addon";
            break;
        case UserStepKind.Emote:
            step.InteractionType = "Emote";
            step.StopDistance = null;
            break;
        case UserStepKind.UseAction:
            step.InteractionType = "Action";
            step.StopDistance = null;
            break;
        case UserStepKind.WalkToLocation:
            step.InteractionType = "WalkTo";
            step.DataId = null;
            step.StopDistance ??= 0.25f;
            break;
        case UserStepKind.Combat:
            step.InteractionType = "Combat";
            step.StopDistance ??= 0.5f;
            step.EnemySpawnType ??= "AfterInteraction";
            break;
        case UserStepKind.Craft:
            step.InteractionType = "Craft";
            step.StopDistance = null;
            step.ItemCount ??= 1;
            step.ItemQuality ??= "Any";
            break;
        case UserStepKind.Gather:
            step.InteractionType = "Gather";
            step.StopDistance = null;
            step.ItemsToGatherJson ??= "[]";
            break;
        case UserStepKind.SwitchClass:
            step.InteractionType = "SwitchClass";
            step.StopDistance = null;
            break;
        case UserStepKind.Duty:
            step.InteractionType = "Duty";
            step.StopDistance = null;
            break;
        case UserStepKind.Jump:
            step.InteractionType = "Jump";
            step.StopDistance = null;
            break;
        case UserStepKind.Dive:
            step.InteractionType = "Dive";
            step.StopDistance = null;
            break;
        case UserStepKind.CompleteQuest:
            step.InteractionType = "CompleteQuest";
            step.StopDistance ??= 3f;
            break;
        case UserStepKind.WaitForManualProgress:
            step.InteractionType = "WaitForManualProgress";
            step.StopDistance = null;
            step.Comment ??= "Manual progress required.";
            break;
        case UserStepKind.Instruction:
            step.InteractionType = "Instruction";
            step.StopDistance = null;
            step.Comment ??= "Contributor instruction.";
            break;
        case UserStepKind.CustomAdvanced:
            if (string.IsNullOrWhiteSpace(step.InteractionType))
                step.InteractionType = "Interact";
            break;
    }
}

private IReadOnlyList<string> ValidateState(QuestWizardState state)
{
    var errors = new List<string>();

    if (state.Quest is null)
        errors.Add("Pick and confirm a quest first.");

    if (!state.QuestLocked)
        errors.Add("Confirm the quest details first so the quest becomes locked.");

    if (string.IsNullOrWhiteSpace(state.Author))
        errors.Add("Author is required.");

    var duplicateSequences = state.MiddleSequences
        .GroupBy(x => x.Sequence)
        .Where(g => g.Count() > 1)
        .Select(g => g.Key)
        .ToList();

    foreach (var duplicateSequence in duplicateSequences)
        errors.Add($"Sequence {duplicateSequence} exists more than once.");

    foreach (var sequence in state.MiddleSequences)
    {
        for (var i = 0; i < sequence.Steps.Count; i++)
        {
            var step = sequence.Steps[i];
            var prefix = $"Sequence {sequence.Sequence}, Step {i + 1}";

            if (step.TerritoryId == 0)
                errors.Add($"{prefix}: TerritoryId is required.");

            if (string.IsNullOrWhiteSpace(step.InteractionType))
                errors.Add($"{prefix}: InteractionType is required.");

            switch (step.InteractionType)
            {
                case "Say":
                    if (string.IsNullOrWhiteSpace(step.ChatMessageKey))
                        errors.Add($"{prefix}: Say requires ChatMessage Key.");
                    break;
                case "Emote":
                    if (string.IsNullOrWhiteSpace(step.EmoteName))
                        errors.Add($"{prefix}: Emote requires an Emote value.");
                    break;
                case "Action":
                    if (string.IsNullOrWhiteSpace(step.ActionName))
                        errors.Add($"{prefix}: Action requires an Action value.");
                    break;
                case "Combat":
                    ValidateJson(prefix, "ComplexCombatData", step.ComplexCombatDataJson, errors);
                    ValidateJson(prefix, "CombatItemUse", step.CombatItemUseJson, errors);
                    ValidateJson(prefix, "CompletionQuestVariablesFlags", step.CompletionQuestVariablesFlagsJson, errors);
                    if (!string.IsNullOrWhiteSpace(step.KillEnemyDataIdsText))
                    {
                        var ids = ParseUIntList(step.KillEnemyDataIdsText);
                        if (ids.Count == 0)
                            errors.Add($"{prefix}: KillEnemyDataIds could not be parsed.");
                    }
                    break;
                case "Craft":
                    if (step.ItemId is null or 0)
                        errors.Add($"{prefix}: Craft requires ItemId.");
                    if (step.ItemCount is null or <= 0)
                        errors.Add($"{prefix}: Craft requires ItemCount.");
                    break;
                case "Gather":
                    if (string.IsNullOrWhiteSpace(step.ItemsToGatherJson))
                        errors.Add($"{prefix}: Gather requires ItemsToGather JSON.");
                    else
                        ValidateJson(prefix, "ItemsToGather", step.ItemsToGatherJson, errors);
                    break;
                case "SwitchClass":
                    if (string.IsNullOrWhiteSpace(step.TargetClassName))
                        errors.Add($"{prefix}: SwitchClass requires TargetClass.");
                    break;
                case "WaitForManualProgress":
                case "Instruction":
                case "Snipe":
                    if (string.IsNullOrWhiteSpace(step.Comment))
                        errors.Add($"{prefix}: {step.InteractionType} requires Comment.");
                    break;
            }
        }
    }

    return errors;
}

private static void ValidateJson(string prefix, string fieldName, string? raw, List<string> errors)
{
    if (string.IsNullOrWhiteSpace(raw))
        return;

    try
    {
        System.Text.Json.JsonDocument.Parse(raw);
    }
    catch (Exception ex)
    {
        errors.Add($"{prefix}: {fieldName} is not valid JSON ({ex.Message}).");
    }
}

private static List<uint> ParseUIntList(string text)
{
    var result = new List<uint>();
    var parts = text.Split(new[] { ',', ';', ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);
    foreach (var part in parts)
    {
        if (uint.TryParse(part.Trim(), out var value) && value > 0)
            result.Add(value);
    }

    return result;
}

    public void Dispose()
    {
        implementedQuestService.Dispose();
    }
}
