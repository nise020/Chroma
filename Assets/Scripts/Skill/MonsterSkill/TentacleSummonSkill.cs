using UnityEngine;
using static Enums;

public class TentacleSummonSkill : IBasicSkill
{
    private MonsterBase monster;
   // GameObject WeaponObj;
    //public SkillData skillData { get; set; }
    //public override SKILL_ID_TYPE SkILL_Id => SKILL_ID_TYPE.TentacleSummon;

    public override void Init(CharacterBase _user) 
    {
        monster = (MonsterBase)_user;
        //WeaponObj = monster.GetWeaponObj();
        //skillData = Shared.Instance.DataManager.Skill_Table.Get((int)SkILL_Id);
        base.Init(null);
    }
    public override void OnUpdate() { }
    public override void TriggerOut() { }
    public override void OnTrigger(CharacterBase _defender) 
    {
        WeaponObj.SetActive(true);
        Debug.Log("ÃË¼ö ¼ÒÈ¯");
    }


}
