using XLua;


namespace SG
{



    public enum AudioType
    {
        AT_ATTACK_PLAYERWEAPON = 0,
        AT_ATTACK_PLAYERACTOR = 1,
        AT_BEHIT_EFFECT = 2,
        AT_BEHIT_PLAYER = 3,
        AT_BEHIT_MONSTER = 4,
        AT_DIE_PLAYER = 5,
        AT_ACTION_MONSTER = 6,
        AT_DIE_MONSTER = 7,
        AT_DIE_POSUI = 8,
    }
    public enum MainlandOverType
    {
        MOT_DEFAULT = 0,
        MOT_TIME = 1,
        MOT_CREATURE = 2,
    }
    public enum DropCondition
    {
        DROPCOND_DEATH = 0,
        DROPCOND_BEHIT = 1,
    }


    public enum DropType
    {
        DROPTYPE_MONEY = 0,
        DROPTYPE_ITEM = 1,
        DROPTYPE_MONSTER = 2,
    }

[Hotfix]
public class  gameMacros
{
    public const int RES_SKILL_NAME_LEN = 32; // ����������
    public const int RES_SKILL_INTRODUCE_LEN = 128; // ������������
    public const int RES_PATH_LEN = 256; // ·������
    public const int MAX_SPECIAL_SKILL_NUM = 5; // ���⼼������ֵ
    public const int RES_CREATE_NAME_LEN = 64; // ����������
    public const int MAX_SKILL_COMBO_KEY_NUM = 5; // �������б����󰴼���

   
}

}

