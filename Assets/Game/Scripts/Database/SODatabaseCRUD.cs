using GABackend;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static GABackend.MKRLeaderboard;

[CreateAssetMenu(fileName = "Database CRUD", menuName = "Scriptable Objects / database CRUD")]
public class SODatabaseCRUD : ScriptableObject
{
    public List<LeaderBoardItem> players { get; private set; } = new List<LeaderBoardItem>();
    public string errorMessage { get; private set; } = string.Empty;
    public LeaderBoardItem playerData { get; private set; }

    public async Task<Tuple<List<LeaderBoardItem>, string>> GetTodayDailyLeaderboardAsync()
    {
        var tcs = new TaskCompletionSource<Tuple<List<LeaderBoardItem>, string>>();

        MKRLeaderboard.instance.GetTodayDailyLeaderboard(
            (leaderboardData) =>
            {
                players = new List<LeaderBoardItem>(leaderboardData.leaderboard);
                playerData = leaderboardData.player;
                tcs.SetResult(Tuple.Create(players, (string)null));
            },
            (error) =>
            {
                errorMessage = error;
                tcs.SetResult(Tuple.Create<List<LeaderBoardItem>, string>(null, error));
            }
        );
        return await tcs.Task;
    }

    public async Task<Tuple<List<LeaderBoardItem>, string>> GetTodayWeeklyLeaderboardAsync()
    {
        var tcs = new TaskCompletionSource<Tuple<List<LeaderBoardItem>, string>>();

        MKRLeaderboard.instance.GetTodayWeeklyLeaderboard(
            (leaderboardData) =>
            {
                players = new List<LeaderBoardItem>(leaderboardData.leaderboard);
                playerData = leaderboardData.player;
                tcs.SetResult(Tuple.Create(players, (string)null));
            },
            (error) =>
            {
                errorMessage = error;
                tcs.SetResult(Tuple.Create<List<LeaderBoardItem>, string>(null, error));
            }
        );
        return await tcs.Task;
    }

    public async Task<Tuple<List<LeaderBoardItem>, string>> GetTodayMonthlyLeaderboardAsync()
    {
        var tcs = new TaskCompletionSource<Tuple<List<LeaderBoardItem>, string>>();

        MKRLeaderboard.instance.GetTodayMonthlyLeaderboard(
            (leaderboardData) =>
            {
                players = new List<LeaderBoardItem>(leaderboardData.leaderboard);
                playerData = leaderboardData.player;
                tcs.SetResult(Tuple.Create(players, (string)null));
            },
            (error) =>
            {
                errorMessage = error;
                tcs.SetResult(Tuple.Create<List<LeaderBoardItem>, string>(null, error));
            }
        );
        return await tcs.Task;
    }
}