using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using SimpleJSON;

public class SkCard : MonoBehaviour
{
    private Image _icon;
    private Text _name;
    private Text _magic;

    private int _skId = 0;
    private bool _targetSelf = false; //目标技能是不是本身(前提)
    private bool _useSk = false; //是否决定使用该技能
    private JSONNode _skCfg;
    private bool _enable = true;

    private ARPlayer _playerCtrl;

    void Awake()
    {
        Transform skIcon = transform.FindChild("skIcon");
        _icon = skIcon.GetComponent<Image>();

        Transform magicLab = transform.FindChild("magicLab");
        _magic = magicLab.GetComponent<Text>();

        Transform skNameLab = transform.FindChild("skNameLab");
        _name = skNameLab.GetComponent<Text>();
    }


    void Start()
    {

        JSONNode t = ARPlayer._tSkill;
        _skCfg = t["" + _skId];
        _icon.overrideSprite = Resources.Load("SkillIcon/" + _skCfg["icon"], typeof(Sprite)) as Sprite;
        _magic.text = _skCfg["skillcost"];
        _name.text = _skCfg["skillname"];
    }

    void Update()
    {
        
    }

    public void setSkId(int id)
    {
        _skId = id;
    }

    public void setPlayer(ARPlayer ctrl)
    {
        _playerCtrl = ctrl;
    }

    public bool Enable
    {
        get { return _enable; }
        set { _enable = value; }
    }

    public void onPointerDown()
    {
        if (!_enable || _playerCtrl.TargetSk >= 0)
        {
            return;
        }
        else
        {
            _targetSelf = true;
            _playerCtrl.TargetSk = _skId;
        }
        transform.localScale = new Vector3(0.9f, 0.9f, 1);
    }

    public void onBeginDrag()
    {
        if (!_enable || !_targetSelf) return;
    }

    public void onPointerExit()
    {
        if (!_enable || !_targetSelf) return;

        if(_skCfg["skilltype"].AsInt == 1)
        {
            _playerCtrl.showAtkDirect(true);
        }
        else
        {
            _playerCtrl.showAtkPos(true);
        }
        _useSk = true;
    }

    public void onPointerUp()
    {
        if (!_enable || !_targetSelf) return;

        if (_skCfg["skilltype"].AsInt == 1)
        {
            _playerCtrl.showAtkDirect(false);
        }
        else
        {
            _playerCtrl.showAtkPos(false);
        }

        transform.localScale = Vector3.one;
    }

    public void onPointerClick()
    {
        if (!_enable || !_targetSelf) return;

        _useSk = false;
        _targetSelf = false;
        _playerCtrl.clearTargetSk();
    }

    public void onEndDrag()
    {
        if (!_enable || !_targetSelf) return;

        if (_useSk)
        {
            if(_playerCtrl.useSkill())
            {
                Destroy(transform.gameObject);
            }
        }
        _targetSelf = false;
        _playerCtrl.clearTargetSk();
    }
}
