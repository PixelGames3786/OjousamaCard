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

    //true 敵　false 自分
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

        //アイコン追加
        Transform Icon = Instantiate(BuffIconPrefab, Parent).transform;

        Icon.localPosition = new Vector3(Buffs.Count * 50, 0, 0);

        //アイコンの画像設定
        Icon.GetComponent<Image>().sprite = Add.BuffIcon;

        Buffs.Add(Icon);
    }

    public void RemoveBuff(List<int> Num,bool MeOrEnemy)
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

        foreach (int num in Num)
        {
            Destroy(Buffs[num].gameObject);

        }

        for (int i=0;i<Buffs.Count;i++)
        {
            print(i*50);

            Buffs[i].localPosition = new Vector3(i*50,0,0);
        }

    }


}
