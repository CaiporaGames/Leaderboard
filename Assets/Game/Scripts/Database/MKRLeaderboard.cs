using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace GABackend
{
    public class MKRLeaderboard : MonoBehaviour
    {
        public static MKRLeaderboard instance;

        private int leaderboardListSize = 10;

        // API end point url
        private string apiEndPoint = "https://oxhpkseqb0.execute-api.eu-central-1.amazonaws.com/beta/";

        // API key
        private string apiKey = "aj0ukIgQo33UV4wwAnZug7Hsvdijh6Eq7yxUihWe";

        // API Resouces
        private string apiResourceUrl(string _resource) { return apiEndPoint + _resource; }

        private long TimestampBackend { get { return (long)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds; } }

        private string gameid = "mkr";
        private string playerid = "78babe69d201479f91bc4693db6c81d2";
        private string sessionToken = "WbgSxH6ndfvTynrBGKzV4CJqkPxVhls7B16TK7qW9v0=";

        public enum LeaderboardStatId
        {
            day,
            week,
            month
        }

        public enum LeaderboardType
        {
            xp,
            wins,
            laptime
        }

        private void Awake()
        {
            // If there is already one instance, destroy this one
            if (instance != null)
            {
                DestroyImmediate(this.gameObject);
            }
            else
            {
                // Only if this is te first run
                instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
        }

        public void GetTodayDailyLeaderboard(Action<LeaderBoardData> _callback, Action<string> _errorCallback)
        {
            GetLeaderboard(LeaderboardStatId.day, LeaderboardType.xp, getDailyId(DateTime.UtcNow), leaderboardListSize, _callback, _errorCallback);
        }

        public void GetTodayWeeklyLeaderboard(Action<LeaderBoardData> _callback, Action<string> _errorCallback)
        {
            GetLeaderboard(LeaderboardStatId.week, LeaderboardType.xp, getWeeklyId(DateTime.UtcNow), leaderboardListSize, _callback, _errorCallback);
        }

        public void GetTodayMonthlyLeaderboard(Action<LeaderBoardData> _callback, Action<string> _errorCallback)
        {
            GetLeaderboard(LeaderboardStatId.month, LeaderboardType.xp, getMonthlyId(DateTime.UtcNow), leaderboardListSize, _callback, _errorCallback);
        }

        private string getDailyId(DateTime _now)
        {
            return getDateId(_now);
        }
        private string getWeeklyId(DateTime _now)
        {
            return getDateId(getMonday(_now));
        }

        private string getMonthlyId(DateTime _now)
        {
            return getDateId(new DateTime(_now.Year, _now.Month, 1));
        }

        private DateTime getMonday(DateTime _now)
        {
            var _date = new DateTime(_now.Year, _now.Month, _now.Day);
            // Get the Monday of that week as our week count is based on monday
            return _date.AddDays(-(((int)_date.DayOfWeek + 6) % 7));
        }

        private string getDateId(DateTime _now)
        {
            DateTime _date = new DateTime(_now.Year, _now.Month, _now.Day);
            return Clean(_date.Day) + Clean(_date.Month) + _date.Year;
        }
        private string Clean(int _value)
        {
            return ((_value <= 9) ? "0" : "") + _value.ToString();
        }

        // Note: _date is the leaderboard date, either today, or the first day of the month or the first monday of the week. Format: "DDMMYYYY" like in "25032015" or "04122023"
        private void GetLeaderboard(LeaderboardStatId _statid, LeaderboardType _type, string _date, int _qtd, Action<LeaderBoardData> _OnSuccess, Action<string> _OnError)
        {
            LeaderBoardInfo request = new LeaderBoardInfo(_statid, _type, _date, _qtd, -1, -1);
            Post("leaderboard", request,
                (response) =>
                {
                    LeaderBoardData session = response.GetPayLoad<LeaderBoardData>();
                    _OnSuccess(session);
                },
            _OnError);
        }


        // Low level backend communication
        private void Post(string _resource, object _payload, Action<DataResponse> _OnSuccess, Action<string> _OnError)
        {
            StartCoroutine(DoPost(_resource, _payload, (response) =>
            {
                if (response.succeed)
                {
                    if (response.HasPayload)
                    {
                        _OnSuccess(response);
                    }
                    else
                    {
                        _OnError("No payload received");
                    }
                }
                else
                {
                    _OnError(response.ErrorMessage);
                }
            }, _OnError));
        }

        IEnumerator DoPost(string _resource, object _payload, Action<DataResponse> _OnSuccess, Action<string> _OnError)
        {
            // Set final communication data
            DataAPI dataRequest = new DataRequest(gameid, playerid, TimestampBackend, sessionToken, _payload);
            if (_payload != null)
                Debug.Log(_resource + " request: " + JsonUtility.ToJson(_payload));
            else
                Debug.Log(_resource + " no payload");

            bool success = false;
            int tries = 0;
            while (!success && (tries < 1))
            {
                //Debug.Log("Network _payload: " + _payload);
                UnityWebRequest webRequest = PortRequest(_resource, dataRequest.AsString());
                yield return webRequest.SendWebRequest();

                if (webRequest.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError(_resource + " Network error: " + webRequest.error);
                    _OnError(webRequest.error);
                }
                else
                {
                    string result = webRequest.downloadHandler.text;
                    Debug.Log(_resource + " response: " + result);
                    try
                    {
                        if (result.Contains("Error"))
                        {
                            _OnError(result);
                        }
                        else
                        {
                            DataResponse dataResponse = JsonUtility.FromJson<DataResponse>(result);
                            success = true;
                            _OnSuccess(dataResponse);
                        }
                    }
                    catch (Exception err)
                    {
                        _OnError(err.Message);
                    }
                }

                if (!success)
                {
                    tries++;
                    yield return new WaitForSeconds(1.0f);
                }
            }

            if (!success)
                _OnError("Timeout Error");
        }

        private UnityWebRequest PortRequest(string _resource, string _payload)
        {
            string uri = apiResourceUrl(_resource);
            byte[] postData = System.Text.Encoding.UTF8.GetBytes(_payload);

            UnityWebRequest webRequest = new UnityWebRequest(uri);
            webRequest.method = UnityWebRequest.kHttpVerbPOST;
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("Accept", "application/json");
            webRequest.SetRequestHeader("x-api-key", apiKey);
            webRequest.timeout = 10;
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.uploadHandler = new UploadHandlerRaw(postData);
            webRequest.uploadHandler.contentType = "application/json";
            return webRequest;
        }

        // The base class to handle a response from the backend
        public class DataResponse : DataAPI
        {
            public bool update;
            public bool succeed;
        }

        public class DataRequest : DataAPI
        {
            public DataRequest(string gameid, string playerid, long timestamp, string sessionToken, object payload)
            {
                this.gameid = gameid;
                this.playerid = playerid;
                this.timestamp = timestamp;
                this.sessionToken = sessionToken;
                if (payload != null)
                    SetPayLoad(payload);
                else
                    this.payload = "";
            }
        }

        public class DataAPI
        {
            public string gameid; // The current game id
            public string playerid; // The logged user id
            public long timestamp; // The communication timestamp
            public string sessionToken; // Session token
            public string payload; // The profileImagePayload

            public bool HasPayload { get { return (payload != ""); } }
            public void SetPayLoad(object _payload) { payload = JsonUtility.ToJson(_payload); }
            public T GetPayLoad<T>() { return JsonUtility.FromJson<T>(payload); }
            public string AsString() { return JsonUtility.ToJson(this); }

            public string ErrorMessage
            {
                get
                {
                    string errorMessage = "No error message";
                    try
                    {
                        if (HasPayload)
                        {
                            ErrorResponse errorResponse = JsonUtility.FromJson<ErrorResponse>(payload);
                            errorMessage = errorResponse.error;
                        }
                    }
                    catch
                    {
                        errorMessage = "No valid error message";
                    }
                    return errorMessage;
                }
            }
        }
        public class ErrorResponse
        {
            public string error;
        }

        [Serializable]
        public class LeaderBoardData
        {
            public LeaderBoardItem player;
            public LeaderBoardItem[] leaderboard;

            public LeaderBoardData(LeaderBoardData _data)
            {
                player = new LeaderBoardItem(_data.player);
                leaderboard = new LeaderBoardItem[_data.leaderboard.Length];
                for (int i = 0; i < leaderboard.Length; i++)
                    leaderboard[i] = new LeaderBoardItem(_data.leaderboard[i]);
            }
        }

        [Serializable]
        public class LeaderBoardItem
        {
            public string playerid;
            public string displayName;
            public int position;
            public int score;
            public float timestamp;

            public LeaderBoardItem()
            {
                score = -1;
            }

            public LeaderBoardItem(LeaderBoardItem _item)
            {
                playerid = _item.playerid;
                displayName = _item.displayName;
                position = _item.position;
                score = _item.score;
                timestamp = _item.timestamp;
            }
        }


        public class LeaderBoardInfo
        {
            public string statid;
            public string type;
            public string date;
            public int quantity;
            public int nextScore;
            public int nextPosition;

            // Register or login
            public LeaderBoardInfo(LeaderboardStatId _statid, LeaderboardType _type, string _date, int _qtd, int _nextScore, int _nextPosition)
            {
                statid = _statid.ToString().ToLower();
                type = _type.ToString().ToLower();
                date = _date;
                quantity = _qtd;
                nextPosition = _nextPosition;
                nextScore = _nextScore;
            }

            // Copy class
            public LeaderBoardInfo(LeaderBoardInfo _info)
            {
                statid = _info.statid;
                type = _info.type;
                date = _info.date;
                quantity = _info.quantity;
                nextPosition = _info.nextPosition;
                nextScore = _info.nextScore;
            }
        }
    }
}