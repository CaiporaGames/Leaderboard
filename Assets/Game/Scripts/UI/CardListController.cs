using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static GABackend.MKRLeaderboard;

public class CardListController : MonoBehaviour
{
    [Header("Card List Settings")]
    [SerializeField] SODatabaseCRUD databaseCRUD;
    [SerializeField] GameObject cardPrefab;

    private void OnEnable()
    {
        ButtonController.buttonClicked += AddCardToLeaderboard;
    }

    private void Start()
    {
        _ = AddCardToLeaderboard("DailyButton");
    }

    //TODO: Here we could use a factory design pattern if the amount of buttons get bigger.
    public async Task AddCardToLeaderboard(string buttonName)
    {
        Tuple<List<LeaderBoardItem>, string> result = null;

        try
        {
            switch (buttonName)
            {
                case "DailyButton":
                    result = await databaseCRUD.GetTodayDailyLeaderboardAsync();
                    break;
                case "WeeklyButton":
                    result = await databaseCRUD.GetTodayWeeklyLeaderboardAsync();
                    break;
                case "MonthlyButton":
                    result = await databaseCRUD.GetTodayMonthlyLeaderboardAsync();
                    break;
                default:
                    Debug.LogError($"Invalid button name: {buttonName}");
                    return;
            }

            if(result.Item1.Count > 0) SetCardFields(result.Item1);
            else ClearContent();
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            return;
        }
    }

    private void SetCardFields(List<LeaderBoardItem> players)
    {
        ClearContent();
        foreach (LeaderBoardItem player in players)
        {
            GameObject cardObject = Instantiate(cardPrefab, transform.position, Quaternion.identity);
            CardController cardController = cardObject.transform.GetChild(0).GetComponent<CardController>();
            cardController.InitializeCardFields(player.position, player.displayName, player.score, player.timestamp);
            cardObject.transform.SetParent(transform, false);
        }
    }

    //TODO: We should use a object pooling here but this is just a test i will let as is.
    void ClearContent()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
    private void OnDestroy()
    {
        ButtonController.buttonClicked -= AddCardToLeaderboard;
    }
}
