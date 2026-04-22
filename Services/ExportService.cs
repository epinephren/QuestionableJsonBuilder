using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;
using QuestionableJsonBuilder.ExportModels;
using QuestionableJsonBuilder.Models;

namespace QuestionableJsonBuilder.Services;

public sealed class ExportService
{
    public string Export(QuestWizardState state, bool prettyPrintJson = true)
    {
        if (state.Quest is null)
            throw new InvalidOperationException("No quest selected.");

        var sequences = new List<ExportQuestSequence>
        {
            new()
            {
                Sequence = 0,
                Comment = "Auto-generated quest start sequence.",
                Steps = null,
            }
        };

        sequences.AddRange(
            state.MiddleSequences
                .OrderBy(x => x.Sequence)
                .Select(sequence => new ExportQuestSequence
                {
                    Sequence = sequence.Sequence,
                    Comment = sequence.Comment,
                    Steps = sequence.Steps.Count == 0
                        ? null
                        : sequence.Steps.Select(BuildExportStep).ToList(),
                }));

        sequences.Add(new ExportQuestSequence
        {
            Sequence = 255,
            Comment = state.QuestCompletedConfirmed
                ? "Auto-generated quest end sequence. Contributor confirmed the quest is done."
                : "Auto-generated quest end sequence.",
            Steps = null,
        });

        var root = new ExportQuestRoot
        {
            Author = new List<string> { state.Author },
            Interruptible = state.Interruptible,
            Comment = state.QuestComment,
            QuestSequence = sequences,
        };

        var options = new JsonSerializerOptions
        {
            WriteIndented = prettyPrintJson,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

        return JsonSerializer.Serialize(root, options);
    }

    private static ExportQuestStep BuildExportStep(StepState step)
    {
        return new ExportQuestStep
        {
            DataId = step.DataId,
            ItemId = step.ItemId,
            Position = ToExportVector3(step.Position),
            TerritoryId = step.TerritoryId,
            InteractionType = step.InteractionType,
            StopDistance = step.StopDistance,
            EnemySpawnType = NullIfWhiteSpace(step.EnemySpawnType),
            KillEnemyDataIds = BuildKillEnemyDataIds(step.KillEnemyDataIdsText),
            CombatDelaySecondsAtStart = step.CombatDelaySecondsAtStart,
            ComplexCombatData = ParseJsonElementOrNull(step.ComplexCombatDataJson),
            CombatItemUse = ParseJsonElementOrNull(step.CombatItemUseJson),
            CompletionQuestVariablesFlags = ParseJsonElementOrNull(step.CompletionQuestVariablesFlagsJson),
            ChatMessage = BuildChatMessage(step),
            Emote = NullIfWhiteSpace(step.EmoteName),
            Action = NullIfWhiteSpace(step.ActionName),
            ItemCount = step.ItemCount,
            ItemQuality = NullIfWhiteSpace(step.ItemQuality),
            AllowHighQuality = step.AllowHighQuality,
            GroundTarget = step.GroundTarget,
            ItemsToGather = ParseJsonElementOrNull(step.ItemsToGatherJson),
            GatheringPoint = step.GatheringPoint,
            TargetClass = NullIfWhiteSpace(step.TargetClassName),
            PickUpQuestId = step.PickUpQuestId,
            TurnInQuestId = step.TurnInQuestId,
            NextQuestId = step.NextQuestId,
            Comment = BuildStepComment(step),
        };
    }

    private static ExportChatMessage? BuildChatMessage(StepState step)
    {
        if (string.IsNullOrWhiteSpace(step.ChatMessageKey))
            return null;

        return new ExportChatMessage
        {
            ExcelSheet = NullIfWhiteSpace(step.ChatMessageExcelSheet),
            Key = step.ChatMessageKey!,
        };
    }

    private static List<uint>? BuildKillEnemyDataIds(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return null;

        var result = new List<uint>();
        var parts = text.Split(new[] { ',', ';', ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var part in parts)
        {
            if (uint.TryParse(part.Trim(), out var value) && value > 0)
                result.Add(value);
        }

        return result.Count == 0 ? null : result;
    }

    private static JsonElement? ParseJsonElementOrNull(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return null;

        using var doc = JsonDocument.Parse(raw);
        return doc.RootElement.Clone();
    }

    private static ExportVector3? ToExportVector3(Vector3? value)
    {
        if (value is null)
            return null;

        return new ExportVector3
        {
            X = value.Value.X,
            Y = value.Value.Y,
            Z = value.Value.Z,
        };
    }

    private static string? BuildStepComment(StepState step)
    {
        if (string.IsNullOrWhiteSpace(step.Comment))
            return null;

        return step.Comment;
    }

    private static string? NullIfWhiteSpace(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value;
}
