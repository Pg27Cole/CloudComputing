using UnityEngine;
using TMPro;
using System;
using UnityEngine.SceneManagement;
using System.IO;
using Unity.VisualScripting.Antlr3.Runtime;
public class MainMenuManager : MonoBehaviour
{
    [Header("Sign Up Fields")]
    public TMP_InputField signUpUsernameField;
    public TMP_InputField signUpEmailField;
    public TMP_InputField signUpPasswordField;

    [Header("Sign In Fields")]
    public TMP_InputField signInUsernameField;
    public TMP_InputField signInPasswordField;

    [Header("Auth References")]
    public AuthService authService;

    [Header("Login Scene")]
    public string gameSceneName = "Game";

    public void SignUpButton()
    {
        string username = signUpUsernameField.text.Trim();
        string email = signUpEmailField.text.Trim();
        string password = signUpPasswordField.text.Trim();
        StartCoroutine(authService.SignUp(username, email, password, OnSignUpCompleted));
    }

    private void OnSignUpCompleted(bool success, string responseData)
    {
        if(success)
        {
            Debug.Log("Sign Up Successful:" + responseData);
        }
        else
        {
            Debug.Log("Sign Up Failed" + responseData);
        }
    }

    public void SignInButton()
    {
        string username = signInUsernameField.text.Trim();
        string password = signInPasswordField.text;
        StartCoroutine(authService.SignIn(username, password, OnSignInCompleted));
    }

    private void OnSignInCompleted(bool success, string responseData)
    {
        if (success)
        {
            AuthService.SignInResponse signInResponse = JsonUtility.FromJson<AuthService.SignInResponse>(responseData);
            if (string.IsNullOrEmpty(signInResponse.token))
            {
                Debug.Log("No Token In Response: " + responseData);
                return;
            }
            SessionManager.Instance.SetAuthToken(signInResponse.token);
            string path = Path.Combine(Application.persistentDataPath, "token.txt");
            File.WriteAllText(path, signInResponse.token);
            Debug.Log("Login Successful" + signInResponse.token);
            SceneManager.LoadScene(gameSceneName);  
        }
        else
        {
            Debug.Log("Login Failed: " + responseData);
        }
    }
}