using System;
using static Definitions;

// Legacy Quest V1 model kept for backward compatibility with old save files.
// New quest progression should be tracked in Quest V2 runtime state.
public enum Progress
{
    NotStarted,
    InProgress,
    Done,
};

[Serializable]
public class Quest
{
    public Quests id;
    public string title;
    public string description;
    public Quests? isUnlockedBy;
    public bool IsRepeatable = false;
    public int currentDialogue = 0;

    public Progress currentProgress = Progress.NotStarted;
}
