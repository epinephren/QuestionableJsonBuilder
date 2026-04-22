using System.Text.Json;
using QuestionableJsonBuilder.Models;

namespace QuestionableJsonBuilder.Services;

public sealed class ValidationService
{
    public IReadOnlyList<string> Validate(QuestWizardState state)
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
            JsonDocument.Parse(raw);
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
}
