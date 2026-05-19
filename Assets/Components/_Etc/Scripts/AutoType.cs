using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AutoType : MonoBehaviour
{
	//private float letterPause = 0.0001f;
	string message;
    private void Awake()
    {
		ResetText(message != null ? message : GetComponent<Text>().text);
	}

    // Use this for initialization
    void OnEnable()
	{
		GetComponent<Text>().text = "";
		StartCoroutine(TypeText());
	}

	IEnumerator TypeText()
	{
		foreach (char letter in message.ToCharArray())
		{
			GetComponent<Text>().text += letter;
			yield return 0;
			//yield return new WaitForMill(letterPause);
		}
	}

	public void ResetText(string newText)
	{
		message = newText;
	}

	public void Refresh()
	{
		enabled = false;
		enabled = true;
	}

	public string GetMessage()
	{
		return message;
	}
}