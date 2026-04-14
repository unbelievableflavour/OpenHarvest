using UnityEngine;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour
{
    [Header("SpeakUIFields")]
    public Text DialogueHeading;
    public Text DialogueDescription;
    public Button DialogueLeftButton;
    public Button DialogueRightButton;

    public void UpdateDialogue(string heading, QuestDialogue dialogue)
    {
        DialogueHeading.text = heading;
        DialogueDescription.GetComponent<AutoType>().ResetText(dialogue.text);
        DialogueDescription.text = dialogue.text;

        if(dialogue.leftButton.text != "")
        {
            DialogueLeftButton.gameObject.SetActive(true);
            DialogueLeftButton.onClick = dialogue.leftButton.onClick;
            var text = DialogueLeftButton.GetComponentInChildren<Text>();
            text.text = dialogue.leftButton.text;
        } else {
            DialogueLeftButton.gameObject.SetActive(false);
        }

        if (dialogue.rightButton.text != "")
        {
            DialogueRightButton.gameObject.SetActive(true);
            DialogueRightButton.onClick = dialogue.rightButton.onClick;
            var text = DialogueRightButton.GetComponentInChildren<Text>();
            text.text = dialogue.rightButton.text;
        } else {
            DialogueRightButton.gameObject.SetActive(false);
        }
    }
}
