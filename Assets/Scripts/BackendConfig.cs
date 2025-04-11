using UnityEngine;

[CreateAssetMenu(fileName = "BackendConfig", menuName = "Scriptable Objects/BackendConfig")]
public class BackendConfig : ScriptableObject
{
    [Header("Server URL")]
    public string baseUrl;

    [Header("Telementry Endpoint")]
    public string telemetryEndpoint = "http://localhost:3000/telemetry";

    [Header("Data Sync Endpoint")]
    public string dataSyncEndpoint = "http://localhost:3000/sync";

    [Header("File Name")]
    public string fileName = "serverDataNoEnc.json";
}
