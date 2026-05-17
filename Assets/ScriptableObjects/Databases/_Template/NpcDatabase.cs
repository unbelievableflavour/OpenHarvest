using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Npc Database", menuName = "Harvest/Databases/NPCs")]
public class NpcDatabase : ScriptableObject
{
    public List<NpcInteractableDefinition> npcs = new List<NpcInteractableDefinition>();

    public NpcInteractableDefinition FindById(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return null;
        }

        for (int i = 0; i < npcs.Count; i++)
        {
            NpcInteractableDefinition def = npcs[i];
            if (def == null)
            {
                continue;
            }

            if (string.Equals(def.npcId, id, System.StringComparison.Ordinal))
            {
                return def;
            }
        }

        return null;
    }
}
