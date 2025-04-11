using TMPro;
using UnityEngine;

public class Preferences : MonoBehaviour
{
    public TMP_Text userNameText;

    public void Awake()
    {
        if (userNameText == null) return;
        LoadData();
    }

    public void SaveData(TMP_Text textInput)
    {
        PlayerPrefs.SetString("UserName", textInput.text);
        PlayerPrefs.Save();
    }

    public void LoadData()
    {
        string userName = PlayerPrefs.GetString("UserName");
        userNameText.text = userName;
    }
}
