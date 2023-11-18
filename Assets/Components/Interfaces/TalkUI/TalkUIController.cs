using UnityEngine;
using UnityEngine.UI;

public class TalkUIController : MonoBehaviour
{
    public GameObject talkUI;
    public GameObject miniUI;
    public Transform addedTalkOptions;
    public Transform talkOptionsButtons;
    public GameObject talkButtonPrefab;
    public Text talkUIHeading;

    [Header("Dialog Specific")]

    public string title;
    public NPCController npc;
    public TalkUIOption[] dialogOptions;

    private void Start()
    {
        miniUI.SetActive(false);
        LoadDialogueOptions();
        talkUIHeading.text = title;
    }

    private void DisableAll()
    {
        miniUI.SetActive(false);
        talkUI.SetActive(false);
        foreach (Transform talkOption in addedTalkOptions)
        {
            talkOption.gameObject.SetActive(false);
        }
    }

    public void Reset()
    {
        DisableAll();
        miniUI.SetActive(true);
    }

    public void EnableTalkUI()
    {
        DisableAll();
        npc.BackToIdle();
        talkUI.SetActive(true);
    }

    public void EnableWindow(GameObject dialog)
    {
        DisableAll();
        dialog.SetActive(true);
        npc.NPCTalks(dialog);
    }

    private GameObject InstantiateDialog(TalkUIOption talkUIOption)
    {
        talkUIOption.SetTalkUIController(this);
        talkUIOption.SetNPCController(npc);

        GameObject newDialog = Instantiate(talkUIOption.Dialog);

        newDialog.transform.SetParent(addedTalkOptions);
        newDialog.transform.localPosition = new Vector3(0, 0, 0);
        newDialog.transform.localRotation = new Quaternion(0, 0, 0, 0);
        newDialog.SetActive(false);

        AddButton(newDialog, talkUIOption);

        return newDialog;
    }

    private void AddButton(GameObject newDialog, TalkUIOption talkOption)
    {
        void buttonTask()
        {
            EnableWindow(newDialog);
        }

        GameObject talkButton = Instantiate(talkButtonPrefab);
        talkButton.GetComponentInChildren<Text>().text = talkOption.title;
        talkButton.transform.Find("MechanicSprite").GetComponent<Image>().sprite = talkOption.icon;
        talkButton.GetComponent<Button>().onClick.AddListener(buttonTask);
        talkButton.transform.SetParent(talkOptionsButtons, false);
    }

    private void LoadDialogueOptions()
    {
        foreach (TalkUIOption talkUIOption in dialogOptions)
        {
            GameObject newDialog = InstantiateDialog(talkUIOption);
            var talkOption = talkUIOption.GetComponent<TalkOption>();
            if (talkOption)
            {
                talkOption.SetTalkDialog(newDialog.GetComponent<Dialogue>());
                continue;
            }

            var storeOption = talkUIOption.GetComponent<StoreOption>();
            if (storeOption)
            {
                storeOption.SetInstantiatedStore(newDialog);
                continue;
            }
        }
    }
}
