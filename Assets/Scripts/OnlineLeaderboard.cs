using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

internal class LeaderboardEntryRequest
{
    [JsonProperty("name")] public string Name { get; set; }
    [JsonProperty("score")] public long Score { get; set; }
    [JsonProperty("time")] public string Time { get; set; }
}

public class LeaderboardEntry
{
    [JsonProperty("id")] public int Id { get; set; }
    [JsonProperty("name")] public string Name { get; set; }
    [JsonProperty("score")] public long Score { get; set; }
    [JsonProperty("time")] public string Time { get; set; }
}

public enum SortBy
{
    Name,
    Time,
    Score,
}

public class OnlineLeaderboard : MonoBehaviour
{
    private const string BaseUrl = "https://volcanoleaderboard.gudmunduro.com";
    public Dictionary<int, List<LeaderboardEntry>> LeaderboardPages { get; private set; }
    public int PageCount { get; private set; }
    private int _sortBy;

    private void Awake()
    {
        LeaderboardPages = new Dictionary<int, List<LeaderboardEntry>>();
        PageCount = 1;
        _sortBy = 0;
    }

    public void SetSort(SortBy type)
    {
        switch (type)
        {
            case SortBy.Name:
                _sortBy = 2;
                break;
            case SortBy.Time:
                _sortBy = 1;
                break;
            default:
                _sortBy = 0;
                break;
        }

        ClearCache();
    }

    public IEnumerator SubmitEntry(string name, long score, string time)
    {
        var requestObject = new LeaderboardEntryRequest
        {
            Name = name,
            Score = score,
            Time = time
        };

        var jsonBody = JsonConvert.SerializeObject(requestObject);
        var request = new UnityWebRequest($"{BaseUrl}/leaderboard/entries", "POST");
        var bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();
    }

    public IEnumerator LoadLeaderboard(int page)
    {
        var request = UnityWebRequest.Get($"{BaseUrl}/leaderboard/entries?page={page}&order-by={_sortBy}");
        request.SetRequestHeader("Accept", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            var jsonResponse = request.downloadHandler.text;
            var entries = JsonConvert.DeserializeObject<List<LeaderboardEntry>>(jsonResponse);

            LeaderboardPages[page] = entries;
        }
    }

    public IEnumerator LoadTabCount()
    {
        var request = UnityWebRequest.Get($"{BaseUrl}/leaderboard/tab-count");
        request.SetRequestHeader("Accept", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            var count = request.downloadHandler.text;

            if (count.All(char.IsDigit))
            {
                PageCount = int.Parse(count);
            }
        }
    }

    public bool IsPageLoaded(int page)
    {
        return LeaderboardPages.ContainsKey(page);
    }

    public void ClearCache()
    {
        LeaderboardPages.Clear();
    }
}