using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using static System.Linq.Enumerable;

public class LeaderboardUI : MonoBehaviour
{
    private const float EntryHeight = 40f;
    
    public OnlineLeaderboard onlineLeaderboard;
    public Transform leaderboardEntryPrefab;
    public GameObject leaderboardContainer;
    public StartScript startScript;
    public TextMeshProUGUI currentPageText;
    
    private bool _isTableFilled = false;
    private int _currentPage = 0;
    
    void Start()
    {
        StartCoroutine(onlineLeaderboard.LoadTabCount());
        StartCoroutine(onlineLeaderboard.LoadLeaderboard(0));
    }
    
    void Update()
    {
        if (!_isTableFilled && onlineLeaderboard.IsPageLoaded(_currentPage))
        {
            _addEntriesToScene(onlineLeaderboard.LeaderboardPages[_currentPage]);
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
        if (_currentPage >= onlineLeaderboard.PageCount-1) return;
        
        _currentPage += 1;
        _clearTableEntries();

        if (!onlineLeaderboard.IsPageLoaded(_currentPage))
        {
            StartCoroutine(onlineLeaderboard.LoadLeaderboard(_currentPage));
        }
        
        _updateCurrentPage();
    }

    public void PreviousPage()
    {
        if (_currentPage == 0) return;

        _currentPage -= 1;
        _clearTableEntries();
        
        _updateCurrentPage();
    }

    public void BackToStart()
    {
        onlineLeaderboard.ClearCache();
        startScript.CloseLeaderBoard();
    }

    private void _updateCurrentPage()
    {
        currentPageText.text = (_currentPage+1).ToString();
    }
}
