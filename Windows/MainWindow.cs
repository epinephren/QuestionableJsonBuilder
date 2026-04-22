using System.Diagnostics;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;
using Dalamud.Interface.Utility.Raii;
using QuestionableJsonBuilder.Models;
using QuestionableJsonBuilder.Services;

namespace QuestionableJsonBuilder.Windows;

public sealed class MainWindow : Window
{
    private static readonly string[] EnemySpawnTypeOptions =
    {
        "AfterInteraction",
        "AfterItemUse",
        "AfterAction",
        "AfterEmote",
        "AutoOnEnterArea",
        "OverworldEnemies",
        "FateEnemies",
        "FinishCombatIfAny",
        "QuestInterruption"
    };

    private static readonly string[] EmoteOptions =
    {
        "Surprised",
        "Angry",
        "Furious",
        "Blush",
        "Bow",
        "Cheer",
        "Clap",
        "Beckon",
        "Comfort",
        "Cry",
        "Dance",
        "Doubt",
        "Doze",
        "Fume",
        "Goodbye",
        "Wave",
        "Huh",
        "Joy",
        "Kneel",
        "Chuckle",
        "Laugh",
        "Lookout",
        "Me",
        "No",
        "Deny",
        "Panic",
        "Point",
        "Poke",
        "Congratulate",
        "Psych",
        "Salute",
        "Shocked",
        "Shrug",
        "Rally",
        "Soothe",
        "Stagger",
        "Stretch",
        "Sulk",
        "Think",
        "Upset",
        "Welcome",
        "Yes",
        "ThumbsUp",
        "ExamineSelf",
        "Pose",
        "BlowKiss",
        "Grovel",
        "Happy",
        "Disappointed",
        "Lounge",
        "GroundSit",
        "AirQuotes",
        "GcSalute",
        "Pray",
        "ImperialSalute",
        "Visor",
        "Megaflare",
        "CrimsonLotus",
        "Charmed",
        "CheerOn",
        "CheerWave",
        "CheerJump",
        "StraightFace",
        "Smile",
        "Grin",
        "Smirk",
        "Taunt",
        "ShutEyes",
        "Sad",
        "Scared",
        "Amazed",
        "Ouch",
        "Annoyed",
        "Alert",
        "Worried",
        "BigGrin",
        "Reflect",
        "Furrow",
        "Scoff",
        "Throw",
        "ChangePose",
        "StepDance",
        "HarvestDance",
        "BallDance",
        "MandervilleDance",
        "Pet",
        "HandOver",
        "BombDance",
        "Hurray",
        "Slap",
        "Hug",
        "Embrace",
        "Hildibrand",
        "FistBump",
        "ThavDance",
        "GoldDance",
        "SundropDance",
        "BattleStance",
        "VictoryPose",
        "Backflip",
        "EasternGreeting",
        "Eureka",
        "MogDance",
        "Haurchefant",
        "EasternStretch",
        "EasternDance",
        "RangerPose1R",
        "RangerPose2R",
        "RangerPose3R",
        "Wink",
        "RangerPose1L",
        "RangerPose2L",
        "RangerPose3L",
        "Facepalm",
        "Zantetsuken",
        "Flex",
        "Respect",
        "Sneer",
        "PrettyPlease",
        "PlayDead",
        "IceHeart",
        "MoonLift",
        "Dote",
        "Spectacles",
        "Songbird",
        "WaterFloat",
        "WaterFlip",
        "PuckerUp",
        "PowerUp",
        "EasternBow",
        "Squats",
        "PushUps",
        "SitUps",
        "BreathControl",
        "Converse",
        "Concentrate",
        "Disturbed",
        "Simper",
        "Beam",
        "Attention",
        "AtEase",
        "Box",
        "RitualPrayer",
        "Tremble",
        "Winded",
        "Aback",
        "Greeting",
        "BoxStep",
        "SideStep",
        "Ultima",
        "YolDance",
        "Splash",
        "Sweat",
        "Shiver",
        "Elucidate",
        "Ponder",
        "LeftWink",
        "GetFantasy",
        "PopotoStep",
        "Hum",
        "Confirm",
        "Scheme",
        "Endure",
        "Tomestone",
        "HeelToe",
        "GoobbueDouble",
        "Gratuity",
        "FistPump",
        "Reprimand",
        "Sabotender",
        "MandervilleMambo",
        "LaliHo",
        "SimulationM",
        "SimulationF",
        "Toast",
        "Lean",
        "Headache",
        "Snap",
        "BreakFast",
        "Read",
        "Insist",
        "Consider",
        "Wasshoi",
        "FlowerShower",
        "FlameDance",
        "HighFive",
        "Guard",
        "Malevolence",
        "BeesKnees",
        "LaliHop",
        "EatRiceBall",
        "EatApple",
        "WringHands",
        "Sweep",
        "PaintBlack",
        "PaintRed",
        "PaintYellow",
        "PaintBlue",
        "FakeSmile",
        "Pantomime",
        "Vexed",
        "Shush",
        "EatPizza",
        "ClutchHead",
        "EatChocolate",
        "EatEgg",
        "Content",
        "Sheathe",
        "Draw",
        "Tea",
        "Determined",
        "ShowRight",
        "ShowLeft",
        "Deride",
        "Wow",
        "EatPumpkinCookie",
        "Spirit",
        "MagicTrick",
        "LittleLadiesDance",
        "Linkpearl",
        "EarWiggle",
        "Frighten",
        "AdventOfLight",
        "JumpForJoy1",
        "JumpForJoy2",
        "JumpForJoy3",
        "JumpForJoy4",
        "JumpForJoy5",
        "HandToHeart",
        "CheerOnBright",
        "CheerWaveViolet",
        "CheerJumpGreen",
        "AllSaintsCharm",
        "LopHop",
        "Reference",
        "EatChicken",
        "Sundering",
        "Slump",
        "LoveHeart",
        "HumbleTriumph",
        "VictoryReveal",
        "FryEgg",
        "Uchiwasshoi",
        "Attend",
        "Water",
        "ShakeDrink",
        "Unbound",
        "Bouquet",
        "BlowBubbles",
        "Ohokaliy",
        "Visage",
        "Photograph",
        "Overreact",
        "Twirl",
        "Dazed",
        "Rage",
        "TomeScroll",
        "Study",
        "GridanianSip",
        "UldahnSip",
        "LominsanSip",
        "GridanianGulp",
        "UldahnGulp",
        "LominsanGulp",
        "Pen"
    };

    private static readonly string[] ActionOptions =
    {
        "DutyAction1",
        "DutyAction2",
        "HeavySwing",
        "Bootshine",
        "TwinSnakes",
        "Demolish",
        "DragonKick",
        "HeavyShot",
        "Cure",
        "Cure2",
        "Eukrasia",
        "Diagnosis",
        "EukrasianDiagnosis",
        "Esuna",
        "Physick",
        "AspectedBenefic",
        "FormShift",
        "FieryBreath",
        "BuffetSanuwa",
        "BuffetGriffin",
        "Trample",
        "Fumigate",
        "Roar",
        "Seed",
        "MagitekPulse",
        "MagitekThunder",
        "Inhale",
        "SiphonSnout",
        "PeculiarLight",
        "Cannonfire",
        "RedGulal",
        "YellowGulal",
        "BlueGulal",
        "ElectrixFlux",
        "HopStep",
        "Hide",
        "Ten",
        "Ninjutsu",
        "Chi",
        "Jin",
        "FumaShuriken",
        "Katon",
        "Raiton",
        "RabbitMedium",
        "SlugShot",
        "BosomBrook",
        "Souleater",
        "Fire3",
        "Adloquium",
        "WaterCannon",
        "Wasshoi",
        "ShroudedLuminescence",
        "BigSneeze",
        "TrickstersTreat",
        "TreatersTrick",
        "PruningPirouette",
        "RoaringEggscapade",
        "TheSpriganator",
        "Prospect",
        "CollectMiner",
        "LuckOfTheMountaineer",
        "ScourMiner",
        "MeticulousMiner",
        "ScrutinyMiner",
        "Triangulate",
        "CollectBotanist",
        "LuckOfThePioneer",
        "ScourBotanist",
        "MeticulousBotanist",
        "ScrutinyBotanist",
        "SharpVision1",
        "SharpVision2",
        "SharpVision3",
        "FieldMastery1",
        "FieldMastery2",
        "FieldMastery3",
        "FSHCast",
        "FSHQuit"
    };

    private readonly QuestWizardController controller;
    private readonly Configuration configuration;
    private readonly Action? openDebugUi;
    private readonly Action? openHelpUi;

    private string questSearchText = string.Empty;
    private int selectedQuestIndex = -1;
    private readonly Dictionary<int, string> stepAddSearchBySequence = new();
    private readonly Dictionary<string, string> comboSearch = new();

    public MainWindow(QuestWizardController controller, Configuration configuration)
        : this(controller, configuration, null, null)
    {
    }

    public MainWindow(QuestWizardController controller, Configuration configuration, Action? openDebugUi, Action? openHelpUi)
        : base("Questionable JSON Builder")
    {
        this.controller = controller;
        this.configuration = configuration;
        this.openDebugUi = openDebugUi;
        this.openHelpUi = openHelpUi;

        this.Size = new Vector2(1180, 860);
        this.SizeCondition = ImGuiCond.FirstUseEver;
        this.SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(920, 680),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue),
        };
    }

    public override void Draw()
    {
        if (ImGui.BeginTable("MainLayout", 2, ImGuiTableFlags.Resizable | ImGuiTableFlags.BordersInnerV))
        {
            ImGui.TableSetupColumn("Builder", ImGuiTableColumnFlags.WidthStretch, 0.62f);
            ImGui.TableSetupColumn("Output", ImGuiTableColumnFlags.WidthStretch, 0.38f);

            ImGui.TableNextColumn();
            DrawBuilderPane();

            ImGui.TableNextColumn();
            DrawOutputPane();

            ImGui.EndTable();
        }
    }

    private void DrawBuilderPane()
    {
        if (ImGui.BeginChild("BuilderPane", new Vector2(0, 0), true))
        {
            DrawQuestSearchSection();
            ImGui.Separator();
            DrawSequenceSection();
            ImGui.Separator();
            DrawFinishSection();
            ImGui.Separator();
            DrawActionsSection();
        }
        ImGui.EndChild();
    }

    private void DrawOutputPane()
    {
        if (ImGui.BeginChild("OutputPane", new Vector2(0, 0), true))
        {
            ImGui.TextUnformatted("Output / Validation");
            ImGui.Separator();
            ImGui.TextWrapped(controller.StatusText);
            ImGui.Separator();
            if (ImGui.BeginChild("OutputTextChild", new Vector2(0, 0), false, ImGuiWindowFlags.HorizontalScrollbar))
                ImGui.TextUnformatted(controller.OutputText);
            ImGui.EndChild();
        }
        ImGui.EndChild();
    }

    private void DrawQuestSearchSection()
    {
        ImGui.TextUnformatted("Quest Selection");
        if (openDebugUi is not null)
        {
            if (ImGui.Button("Open Quest Debug"))
                openDebugUi();
            ImGui.SameLine();
        }

        using (ImRaii.PushColor(ImGuiCol.Text, new Vector4(0.95f, 0.25f, 0.45f, 1f)))
        {
            if (ImGui.SmallButton("♥ Buy Me a Coffee"))
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "https://buymeacoffee.com/epinephren",
                        UseShellExecute = true,
                    });
                }
                catch
                {
                }
            }
        }

        ImGui.SameLine();
        if (ImGui.SmallButton("Help"))
            this.openHelpUi?.Invoke();

        var disabled = controller.State.QuestLocked;
        if (disabled)
            ImGui.BeginDisabled();

        ImGui.InputText("Search Quest", ref questSearchText, 256);

        var results = controller.SearchQuests(questSearchText);
        if (selectedQuestIndex >= results.Count)
            selectedQuestIndex = -1;

        if (ImGui.BeginChild("QuestSearchResults", new Vector2(0, 180), true, ImGuiWindowFlags.HorizontalScrollbar))
        {
            for (var i = 0; i < results.Count; i++)
            {
                var quest = results[i];
                var selected = i == selectedQuestIndex;
                var color = quest.Implemented
                    ? new Vector4(0.25f, 0.85f, 0.35f, 1f)
                    : new Vector4(0.92f, 0.28f, 0.28f, 1f);

                if (quest.Implemented)
                    ImGui.BeginDisabled();

                using (ImRaii.PushColor(ImGuiCol.Text, color))
                {
                    var marker = quest.Implemented ? "✓" : "✗";
                    if (ImGui.Selectable($"{marker} {quest.DisplayText}", selected))
                        selectedQuestIndex = i;
                }

                if (quest.Implemented)
                    ImGui.EndDisabled();
            }
        }
        ImGui.EndChild();

        if (ImGui.Button("Use Selected Quest") && selectedQuestIndex >= 0 && selectedQuestIndex < results.Count)
        {
            var selected = results[selectedQuestIndex];
            if (!selected.Implemented)
                controller.SetQuestDraft(selected.QuestId);
        }

        ImGui.SameLine();
        if (ImGui.Button("Refresh Lists"))
            controller.RefreshQuestIndex();

        if (disabled)
            ImGui.EndDisabled();

        var questState = controller.State.Quest;
        if (questState is not null)
            ImGui.TextUnformatted($"Selected Quest: {questState.QuestName} ({questState.QuestId})");

        if (!controller.State.QuestLocked)
        {
            if (ImGui.Button("Confirm and Lock Quest"))
                controller.ConfirmQuestLock();
        }
        else
        {
            if (ImGui.Button("Unlock Quest Details"))
                controller.UnlockQuest();
        }

        var author = controller.State.Author;
        if (ImGui.InputText("Author", ref author, 128))
            controller.State.Author = author;

        var interruptible = controller.State.Interruptible;
        if (ImGui.Checkbox("Interruptible", ref interruptible))
            controller.State.Interruptible = interruptible;

        var comment = controller.State.QuestComment ?? string.Empty;
        if (ImGui.InputText("Quest Comment", ref comment, 256))
            controller.State.QuestComment = string.IsNullOrWhiteSpace(comment) ? null : comment;
    }

    private void DrawSequenceSection()
    {
        ImGui.TextUnformatted("Sequences");

        if (ImGui.Button("Add Middle Sequence"))
            controller.AddMiddleSequence();

        if (ImGui.BeginChild("SequenceScrollArea", new Vector2(0, 330), true))
        {
            for (var i = 0; i < controller.State.MiddleSequences.Count; i++)
            {
                var sequence = controller.State.MiddleSequences[i];
                if (!ImGui.CollapsingHeader($"Sequence {sequence.Sequence}##seq{i}"))
                    continue;

                DrawSequenceHeader(i, sequence);
                DrawSequenceBody(i, sequence);
                ImGui.Separator();
            }
        }
        ImGui.EndChild();
    }

    private void DrawSequenceHeader(int index, SequenceState sequence)
    {
        var seqValue = (int)sequence.Sequence;
        if (ImGui.InputInt($"Sequence Number##{index}", ref seqValue))
            sequence.Sequence = (byte)Math.Clamp(seqValue, 1, 254);

        var sequenceComment = sequence.Comment ?? string.Empty;
        if (ImGui.InputText($"Comment##sequence{index}", ref sequenceComment, 256))
            sequence.Comment = string.IsNullOrWhiteSpace(sequenceComment) ? null : sequenceComment;

        DrawStepAdderDropdown(index);

        ImGui.SameLine();
        if (ImGui.Button($"Delete Sequence##{index}"))
            controller.RemoveMiddleSequenceAt(index);
    }

    private void DrawStepAdderDropdown(int sequenceIndex)
    {
        if (!stepAddSearchBySequence.TryGetValue(sequenceIndex, out var search))
            search = string.Empty;

        ImGui.AlignTextToFramePadding();
        ImGui.TextUnformatted("Add Step");
        ImGui.SameLine();

        const float comboWidth = 260f;
        ImGui.SetNextItemWidth(comboWidth);
        ImGui.SetNextWindowSizeConstraints(new Vector2(comboWidth, 0), new Vector2(comboWidth, 320));

        if (!ImGui.BeginCombo($"##AddStep{sequenceIndex}", "Select Step Type"))
            return;

        ImGui.SetNextItemWidth(-1);
        if (ImGui.InputText($"##StepAddSearch{sequenceIndex}", ref search, 128))
            stepAddSearchBySequence[sequenceIndex] = search;

        ImGui.Separator();

        if (ImGui.BeginChild($"StepAddResults{sequenceIndex}", new Vector2(0, 220), false))
        {
            foreach (var entry in UserStepKindInfo.All)
            {
                if (!string.IsNullOrWhiteSpace(search))
                {
                    var labelMatch = entry.Label.Contains(search, StringComparison.OrdinalIgnoreCase);
                    var hintMatch = entry.Hint.Contains(search, StringComparison.OrdinalIgnoreCase);
                    if (!labelMatch && !hintMatch)
                        continue;
                }

                if (ImGui.Selectable(entry.Label))
                {
                    controller.AddStep(sequenceIndex, entry.Kind);
                    stepAddSearchBySequence[sequenceIndex] = string.Empty;
                    ImGui.CloseCurrentPopup();
                }

                if (ImGui.IsItemHovered())
                    ImGui.SetTooltip(entry.Hint);
            }
        }
        ImGui.EndChild();

        ImGui.EndCombo();
    }

    private void DrawSequenceBody(int sequenceIndex, SequenceState sequence)
    {
        if (sequence.Steps.Count == 0)
        {
            ImGui.TextWrapped("No steps yet.");
            return;
        }

        for (var stepIndex = 0; stepIndex < sequence.Steps.Count; stepIndex++)
        {
            var step = sequence.Steps[stepIndex];
            if (!ImGui.TreeNode($"Step {stepIndex + 1}##{sequenceIndex}_{stepIndex}"))
                continue;

            DrawStepEditor(sequenceIndex, stepIndex, step);
            ImGui.TreePop();
        }
    }

    private void DrawStepEditor(int sequenceIndex, int stepIndex, StepState step)
    {
        ImGui.TextWrapped(UserStepKindInfo.GetHint(step.UserStepKind));

        var preview = UserStepKindInfo.GetLabel(step.UserStepKind);
        if (ImGui.BeginCombo($"Step Type##{sequenceIndex}_{stepIndex}", preview))
        {
            foreach (var entry in UserStepKindInfo.All)
            {
                var selected = step.UserStepKind == entry.Kind;
                if (ImGui.Selectable(entry.Label, selected))
                {
                    step.UserStepKind = entry.Kind;
                    controller.ApplyStepKindDefaults(step);
                }
                if (selected)
                    ImGui.SetItemDefaultFocus();
            }
            ImGui.EndCombo();
        }

        var interactionType = step.InteractionType;
        if (ImGui.InputText($"Interaction Type##{sequenceIndex}_{stepIndex}", ref interactionType, 64))
            step.InteractionType = interactionType;

        var dataId = (int)(step.DataId ?? 0);
        if (ImGui.InputInt($"DataId##{sequenceIndex}_{stepIndex}", ref dataId))
            step.DataId = dataId <= 0 ? null : (uint)dataId;

        var territoryId = (int)step.TerritoryId;
        if (ImGui.InputInt($"Territory Id##{sequenceIndex}_{stepIndex}", ref territoryId))
            step.TerritoryId = (ushort)Math.Clamp(territoryId, 0, ushort.MaxValue);

        var stopDistance = step.StopDistance ?? 0f;
        if (ImGui.InputFloat($"Stop Distance##{sequenceIndex}_{stepIndex}", ref stopDistance))
            step.StopDistance = stopDistance <= 0 ? null : stopDistance;

        var position = step.Position ?? Vector3.Zero;
        if (ImGui.InputFloat3($"Position##{sequenceIndex}_{stepIndex}", ref position))
            step.Position = position;

        ImGui.SameLine();
        if (ImGui.Button($"Use Current Position##{sequenceIndex}_{stepIndex}"))
            controller.UseCurrentPosition(step);

        DrawSchemaSpecificFields(sequenceIndex, stepIndex, step);

        var comment = step.Comment ?? string.Empty;
        if (ImGui.InputText($"Comment##step{sequenceIndex}_{stepIndex}", ref comment, 256))
            step.Comment = string.IsNullOrWhiteSpace(comment) ? null : comment;

        if (ImGui.Button($"Delete Step##{sequenceIndex}_{stepIndex}"))
            controller.RemoveStep(sequenceIndex, stepIndex);
    }

    private void DrawSchemaSpecificFields(int sequenceIndex, int stepIndex, StepState step)
    {
        if (step.InteractionType == "Say")
        {
            var excelSheet = step.ChatMessageExcelSheet ?? string.Empty;
            if (ImGui.InputText($"ChatMessage ExcelSheet##{sequenceIndex}_{stepIndex}", ref excelSheet, 128))
                step.ChatMessageExcelSheet = string.IsNullOrWhiteSpace(excelSheet) ? null : excelSheet;

            var key = step.ChatMessageKey ?? string.Empty;
            if (ImGui.InputText($"ChatMessage Key##{sequenceIndex}_{stepIndex}", ref key, 128))
                step.ChatMessageKey = string.IsNullOrWhiteSpace(key) ? null : key;
        }

        if (step.InteractionType == "Emote")
        {
            var emoteName = step.EmoteName;
            DrawSearchableStringCombo($"Emote##{sequenceIndex}_{stepIndex}", EmoteOptions, ref emoteName);
            step.EmoteName = emoteName;
        }

        if (step.InteractionType == "Action")
        {
            var actionName = step.ActionName;
            DrawSearchableStringCombo($"Action##{sequenceIndex}_{stepIndex}", ActionOptions, ref actionName);
            step.ActionName = actionName;
        }

        if (step.InteractionType == "Combat")
        {
            var enemySpawnType = step.EnemySpawnType;
            DrawSimpleCombo($"Enemy Spawn Type##{sequenceIndex}_{stepIndex}", EnemySpawnTypeOptions, ref enemySpawnType);
            step.EnemySpawnType = enemySpawnType;

            var killIds = step.KillEnemyDataIdsText ?? string.Empty;
            if (ImGui.InputText($"KillEnemyDataIds##{sequenceIndex}_{stepIndex}", ref killIds, 256))
                step.KillEnemyDataIdsText = string.IsNullOrWhiteSpace(killIds) ? null : killIds;

            var combatDelay = step.CombatDelaySecondsAtStart ?? 0f;
            if (ImGui.InputFloat($"Combat Delay At Start##{sequenceIndex}_{stepIndex}", ref combatDelay))
                step.CombatDelaySecondsAtStart = combatDelay <= 0 ? null : combatDelay;

            var complexCombatDataJson = step.ComplexCombatDataJson;
            DrawMultilineJsonField($"ComplexCombatData JSON##{sequenceIndex}_{stepIndex}", ref complexCombatDataJson, 4);
            step.ComplexCombatDataJson = complexCombatDataJson;

            var combatItemUseJson = step.CombatItemUseJson;
            DrawMultilineJsonField($"CombatItemUse JSON##{sequenceIndex}_{stepIndex}", ref combatItemUseJson, 3);
            step.CombatItemUseJson = combatItemUseJson;

            var completionQuestVariablesFlagsJson = step.CompletionQuestVariablesFlagsJson;
            DrawMultilineJsonField($"CompletionQuestVariablesFlags JSON##{sequenceIndex}_{stepIndex}", ref completionQuestVariablesFlagsJson, 3);
            step.CompletionQuestVariablesFlagsJson = completionQuestVariablesFlagsJson;
        }

        if (step.InteractionType == "Craft")
        {
            var itemId = (int)(step.ItemId ?? 0);
            if (ImGui.InputInt($"ItemId##{sequenceIndex}_{stepIndex}", ref itemId))
                step.ItemId = itemId <= 0 ? null : (uint)itemId;

            var itemCount = step.ItemCount ?? 1;
            if (ImGui.InputInt($"ItemCount##{sequenceIndex}_{stepIndex}", ref itemCount))
                step.ItemCount = itemCount <= 0 ? null : itemCount;

            var itemQuality = step.ItemQuality ?? "Any";
            if (ImGui.InputText($"ItemQuality##{sequenceIndex}_{stepIndex}", ref itemQuality, 32))
                step.ItemQuality = string.IsNullOrWhiteSpace(itemQuality) ? null : itemQuality;

            var allowHighQuality = step.AllowHighQuality;
            DrawNullableBoolCheckbox($"Allow High Quality##{sequenceIndex}_{stepIndex}", ref allowHighQuality);
            step.AllowHighQuality = allowHighQuality;
        }

        if (step.InteractionType == "Gather")
        {
            var itemsToGatherJson = step.ItemsToGatherJson;
            DrawMultilineJsonField($"ItemsToGather JSON##{sequenceIndex}_{stepIndex}", ref itemsToGatherJson, 3);
            step.ItemsToGatherJson = itemsToGatherJson;

            var gatheringPoint = (int)(step.GatheringPoint ?? 0);
            if (ImGui.InputInt($"GatheringPoint##{sequenceIndex}_{stepIndex}", ref gatheringPoint))
                step.GatheringPoint = gatheringPoint <= 0 ? null : (ushort)gatheringPoint;
        }

        if (step.InteractionType == "SwitchClass")
        {
            var targetClass = step.TargetClassName ?? string.Empty;
            if (ImGui.InputText($"TargetClass##{sequenceIndex}_{stepIndex}", ref targetClass, 64))
                step.TargetClassName = string.IsNullOrWhiteSpace(targetClass) ? null : targetClass;
        }

        if (step.InteractionType == "AcceptQuest")
        {
            var pickUpQuestId = step.PickUpQuestId;
            DrawOptionalQuestIdInput($"PickUpQuestId##{sequenceIndex}_{stepIndex}", ref pickUpQuestId);
            step.PickUpQuestId = pickUpQuestId;
        }

        if (step.InteractionType == "CompleteQuest")
        {
            var turnInQuestId = step.TurnInQuestId;
            DrawOptionalQuestIdInput($"TurnInQuestId##{sequenceIndex}_{stepIndex}", ref turnInQuestId);
            step.TurnInQuestId = turnInQuestId;

            var nextQuestId = step.NextQuestId;
            DrawOptionalQuestIdInput($"NextQuestId##{sequenceIndex}_{stepIndex}", ref nextQuestId);
            step.NextQuestId = nextQuestId;

            var allowHighQuality = step.AllowHighQuality;
            DrawNullableBoolCheckbox($"Allow High Quality##{sequenceIndex}_{stepIndex}", ref allowHighQuality);
            step.AllowHighQuality = allowHighQuality;
        }

        if (step.InteractionType == "UseItem" || step.InteractionType == "Action")
        {
            var itemId = (int)(step.ItemId ?? 0);
            if (ImGui.InputInt($"ItemId##{sequenceIndex}_{stepIndex}", ref itemId))
                step.ItemId = itemId <= 0 ? null : (uint)itemId;

            var groundTarget = step.GroundTarget;
            DrawNullableBoolCheckbox($"Ground Target##{sequenceIndex}_{stepIndex}", ref groundTarget);
            step.GroundTarget = groundTarget;
        }
    }

    private void DrawSearchableStringCombo(string id, string[] options, ref string? value)
    {
        if (!comboSearch.TryGetValue(id, out var search))
            search = string.Empty;

        var current = string.IsNullOrWhiteSpace(value) ? "<select>" : value!;
        if (!ImGui.BeginCombo(id, current))
            return;

        ImGui.SetNextItemWidth(-1);
        if (ImGui.InputText($"##search_{id}", ref search, 128))
            comboSearch[id] = search;

        ImGui.Separator();
        if (ImGui.BeginChild($"##results_{id}", new Vector2(0, 200), false))
        {
            foreach (var option in options)
            {
                if (!string.IsNullOrWhiteSpace(search) && !option.Contains(search, StringComparison.OrdinalIgnoreCase))
                    continue;

                var selected = string.Equals(value, option, StringComparison.Ordinal);
                if (ImGui.Selectable(option, selected))
                {
                    value = option;
                    comboSearch[id] = string.Empty;
                    ImGui.CloseCurrentPopup();
                }
            }
        }
        ImGui.EndChild();
        ImGui.EndCombo();
    }

    private static void DrawSimpleCombo(string label, string[] options, ref string? value)
    {
        var current = string.IsNullOrWhiteSpace(value) ? options[0] : value!;
        if (ImGui.BeginCombo(label, current))
        {
            foreach (var option in options)
            {
                var selected = string.Equals(value, option, StringComparison.Ordinal);
                if (ImGui.Selectable(option, selected))
                    value = option;
            }
            ImGui.EndCombo();
        }
    }

    private static void DrawMultilineJsonField(string label, ref string? value, int lines)
    {
        var text = value ?? string.Empty;
        if (ImGui.InputTextMultiline(label, ref text, 4096, new Vector2(-1, ImGui.GetTextLineHeight() * lines + 18)))
            value = string.IsNullOrWhiteSpace(text) ? null : text;
    }

    private static void DrawOptionalQuestIdInput(string label, ref uint? value)
    {
        var v = (int)(value ?? 0);
        if (ImGui.InputInt(label, ref v))
            value = v <= 0 ? null : (uint)v;
    }

    private static void DrawNullableBoolCheckbox(string label, ref bool? value)
    {
        var current = value ?? false;
        if (ImGui.Checkbox(label, ref current))
            value = current;
        ImGui.SameLine();
        if (ImGui.SmallButton($"Clear##{label}"))
            value = null;
    }

    private void DrawFinishSection()
    {
        ImGui.TextUnformatted("Finish");
        var done = controller.State.QuestCompletedConfirmed;
        if (ImGui.Checkbox("Quest is done", ref done))
            controller.MarkQuestDone(done);
    }

    private void DrawActionsSection()
    {
        if (ImGui.Button("Validate"))
            controller.Validate();

        ImGui.SameLine();
        if (ImGui.Button("Build JSON Preview"))
            controller.BuildJsonPreview();

        ImGui.SameLine();
        if (ImGui.Button("Save File"))
            controller.SaveToDisk();

        var exportDirectory = string.IsNullOrWhiteSpace(configuration.DefaultExportDirectory)
            ? Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)
            : configuration.DefaultExportDirectory;

        ImGui.TextWrapped($"Export directory: {exportDirectory}");

        using (ImRaii.PushColor(ImGuiCol.Text, new Vector4(1f, 0.25f, 0.25f, 1f)))
        {
            ImGui.TextWrapped("DO NOT CHANGE THE FILENAME. Only the export directory should be changed. You can change it in the settings.");
        }
    }
}
