using System.Numerics;

namespace QuestionableJsonBuilder.Models;

public sealed class StepState
{
    public UserStepKind UserStepKind { get; set; } = UserStepKind.TalkToNpc;
    public string InteractionType { get; set; } = "Interact";

    public uint? DataId { get; set; }
    public uint? ItemId { get; set; }
    public Vector3? Position { get; set; }
    public ushort TerritoryId { get; set; }
    public float? StopDistance { get; set; }
    public string? Comment { get; set; }

    public string? ChatMessageExcelSheet { get; set; }
    public string? ChatMessageKey { get; set; }

    public string? EmoteName { get; set; }
    public string? ActionName { get; set; }

    public string? EnemySpawnType { get; set; }
    public string? KillEnemyDataIdsText { get; set; }
    public float? CombatDelaySecondsAtStart { get; set; }

    public string? ComplexCombatDataJson { get; set; }
    public string? CombatItemUseJson { get; set; }
    public string? CompletionQuestVariablesFlagsJson { get; set; }

    public int? ItemCount { get; set; }
    public string? ItemQuality { get; set; }
    public bool? AllowHighQuality { get; set; }
    public bool? GroundTarget { get; set; }

    public string? ItemsToGatherJson { get; set; }
    public ushort? GatheringPoint { get; set; }

    public string? TargetClassName { get; set; }

    public uint? PickUpQuestId { get; set; }
    public uint? TurnInQuestId { get; set; }
    public uint? NextQuestId { get; set; }

    // Backward-compatible generic field kept only for older drafts.
    public string? UserTextValue { get; set; }
}
