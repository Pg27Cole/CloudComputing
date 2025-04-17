using System;
using UnityEngine;
using UnityEngine.UI;

public class HouseRotateLogger : MonoBehaviour
{
    [SerializeField] private Slider slider;
    private int _timesUsed;
    private DateTime lastTimeChanged;

    private void Awake()
    {
        if (slider != null)
        {
            slider.onValueChanged.AddListener((_) => { 
                if ((DateTime.UtcNow - lastTimeChanged).Seconds <= 0.5f) return;
                _timesUsed++; 
                lastTimeChanged = DateTime.UtcNow;
            });
        }
    }

    public void SendData()
    {
        TelemetryManager.Instance.LogEvent("LogSeedChanged", new System.Collections.Generic.Dictionary<string, object>
        {
            {"timesUsed",  _timesUsed},
        });
    }
}
