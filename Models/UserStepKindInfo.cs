namespace QuestionableJsonBuilder.Models;

public static class UserStepKindInfo
{
    public static readonly (UserStepKind Kind, string Label, string Hint)[] All =
    {
        (UserStepKind.AcceptQuest, "Accept quest", "Use this for quest acceptance steps."),
        (UserStepKind.TalkToNpc, "Talk to NPC", "Use this for normal NPC interactions."),
        (UserStepKind.HandOverToNpc, "Hand over to NPC", "Use this when the quest requires giving something to an NPC."),
        (UserStepKind.UseItem, "Use item", "Use this when the quest needs an item to be used."),
        (UserStepKind.HaveItem, "Have item", "Use this when the quest needs the player to have an item."),
        (UserStepKind.SayInChat, "Say", "Schema uses ChatMessage, not free text comments."),
        (UserStepKind.Emote, "Emote", "Schema uses the EEmote enum, not /command text."),
        (UserStepKind.UseAction, "Action", "Schema uses the EAction enum."),
        (UserStepKind.WalkToLocation, "Walk to location", "Use this when the step is just reaching a place."),
        (UserStepKind.Combat, "Combat", "Use EnemySpawnType, KillEnemyDataIds and optional advanced combat JSON."),
        (UserStepKind.Craft, "Craft", "Schema requires ItemId and ItemCount."),
        (UserStepKind.Gather, "Gather", "Schema requires ItemsToGather and optionally GatheringPoint."),
        (UserStepKind.SwitchClass, "Switch class", "Schema requires TargetClass."),
        (UserStepKind.Duty, "Duty", "Use this for duty-related transition steps."),
        (UserStepKind.Jump, "Jump", "Use this for jump-based movement steps."),
        (UserStepKind.Dive, "Dive", "Use this for dive-based movement steps."),
        (UserStepKind.CompleteQuest, "Complete quest", "Use this when the sequence ends with turning in or finishing the quest."),
        (UserStepKind.WaitForManualProgress, "Wait for manual progress", "Schema requires Comment for this step type."),
        (UserStepKind.Instruction, "Instruction", "Schema requires Comment for this step type."),
        (UserStepKind.CustomAdvanced, "Custom / advanced", "Use this only if the common options do not fit."),
    };

    public static string GetLabel(UserStepKind kind)
        => All.First(x => x.Kind == kind).Label;

    public static string GetHint(UserStepKind kind)
        => All.First(x => x.Kind == kind).Hint;
}
