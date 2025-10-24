using Cysharp.Threading.Tasks;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public partial class Server : MonoBehaviour
{
    public static Server instanse;
    //string Http = "http://58.78.211.182:3000/";//서버
    string Http = "http://112.159.65.67:3000/";//서버
    //http://112.159.65.128:3000/
    //3000<--통로
    //12/22 한정으로 사용 가능
    //https<--보안됨
    //string ConnectUrl = "process/dbconnect";

    //-login-
    string loginUrl = "process/login";
    //-logincreate-
    string accountCreatUrl = "process/logincreate";
    //-namecreate
    string nameCreatUrl = "process/namecreate";


    public string UserId { get; private set; }
    public string Username { get; private set; }
    public string Userpw { get; private set; }

    //string DisConnectUrl = "process/dbdisconnect";
    //string UserSelectUrl = "process/userselect";//유저 데이터
    public LoginTab loginTab { get; set; }

    private void Awake()
    {
        if (instanse == null)
        {
            instanse = this;
            DontDestroyOnLoad(this);
        }
        else 
        {
            Destroy(this);
        }
        //on().Forget();
    }

    async UniTask on()
    {
        string url = "http://112.159.65.128:3000/process/rankinglist";
        Debug.Log($"[TEST] Requesting: {url}");

        UnityWebRequest request = UnityWebRequest.Get(url);
        request.timeout = 10; // 10초 타임아웃

        try
        {
            UnityWebRequestAsyncOperation operation = request.SendWebRequest();
            
            while (!operation.isDone)
            {
                Debug.Log($"[TEST] Progress: {operation.progress}");
                await UniTask.Delay(500);
            }

            Debug.Log($"[TEST] Done. Result: {request.result}");
            Debug.Log($"[TEST] Response Code: {request.responseCode}");
            Debug.Log($"[TEST] Error: {request.error}");
            Debug.Log($"[TEST] Downloaded Text: {request.downloadHandler.text}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[TEST] Exception: {e.Message}");
        }
    }


    //public void OnBtnConnect() 
    //{
    //    //StartCoroutine(DBPost(Http+ LoginUrl, "dev"));
    //}
    public async UniTask AccountCreat(string id, string passward)
    {
        StartCoroutine(CraetDBPost(Http + accountCreatUrl, id, passward));
        await UniTask.CompletedTask;
    }

    public async UniTask NameDBPost(string id, string name)
    {
        StartCoroutine(NameDBPost(Http + nameCreatUrl, id, name));
        await UniTask.CompletedTask;
    }
    public void MessageOn(JSONNode _json) 
    {
        string text = "";

        if (_json.HasKey("db"))
        {
            string resultMessage = _json["db"];
            switch (resultMessage)
            {
                case "failure id":
                    text = "존재하지 않는 ID입니다.";
                    Debug.LogWarning("존재하지 않는 ID입니다.");
                    break;

                case "failure password":
                    text = "비밀번호가 올바르지 않습니다.";
                    Debug.LogWarning("비밀번호가 올바르지 않습니다.");
                    break;

                case "login success":
                    text = "로그인 성공!";
                    Debug.Log("로그인 성공!");
                    break;
            }
        }
        else if (_json.HasKey("err"))
        {
            string errorMessage = _json["err"];
            switch (errorMessage)
            {
                case "not id":
                    text = "ID를 입력하지 않았습니다.";
                    Debug.LogWarning("ID를 입력하지 않았습니다.");
                    break;

                case "not id or not pw":
                    text = "ID 또는 비밀번호를 입력하지 않았습니다.";
                    Debug.LogWarning("ID 또는 비밀번호를 입력하지 않았습니다.");
                    break;
            }
        }
        else
        {
            Debug.LogWarning("알 수 없는 서버 응답입니다.");
        }
        loginTab.ErrorMessege(text);
    }

    public async UniTask LoginDBPost(string _id, string passward)
    {
        StartCoroutine(LoginDBPost(Http + loginUrl, _id, passward));
        beforeScore = await ScoreDataLoad(_id);
        await UniTask.CompletedTask;
    }
    IEnumerator LoginDBPost(string Url, string _id, string _passward)
    {
        Debug.Log($"Requesting: {Url}");
        WWWForm form = new WWWForm();
        form.AddField("id", _id);
        form.AddField("pw", _passward);

        UnityWebRequest www = UnityWebRequest.Post(Url,form);
        bool loginSuccess = false;

        yield return www.SendWebRequest();

        Debug.Log(www.downloadHandler.text);

        if (www.result == UnityWebRequest.Result.Success) 
        {
            string response = www.downloadHandler.text;
            Debug.Log("Server : " + response);

            JSONNode node = JSONNode.Parse(www.downloadHandler.text);

            if (node.HasKey("db") || node.HasKey("err")) 
            {
                MessageOn(node);
                yield break;
            }
            
            for (int i = 0; i < node.Count; i++)
            {
                string dbId = node[i]["id"];
                string dbPw = node[i]["pw"];
                string name = node[i]["name"];

                if (dbId == _id) 
                {
                    if (dbPw == _passward) 
                    {
                        UserId = _id;
                        Userpw = _passward;
                        Username = name;

                        loginSuccess = true;
                        loginTab.ErrorMessege($"{name}님이 로그인 하셨습니다!");
                        Debug.Log("로그인 성공!");
                        break;
                    }
                }
            }
        }
        else
        {
            Debug.LogError("전송 실패: " + www.error);
        }
        if (loginSuccess)
        {
            loginTab.GamePlay();
        }
    }
    IEnumerator NameDBPost(string Url, string _id, string _name)
    {
        WWWForm form = new WWWForm();
        form.AddField("id", _id);
        form.AddField("name", _name);

        UnityWebRequest www = UnityWebRequest.Post(Url, form);
        //www.timeout = 20;
        yield return www.SendWebRequest();

        Debug.Log(www.downloadHandler.text);

        if (www.result == UnityWebRequest.Result.Success)
        {
            JSONNode node = JSONNode.Parse(www.downloadHandler.text);

            string response = www.downloadHandler.text;
            Debug.Log("서버 응답: " + response);

           
        }
    }
    IEnumerator CraetDBPost(string url, string id, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("id", id);
        form.AddField("pw", password);

        UnityWebRequest www = UnityWebRequest.Post(url, form);
        www.timeout = 20;
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("서버 응답: " + www.downloadHandler.text);
        }
        else
        {
            Debug.LogError("전송 실패: " + www.error);
        }
    }
    IEnumerator DBPost(string Url, string id)
    {
        WWWForm form = new WWWForm();
        form.AddField("id", id);
       //form.AddField("pw", passward);

        UnityWebRequest www = UnityWebRequest.Post(Url, form);
        www.timeout = 20;
        yield return www.SendWebRequest();//데이터를 받으면 아래 처리

        Debug.Log(www.downloadHandler.text);
        JSONNode node = JSONNode.Parse(www.downloadHandler.text);
        for (int i = 0; i < node.Count; i++)
        {
            node["id"] = i;
            node["pw"] = i;
        }

    }

}


