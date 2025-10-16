using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Collections;
using UnityEngine;
using UnityEngine.AI;
using static Enums;
using static Enums.ANIMATION_PATAMETERS_TYPE;
using static Enums.CHARACTER_DATA;
using static Enums.CHARACTER_STATUS;
//using Unity.Android.Gradle.Manifest;

public partial class Monster_Base : Character_Base
{
    

    public override CONFIG_OBJECT_TYPE ObjectType => CONFIG_OBJECT_TYPE.Monster;
    //public Dictionary<CHARACTER_DATA, object> MonsterStatus { get; set; } = new();
    protected Player Player { get; set; }

    protected Transform PlayerTrans { get; set; }

    protected IEnumerator AttackCorutine { get; set; }

    


    //public void StateInit() => infoData = Infos[CHARACTER_DATA.State] as Dictionary<MONSTER_STATE, object>;
    protected bool AttackDelay { get; set; }//DelayType Only

    //protected bool ColorState { get; set; } = false;

    //public Dictionary<CHARACTER_DATA, object> Infos { get; set; } = new();
    protected Spawn Spawn { get; set; }
    protected Vector3 MovePosition { get; set; }
    protected Vector3 StartPoint { get; set; } 
    protected List<Vector3> MovePositionList { get; set; }
    protected int SlotCount { get; set; }
    //protected float StopDistance { get; set; } = 0.2f;

    protected float MoveDelrayTimer { get; set; } 
    protected float MoveDelrayTime { get; set; }
    protected float StopDelrayTimer { get; set; } 
    protected float StopDelrayTime { get; set; } 
    protected float AttackDelayTimer { get; set; } 
    protected float AttackDelrayTime { get; set; } 
    //protected int AttackCount { get; set; }
    //protected int AttackMaxCount { get; set; } 

    //protected CancellationTokenSource AttackCTS { get; set; } = null;
    //protected CancellationTokenSource MoveCTS { get; set; } = null;
    //protected CancellationTokenSource DashCTS { get; set; } = null;
    //protected CancellationTokenSource AniEventCTS { get; set; } = null;
    protected Dictionary<Item, GameObject> DropItemData { get; set; }

    protected List<Item> ITEMLists { get; set; }

    public MONSTER_ATTACK_TYPE Attack_Type = MONSTER_ATTACK_TYPE.None;

    protected bool isFalling;
    protected MonsterWeapon MainWeapon { get; set; }
    protected List<GameObject> WeaponObjectList { get; set; }
    protected Dictionary<GameObject, bool> WeaponCheckDatas { get; set; }
    protected Transform Creatab { get; set; }

    public ParticleSystem deathEffect { get; set; }
    public ParticleSystem SpownEffect { get; set; }

    protected UIStateBar StateBar { get; set; }



    protected Vector3 WeightPos;

    [SerializeField] protected List<SKILL_ID_TYPE> SkillType;
    protected NavMeshAgent Agent { get; set; }
    protected override void Awake()
    {
        base.Awake();
        WeaponObj = GetWeaponObj();

        WeaponSoundPlayer = WeaponObj.AddComponent<AudioSource>();
        WeaponSoundPlayer.outputAudioMixerGroup = Shared.Instance.SoundManager.GetMixser(OptionHandler.SOUND.Effect);
        WeaponSoundPlayer.spatialBlend = 1.0f;

        CharacterSoundPlayer = gameObject.AddComponent<AudioSource>();
        CharacterSoundPlayer.outputAudioMixerGroup = Shared.Instance.SoundManager.GetMixser(OptionHandler.SOUND.Effect);
        CharacterSoundPlayer.spatialBlend = 1.0f;
    }
    protected override void Claer()
    {
        base.Claer();

        AttackDelay = false;

        MovePositionList = new();
        MovePosition = new();
        StartPoint = new();

        WeightPos = new Vector3();
        isFalling = false;
        DropItemData = new();
        ITEMLists = new();
        WeaponObjectList = new List<GameObject>();
        WeaponCheckDatas = new Dictionary<GameObject, bool>();

        SlotCount = 0;
        //StopDistance = 0.2f;

        MoveDelrayTimer = 0.0f;
        MoveDelrayTime = 2.0f;
        StopDelrayTimer = 0.0f;
        StopDelrayTime = 2.0f;
        AttackDelayTimer = 0.0f;
        AttackDelrayTime = 2.0f;
        //AttackCount  = 0;
        //AttackMaxCount = 3;
    }
    public void GetData() 
    {
        Claer();

        var Infos = Shared.Instance.DataManager.Character_Table.Get((int)IdType);
        if (Infos != null)
        {
            InfoData.Add(Id, Infos.Id);
            InfoData.Add(CHARACTER_DATA.Type, Infos.Type);
            InfoData.Add(StateId, Infos.StateId);
            InfoData.Add(BookId, Infos.BookId);
            InfoData.Add(FOVLength, Infos.FOVLength);
            InfoData.Add(AttackLength, Infos.AttackLength);
            InfoData.Add(Exp, Infos.Exp);
            InfoData.Add(WalkSoundId, Infos.WalkSoundId);
            InfoData.Add(AttackSoundId, Infos.AttackSoundId);

            AudioClip audioClip1 = Shared.Instance.SoundManager.ClipGet((int)InfoData[WalkSoundId]);
            SoundDatas.Add((int)InfoData[WalkSoundId], audioClip1);

            AudioClip audioClip2 = Shared.Instance.SoundManager.ClipGet((int)InfoData[AttackSoundId]);
            SoundDatas.Add((int)InfoData[AttackSoundId], audioClip2);


            PathData.Add(Name, Infos.Name);
            PathData.Add(Dec, Infos.Dec);
            PathData.Add(Icon, Infos.Icon);
            PathData.Add(Prefab, Infos.Prefab);

            var state = Shared.Instance.DataManager.Stat_Table.Get(Infos.StateId);
            if (state != null)
            {
                StatusData.Add(Hp, state.Hp);
                StatusData.Add(MaxHp, state.Hp);
                StatusData.Add(Def, state.Def);
                StatusData.Add(Speed, state.Speed);
                StatusData.Add(Atk, state.Atk);
            }

        }
    }
    protected override void Start()
    {
        IsPaused = true;
        base.Start();

        //basicSkillSystem.Init(this);
        skillDataAdd();


        BuffSystem = new BuffSystem();
        BuffSystem.Init(this);

        StateCheckData.Add(ANIMATION_PATAMETERS_TYPE.Attack.ToString(), false);

        //StateCheckData.Add(ANIMATION_PATAMETERS_TYPE.Death.ToString(), false);
        //StateCheckData.Add(ANIMATION_PATAMETERS_TYPE.Walk.ToString(), false);
        //StateCheckData.Add(ANIMATION_PATAMETERS_TYPE.KnockBack.ToString(), false);

        FindWeapon();
        Agent = GetComponent<NavMeshAgent>();
        Agent.speed = (int)StatusData[Speed]/2;
        //Agent.stoppingDistance = InfoData[AttackLength];
        Agent.updateRotation = false;
        Agent.updatePosition = false;

        StartCoroutine(SpownEffectOn(this, () => 
        {
            IsPaused = false;
        }));
    }
    public override void DamageImageOn(float _damage)
    {
        StateBar.AttackDamageEvent?.Invoke((int)_damage);
    }
    public void SetAvoidanceDirection(Vector3 _pos) 
    {
        WeightPos = _pos;
    }
    class Skilllist
    {
        public Dictionary<int, string> skillTypeData = new Dictionary<int, string>();
    }

    protected virtual void Update()
    {
        if (IsJumping) 
        {
            Velocity.y = 0f;
            return;
        }
        GravityOperations();
        //if (rg) 
        //    rg.linearVelocity = Velocity;
    }


    protected override void GravityOperations()
    {
        Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;
        IsGrounded = Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hitInfo1, GroundDistance, GroundMask);
        IsDeathed = Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hitInfo2, GroundDistance, DeathMask);

        if (IsDeathed)
        {
            if (Velocity.y < 0f) Velocity.y = -1f;

            HpUpdate(0.0f);
            IsPaused = true;
        }
        else if (IsGrounded)
        {

            if (Velocity.y < 0f) Velocity.y = -1f;
        }
        else
        {
            // 중력 적용
            Velocity.y += Gravity * Time.deltaTime;
        }

        rg.linearVelocity = Velocity;
    }
    protected void skillDataAdd()
    {
        for (int i = 0; i < SkillType.Count; i++)
        {
            if (SkillType[i] != SKILL_ID_TYPE.None)
            {
                SkillKeyWardData.Add(i, SkillType[i]);
            }

        }
    }
    public override GameObject GetWeaponObj()
    {
        if (WeaponObj == null) 
        {
            return gameObject;
        }
        return WeaponObj;
    }
    protected void FindWeapon() 
    {
        MonsterWeapon[] Weapons = GetComponentsInChildren<MonsterWeapon>();
        if (Weapons.Length == 0) return;

        if (Weapons.Length == 1)
        {
            MainWeapon = Weapons[0];
        }

    }

    public void StateUpdate(bool paused) 
    {
        if (StatusData[CHARACTER_STATUS.Hp] <= 0) return;
        else 
        {
            IsPaused = paused;
        }

        var anim = CharacterAnimator;
        if (anim == null)
        {
            CharacterAnimator = anim = GetComponentInChildren<Animator>();
            if (anim == null) return;
        }

        anim.speed = paused ? 0f : 1f;

        if (CharacterSoundPlayer.isPlaying)
        {
            CharacterSoundPlayer.Stop();
        }
    }
    
   
    public List<GameObject> GetWeaponsKey() 
    {
        return WeaponObjectList;
    }
    public Dictionary<GameObject, bool> GetWeaponsData()
    {
        return WeaponCheckDatas;
    }
    public void WeaponsStateUpdate(GameObject monsterObj, bool isActive)
    {
        if (WeaponCheckDatas.ContainsKey(monsterObj))
        {
            WeaponCheckDatas[monsterObj] = isActive;
        }
    }
}
