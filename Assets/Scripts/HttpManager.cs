using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HttpManager : MonoBehaviour
{
    [SerializeField] private string URLLeaderboard;
    [SerializeField] private string URLSignUp;

    [SerializeField] Text[] players;
    [SerializeField] InputField usernameField;
    [SerializeField] InputField passwordField;

    private string token;
    private string username;

    private void Start()
    {
        token = PlayerPrefs.GetString("token");
        username = PlayerPrefs.GetString("username");

        StartCoroutine(GetPerfil());
    }

    public void ClickGetScores()
    {
        StartCoroutine(GetScores());
    }

    public void ClickSignUp()
    {
        string postData = GetInputData();

        StartCoroutine(SignUp(postData));
    }

    public void ClickLogIn()
    {
        string postData = GetInputData();

        StartCoroutine(LogIn(postData));
    }

    private string GetInputData()
    {
        AuthData data = new AuthData();

        data.username = usernameField.text;
        data.password = passwordField.text;

        string postData = JsonUtility.ToJson(data);

        return postData;
    }

    IEnumerator GetScores()
    {
        string url = URLLeaderboard + "/leaderboard";
        UnityWebRequest www = UnityWebRequest.Get(url);

        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR " + www.error);
        }
        else if (www.responseCode == 200)
        {
            Scores resData = JsonUtility.FromJson<Scores>(www.downloadHandler.text);

            for (int i = 0; i < resData.scores.Length; i++)
            {
                players[i].text = resData.scores[i].userId + ". " + resData.scores[i].name + " | " + resData.scores[i].value;
            }
        }
        else
        {
            Debug.Log(www.error);
        }
    }

    IEnumerator SignUp(string postData)
    {
        string url = URLSignUp + "/api/usuarios";
        UnityWebRequest www = UnityWebRequest.Put(url, postData);
        www.method = "POST";
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR " + www.error);
        }
        else if (www.responseCode == 200)
        {
            AuthData resData = JsonUtility.FromJson<AuthData>(www.downloadHandler.text);

            Debug.Log("Te has registrado " + resData.usuario.username);

            StartCoroutine(LogIn(postData));
        }
        else
        {
            Debug.Log(www.error);
            Debug.Log(www.downloadHandler.text);
        }
    }

    IEnumerator LogIn(string postData)
    {
        string url = URLSignUp + "/api/auth/login";
        UnityWebRequest www = UnityWebRequest.Put(url, postData);
        www.method = "POST";
        www.SetRequestHeader("content-type", "application/json");

        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR " + www.error);
        }
        else if (www.responseCode == 200)
        {
            //Debug.Log(www.downloadHandler.text);
            AuthData resData = JsonUtility.FromJson<AuthData>(www.downloadHandler.text);
            Debug.Log("Bienvenido " + resData.usuario.username + ", id:" + resData.usuario._id);
            Debug.Log("TOKEN: " + resData.token);

            PlayerPrefs.SetString("token", resData.token);
            PlayerPrefs.SetString("username", resData.usuario.username);
            SceneManager.LoadScene("Game");
        }
        else
        {
            Debug.Log(www.error);
            Debug.Log(www.downloadHandler.text);
        }
    }

    IEnumerator GetPerfil()
    {
        string url = URLSignUp + "/api/usuarios/" + username;
        UnityWebRequest www = UnityWebRequest.Get(url);
        www.SetRequestHeader("x-token", token);
        yield return www.SendWebRequest();
        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR " + www.error);
        }
        else if (www.responseCode == 200)
        {
            AuthData resData = JsonUtility.FromJson<AuthData>(www.downloadHandler.text);
            Debug.Log("Token valido " + resData.usuario.username + ", id:" + resData.usuario._id + " y su score es: " + resData.usuario.score);
            SceneManager.LoadScene("Game");
        }
        else
        {
            Debug.Log(www.error);
            Debug.Log(www.downloadHandler.text);
        }
    }

}

[System.Serializable]
public class ScoreData
{
    public int userId;
    public int value;
    public string name;

}

[System.Serializable]
public class Scores
{
    public ScoreData[] scores;
}

[System.Serializable]
public class AuthData
{
    public string username;
    public string password;
    public UserData usuario;
    public string token;
}

[System.Serializable]
public class UserData
{
    public string _id;
    public string username;
    public bool estado;
    public int score;
}
