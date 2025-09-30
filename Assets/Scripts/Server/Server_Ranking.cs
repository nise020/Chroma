using Cysharp.Threading.Tasks;
using NUnit.Framework;
using SimpleJSON;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public partial class Server : MonoBehaviour
{
    //string Http = "http://58.78.211.182:3000/";//서버 

    //Rank List Load
    string rankDataListLoadUrl = "process/rankinglist";
    //Ranf Score Save
    string rankDataSaveUrl = "process/rankingsave";
    //Rank Score Load
    string rankDataScoreLoadUrl = "process/rankingscore";

    public int beforeScore { get; private set; }
    public async Task<List<RankEntry>> LoadRankData()
    {
        List<RankEntry> serverData = await RankListLoad(Http + rankDataListLoadUrl);
        return serverData;
    }
    public async Task<List<RankEntry>> RankListLoad(string url)
    {
        UnityWebRequest www = UnityWebRequest.Get(url);

        await www.SendWebRequest();

        List<RankEntry> serverData = new();
        if (www.result == UnityWebRequest.Result.Success)
        {
            JSONNode node = JSONNode.Parse(www.downloadHandler.text);
            Debug.Log("서버 응답: " + www.downloadHandler.text);

            if (node.Count == 0)

            { return null; }

            if (node.IsArray && node.Count > 0)
            {
                for (int i = 0; i < node.Count; i++)
                {
                    string dbId = node[i];
                    int dbScore = await ScoreDataLoad(dbId);

                    RankEntry rankEntry = new RankEntry();

                    rankEntry.id = dbId;
                    rankEntry.score = dbScore;

                    serverData.Add(rankEntry);
                }
            }

        }
        return serverData;
    }
    public async UniTask SaveRankData(int _newScore)
    {
        beforeScore = await ScoreDataLoad(UserId);

        if (beforeScore < _newScore)
        {
            await PostRankDataSave(Http + rankDataSaveUrl, UserId, _newScore);
            await UniTask.CompletedTask;
        }
        else 
        {
            Debug.LogWarning($"before = {beforeScore} > now = {_newScore}");
        }
        await UniTask.CompletedTask;
    }

    public async Task<int> ScoreDataLoad(string _id)
    {
        int score = await RankScoreLoad(Http + rankDataScoreLoadUrl, _id);
        return score;
    }
   
    public async Task<int> RankScoreLoad(string url, string _id)
    {
        int score = 0;
        WWWForm form = new WWWForm();
        form.AddField("id", _id);

        UnityWebRequest www = UnityWebRequest.Post(url,form);

        await www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            JSONNode node = JSONNode.Parse(www.downloadHandler.text);

            score = node["score"];
        }
        return score;
    }
    public async UniTask PostRankDataSave(string url,string _id,int _score)
    {
        WWWForm form = new WWWForm();

        form.AddField("id", _id);
        form.AddField("score", _score);

        UnityWebRequest www = UnityWebRequest.Post(url,form);

        await www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("서버 응답: " + www.downloadHandler.text);
        }
        else
        {
            Debug.LogError("전송 실패: " + www.error);
        }
        await UniTask.CompletedTask;
    }
}
