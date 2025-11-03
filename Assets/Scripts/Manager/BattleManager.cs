using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using static Enums;

public class BattleManager : MonoBehaviour
{
    [Header("Player")]
    //[SerializeField] Player PLAYER;
    //public GameObject playerObj;
    //PlayerStateBar StateBar;

    [Header("StartPoint")]
    [SerializeField] GameObject PlayerStartObject;

    [Header("HpBar")]
    [SerializeField] UIStateBar HPBAR;
    [SerializeField] GameObject hpBarObj;

    [Header("Creat")]
    [SerializeField] Transform Creatab;

    [Header("Ui")]
    public GameObject CanvasTab;


    [Header("Damage")]
    Shader damageShader;

    Material damageMaterial;

    [Header("Effect")]

    [SerializeField] List<GameObject> effectObj = new List<GameObject>();

    private readonly HashSet<int> _killReported = new HashSet<int>();

    public async UniTask InitAsync()
    {
        GameShard.Instance.BattleManager = this;
        damageShader = Shared.Instance.ResourcesManager.damageShader;
        damageMaterial = Shared.Instance.ResourcesManager.damageMaterial;
        await UniTask.Yield(); // 필요시 프레임 분산
    }


    public void DamageCheck(CharacterBase _attacker, CharacterBase _defender,
        SkillData _skill)
    {
        float defenserHp = _defender.StatusTypeLoad(CHARACTER_STATUS.Hp);

        if (defenserHp <= 0){ return; }

        float defenserDfs = _defender.StatusTypeLoad(CHARACTER_STATUS.Def) / 100;
        float attakerPower = _attacker.StatusTypeLoad(CHARACTER_STATUS.Atk);

        float skillPower = Random.Range(
            ((float)_skill.value / 100), 
            ((float)_skill.valueMax / 100));

        float damage = attakerPower * skillPower * (1f - defenserDfs);

        defenserHp -= damage;

        //KnockBack
        if (defenserHp > 0 && damage > 0)
        {
            _defender.KnockBackOn(damage);
        }

        _defender.HpUpdate(defenserHp);

        //Damage Value Ui
        _defender.DamageImageOn(damage);

        //Debuff
        if (_skill.buffId >= 2000) {_defender.BuffSystem.ApplyBuff((int)_skill.buffId);}
        
        //Ai
        if (defenserHp <= 0) { _attacker.AttackEvent?.Invoke(true); }
        else { _attacker.AttackEvent?.Invoke(false); }

        //Material
        if (_defender.DamageEventCheck())
        {
            _defender.DamageEventUpdate(true);
            DamageColor(_defender);
        }

        if (defenserHp <= 0 && _defender is MonsterBase)
        {
            Player player = _attacker as Player;
            MonsterBase monster = _defender as MonsterBase;

            int exp = _defender.ExpLoad();
            GameShard.Instance.GameUiManager.PlayerStateBar.ExpUpdateEvent?.Invoke(exp);
            GameShard.Instance.GameUiManager.PlayerStateBar.BurstExpUpdateEvent?.Invoke(1);//- = test

            int monsterId = (int)monster.IdType;
            GameShard.Instance.QuestManager.OnObjective(Quest_Type.Kill, monsterId, 1);

            int score = Shared.Instance.DataManager.Character_Table.GetScore(monsterId);
            if (score > 0)
            {
                Debug.LogWarning($"[Score] {monsterId} Score : {score}");
                GameShard.Instance.GameManager.PlusGameScore(score);
            } 
        }

    }

    /// <summary>
    /// Hit Color Change
    /// </summary>
    /// <param name="_defender"></param>
    private void DamageColor(CharacterBase _defender)
    {
        SkinnedMeshRenderer[] renderers = _defender.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
        //originalShaders.Clear();

        List<(SkinnedMeshRenderer renderer, Material[] originalMats)> backup = new();

        foreach (SkinnedMeshRenderer renderer in renderers)
        {
            Material[] originalMats = renderer.sharedMaterials;
            Material[] damageMet = new Material[originalMats.Length];

            for (int i = 0; i < originalMats.Length; i++)
            {
                Material damageInstance = new Material(damageMaterial);
                if (originalMats[i].HasProperty("_MainTex"))
                    damageInstance.SetTexture("_MainTex", originalMats[i].GetTexture("_MainTex"));

                damageMet[i] = damageInstance;

            }
            renderer.materials = damageMet;
            backup.Add((renderer, originalMats));

        }

        StartCoroutine(MaterialBackUp(_defender, backup, 0.1f));
    }
    IEnumerator MaterialBackUp(CharacterBase _defender, List<(SkinnedMeshRenderer renderer,
        Material[] originalMats)> backup, float delay)
    {
        yield return new WaitForSeconds(delay);

        foreach (var (renderer, originalMats) in backup)
        {
            renderer.sharedMaterials = originalMats;
            _defender.DamageEventUpdate(false);
        }
    }

}
