using System;
using static Definitions;

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
