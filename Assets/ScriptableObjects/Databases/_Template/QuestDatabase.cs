using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quest Database", menuName = "Harvest/Databases/Quests")]
public class QuestDatabase : ScriptableObject
{
    public List<QuestGraph> quests = new List<QuestGraph>();

    public QuestGraph FindById(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return null;
        }

        for (int i = 0; i < quests.Count; i++)
        {
            QuestGraph q = quests[i];
            if (q == null)
            {
                continue;
            }

            if (string.Equals(q.questId, id, System.StringComparison.Ordinal))
            {
                return q;
            }
        }

        return null;
    }
}
