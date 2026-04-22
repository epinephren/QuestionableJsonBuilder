using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;
using System.Numerics;

namespace QuestionableJsonBuilder.Windows;

public sealed class HelpWindow : Window
{
    public HelpWindow() : base("Help")
    {
        this.Size = new Vector2(520, 360);
        this.SizeCondition = ImGuiCond.FirstUseEver;

        this.SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(420, 200),
            MaximumSize = new Vector2(700, 360),
        };

        this.RespectCloseHotkey = true;
    }


    public override void Draw()
    {
        ImGui.TextUnformatted("Questionable JSON Builder Help");
        ImGui.Separator();

        ImGui.TextWrapped("This tool helps you create Questionable-compatible quest JSON without manually writing the file.");
        ImGui.Spacing();

        ImGui.TextUnformatted("Basic flow");
        ImGui.BulletText("Select a quest from the list.");
        ImGui.BulletText("Add middle sequences and steps.");
        ImGui.BulletText("Validate or build the JSON preview.");
        ImGui.BulletText("Export to your selected directory.");

        ImGui.Spacing();
        ImGui.TextUnformatted("Important");
        ImGui.BulletText("Sequence 0 and 255 are created automatically.");
        ImGui.BulletText("Use exact quest data from game data.");
        ImGui.BulletText("Use Questionable to get correct NPCIds and ItemIDs from there.");
        ImGui.BulletText("Always use current position to avoid JSON Validation File");
        ImGui.BulletText("Only change the export directory, not the generated filename.");

        ImGui.Spacing();
        ImGui.TextUnformatted("Step hints");
        ImGui.BulletText("Say uses ChatMessage fields.");
        ImGui.BulletText("Emote uses a predefined emote value.");
        ImGui.BulletText("Action uses a predefined action value.");
        ImGui.BulletText("Combat can use EnemySpawnType, KillEnemyDataIds, and advanced JSON fields.");

        
        ImGui.Spacing();
        ImGui.TextUnformatted("Final Notes");
        ImGui.BulletText("If you encounter any Error first check everything is filled correct");
        ImGui.BulletText("If you need help reach out to the official discord. Check: https://puni.sh/");
        ImGui.BulletText("Never change the Exported FileName for the JSON-Builded-Quest");
        ImGui.BulletText("Always remember you're awesome!");
    }
}
