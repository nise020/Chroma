using Cysharp.Threading.Tasks;
using UnityEditor;

public class DataManager : ManagerBase
{
    public CharacterTable Character_Table = new CharacterTable();
    public StatTable Stat_Table = new StatTable();
    public ItemTable Item_Table = new ItemTable();
    //public BookTable Book_Table = new BookTable();
    public RewardTable Reward_Table = new RewardTable();
    public StageTable Stage_Table = new StageTable();
    public QuestTable Quest_Table = new QuestTable();
    //public CombinationTable Combination_Table = new CombinationTable();
    public BuffTable Buff_Table = new BuffTable();
    public SkillTable Skill_Table = new SkillTable();
    public LevelTable Level_Table = new LevelTable();
    public SoundTable Sound_Table = new SoundTable();
    public LanguageTable Language_Table = new LanguageTable();
    //public object CharacterDictionary { get; internal set; }

    public enum TableType
    {
        Item,
        Quest,
        Skill,
        Stage,
        Book,
        Buff,
        Reward,
        Combination,
        Character,
        Stat,
        Level,
        Sound,
        Language,
    }

    public void Init()
    {
#if UNITY_EDITOR
        Character_Table.Init_Csv(TableType.Character.ToString(), 1, 0);
        Stat_Table.Init_Csv(TableType.Stat.ToString(), 1, 0);
        Buff_Table.Init_Csv(TableType.Buff.ToString(), 1, 0);
        Skill_Table.Init_Csv(TableType.Skill.ToString(), 1, 0);
        Item_Table.Init_Csv(TableType.Item.ToString(), 1, 0);
        Reward_Table.Init_Csv(TableType.Reward.ToString(), 1, 0);
        Quest_Table.Init_Csv(TableType.Quest.ToString(), 1, 0);
        Stage_Table.Init_Csv(TableType.Stage.ToString(), 1, 0);
        Level_Table.Init_Csv(TableType.Level.ToString(), 1, 0);
        Sound_Table.Init_Csv(TableType.Sound.ToString(), 1, 0);
        Language_Table.Init_Csv(TableType.Language.ToString(), 1, 0);

        //Book_Table.Init_Csv(TableType.Book.ToString(), 1, 0);
        //Combination_Table.Init_Csv(TableType.Combination.ToString(), 1, 0); 
#else
        Character_Table.Init_Binary(TableType.Character.ToString());
        Stat_Table.Init_Binary(TableType.Stat.ToString());
        Buff_Table.Init_Binary(TableType.Buff.ToString());
        Skill_Table.Init_Binary(TableType.Skill.ToString());
        Item_Table.Init_Binary(TableType.Item.ToString());
        Reward_Table.Init_Binary(TableType.Reward.ToString());
        Quest_Table.Init_Binary(TableType.Quest.ToString());
        Stage_Table.Init_Binary(TableType.Stage.ToString());
        Level_Table.Init_Binary(TableType.Level.ToString());
        Sound_Table.Init_Binary(TableType.Sound.ToString());
        Language_Table.Init_Binary(TableType.Language.ToString());
#endif
    }

    public void Save()
    {
        Character_Table.Save_Binary(TableType.Character.ToString());
        Stat_Table.Save_Binary(TableType.Stat.ToString());
        Buff_Table.Save_Binary(TableType.Buff.ToString());
        Skill_Table.Save_Binary(TableType.Skill.ToString());
        Item_Table.Save_Binary(TableType.Item.ToString());
        Reward_Table.Save_Binary(TableType.Reward.ToString());
        Quest_Table.Save_Binary(TableType.Quest.ToString());
        Stage_Table.Save_Binary(TableType.Stage.ToString());
        Level_Table.Save_Binary(TableType.Level.ToString());
        Sound_Table.Save_Binary(TableType.Sound.ToString());
        Language_Table.Save_Binary(TableType.Language.ToString());

#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
    }

    public async override UniTask Initialize(string _str)
    {
        Init();
        await base.Initialize(_str);
    }
   

}
