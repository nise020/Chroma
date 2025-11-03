using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Enums;

public partial class PlayerStateBar : UIStateBar
{
    [Header("ExpImg")]
    [SerializeField] private Image expImage;

    [Header("Skill")]
    [SerializeField] private Image Skill_1Image;
    [SerializeField] private TMP_Text Skill_1Text;

    [SerializeField] private Image Skill_2Image;
    [SerializeField] private TMP_Text Skill_2Text;

    [SerializeField] private Image Skill_3Image;
    [SerializeField] private TMP_Text Skill_3Text;

    [SerializeField] private Image Skill_4Image;
    [SerializeField] private Image Skill_4_Back_Image;
    [SerializeField] private TMP_Text BurstText;

    [Header("HpText")]
    [SerializeField] private Text maxHpText;
    [SerializeField] private Text hpText;

    [SerializeField] private Text maxExpText;
    [SerializeField] private Text expText;

    //[SerializeField] public BuffHUD buffHud;
    //private const float maxDuration = 2;
    [Header("ExpEvent")]
    public Action<float> ExpUpdateEvent { get; set; }
    public Action<float> BurstExpUpdateEvent { get; set; }
    public event Action<int> LevelUpEvent;
    Coroutine burstCharging { get; set; }
    //StatTab stateControllUi;

    [Header("ToolTip")]
    [SerializeField] private GameObject GuidTab;
    [SerializeField] List<GameObject> ToolTipPanel;

    [Header("Skip")]
    [SerializeField] Button NextStage;
    [SerializeField] Button bOSSoN;

    [Header("StatValue")]
    [ReadOnly] public int currentLevel;
    [ReadOnly] public float currentExp;
    [ReadOnly] public float expToLevelUp;
    [ReadOnly] public float expIncreaseRate;
    [ReadOnly] public float currentBurst;
    protected float ExpeffectTime;

    //private IEnumerator ExpBarEvent { get; set; } = null;
    private float targetFill;

    public BuffHUD buffHUD;

    int Level;
    int LimitLevel;

    Dictionary<SKILL_ID_TYPE, SkillData> SkillDatas { get; set; }
    // Dictionary<SkillKeyType, SkillData> SkillKeyDatas = new Dictionary<SkillKeyType, SkillData>();
    bool isBurst { get; set; }

    protected override void Clear()
    {
        base.Clear();
        Level = 1;
        isBurst = false;
        SkillDatas = new();

        currentLevel = 1;
        currentExp = 0;
        expToLevelUp = 0;
        expIncreaseRate = 1.5f;
        currentBurst = 0;
        ExpeffectTime = 1.0f;

        burstCharging = null;
    }
    protected override void Awake()
    {
        base.Awake();

        var table = Shared.Instance.DataManager.Skill_Table.SkillTableData;

        foreach (var value in table)
        {
            SKILL_ID_TYPE key = (SKILL_ID_TYPE)value.Key;
            SkillData data = value.Value;
            SkillDatas.Add(key, data);
        }

        buffHUD = GetComponentInChildren<BuffHUD>();
    }

    protected override void Start()
    {
        base.Start();
        ExpUpdateEvent += SetExp;
        BurstExpUpdateEvent += SetBurst;
        if (Skill_4Image.fillAmount != 0) 
        {
            Skill_4Image.fillAmount = 0.0f;
        }

        if (Shared.Instance.isReplay == false) 
        {
            Shared.Instance.isReplay = true;
            GuidTabOn();
        }

        NextStage.onClick.AddListener(GameShard.Instance.StageManager.SkipStage);
        bOSSoN.onClick.AddListener(GameShard.Instance.StageManager.BossTimerSkip);
    }


    public override void InitializeCharacter(CharacterBase character)
    {
        base.InitializeCharacter(character);
    }
    public override void InitializeImage()
    {
        base .InitializeImage();
        if (expImage.fillAmount != 0)
        {
            expImage.fillAmount = 0.0f;
        }
        
    }
    public override void SetHP(float _MaxHP, float _CurHP)
    {
        maxHpText.text = _MaxHP.ToString();
        hpText.text = _CurHP.ToString();

         base.SetHP(_MaxHP, _CurHP);
    }
    public void GuidTabOn() 
    {
        GuidTab.SetActive(true);
        GameShard.Instance.GameUiManager.UiActiveSatckData.Push(GuidTab);
        GameShard.Instance.InputManager.isUIOpen = true;
        GameShard.Instance.MonsterManager.UiStateUpdate(true);
    }

    public void GuidTabOFF()
    {
        GuidTab.SetActive(false);
        GameShard.Instance.GameUiManager.UiActiveSatckData.Pop();
        GameShard.Instance.InputManager.isUIOpen = false;
        GameShard.Instance.InputManager.isMouse = false;
        GameShard.Instance.MonsterManager.UiStateUpdate(false);
    }

}
