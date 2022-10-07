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

    private int TurnCount = 1;

    public Transform HandCardParent;

    public Transform HandField, OrderField;
    public GameObject CardPrefab;
    public TextMeshProUGUI TurnText, MyCostText, EnemyCostText, MyHPText, EnemyHPText;

    public BattleStatus MyChara = new BattleStatus(),
                        Enemy = new BattleStatus();

    public CharaBase Chara, Enemykari;

    //�J�[�h����D�ŊǗ�����
    [NonSerialized]
    public List<int> Deck = new List<int>(), HandCard = new List<int>(), ChoicedCard = new List<int>();

    [NonSerialized]
    public List<int> EnemyDeck = new List<int>(), EnemyHand = new List<int>(), EnemyChoiced = new List<int>();

    private Transform[] HandCardTrans = new Transform[5];

    public bool EnemyAttackSkip;

    public bool[] MoveOrder;

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

        Enemykari = (CharaBase)Activator.CreateInstance(Enemytype);
        Enemykari.Para = CharaDataBase.GetCharaPara(BattleInfo.EnemyParaName);

        Chara.Enemy = Enemykari;
        Enemykari.Enemy = Chara;

        Chara.Initialize();
        Enemykari.Initialize();

        MyChara.Name = "MyChara";
        Enemy.Name = "Enemy";

        BattleStart();
    }

    //�f���G�����n�܂�����
    private void BattleStart()
    {
        Chara.DeckInitialize(SaveLoadManager.instance.Data.MyDecks);
        Enemykari.DeckInitialize(EnemyDecks.GetDeck(BattleInfo.EnemyDeckNum));

        //�e�X�g�̂��߂Ɉ�U�ύX�@��Œ���
        //Deck = SaveLoadManager.instance.Data.MyDecks;
        Deck = EnemyDecks.GetDeck(1);
        EnemyDeck = EnemyDecks.GetDeck(BattleInfo.EnemyDeckNum);

        CharaDisplayManager.CDM.CharaInstantiate(BattleInfo);

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
        Enemykari.Draw(5);

        SetOrder();

        //Draw(5);
        //EnemyDraw(5);
    }

    //�s�����ݒ� true���Ǝ����̓����@false�͓G�̓���
    public void SetOrder()
    {
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
        for (int i = 0; i < Cards.Count; i++)
        {
            //�J�[�h����
            CardController CreatedCard = Instantiate(CardPrefab, HandField).GetComponent<CardController>();

            CreatedCard.CardNumber = Cards[i];

            CreatedCard.BM = this;

            HandCardTrans[i] = HandCardParent.GetChild(i);
            HandCardTrans[i].localPosition = new Vector3(-500 + i * 250, 0, 0);
            HandCardTrans[i].GetComponent<CardController>().HandNumber = i;
        }
    }

    //�h���[����&�J�[�h����
    private void Draw(int DrawNum)
    {
        //�h���[����&�J�[�h����
        for (int i = 0; i < DrawNum; i++)
        {
            HandCard.Add(Deck[i]);

            //�J�[�h����
            CardController CreatedCard = Instantiate(CardPrefab, HandField).GetComponent<CardController>();

            //�J�[�h�ԍ�����
            CreatedCard.CardNumber = Deck[i];

            CreatedCard.BM = this;
        }
        //�f�b�L����h���[�������̃J�[�h���폜
        Deck.RemoveRange(0, DrawNum);

        //�J�[�h�i���o�[�U�蒼��
        for (int i = 0; i < 5; i++)
        {
            HandCardTrans[i] = HandCardParent.GetChild(i);
            HandCardTrans[i].localPosition = new Vector3(-500 + i * 250, 0, 0);
            HandCardTrans[i].GetComponent<CardController>().HandNumber = i;
        }
    }

    //����̎�D�Ǘ�
    private void EnemyDraw(int DrawNum)
    {
        for (int i = 0; i < DrawNum; i++)
        {
            EnemyHand.Add(EnemyDeck[i]);
        }

        //�h���[�������̃J�[�h���폜
        EnemyDeck.RemoveRange(0, DrawNum);
    }

    //�^�[���G���h����
    public void TurnEnd()
    {
        StartCoroutine("Battle");
    }

    //�^�[���J�E���g���₵����h���[������
    public void TurnChange()
    {
        //�h���[
        Chara.Draw(5 - Chara.HandCard.Count);
        Enemykari.Draw(5 - Enemykari.HandCard.Count);

        //�^�[���J�E���g���₷
        TurnCount++;
        TurnText.text = TurnCount.ToString();

        //����Ǝ����̃}�i���₷
        Chara.Cost++;
        if (Chara.Cost > Chara.Para.MaxCost) Chara.Cost = Chara.Para.MaxCost;

        MyCostText.text = Chara.Cost.ToString();

        Enemykari.Cost++;
        if (Enemykari.Cost > Enemykari.Para.MaxCost) Enemykari.Cost = Enemykari.Para.MaxCost;
        EnemyCostText.text = Enemykari.Cost.ToString();

        SetOrder();
    }

    //�^�[���I�����̎����̓���
    private IEnumerator MyCharaMove()
    {
        print("���g��������");

        //�I�������J�[�h�̏���
        foreach (int Num in ChoicedCard)
        {
            Type type = Type.GetType(CardDataBase.GetCardParameter(HandCard[Num]).ScriptName);

            CardBase Card = (CardBase)Activator.CreateInstance(type);
            Card.Parameter = CardDataBase.GetCardParameter(HandCard[Num]);


            Card.Coroutine(this, true);

            yield return new WaitForSeconds(Card.Parameter.WaitTime);
        }

        ChoicedCard.Sort((a, b) => b - a);

        //��D����폜
        foreach (int Num in ChoicedCard)
        {
            HandCard.RemoveAt(Num);
            Destroy(HandCardTrans[Num].gameObject);
        }

        ChoicedCard.Clear();

        yield return new WaitForSeconds(1f);

        CharaDisplayManager.CDM.CharaReset();

        //�G�̍s���Ɉڂ�
        StartCoroutine("EnemyCharaMove");
    }

    //�^�[���I�����̑���̓���
    private IEnumerator EnemyCharaMove()
    {
        print("�G��������");

        //��Œ����@�f�o�b�O�p
        if (EnemyAttackSkip)
        {
            //�o�t������
            EndBuffProcess();

            yield break;
        }

        EnemyChoiced.Clear();
        int LoopCount = 0, CostCount = 0;

        //�G���g���J�[�h��I���A����������AI�͌�Ŏ�������\��
        while (true)
        {
            //�����ǉ����悤�Ƃ��Ă���J�[�h�̃R�X�g������𒴂��Ă�����ǉ����Ȃ�
            if ((CardDataBase.GetCardParameter(EnemyHand[LoopCount]).Cost + CostCount) <= Enemy.NowHaveCost)
            {
                EnemyChoiced.Add(EnemyHand[LoopCount]);

                CostCount += CardDataBase.GetCardParameter(EnemyHand[LoopCount]).Cost;
            }

            if (EnemyChoiced.Count >= 3 || LoopCount >= EnemyHand.Count - 1 || CostCount >= Enemy.NowHaveCost)
            {
                break;
            }

            LoopCount++;
        }

        //�I�������J�[�h�̏���
        foreach (int Num in EnemyChoiced)
        {
            Type type = Type.GetType(CardDataBase.GetCardParameter(EnemyHand[Num]).ScriptName);

            CardBase Card = (CardBase)Activator.CreateInstance(type);
            Card.Parameter = CardDataBase.GetCardParameter(EnemyHand[Num]);

            Card.Coroutine(this, false);

            yield return new WaitForSeconds(Card.Parameter.WaitTime);
        }

        //�g�p�\�R�X�g�����炷
        Enemy.NowHaveCost -= CostCount;
        EnemyCostText.text = Enemy.NowHaveCost.ToString();

        yield return new WaitForSeconds(1f);

        CharaDisplayManager.CDM.CharaReset();

        //�o�t������
        EndBuffProcess();
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

        Enemykari.ChoiceUseCard();

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

                    Chara.Choiced.RemoveAt(0);
                }
            }
            //�G�s��
            else
            {
                if (Enemykari.Choiced.Count != 0)
                {
                    int CardNumber = Enemykari.HandCard[Enemykari.Choiced[0]];

                    Type type = Type.GetType(CardDataBase.GetCardParameter(CardNumber).ScriptName);

                    CardBase Card = (CardBase)Activator.CreateInstance(type);
                    Card.Parameter = CardDataBase.GetCardParameter(CardNumber);

                    Card.Coroutine(this, false);

                    yield return new WaitForSeconds(Card.Parameter.WaitTime);

                    Enemykari.Choiced.RemoveAt(0);
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
        Filtered = MyChara.Buffs.FindAll(Buff => Buff.UseType == BuffBase.BuffUseType.OnTurnEnd);

        StartCoroutine(BuffProcess(Filtered, false));

        //���ɓG�̃o�t���s��
        Filtered = Enemy.Buffs.FindAll(Buff => Buff.UseType == BuffBase.BuffUseType.OnTurnEnd);

        StartCoroutine(BuffProcess(Filtered, true));

        //�����̃o�t�̃^�[���J�E���g�����炷
        Filtered = MyChara.Buffs.FindAll(Buff => Buff.DecreaseType == BuffBase.BuffDecreaseType.OnTurnEnd || Buff.DecreaseType == BuffBase.BuffDecreaseType.OnUseAndEnd);

        foreach (BuffBase Buff in Filtered)
        {
            Buff.TurnCountDecrease(Chara, false);
        }

        //�G�̃o�t�̃^�[���J�E���g�����炷
        Filtered = Enemy.Buffs.FindAll(Buff => Buff.DecreaseType == BuffBase.BuffDecreaseType.OnTurnEnd || Buff.DecreaseType == BuffBase.BuffDecreaseType.OnUseAndEnd);

        foreach (BuffBase Buff in Filtered)
        {
            Buff.TurnCountDecrease(Enemykari, true);
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
        CharaBase Target = MeOrEnemy ? Enemykari : Chara;

        foreach (BuffBase Buff in Buffs)
        {
            Buff.TurnCountDecrease(Target, MeOrEnemy);
        }
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
