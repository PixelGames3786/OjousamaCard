using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using UniRx;

public class FieldManager : MonoBehaviour
{
    public static FieldManager FM;

    public TextMeshProUGUI MyHP, EnemyHP;
    public Transform MyBuffParent, EnemyBuffParent;
    public RectTransform MyHPGauge, EnemyHPGauge;

    private List<Transform> MyBuffs=new List<Transform>(), EnemyBuffs=new List<Transform>();

    public GameObject BuffIconPrefab;

    //true �G�@false ����
    [NonSerialized]
    public bool HPChanger,BuffChanger;

    public Subject<int> HPSub = new Subject<int>();
    public Subject<BuffBase> BuffSub = new Subject<BuffBase>();

    public IObservable<int> OnHPChanged
    {
        get { return HPSub; }
    }

    public IObservable<BuffBase> OnBuffChanged
    {
        get { return BuffSub; }
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
                MyHP.text = HP + "<size=45>/" + BattleManager.BM.MyChara.MaxHP + "</size>";

                int Width = 500 / BattleManager.BM.MyChara.MaxHP;
                MyHPGauge.sizeDelta = new Vector2(Width*HP,20);
            }
            else
            {
                EnemyHP.text = HP + "<size=45>/" + BattleManager.BM.Enemy.MaxHP + "</size>";

                int Width = 500 / BattleManager.BM.Enemy.MaxHP;
                EnemyHPGauge.sizeDelta = new Vector2(Width * HP, 20);
            }
        });

        OnBuffChanged.Subscribe(Buff =>
        {
            AddBuff(Buff);
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

        Icon.localPosition = new Vector3(MyBuffs.Count * 50, 0, 0);

        //�A�C�R���̉摜�ݒ�
        Icon.GetComponent<Image>().sprite = Add.BuffIcon;

        MyBuffs.Add(Icon);
    }

    public void RemoveBuff(int Num)
    {
        Destroy(MyBuffs[Num].gameObject);

        for (int i=Num;i<MyBuffs.Count;i++)
        {
            MyBuffs[Num].localPosition = new Vector3(i*50,0,0);
        }

    }


}
