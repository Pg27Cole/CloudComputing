using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class CloudSaveNoEnc : MonoBehaviour
{
    public LocalSaveManager localSaveManager;
    public BackendConfig BackendConfig;

    public void SyncWithCloud()
    {
        StartCoroutine(SyncRoutine());
    }

    private IEnumerator SyncRoutine()
    {
        PlayerData localData = localSaveManager.playerData;
        string localJson = JsonUtility.ToJson(localData);

        WWWForm form = new WWWForm();

        form.AddField("plainJson", localJson);

        using (UnityWebRequest www = UnityWebRequest.Post(BackendConfig.dataSyncEndpoint, form))
        {
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"SyncError: {www.error}");
            }
            else
            {
                string serverResponse = www.downloadHandler.text;
                PlayerData serverData = JsonUtility.FromJson<PlayerData>(serverResponse);

                Debug.Log(serverData.playerName);
                localSaveManager.playerData = serverData;
                localSaveManager.SaveToLocal();

                Debug.Log("Data Synced With Local Storage Successfully");
            }
        }
    }
}