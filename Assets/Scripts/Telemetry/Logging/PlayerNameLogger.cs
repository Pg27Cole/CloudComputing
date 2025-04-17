using TMPro;
using UnityEngine;

public class PlayerNameLogger : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI usernameTextBox;
    public void SendData()
    {
        TelemetryManager.Instance.LogEvent("LogPlayerName", new System.Collections.Generic.Dictionary<string, object>
        {
            {"playerName", usernameTextBox.text}
        });
    }
}
