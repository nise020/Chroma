using System;
using System.Collections.Generic;
using UnityEngine;
using static Enums;
using static Enums.SKILL_DATA;
using static Enums.SKILL_ID_TYPE;
using static IBasicSkill;

public class BasicSkillSystem 
{
    private List<IBasicSkill> basicSkills = new();
    private Player player;
    //private Monster_Base monster;
    private CharacterBase character;

    Dictionary<SKILL_ID_TYPE, IBasicSkill> playerSkillData = new Dictionary<SKILL_ID_TYPE, IBasicSkill>();
    Dictionary<SKILL_ID_TYPE, IBasicSkill> MonsterSkillData = new Dictionary<SKILL_ID_TYPE, IBasicSkill>();

    bool ischeck = false;

    //SkillData skill;
    //Dictionary<SKILL_ID_TYPE, SkillData> skillData { get; set; } = new();
    public IBasicSkill SkillCreat(SKILL_ID_TYPE _id,SkILLTYPE _type) 
    {
        IBasicSkill basicSkill = null;  

        if (_type == SkILLTYPE.Short) { basicSkill = new ShortSkill ();}
        else if (_type == SkILLTYPE.Dash)  { basicSkill = new PlayerDashSkill (); }
        else if (_type == SkILLTYPE.Area)  { basicSkill = new RangeSkill(); }
        else if (_type == SkILLTYPE.Auto)  { basicSkill = new AreaSkill (); }
        else if (_type == SkILLTYPE.Burst) { basicSkill = new AreaSkill(); }

        basicSkill.SkILL_Id = _id;
        return basicSkill;
    }
    public void Init(CharacterBase _character)
    {
        if (ischeck) return; 
        ischeck = true;
        character = _character;            

        if (_character is Player)
        {

            var data = Shared.Instance.DataManager.Skill_Table.SkillIdData;

            for (int i = 0; i < data.Count; i++) 
            {
                if (data[i] < (int)SKILL_ID_TYPE.Dash_Monster_nomal1)
                {
                    SKILL_ID_TYPE id = (SKILL_ID_TYPE)data[i];
                    SkillData skillData = Shared.Instance.DataManager.Skill_Table.Get((int)id);
                    IBasicSkill skill = SkillCreat(id, skillData.type);
                    skill.Init(_character);
                    playerSkillData.Add(id, skill);
                }
                else 
                {
                    continue;
                }
            }
            #region Hard

            //foreach (var dd in data) 
            //{
            //    int id = dd.Key;
            //    SkillData skill = dd.Value;

            //    if (id < (int)SKILL_ID_TYPE.Dash_Monster_nomal1)
            //    {
            //        playerSkillData.Add((SKILL_ID_TYPE)id, new ShortSkill { SkILL_Id = (SKILL_ID_TYPE)id});
            //    }
            //    else 
            //    {

            //    }
            //}


            //front
            //playerSkillData.Add(Shot_Type_1, new ShortSkill { SkILL_Id = SKILL_ID_TYPE.Shot_Type_1,});
            //playerSkillData.Add(Shot_Type_2, new ShortSkill { SkILL_Id = SKILL_ID_TYPE.Shot_Type_2 ,});
            //playerSkillData.Add(Shot_Type_3, new ShortSkill { SkILL_Id = SKILL_ID_TYPE.Shot_Type_3 ,});
            ////range
            //playerSkillData.Add(Shot_Type_4, new RangeSkill  { SkILL_Id = SKILL_ID_TYPE.Shot_Type_4 ,});

            ////dash,front
            //playerSkillData.Add(Dash_Type_1, new PlayerDashSkill  { SkILL_Id = SKILL_ID_TYPE.Dash_Type_1,});
            //playerSkillData.Add(Dash_Type_2, new PlayerDashSkill  { SkILL_Id = SKILL_ID_TYPE.Dash_Type_2, });
            //playerSkillData.Add(Dash_Type_3, new PlayerDashSkill  { SkILL_Id = SKILL_ID_TYPE.Dash_Type_3,});
            //playerSkillData.Add(Dash_Type_4, new PlayerDashSkill  { SkILL_Id = SKILL_ID_TYPE.Dash_Type_4,});

            ////range
            //playerSkillData.Add(Area_Type_1, new RangeSkill { SkILL_Id = SKILL_ID_TYPE.Area_Type_1,});
            //playerSkillData.Add(Area_Type_2, new RangeSkill { SkILL_Id = SKILL_ID_TYPE.Area_Type_2,});
            //playerSkillData.Add(Area_Type_3, new RangeSkill { SkILL_Id = SKILL_ID_TYPE.Area_Type_3, });
            //playerSkillData.Add(Area_Type_4, new RangeSkill { SkILL_Id = SKILL_ID_TYPE.Area_Type_4, });

            ////auto
            //playerSkillData.Add(Auto_Type_1, new AreaSkill { SkILL_Id = SKILL_ID_TYPE.Auto_Type_1, });
            //playerSkillData.Add(Auto_Type_2, new AreaSkill { SkILL_Id = SKILL_ID_TYPE.Auto_Type_2, });
            //playerSkillData.Add(Auto_Type_3, new AreaSkill { SkILL_Id = SKILL_ID_TYPE.Auto_Type_3, });
            //playerSkillData.Add(Auto_Type_4, new AreaSkill { SkILL_Id = SKILL_ID_TYPE.Auto_Type_4, });

            ////burst
            //playerSkillData.Add(Burst_Type_1, new AreaSkill  { SkILL_Id = SKILL_ID_TYPE.Burst_Type_1,});
            //playerSkillData.Add(Burst_Type_2, new AreaSkill  { SkILL_Id = SKILL_ID_TYPE.Burst_Type_2, });
            //playerSkillData.Add(Burst_Type_3, new AreaSkill  { SkILL_Id = SKILL_ID_TYPE.Burst_Type_3 , });
            //playerSkillData.Add(Burst_Type_4, new AreaSkill  { SkILL_Id = SKILL_ID_TYPE.Burst_Type_4 , });

            //foreach (var skill in playerSkillData.Values)
            //{
            //    skill.Init(_character);
            //}
            #endregion
        }
        else 
        {
            //character = (Monster_Base)_character;

            MonsterSkillData.Add(Grab_Nomal, new GrabSkill { SkILL_Id = SKILL_ID_TYPE.Grab_Nomal });

            MonsterSkillData.Add(Dash_Monster_nomal1, new DashSkill { SkILL_Id = SKILL_ID_TYPE.Dash_Monster_nomal1 });
            MonsterSkillData.Add(Dash_Monster_nomal2, new DashSkill { SkILL_Id = SKILL_ID_TYPE.Dash_Monster_nomal2 });

            MonsterSkillData.Add(Shot_Monster_nomal1, new ShortSkill { SkILL_Id = SKILL_ID_TYPE.Shot_Monster_nomal1 });
            MonsterSkillData.Add(Shot_Monster_Boss1, new ShortSkill { SkILL_Id = SKILL_ID_TYPE.Shot_Monster_Boss1,});
            MonsterSkillData.Add(Shot_Monster_Boss2, new ShortSkill { SkILL_Id = SKILL_ID_TYPE.Shot_Monster_Boss2, });

            MonsterSkillData.Add(Jump_Nomal, new JumpSkill { SkILL_Id = SKILL_ID_TYPE.Jump_Nomal });
            MonsterSkillData.Add(Jump_Boss, new JumpSkill { SkILL_Id = SKILL_ID_TYPE.Jump_Boss });

            //MonsterSkillData.Add(Trow, new TrowSkill { SkILL_Id = SKILL_ID_TYPE.Trow });

            foreach (var skill in MonsterSkillData.Values)
            {
                skill.Init(_character);
            }
        }
    }
    public bool SkillState(IBasicSkill _skill) 
    {
        return _skill.State();
    }
    public bool SkillActive(SKILL_ID_TYPE _skillType) 
    {
        IBasicSkill basicSkill = playerSkillData[_skillType];

        if (!SkillState(basicSkill))
        {
            basicSkill.OnTrigger(null);
            return true;
        }
        else 
        {
            Debug.LogError($"{_skillType} is CoolTime");
            return false;
        }

    }
    public bool CooltimeCheck(SKILL_ID_TYPE _skillType) 
    {
        if (character is Player)
        {
            IBasicSkill basicSkill = playerSkillData[_skillType];

            if (!SkillState(basicSkill))
            {
                return true;
            }
            else
            {
                //Debug.LogError($"{basicSkill} is CoolTime");
                return false;
            }
        }
        else 
        {
            IBasicSkill basicSkill = MonsterSkillData[_skillType];

            if (!SkillState(basicSkill))
            {
                //basicSkill.Timer();
                return true;
            }
            else
            {
                //Debug.LogError($"character = {character},{basicSkill} is CoolTime");
                return false;
            }
        }
        
    }
    public bool SkillActive(SKILL_ID_TYPE _skillType,CharacterBase character)
    {
        IBasicSkill basicSkill = MonsterSkillData[_skillType];

        if (!SkillState(basicSkill))
        {
            basicSkill.OnTrigger(character);
            return true;
        }
        else
        {
            Debug.LogError($"{_skillType} is CoolTime");
            return false;
        }

    }
    //public void SkillOff(SKILL_ID_TYPE _skillType) 
    //{
    //    if (character is Player)
    //    {
    //        IBasicSkill basicSkill = playerSkillData[_skillType];

    //        basicSkill.TriggerOut();
    //    }
    //    else 
    //    {
    //        IBasicSkill basicSkill = MonsterSkillData[_skillType];

    //        basicSkill.TriggerOut();
    //    }
       
    //}

    public void MonsterSkillActiv(MonsterBase monster) 
    {

    }


}
