using QuestionableJsonBuilder.Models;

namespace QuestionableJsonBuilder.Services;

public sealed class QuestDataService
{
    public QuestIdentity? BuildManualQuest(ushort questId, string exactQuestName)
    {
        if (questId == 0 || string.IsNullOrWhiteSpace(exactQuestName))
            return null;

        return new QuestIdentity(questId, exactQuestName.Trim());
    }
}
