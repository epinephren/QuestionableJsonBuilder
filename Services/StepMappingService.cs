using QuestionableJsonBuilder.Models;

namespace QuestionableJsonBuilder.Services;

public sealed class StepMappingService
{
    public void ApplyDefaultsForKind(StepState step)
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
}
