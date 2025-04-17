using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class AuthService : MonoBehaviour
{
    [Header("URL Reference")]
    public BackendConfig config;


    public async void Start()
    {
        string path = Path.Combine(Application.persistentDataPath, "token.txt");
        if (!File.Exists(path)) return;
        string payload = File.ReadAllText(path);
        Debug.Log($"payload: {payload}");
        if (await CheckTokenValidity(payload))
        {
            SceneManager.LoadScene("Game");
        }
    }

    public IEnumerator SignUp(string username, string email, string password, System.Action<bool, string> callback)
    {
        var payload = new SignUpRequest { username = username, email = email, password = password};
        string jsonData = JsonUtility.ToJson(payload);

        string url = config.baseUrl + "/api/auth/signup";

        using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
        {
            www.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonData));
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            yield return www.SendWebRequest();

            callback(www.result == UnityWebRequest.Result.Success, www.downloadHandler.text);
        }
    }

    public IEnumerator SignIn(string username, string password, System.Action<bool, string> callback)
    {
        var payload = new SignInRequest { username = username, password = password };
        string jsonData = JsonUtility.ToJson(payload);
        
        string url = config.baseUrl + "/api/auth/signin";
        using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
        {
            www.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonData));
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            yield return www.SendWebRequest();

            callback(www.result == UnityWebRequest.Result.Success, www.downloadHandler.text);
        }
        
    }

    public async Task<bool> CheckTokenValidity(string token)
    {
        using (UnityWebRequest www = new UnityWebRequest(config.baseUrl + "/api/protected", "GET"))
        {
            www.SetRequestHeader("authorization", $"bearer {token}");
            await www.SendWebRequest();
            return www.result == UnityWebRequest.Result.Success;
        }
    }

    [System.Serializable]
    public class SignUpRequest
    {
        public string username;
        public string email;
        public string password;
    }

    [System.Serializable]
    public class SignInRequest
    {
        public string username;
        public string password;
    }

    [System.Serializable]
    public class SignInResponse
    {
        public string token;
        public string message;
    }
}
