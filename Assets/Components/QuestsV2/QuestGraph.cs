using UnityEngine;
using XNode;
using System.Text;

[CreateAssetMenu(
    fileName = "Quest Graph",
    menuName = "Harvest/DataTypes/Quest Graph",
    order = 30)]
public class QuestGraph : NodeGraph
{
    [ReadOnly]
    public string questId = "quest_id";
    public string displayName = "Quest";
    [Tooltip("Shared chat UI prefab for all chat nodes in this quest graph.")]
    public GameObject chatUIPrefab;

    [Tooltip("First node used when this quest starts.")]
    public QuestNodeBase entryNode;

    private void OnValidate()
    {
        string generated = GenerateQuestId(displayName);
        if (!string.IsNullOrWhiteSpace(generated))
        {
            questId = generated;
        }
    }

    public QuestNodeBase GetEntryNode()
    {
        return entryNode;
    }

    private static string GenerateQuestId(string source)
    {
        if (string.IsNullOrWhiteSpace(source))
        {
            return "quest";
        }

        string trimmed = source.Trim().ToLowerInvariant();
        var sb = new StringBuilder(trimmed.Length);
        bool previousWasUnderscore = false;

        for (int i = 0; i < trimmed.Length; i++)
        {
            char c = trimmed[i];
            bool isAlphaNumeric = (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9');
            if (isAlphaNumeric)
            {
                sb.Append(c);
                previousWasUnderscore = false;
                continue;
            }

            if (previousWasUnderscore)
            {
                continue;
            }

            sb.Append('_');
            previousWasUnderscore = true;
        }

        string value = sb.ToString().Trim('_');
        return string.IsNullOrWhiteSpace(value) ? "quest" : value;
    }
}
