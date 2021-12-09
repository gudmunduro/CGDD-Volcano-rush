using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static System.Linq.Enumerable;

public class LeaderboardUI : MonoBehaviour
{
    private const float EntryHeight = 40f;
    
    public OnlineLeaderboard onlineLeaderboard;
    public Transform leaderboardEntryPrefab;
    public GameObject leaderboardContainer;
    public StartScript startScript;
    
    private bool _isTableFilled = false;
    private int _currentPage = 0;
    
    void Start()
    {
        StartCoroutine(onlineLeaderboard.LoadTabCount());
        StartCoroutine(onlineLeaderboard.LoadLeaderboard(0));
    }
    
    void Update()
    {
        if (!_isTableFilled && onlineLeaderboard.IsPageLoaded(0))
        {
            _addEntriesToScene(onlineLeaderboard.LeaderboardPages[0]);
            _isTableFilled = true;
        }
    }

    private void _addEntriesToScene(List<LeaderboardEntry> entries)
    {
        var entryY = -(EntryHeight / 2);

        foreach (var entry in entries)
        {
            var entryUI = Instantiate(leaderboardEntryPrefab, leaderboardContainer.transform);
            entryUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, entryY);

            entryUI.Find("Name").GetComponent<TextMeshProUGUI>().text = entry.Name;
            entryUI.Find("Time").GetComponent<TextMeshProUGUI>().text = entry.Time;
            entryUI.Find("Score").GetComponent<TextMeshProUGUI>().text = entry.Score.ToString();

            entryY -= EntryHeight;
        }
    }

    private void _clearTableEntries()
    {
        foreach (Transform entry in leaderboardContainer.transform)
        {
            Destroy(entry.gameObject);
        }

        _isTableFilled = false;
    }

    public void NextPage()
    {
        if (_currentPage >= onlineLeaderboard.TabCount-1) return;
        
        _currentPage += 1;
        _clearTableEntries();

        if (!onlineLeaderboard.IsPageLoaded(_currentPage))
        {
            StartCoroutine(onlineLeaderboard.LoadLeaderboard(_currentPage));
        }
    }

    public void PreviousPage()
    {
        if (_currentPage == 0) return;

        _currentPage -= 1;
        _clearTableEntries();
    }

    public void BackToStart()
    {
        onlineLeaderboard.ClearCache();
        startScript.CloseLeaderBoard();
    }
}
