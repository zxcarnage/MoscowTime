using System.Collections;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TimeController : MonoBehaviour
{
    [DllImport("__Internal")] 
    private static extern void AlertTime(string time);
    
    [SerializeField] private string _serverUrl;
    [SerializeField] private Button _button;
    
    private string _serverTime;

    private void OnEnable()
    {
        _button.onClick.AddListener(GetMoscowTime);
    }
    
    private void OnDisable()
    {
        _button.onClick.RemoveListener(GetMoscowTime);
    }

    private void GetMoscowTime()
    {
        StartCoroutine(GetMoscowTimeRoutine());
    }

    private IEnumerator GetMoscowTimeRoutine()
    {
        UnityWebRequest request = UnityWebRequest.Get(_serverUrl);
        yield return request.SendWebRequest();
        if(IsThereErrors(request))
            yield break;
        HandleResponse(request);
    }

    private void HandleResponse(UnityWebRequest request)
    {
        var json = request.downloadHandler.text;
        var response = JsonUtility.FromJson<DateTime>(json);
        _serverTime = ParseTime(response.datetime);
        AlertTime(_serverTime);
    }

    private bool IsThereErrors(UnityWebRequest request)
    {
        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Connection error");
            return true;
        }

        if (request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log("HTTP error");
            return true;
        }

        return false;
    }

    private string ParseTime(string datetime)
    {
        var time = Regex.Match(datetime, @"\d{2}:\d{2}:\d{2}");
        return time.Value;
    }
}
