using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class BattleManager : MonoBehaviour
{
    public static BattleManager BM;

    [NonSerialized]
    public CardParameterList CardDataBase;
    [NonSerialized]
    public BuffScriptList BuffDataBase;
    private EnemyDecks EnemyDecks;
    private BattleInfoList InfoList;
    private BattleInformation BattleInfo;
    private CharaParameterList CharaDataBase;

    private int TurnCount = 1,CharaMaxCost,EnemyMaxCost;

    public Transform HandCardParent;

    public Transform HandField, OrderField;
    public GameObject CardPrefab;
    public TextMeshProUGUI TurnText;

    public MiniDiscriptionController MDC;

    public CharaBase Chara, Enemy;

    //�J�[�h����D�ŊǗ�����
    [NonSerialized]
    public List<int> Deck = new List<int>(), HandCard = new List<int>(), ChoicedCard = new List<int>();

    [NonSerialized]
    public List<int> EnemyDeck = new List<int>(), EnemyHand = new List<int>(), EnemyChoiced = new List<int>();

    public Transform[] HandCardTrans = new Transform[5];

    public bool EnemyAttackSkip,CanNext=true;

    private bool[] MoveOrder;

    private void Awake()
    {
        BM = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //�����_���̃V�[�h�l�ݒ�
        Random.InitState(System.DateTime.Now.Millisecond);

        CardDataBase = (CardParameterList)Resources.Load("CardScriptList");
        BuffDataBase = (BuffScriptList)Resources.Load("BuffScriptList");
        EnemyDecks = (EnemyDecks)Resources.Load("EnemyDecks");
        InfoList = (BattleInfoList)Resources.Load("BattleInfoList");
        CharaDataBase = (CharaParameterList)Resources.Load("CharaParameterList");

        BattleInfo = InfoList.GetInfo(SaveLoadManager.instance.NextBattle);

        //�L�����Z�b�g
        Type type = Type.GetType(CharaDataBase.GetCharaPara(BattleInfo.MyCharaParaName).ScriptName);

        Chara = (CharaBase)Activator.CreateInstance(type);
        Chara.Para = CharaDataBase.GetCharaPara(BattleInfo.MyCharaParaName);

        Type Enemytype = Type.GetType(CharaDataBase.GetCharaPara(BattleInfo.EnemyParaName).ScriptName);

        Enemy = (CharaBase)Activator.CreateInstance(Enemytype);
        Enemy.Para = CharaDataBase.GetCharaPara(BattleInfo.EnemyParaName);

        Chara.Enemy = Enemy;
        Enemy.Enemy = Chara;

        Chara.Initialize();
        Enemy.Initialize();

        FieldManager.FM.Initialize();

        CharaDisplayManager.CDM.CharaInstantiate();

        CharaMaxCost = Chara.Para.FirstMaxCost;
        EnemyMaxCost = Chara.Para.FirstMaxCost;

        BattleStart();
    }

    //�f���G�����n�܂�����
    private void BattleStart()
    {
        Chara.DeckInitialize(SaveLoadManager.instance.Data.MyDecks);
        Enemy.DeckInitialize(EnemyDecks.GetDeck(BattleInfo.EnemyDeckNum));

        //�e�X�g�̂��߂Ɉ�U�ύX�@��Œ���
        //Deck = SaveLoadManager.instance.Data.MyDecks;
        Deck = EnemyDecks.GetDeck(1);
        EnemyDeck = EnemyDecks.GetDeck(BattleInfo.EnemyDeckNum);


        //����Ǝ����̃V���b�t������
        {
            // ���� n �̏����l�̓f�b�L�̖���
            int n = 20;

            // n��1��菬�����Ȃ�܂ŌJ��Ԃ�
            while (n > 1)
            {
                n--;

                // k�� 0 �` n+1 �̊Ԃ̃����_���Ȓl
                int k = Random.Range(0, n + 1);

                // k�Ԗڂ̃J�[�h��temp�ɑ��
                int temp = Deck[k];
                Deck[k] = Deck[n];
                Deck[n] = temp;

                // k�� 0 �` n+1 �̊Ԃ̃����_���Ȓl
                k = Random.Range(0, n + 1);

                // k�Ԗڂ̃J�[�h��temp�ɑ��
                int EnemyTemp = EnemyDeck[k];
                EnemyDeck[k] = EnemyDeck[n];
                EnemyDeck[n] = EnemyTemp;
            }
        }

        Chara.Draw(5);
        Enemy.Draw(5);

        SetOrder();
    }

    //�s�����ݒ� true���Ǝ����̓����@false�͓G�̓���
    public void SetOrder()
    {
        //true���Ǝ����̍s��
        MoveOrder = new bool[4];

        int FirstRan = Random.Range(0, 4);

        MoveOrder[FirstRan] = true;

        while (true)
        {
            int SecondRan = Random.Range(0, 4);

            if (SecondRan != FirstRan)
            {
                MoveOrder[SecondRan] = true;

                break;
            }
        }

        for (int i = 0; i < 4; i++)
        {
            if (MoveOrder[i])
            {
                OrderField.GetChild(i).GetComponent<Image>().color = new Color(1, 0, 0);
            }
            else
            {
                OrderField.GetChild(i).GetComponent<Image>().color = new Color(0, 1, 0);
            }
        }
    }

    public void MakeCards(List<int> Cards)
    {
        foreach (Transform a in HandCardTrans)
        {
            if (a!=null)
            {
                Destroy(a.gameObject);
            }
        }

        for (int i = 0; i < Cards.Count; i++)
        {
            //�J�[�h����
            CardController CreatedCard = Instantiate(CardPrefab, HandField).GetComponent<CardController>();

            CreatedCard.Initialize(Cards[i]);

            HandCardTrans[i] = CreatedCard.transform;

            HandCardTrans[i].GetComponent<RectTransform>().anchoredPosition = new Vector3(-500 + i * 250, 0, 0);
            HandCardTrans[i].GetComponent<CardController>().HandNumber = i;
        }
    }

    //�^�[���G���h����
    public void TurnEnd()
    {
        if (!CanNext) return;

        CanNext = false;

        StartCoroutine("Battle");
    }

    //�^�[���J�E���g���₵����h���[������
    public void TurnChange()
    {
        int MyDraw = Chara.Para.DrawNum, EnemyDraw = Enemy.Para.DrawNum;

        if (Chara.HandCard.Count>=5)
        {
            MyDraw = 0;
        }

        if (Enemy.HandCard.Count>=5)
        {
            EnemyDraw = 0;
        }

        Chara.Draw(MyDraw);
        Enemy.Draw(EnemyDraw);

        //�^�[���J�E���g���₷
        TurnCount++;
        TurnText.text = TurnCount.ToString();

        //����Ǝ����̃}�i���₷
        Chara.Cost+=Chara.Para.CostRecover;
        if (Chara.Cost > CharaMaxCost) Chara.Cost = CharaMaxCost;

        FieldManager.FM.CostChanger = false;
        FieldManager.FM.CostSub.OnNext(Chara.Cost);

        Enemy.Cost+=Enemy.Para.CostRecover;
        if (Enemy.Cost > EnemyMaxCost) Enemy.Cost = EnemyMaxCost;

        FieldManager.FM.CostChanger = true;
        FieldManager.FM.CostSub.OnNext(Enemy.Cost);

        SetOrder();
        CostMaxChange();
    }

    private void CostMaxChange()
    {
        bool NextSkip = false;

        if (TurnCount>CharaMaxCost && !Chara.Awaked)
        {
            CharaMaxCost++;

            if (CharaMaxCost>Chara.Para.EndMaxCost)
            {
                CharaMaxCost = Chara.Para.EndMaxCost;

                //�o������
                if (Chara.Cost == CharaMaxCost) 
                {
                    StartCoroutine(Awake(true));
                    NextSkip = true;
                }
            }
        }

        if (TurnCount > EnemyMaxCost &&!Enemy.Awaked)
        {
            EnemyMaxCost++;

            if (EnemyMaxCost > Enemy.Para.EndMaxCost)
            {
                EnemyMaxCost = Enemy.Para.EndMaxCost;

                if(Enemy.Cost==EnemyMaxCost)
                {
                    StartCoroutine(Awake(false));
                    NextSkip = true;
                }
            }
        }

        if (NextSkip)
        {
            return;
        }

        CanNext = true;
    }

    //�퓬
    private IEnumerator Battle()
    {
        //��Œ����@�f�o�b�O�p
        if (EnemyAttackSkip)
        {
            //�o�t������
            EndBuffProcess();

            yield break;
        }

        Enemy.ChoiceUseCard();

        for (int i = 0; i < 4; i++)
        {
            //�����s��
            if (MoveOrder[i])
            {
                if (Chara.Choiced.Count != 0)
                {
                    int CardNumber = Chara.HandCard[Chara.Choiced[0]];

                    Type type = Type.GetType(CardDataBase.GetCardParameter(CardNumber).ScriptName);

                    CardBase Card = (CardBase)Activator.CreateInstance(type);
                    Card.Parameter = CardDataBase.GetCardParameter(CardNumber);

                    Card.Coroutine(this, true);

                    yield return new WaitForSeconds(Card.Parameter.WaitTime);

                    Destroy(HandCardTrans[Chara.Choiced[0]].gameObject);

                    Chara.HandCard.RemoveAt(Chara.Choiced[0]);
                    Chara.Choiced.RemoveAt(0);
                }
            }
            //�G�s��
            else
            {
                if (Enemy.Choiced.Count != 0)
                {
                    int CardNumber = Enemy.HandCard[Enemy.Choiced[0]];

                    Type type = Type.GetType(CardDataBase.GetCardParameter(CardNumber).ScriptName);

                    CardBase Card = (CardBase)Activator.CreateInstance(type);
                    Card.Parameter = CardDataBase.GetCardParameter(CardNumber);

                    Card.Coroutine(this, false);

                    yield return new WaitForSeconds(Card.Parameter.WaitTime);

                    Enemy.HandCard.RemoveAt(Enemy.Choiced[0]);
                    Enemy.Choiced.RemoveAt(0);
                }
            }
        }

        //�g�p�\�R�X�g�����炷
        yield return new WaitForSeconds(1f);

        CharaDisplayManager.CDM.CharaReset();

        EndBuffProcess();
    }

    //��X��������\��
    private void EndBuffProcess()
    {
        List<BuffBase> Filtered;

        //�܂������̃o�t���s��
        Filtered = Chara.NowBuffs.FindAll(Buff => Buff.UseType == BuffBase.BuffUseType.OnTurnEnd);

        StartCoroutine(BuffProcess(Filtered, false));

        //���ɓG�̃o�t���s��
        Filtered = Enemy.NowBuffs.FindAll(Buff => Buff.UseType == BuffBase.BuffUseType.OnTurnEnd);

        StartCoroutine(BuffProcess(Filtered, true));

        //�����̃o�t�̃^�[���J�E���g�����炷
        Filtered = Chara.NowBuffs.FindAll(Buff => Buff.DecreaseType == BuffBase.BuffDecreaseType.OnTurnEnd || Buff.DecreaseType == BuffBase.BuffDecreaseType.OnUseAndEnd);

        foreach (BuffBase Buff in Filtered)
        {
            Buff.TurnCountDecrease(Chara, false);
        }

        //�G�̃o�t�̃^�[���J�E���g�����炷
        Filtered = Enemy.NowBuffs.FindAll(Buff => Buff.DecreaseType == BuffBase.BuffDecreaseType.OnTurnEnd || Buff.DecreaseType == BuffBase.BuffDecreaseType.OnUseAndEnd);

        foreach (BuffBase Buff in Filtered)
        {
            Buff.TurnCountDecrease(Enemy, true);
        }

        TurnChange();
    }

    public IEnumerator BuffProcess(List<BuffBase> Buffs, bool MeOrEnemy)
    {
        foreach (BuffBase Buff in Buffs)
        {
            Buff.BuffProcess(this, MeOrEnemy);

            yield return new WaitForSeconds(Buff.WaitTime);
        }
    }

    public void BuffTurnDecrease(List<BuffBase> Buffs, bool MeOrEnemy)
    {
        CharaBase Target = MeOrEnemy ? Enemy : Chara;

        foreach (BuffBase Buff in Buffs)
        {
            Buff.TurnCountDecrease(Target, MeOrEnemy);
        }
    }

    public IEnumerator Awake(bool MeOrEnemy)
    {
        CharaBase Target = MeOrEnemy ? Chara : Enemy;

        Target.Para = Target.Para.AwakedPara;

        Target.Awaked = true;

        yield return new WaitForSeconds(1f);

        CharaDisplayManager.CDM.CharaAwake(MeOrEnemy);

        CanNext = true;
    }

    public void GameOver()
    {
        //��Œ���
        SceneManager.LoadScene("Title");
    }

    public void Clear()
    {
        //��Œ���
        SceneManager.LoadScene("Lounge");
    }
}
