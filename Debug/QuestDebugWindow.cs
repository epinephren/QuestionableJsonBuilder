using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;
using QuestionableJsonBuilder.Services;

namespace QuestionableJsonBuilder.Debug;

public sealed class QuestDebugWindow : Window
{
    private readonly QuestWizardController controller;
    private string searchText = string.Empty;

    public QuestDebugWindow(QuestWizardController controller)
        : base("Quest Debug")
    {
        this.controller = controller;
        this.Size = new System.Numerics.Vector2(700, 500);
        this.SizeCondition = ImGuiCond.FirstUseEver;
    }

    public override void Draw()
    {
        ImGui.TextUnformatted("Quest Debug");
        ImGui.Separator();

        ImGui.TextWrapped(this.controller.QuestSourceStatus);
        ImGui.TextWrapped(this.controller.StatusText);

        if (ImGui.Button("Refresh Quest Sources"))
            this.controller.RefreshQuestIndex();

        ImGui.Separator();

        ImGui.InputText("Search", ref this.searchText, 256);

        var results = this.controller.SearchQuests(this.searchText);

        ImGui.TextUnformatted($"Quest count: {results.Count}");

        var has2694 = results.Any(q => q.QuestId == 2694);
        ImGui.TextUnformatted($"Has quest 2694 in current results: {(has2694 ? "Yes" : "No")}");

        ImGui.Separator();
        ImGui.TextUnformatted("First 10 results:");

        if (ImGui.BeginChild("QuestDebugResults", new System.Numerics.Vector2(0, 0), true))
        {
            for (var i = 0; i < results.Count && i < 10; i++)
            {
                var quest = results[i];
                var marker = quest.Implemented ? "[✓]" : "[X]";
                ImGui.TextUnformatted($"{marker} {quest.QuestId} - {quest.QuestName}");
            }

            if (results.Count == 0)
                ImGui.TextWrapped("No quests available yet. Log in and refresh quest sources.");

            ImGui.EndChild();
        }
    }
}
