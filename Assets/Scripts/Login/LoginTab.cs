using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;

public class LoginTab : MonoBehaviour
{
    [Header("DefoltView")]
    [SerializeField] GameObject DefoltObj;
    [SerializeField] Button CreatBut;
    [SerializeField] Button LoginViewBut;
    [SerializeField] Button QuitBut;
    [SerializeField] GameObject ErrorUi;
    Coroutine errorEvent;

    [Header("LoginUi")]
    [SerializeField] GameObject LoginObj;
    [SerializeField] Text nameText;
    [SerializeField] GameObject nameObj;

    [SerializeField] Text IdText;
    [SerializeField] Text PasswordText;

    [SerializeField] InputField idField;
    [SerializeField] InputField passwordField;

    [SerializeField] Button LoginBut;
    [SerializeField] Button ReturnBut;
    [SerializeField] GameObject ReturnObj;
    [SerializeField] GameObject LoginButObj;

    [Header("CreatUi")]
    [SerializeField] Button creatBut;
    [SerializeField] GameObject creatButObj;
    //[SerializeField] Toggle AutoLogin;
    //IEnumerator Laoding;
    string IDKey;
    string UserPassKey;
    string NameKey;
    bool isLogin = false;
    private void Awake()
    {
        LoginObj.SetActive(false);
        DefoltObj.SetActive(true);

        creatBut.onClick.AddListener(CreatPrecces);
        LoginBut.onClick.AddListener(LoginPrecces);

        LoginViewBut.onClick.AddListener(() => 
        {
            DefoltObj.SetActive(false);
            LoginObj.SetActive(true);
            LoginButObj.SetActive(true);
            creatButObj.SetActive(false);
            nameObj.SetActive(false);

            ReturnObj.gameObject.SetActive(true);
        });

        CreatBut.onClick.AddListener(() => 
        {
            DefoltObj.SetActive(false);
            LoginObj.SetActive(true);
            creatButObj.SetActive(true);
            LoginButObj.SetActive(false);
            nameObj.SetActive(true);

            ReturnObj.gameObject.SetActive(true);
        });

        ReturnBut.onClick.AddListener(() => 
        {
            DefoltObj.SetActive(true);
            LoginObj.SetActive(false);
            ReturnObj.SetActive(false);
        });

        QuitBut.onClick.AddListener(() =>
        {
            Quit();
        });

        


        ErrorUi.SetActive(false);
    }

    public void Quit()
    {
        Debug.Log("Quit");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
Application.Quit();
#endif
    }

    private void Start()
    {
        Server.instanse.loginTab = this;
    }
    public void AutoLoginCheck() 
    {
        //int value = PlayingDataLoad("AutoLogin");

        //if (value == 1)
        //{
        //    string id = Shared.Instance.PlyerLoginDataLoad("UserID");
        //    string pass = Shared.Instance.PlyerLoginDataLoad("UserPassward");

        //    if ((id != "" && id.Length > 0) &&
        //       (pass != "" && pass.Length > 0))
        //    {
        //        Shared.Instance.isPlay = true;
        //    }
        //    else 
        //    {
        //        if (errorEvent != null) StopCoroutine(errorEvent);
        //        errorEvent = StartCoroutine(TextEvent(ErrorUi));
        //        ErrorUi.SetActive(true);
        //        return;
        //    }
        //}
        //else
        //{
        //    return;
        //}
    }


    public void ToggleEvent() 
    {
        isLogin = !isLogin;
        if (isLogin)
        {
            PlayingHistorygSave("AutoLogin",1).Forget();
        }
        else 
        {
            PlayingHistorygSave("AutoLogin",0).Forget();
        }
    }
    public int PlayingDataLoad(string _Key)
    {
        return PlayerPrefs.GetInt(_Key);
    }
    public void ErrorMessege(string _text)
    {
        if (errorEvent != null) StopCoroutine(errorEvent);
        errorEvent = StartCoroutine(TextEvent(ErrorUi,_text));

        //ErrorUi.SetActive(true);
    }
    public IEnumerator TextEvent(GameObject _ui, string _text)
    {
        TMP_Text text = _ui.GetComponent<TMP_Text>();
        text.text = _text;

        Color color = text.color;
        color.a = 1f;
        text.color = color;

        _ui.SetActive(true);
        float displayTime = 2f;
        yield return new WaitForSeconds(displayTime);

        float fadeDuration = 1f; 
        float startTime = Time.time;

        while (Time.time < startTime + fadeDuration)
        {
            float elapsed = Time.time - startTime;
            color.a = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);

            text.color = color; 
            yield return null;
        }

        color.a = 0f;
        text.color = color;

        _ui.SetActive(false);
        errorEvent = null;
    }
    public async UniTask PlayingHistorygSave(string _Key, int _Value)
    {
        PlayerPrefs.SetInt(_Key, _Value);
        PlayerPrefs.Save();
        await UniTask.CompletedTask;
    }
    public void CreatPrecces()
    {
        if ((IdText.text == "" || IdText.text.Length <= 0) &&
            (PasswordText.text == "" || PasswordText.text.Length <= 0)&&
            (nameText.text == "" || nameText.text.Length <= 0)) return;

        CreatPassword().Forget();
    }
    public void LoginPrecces()
    {
        //if ((IdText.text == "" || IdText.text.Length <= 0) &&
        //    (PasswordText.text == "" || PasswordText.text.Length <= 0)) return;

        LoginOn().Forget();
    }
    //public async UniTask CreatPassword()//계정 생성
    //{
    //    LoginBut.interactable = false;
    //    Debug.Log("1초 후 로비 화면으로 이동합니다");

    //    IDKey = IdText.text;
    //    UserPassKey = PasswordText.text;

    //    await Shared.Instance.PlyerLoginDataSave("UserID", IDKey);
    //    Debug.Log("저장 직후 = " + PlayerPrefs.GetString("UserID"));

    //    await Shared.Instance.PlyerLoginDataSave("UserPassward", UserPassKey);
    //    Debug.Log("저장 직후 = " + PlayerPrefs.GetString("UserPassward"));

    //    //Shared.SceneMgr.SaveFile(UserPassKey);
    //    //ActKey = UserPassKey;

    //    //yield return new WaitForSeconds(5);
    //    await UniTask.Delay(TimeSpan.FromSeconds(1.0f));
    //    //IDSave(IDKey, UserPassKey);
    //    Debug.Log($"UserPassKey={UserPassKey},IDKey={IDKey}");

    //    LoginBut.interactable = true;

    //    Shared.Instance.isPlay = true;

    //    await UniTask.CompletedTask;
    //}
    public async UniTask LoginOn() 
    {
        //IDKey = IdText.text;
        //UserPassKey = PasswordText.text;
        var id = idField.text.Trim();
        var password = passwordField.text.Trim();

        await Server.instanse.LoginDBPost(id, password);
        //await Server.instanse.LoginDBPost(IDKey, UserPassKey);
        //Shared.Instance.isPlay = true;

        await UniTask.CompletedTask;
    }
    public async UniTask CreatPassword()//계정 생성
    {
        LoginBut.interactable = false;
        Debug.Log("1초 후 로비 화면으로 이동합니다");

        IDKey = IdText.text;
        UserPassKey = PasswordText.text;
        NameKey = nameText.text;

        await Server.instanse.AccountCreat(IDKey,UserPassKey);
        await Server.instanse.NameDBPost(IDKey, NameKey);
        

        LoginBut.interactable = true;

        GamePlay().Forget();

        await UniTask.CompletedTask;
    }
    public async UniTask GamePlay() 
    {
        await UniTask.Delay(TimeSpan.FromSeconds(3.0f));
        Shared.Instance.isPlay = true;
    }
}
