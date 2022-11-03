using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using UniRx;
using DG.Tweening;

public class FieldManager : MonoBehaviour
{
    public static FieldManager FM;

    public TextMeshProUGUI MyHP, EnemyHP,MyCost,EnemyCost,MyShield,EnemyShield;
    public Transform MyBuffParent, EnemyBuffParent,OnePointParent;
    public RectTransform MyHPGauge, EnemyHPGauge;

    private List<Transform> MyBuffs=new List<Transform>(), EnemyBuffs=new List<Transform>();

    public GameObject BuffIconPrefab,OnePointPrefab;

    //true �G�@false ����
    [NonSerialized]
    public bool HPChanger,BuffChanger,CostChanger,ShieldChanger;

    public Subject<int> HPSub = new Subject<int>(),CostSub=new Subject<int>(), ShieldSub = new Subject<int>();
    public Subject<BuffBase> BuffSub = new Subject<BuffBase>();

    public IObservable<int> OnHPChanged
    {
        get { return HPSub; }
    }

    public IObservable<int> OnCostChanged
    {
        get { return CostSub; }
    }

    public IObservable<BuffBase> OnBuffChanged
    {
        get { return BuffSub; }
    }

    public IObservable<int> OnShieldChanged
    {
        get { return ShieldSub; }
    }

    private void Awake()
    {
        FM = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        OnHPChanged.Subscribe(HP => 
        {
            if (!HPChanger)
            {
                MyHP.text = HP + "<size=45>/" + BattleManager.BM.Chara.Para.MaxHP + "</size>";

                float Width = 500f / BattleManager.BM.Chara.Para.MaxHP;
                MyHPGauge.sizeDelta = new Vector2(Width*HP,20);
            }
            else
            {
                EnemyHP.text = HP + "<size=45>/" + BattleManager.BM.Enemy.Para.MaxHP + "</size>";

                float Width = 500f / BattleManager.BM.Enemy.Para.MaxHP;
                EnemyHPGauge.sizeDelta = new Vector2(Width * HP, 20);
            }
        });

        OnBuffChanged.Subscribe(Buff =>
        {
            AddBuff(Buff);
        });

        OnCostChanged.Subscribe(Cost =>
        {
            if (!CostChanger)
            {
                MyCost.text = Cost.ToString();
            }
            else
            {
                EnemyCost.text = Cost.ToString();
            }
        });

        OnShieldChanged.Subscribe(Shield=>
        {
            ShieldChange(Shield);
        });
    }

    private void AddBuff(BuffBase Add)
    {

        Transform Parent;
        List<Transform> Buffs = new List<Transform>();

        if (!BuffChanger)
        {
            Parent = MyBuffParent;
            Buffs = MyBuffs;
        }
        else
        {
            Parent = EnemyBuffParent;
            Buffs = EnemyBuffs;
        }

        Add.BuffAddNum = Buffs.Count;

        //�A�C�R���ǉ�
        Transform Icon = Instantiate(BuffIconPrefab, Parent).transform;

        Icon.localPosition = new Vector3(Buffs.Count * 50, 0, 0);

        //�A�C�R���̉摜�ݒ�
        Icon.GetComponent<Image>().sprite = Add.BuffIcon;

        Buffs.Add(Icon);
    }

    private void ShieldChange(int Shield)
    {
        TextMeshProUGUI ShieldText;

        if (!ShieldChanger)
        {
            ShieldText = MyShield;
        }
        else
        {
            ShieldText = EnemyShield;
        }


        ShieldText.text = Shield.ToString();

    }

    public void RemoveBuff(int Num,bool MeOrEnemy)
    {
        List<Transform> Buffs;

        if (!MeOrEnemy)
        {
            Buffs = MyBuffs;
        }
        else
        {
            Buffs = EnemyBuffs;
        }

        Destroy(Buffs[Num].gameObject);
        Buffs.RemoveAt(Num);

        for (int i=0;i<Buffs.Count;i++)
        {
            Buffs[i].localPosition = new Vector3(i*50,0,0);
        }
    }

    public void OnePoint(string Content,Vector2 Start,Vector2 Tag,float FadeTime)
    {
        RectTransform OnePoint = Instantiate(OnePointPrefab,OnePointParent).GetComponent<RectTransform>();

        OnePoint.GetComponent<TextMeshProUGUI>().text = Content;
        OnePoint.anchoredPosition = Start;

        OnePoint.DOAnchorPos(Tag,FadeTime);
        OnePoint.GetComponent<TextMeshProUGUI>().DOFade(0f,FadeTime).OnComplete(()=> 
        {
            Destroy(OnePoint.gameObject);
        });
    }

    public void Initialize()
    {
        int EneHP=BattleManager.BM.Enemy.HP,HP = BattleManager.BM.Chara.HP;
        int EneMaxHP=BattleManager.BM.Enemy.Para.MaxHP,MaxHP = BattleManager.BM.Chara.Para.MaxHP;
        int CharaCost= BattleManager.BM.Chara.Cost, EneCost=BattleManager.BM.Enemy.Cost;

        MyHP.text = HP + "<size=45>/" + MaxHP + "</size>";

        float Width = 500f / MaxHP;
        MyHPGauge.sizeDelta = new Vector2(Width * HP, 20);

        EnemyHP.text = EneHP + "<size=45>/" + EneMaxHP + "</size>";

        Width = 500f / EneMaxHP;
        EnemyHPGauge.sizeDelta = new Vector2(Width * EneHP, 20);

        MyCost.text = CharaCost.ToString();
        EnemyCost.text = EneCost.ToString();
    }
}
