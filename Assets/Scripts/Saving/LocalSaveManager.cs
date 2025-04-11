using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LocalSaveManager : MonoBehaviour
{
    [SerializeField] private BackendConfig _backendConfig;
    [SerializeField] private Slider slider;
    [SerializeField] private HouseData[] houses;
    [SerializeField] private TMP_Text scoreValue;
    [SerializeField] private GameObject HousePrefab;
    [Header("READONLY ATTRIBUTE")] public PlayerData playerData;

    private void Start()
    {
        LoadFromLocal();
    }

    public void LoadFromLocal()
    {
        string path = Path.Combine(Application.persistentDataPath, PlayerPrefs.GetString("UserName") + _backendConfig.fileName);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            playerData = JsonUtility.FromJson<PlayerData>(json);
            Debug.Log("Game Data Loaded From: " + path);
            Debug.Log($"Player: {playerData.playerName}, last updated: {playerData.lastUpdated}");
        }
        else
        {
            Debug.Log("Warning: No save File found at: " + path + "\nCreating New Data File");
            playerData = new PlayerData
            {
                playerName = "Default Name",
                houses = new HouseData[0],
                sliderValue = 0,
                lastUpdated = 0,
                score = 0,
            };
        }

        slider.value = playerData.sliderValue;
        scoreValue.text = $"{playerData.score}";
        houses = playerData.houses;
        LoadHouses();
    }

    public void SaveToLocal()
    {
        playerData.lastUpdated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        playerData.sliderValue = slider.value;
        playerData.houses = houses;

        if(float.TryParse(scoreValue.text, out float score))
        {
            playerData.score = score;
        }
        else
        {
            playerData.score = 0;
        }
        SaveHouses();
        
        string json = JsonUtility.ToJson(playerData, true);

        string path = Path.Combine(Application.persistentDataPath, PlayerPrefs.GetString("UserName") + _backendConfig.fileName);
        File.WriteAllText(path, json);
        Debug.Log("Local Save Complete");
    }

    private void LoadHouses()
    {
        foreach(HouseData house in houses)
        {
            GameObject go = Instantiate(HousePrefab, house.position, Quaternion.Euler(house.rotation));
            go.name = "PlacedPrefab_super_cole";
        }
    }

    private void SaveHouses()
    {
        List<HouseData> placedHouses = new List<HouseData>();
        foreach (Transform go in FindObjectsByType<Transform>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
        {
            if (go.name.StartsWith("PlacedPrefab_"))
            {
                placedHouses.Add(new HouseData { position = go.transform.position, rotation = go.rotation.eulerAngles });
            }
        }
        houses = placedHouses.ToArray();
    }
}

