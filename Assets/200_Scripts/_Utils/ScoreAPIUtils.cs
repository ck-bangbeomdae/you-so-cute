using Newtonsoft.Json;
using System.Collections;
using System.Text;
using UnityEngine.Networking;

public static class ScoreAPIUtils
{
    private const string baseUrl = "http://junhaddi.com:3000/scores";

    // GET 요청으로 모든 점수 가져오기
    public static IEnumerator GetScoresCoroutine(System.Action<string> onSuccess, System.Action<string> onError)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(baseUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                onSuccess?.Invoke(request.downloadHandler.text);
            }
            else
            {
                onError?.Invoke(request.error);
            }
        }
    }

    // POST 요청으로 새로운 점수 기록을 서버에 추가
    public static IEnumerator PostScoreCoroutine(Record record, System.Action<string> onSuccess, System.Action<string> onError)
    {
        string jsonData = JsonConvert.SerializeObject(record);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        using (UnityWebRequest request = new UnityWebRequest(baseUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                onSuccess?.Invoke("Score added successfully");
            }
            else
            {
                onError?.Invoke(request.error);
            }
        }
    }
}
