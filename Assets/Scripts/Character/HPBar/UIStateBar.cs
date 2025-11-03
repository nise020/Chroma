using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public partial class UIStateBar : MonoBehaviour
{
    protected CharacterBase Character { get; set; }
    [SerializeField] private Image hpImage;
    [SerializeField] private Image hpLateImage;
    
    protected float HpeffectTime { get; private set; }

    public Action<int> AttackDamageEvent { get; set; }
    private Coroutine HPBarEvent { get; set; }
    protected Camera MainCamera { get; set; }

    protected virtual void Awake()
    {
        Clear();
    }
    protected virtual void Clear() 
    {
        HpeffectTime = 2.0f;
        HPBarEvent = null;
    }
    protected virtual void Start()
    {
        InitializeImage();
    }
    protected virtual void Initialize()
    {
        MainCamera = Camera.main;
    }
    public virtual void InitializeCharacter(CharacterBase character)
    {
        character.HpBarChanged += SetHP;
        Character = character;
        //Debug.Log($"[HpBar] Character 연결: {Character}");

        if (Character == null)
        {
            Debug.LogError("Character가 null입니다!");
            return;
        }

        Debug.Log("[HpBar] SetHP 이벤트 연결 완료");

    }
    public virtual void InitializeImage()
    {
        if (hpImage.fillAmount != 1) 
        {
            hpImage.fillAmount = 1.0f;
        }
        if( hpLateImage.fillAmount != 1) 
        {
            hpLateImage.fillAmount = 1.0f;
        }
    }

    public virtual void SetHP(float _MaxHP, float _CurHP)
    {
        if (!gameObject.activeSelf) return;
        hpImage.fillAmount = _CurHP / _MaxHP;

        if (_CurHP / _MaxHP > hpLateImage.fillAmount)
        {
            hpLateImage.fillAmount = hpImage.fillAmount;
        }
        else
        {
            if (HPBarEvent != null) StopCoroutine(HPBarEvent);
            HPBarEvent = StartCoroutine(setHpEvent());
        }

        HPBarEvent = StartCoroutine(setHpEvent());
        Debug.Log($"{gameObject}.SetHP Event Start");
    }

    private IEnumerator setHpEvent()
    {
        while (hpLateImage.fillAmount > hpImage.fillAmount)
        {
            hpLateImage.fillAmount -= Time.deltaTime / HpeffectTime;
            if (hpLateImage.fillAmount < hpImage.fillAmount) 
            {
                HPBarEvent = null;
                break;
            }
            yield return null;
        }
        //hpLateImage.fillAmount = hpImage.fillAmount;
        
    }


}