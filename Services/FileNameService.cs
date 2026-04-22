using System.Text;
using QuestionableJsonBuilder.Models;

namespace QuestionableJsonBuilder.Services;

public sealed class FileNameService
{
    public string BuildQuestJsonFileName(QuestIdentity quest)
    {
        var safeName = Sanitize(quest.QuestName);
        return $"{quest.QuestId:D4}_{safeName}.json";
    }

    private static string Sanitize(string value)
    {
        var invalid = Path.GetInvalidFileNameChars();
        var sb = new StringBuilder(value.Length);

        foreach (var c in value)
            sb.Append(invalid.Contains(c) ? '_' : c);

        return sb.ToString().Trim();
    }
}
