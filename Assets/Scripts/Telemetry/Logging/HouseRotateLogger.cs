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
            });
        }
    }

    public void SendData()
    {
        TelemetryManager.Instance.LogEvent("buttonClicked", new System.Collections.Generic.Dictionary<string, object>
        {
            {"timeWhenUsed", DateTime.UtcNow.ToString("O")},
            {"timesUsed",  _timesUsed},
            {"sliderValue", slider.value}
        });
    }
}
