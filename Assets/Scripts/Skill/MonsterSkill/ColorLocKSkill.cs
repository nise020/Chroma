using UnityEngine;
using static Enums;

public class ColorLocKSkill : IBasicSkill
{
    private MonsterBase monster;
    //public SkillData skillData { get; set; }
    //public override SKILL_ID_TYPE SkILL_Id => SKILL_ID_TYPE.ColorLock;
    //bool IsActive { get; set; } = false;
    public override void Init(CharacterBase _user)
    {
        monster = (MonsterBase)_user;
    }
    public override void OnUpdate() { }
    public override void TriggerOut() { }
    public override void OnTrigger(CharacterBase _defender) 
    {
        if (_defender is Player) 
        {
            float timer = 5f;
            Player player = (Player)_defender;
            //player.colorSlotUI.ColorLock((int)timer);
        }
    }

    //public override bool State()
    //{
    //    return IsActive;
    //}
}
