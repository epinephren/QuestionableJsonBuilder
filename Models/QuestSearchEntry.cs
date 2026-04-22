namespace QuestionableJsonBuilder.Models;

public sealed record QuestSearchEntry(
    ushort QuestId,
    string QuestName,
    string DisplayText,
    bool Implemented);
