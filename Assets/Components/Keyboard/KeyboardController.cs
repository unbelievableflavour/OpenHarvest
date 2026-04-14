using UnityEngine;
using UnityEngine.UI;

public class KeyboardController : MonoBehaviour
{
    public GameObject placeholder;
    public Text inputField;
    public GameObject lowKeys;
    public GameObject highKeys;
    public GameObject CapsLockButton;
    public GameObject ShiftButton;
    public Color ActiveButtonColor;

    private bool capsLockIsActive = false;
    private bool shiftIsActive = false;

    private Image capsLockButtonImage;
    private Image shiftButtonImage;
    private Color capsLockButtonImageBackupColor;
    private Color shiftButtonImageBackupColor;


    private Text submitField;

    void Start()
    {
        capsLockButtonImage = CapsLockButton.GetComponent<Image>();
        shiftButtonImage = ShiftButton.GetComponent<Image>();
        capsLockButtonImageBackupColor = capsLockButtonImage.color;
        shiftButtonImageBackupColor = shiftButtonImage.color;
    }

    public void TypeInInput(Text buttonText)
    {
        inputField.text += buttonText.text;
        ShowTypedText();
        if (shiftIsActive)
        {
            Shift();
        }
    }

    public void CapsLock()
    {
        shiftIsActive = false;
        shiftButtonImage.color = shiftButtonImageBackupColor;

        if (capsLockIsActive)
        {
            capsLockButtonImage.color = capsLockButtonImageBackupColor;
            capsLockIsActive = false;
            ToLowerCase();
            return;
        }

        capsLockButtonImage.color = ActiveButtonColor;
        capsLockIsActive = true;
        ToHigherCase();
    }

    public void Shift()
    {
        capsLockIsActive = false;
        capsLockButtonImage.color = capsLockButtonImageBackupColor;

        if (shiftIsActive)
        {
            shiftButtonImage.color = shiftButtonImageBackupColor;
            shiftIsActive = false;
            ToLowerCase();   
            return;
        }

        shiftButtonImage.color = ActiveButtonColor;
        shiftIsActive = true;
        ToHigherCase();        
    }

    public void Backspace()
    {
        inputField.text = inputField.text.Substring(0, inputField.text.Length - 1);
        if (inputField.text == "")
        {
            ShowPlaceholder();
        }
    }

    public void Clear()
    {
        ShowPlaceholder();
        inputField.text = "";
    }

    public void Submit()
    {
        submitField.text = inputField.text.ToString();
        submitField = null;
        Clear();
        this.gameObject.SetActive(false);
    }

    public void Close()
    {
        submitField = null;
        Clear();
        this.gameObject.SetActive(false);
    }

    public void ToLowerCase()
    {
        lowKeys.SetActive(true);
        highKeys.SetActive(false);
    }
    
    public void ToHigherCase()
    {
        lowKeys.SetActive(false);
        highKeys.SetActive(true);
    }

    public void ShowPlaceholder()
    {
        inputField.gameObject.SetActive(false);
        placeholder.SetActive(true  );
    }

    public void ShowTypedText()
    {
        inputField.gameObject.SetActive(true);
        placeholder.SetActive(false);
    }

    public void StartKeyboard(Text field)
    {
        submitField = field;
        this.gameObject.SetActive(true);
    }
}
