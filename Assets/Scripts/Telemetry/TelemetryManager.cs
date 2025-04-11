using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class TelemetryManager : MonoBehaviour
{
    [SerializeField] BackendConfig serverConfig;

    public static TelemetryManager Instance { get; private set; }

    private Queue<Dictionary<string, object>> eventQueue;

    private bool isSending = false;


    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
            eventQueue = new Queue<Dictionary<string, object>>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LogEvent(string eventName, Dictionary<string, object> parameters = null)
    {
        if(parameters == null)
        {
            parameters = new Dictionary<string, object>();
        }

        parameters["eventName"] = eventName;
        parameters["sessionId"] = System.Guid.NewGuid().ToString();
        parameters["deviceTime"] = System.DateTime.UtcNow.ToString("O");

        eventQueue.Enqueue(parameters);

        if(!isSending)
        {
            StartCoroutine(SendEvents());
        }
    }

    private IEnumerator SendEvents()
    {
        isSending = true;

        while(eventQueue.Count > 0)
        {
            Dictionary<string, object> currentEvent = eventQueue.Dequeue();
            string payload = JsonUtility.ToJson(new SerializationWrapper(currentEvent));

            using (UnityWebRequest request = new UnityWebRequest(serverConfig.telemetryEndpoint, "POST"))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(payload);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();

                request.SetRequestHeader("Content-Type", "application/json");
                // TODO: add bearer token --- ex. "Bearer: 37adfba48qadbadfm"

                yield return request.SendWebRequest();

                if(request.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.LogError($"Error: {request.result}");
                    eventQueue.Enqueue(currentEvent);
                    break;
                }
                else
                {
                    Debug.Log("Request Sent: " + payload);
                }
            }
            yield return new WaitForSeconds(0.1f);
        }

        isSending = false;
    }

    [System.Serializable]
    private class SerializationWrapper
    {
        public List<string> keys = new List<string>();
        public List<string> values = new List<string>();

        public SerializationWrapper(Dictionary<string, object> parameters)
        {
            foreach(var kvp in parameters)
            {
                keys.Add(kvp.Key);
                values.Add(kvp.Value.ToString());
            }
        }
    }
}

