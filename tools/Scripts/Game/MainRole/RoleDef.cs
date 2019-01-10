using XLua;



namespace SG
{


/************************************************************************
* �ļ�������ʵ������ID����
* �ļ�˵�������ļ�ͨ�������Զ�����,��Ҫ�ֶ��޸�
************************************************************************/

//tolua_begin
//������Щ�����Ե�ƫ��
public enum enPropEntity
{
    PROP_ENTITY_ID,	// uint   	ʵ����id
    PROP_ENTITY_POSX,	// int    	λ��X
    PROP_ENTITY_POSY,	// int    	λ��Y
    PROP_ENTITY_MODELID,	// int    	ʵ����ģ��ID
    PROP_ENTITY_WEAPON,	// int    	��������
    PROP_ENTITY_ICON,	// int    	ͷ��ID
    PROP_ENTITY_DIR,	// int    	ʵ���ĳ���
    PROP_ENTITY_MAX,	// int    	
};

public enum enPropCreature
{
    PROP_CREATURE_LEVEL =enPropEntity. PROP_ENTITY_MAX,	// int    	�ȼ�
    PROP_CREATURE_HP,	// int    	����
    PROP_CREATURE_MP,	// int    	ħ��
    PROP_CREATURE_MAXHP,	// uint   	��������
    PROP_CREATURE_MAXMP,	// uint   	����ħ��
    PROP_CREATURE_ATTACK,	// int    	��ͨ����
    PROP_CREATURE_SKILL_ATTACK,	// int    	���ܹ���
    PROP_CREATURE_DEFENCE,	// int    	����
    PROP_CREATURE_HITRATE,	// int    	�����ʣ����ֱȣ�
    PROP_CREATURE_VITALRATE,	// int    	�����ʣ����ֱȣ�
    PROP_CREATURE_DODGERATE,	// int    	�����ʣ����ֱȣ�
    PROP_CREATURE_DUCTILITY,	// int    	����
    PROP_CREATURE_DODGEVITAL_____DEPRECATED_____,	// int    	�ⱬ
    PROP_CREATURE_MOVE_SPEED,	// int    	�ƶ��ٶ�
    PROP_CREATURE_ATTACK_SPEED,	// int    	�����ٶ�
    PROP_CREATURE_HP_RENEW,	// int    	�����ָ�
    PROP_CREATURE_MP_RENEW,	// int    	ħ���ָ�
    PROP_CREATURE_STATE,	// int    	������״̬�������������ȵ�
    PROP_CREATURE_MAX,	// int    	
};

// ����
public enum enPropMonster
{
    PROP_MONSTER_BORNPOINT = enPropCreature.PROP_CREATURE_MAX,	// int    	�����ĳ�����
    PROP_MONSTER_MAX,	// int    	
};

// �߼�����
public enum enPropAdvanceAnimal
{
    PROP_ADVANIMAL_STRENGTH = enPropCreature.PROP_CREATURE_MAX,	// int    	����
    PROP_ADVANIMAL_INTELLECT,	// int    	����
    PROP_ADVANIMAL_AGILE,	// int    	����
    PROP_ADVANIMAL_CONSTITUTION,	// int    	����
    PROP_ADVANIMAL_SPIRIT,	// int    	����
    PROP_ADVANIMAL_MAX,	// int    	
};
// ��������
public enum enPropActor
{
    PROP_ACTOR_SEX = enPropAdvanceAnimal.PROP_ADVANIMAL_MAX,	// int    	�Ա�
    PROP_ACTOR_VOCATION,	// int    	ְҵ
    PROP_ACTOR_EXP,	// uint   	����
    PROP_ACTOR_GONGCHENGPOINT,	// int    	����ս����
    PROP_ACTOR_JUNTUANBI,	// int    	���ű�
    PROP_ACTOR_LEVELUPTIME,	// int    	����ʱ��
    PROP_ACTOR_YUANBAO,	// int    	Ԫ��
    PROP_ACTOR_GOLD,	// int    	�ƽ�
    PROP_ACTOR_HUOLI,	// int    	����
    PROP_ACTOR_XIYUBI,	// int    	������
    PROP_ACTOR_GONGCHENGBI,	// int    	���Ǳ�
    PROP_ACTOR_PHYSICAL,	// int    	����
    PROP_ACTOR_GUILDID,	// uint   	����ID
    PROP_ACTOR_RECHARGEYUANBAO,	// uint   	��ǰ��ֵԪ��
    PROP_ACTOR_VIP_LEVEL,	// int    	VIP�ȼ�
    PROP_ACTOR_FIRSTRECHARGECOUNT,	// int    	�����׳���ʯ��Ŀ
    PROP_ACTOR_TOTALRECHARGECOUNT,	// int    	�ܳ�ֵԪ��
    PROP_ACTOR_ACTIVITY,	// int    	���ҵĻ�Ծ��
    PROP_ACTOR_POWER,	// int    	����ս����
    PROP_ACTOR_FLAG,	// int    	���ұ���
    PROP_ACTOR_FIRSTLOGINTIME,	// int    	��һ�ε�½��Ϸʱ��
    PROP_ACTOR_ATTACKPOWER,	// int    	����ս����
    PROP_CAUGHT_EXPIRE_TIME,	// int    	��ץΪ��²�ĵ���ʱ��;�����ڶᱦ��ս����ʱ��
    PROP_ACTOR_SKYFAME,	// int    	��������
    PROP_ACTOR_SKYTOPRANK,	// int    	������������
    PROP_ACTOR_MEDAL,	// int    	��ѫ
    PROP_ACTOR_CAMPID,	// int    	��ӪID
    PROP_ACTOR_CAMPCDENDTIME,	// int    	��Ӫ�ı�CD����ʱ��
    PROP_ACTOR_XUEZHANBI,	// int    	Ѫս��
    PROP_ACTOR_TOPPOWER,	// int    	��ʷ����ս����
    PROP_ACTOR_RESERVED1,	// int    	Ԥ������1;�����ڼ�¼�ϴλָ�������ʱ��
    PROP_ACTOR_RESERVED2,	// int    	Ԥ������2;�����ڼ�¼�ϴλָ�������ʱ��
    PROP_ACTOR_RESERVED3,	// int    	Ԥ������3
    PROP_ACTOR_RESERVED4,	// int    	Ԥ������4
    PROP_ACTOR_RESERVED5,	// int    	Ԥ������5
    PROP_ACTOR_HEROPOINTS,	// int    	Ӣ�۵�
    PROP_ACTOR_HOMERESUPDATETIME,	// int    	��԰��Դ����ʱ��
    PROP_ACTOR_CONSUMECOUNT,	// int    	�ۼ�����Ԫ��
    PROP_RES_UPLIMIT,	// int    	��Դ����
    PROP_ACTOR_CAMP_LEVEL,	// int    	��Ӫ��ְ�ȼ�
    PROP_ACTOR_ENVOY,	// int    	��Ӫ��ʹ����
    PROP_ACTOR_LINE,	// int    	���Ƿ���
    PROP_ACTOR_WARPOS,	// int    	����սְλ
    PROP_2_MAXHP,	// int    	����
    PROP_2_MAXHP_PERCENT,	// float  	�����ٷֱ�
    PROP_2_PATK,	// int    	�﹥
    PROP_2_PATK_PERCENT,	// float  	�﹥�ٷֱ�
    PROP_2_PDEF,	// int    	����
    PROP_2_PDEF_PERCENT,	// float  	�����ٷֱ�
    PROP_2_MATK,	// int    	����
    PROP_2_MATK_PERCENT,	// float  	�����ٷֱ�
    PROP_2_MDEF,	// int    	����
    PROP_2_MDEF_PERCENT,	// float  	�����ٷֱ�
    PROP_2_SPEED,	// int    	�ƶ��ٶ�ֵ
    PROP_2_SPEED_PERCENT,	// float  	�ƶ��ٶȰٷֱ�
    PROP_2_CRIRATE,	// float  	����
    PROP_2_FREECRIDMG_RATE,	// float  	����
    PROP_2_CRIDMG_RATE,	// float  	���˼ӳ�
    PROP_2_DMG_RATE_PERCENT,	// float  	�˺��ӳ�
    PROP_2_DMGRESIST_RATE_PERCENT,	// float  	�˺�����
    PROP_2_ATKSPEED,	// int    	�����ٶ�ֵ
    PROP_2_ATKSPEED_PERCENT,	// float  	�����ٶȰٷֱ�
    PROP_2_SUCK_PERCENT,	// float  	��Ѫ
    PROP_2_PBOUNCE_PERCENT,	// float  	��������
    PROP_2_MBOUNCE_PERCENT,	// float  	ħ������
    PROP_2_HEAL,	// float  	����Ч��
    PROP_2_HIT,	// float  	����
    PROP_2_EVADE,	// float  	����
    PROP_2_EVADERECOVER,	// float  	���ܻ�Ѫ
    PROP_2_IGNORE,	// float  	����
    PROP_ACTOR_MAX,	// int    	
};

//tolua_end

// ��������
public enum PROPERTY_TYPE
{
    PROP_TYPE_UNKNOW,
    PROP_TYPE_INT,
    PROP_TYPE_UINT,
    PROP_TYPE_INT64,
    PROP_TYPE_UINT64,
};











}



