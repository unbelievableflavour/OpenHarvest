using System;
using static Definitions;

[Serializable]
public class ItemOfTheWeek
{
    public int week = 0;
    public string currentItemId = "Tomato";
    public string nextItemId = "Carrot";

    public Items itemId = Items.Axe; // DEPRECATED
}