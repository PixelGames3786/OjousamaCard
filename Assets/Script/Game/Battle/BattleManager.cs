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

    private CardParameterList CardDataBase;
    [NonSerialized]
    public BuffScriptList BuffDataBase;
    private EnemyDecks EnemyDecks;
    private BattleInfoList InfoList;
    private BattleInformation NowBattleInfo;

    private int TurnCount = 1;

    public Transform HandCardParent;

    public Transform HandField;
    public GameObject CardPrefab;
    public TextMeshProUGUI TurnText, MyCostText,EnemyCostText, MyHPText, EnemyHPText;

    public BattleStatus MyChara = new BattleStatus(),
                        Enemy = new BattleStatus();

    //�J�[�h����D�ŊǗ�����
    [NonSerialized]
    public List<int> Deck=new List<int>(), HandCard=new List<int>(), ChoicedCard = new List<int>();

    [NonSerialized]
    public List<int> EnemyDeck=new List<int>(),EnemyHand=new List<int>(),EnemyChoiced = new List<int>();

    private Transform[] HandCardTrans=new Transform[5];

    public bool EnemyAttackSkip;

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

        NowBattleInfo = InfoList.GetInfo(SaveLoadManager.instance.NextBattle);

        MyChara.Name = "MyChara";
        Enemy.Name = "Enemy";

        BattleStart();
    }

    //�f���G�����n�܂�����
    private void BattleStart()
    {
        //�e�X�g�̂��߂Ɉ�U�ύX�@��Œ���
        //Deck = SaveLoadManager.instance.Data.MyDecks;
        Deck = EnemyDecks.GetDeck(1);
        EnemyDeck= EnemyDecks.GetDeck(NowBattleInfo.EnemyDeckNum);

        CharaDisplayManager.CDM.CharaInstantiate(NowBattleInfo);

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

        Draw(5);
        EnemyDraw(5);
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
        for (int i=0;i<5;i++)
        {
            HandCardTrans[i] = HandCardParent.GetChild(i);
            HandCardTrans[i].localPosition = new Vector3(-500 + i * 250, 0, 0);
            HandCardTrans[i].GetComponent<CardController>().HandNumber = i;
        }
    }

    //����̎�D�Ǘ�
    private void EnemyDraw(int DrawNum)
    {
        for (int i =0; i < DrawNum; i++)
        {
            EnemyHand.Add(EnemyDeck[i]);
        }

        //�h���[�������̃J�[�h���폜
        EnemyDeck.RemoveRange(0,DrawNum);
    }

    //�^�[���G���h����
    public void TurnEnd()
    {
        StartCoroutine("MyCharaMove");
    }

    //�^�[���J�E���g���₵����h���[������
    public void TurnChange()
    {
        //�h���[
        Draw(5-HandCard.Count);
        EnemyDraw(5-EnemyHand.Count);

        //�^�[���J�E���g���₷
        TurnCount++;
        TurnText.text = TurnCount.ToString();

        //����Ǝ����̃}�i���₷
        MyChara.NowHaveCost++;
        MyCostText.text = MyChara.NowHaveCost.ToString();

        Enemy.NowHaveCost++;
        EnemyCostText.text = Enemy.NowHaveCost.ToString();
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
        int LoopCount = 0,CostCount = 0;

        //�G���g���J�[�h��I���A����������AI�͌�Ŏ�������\��
        while (true)
        {
            //�����ǉ����悤�Ƃ��Ă���J�[�h�̃R�X�g������𒴂��Ă�����ǉ����Ȃ�
            if ((CardDataBase.GetCardParameter(EnemyHand[LoopCount]).Cost+CostCount)<=Enemy.NowHaveCost)
            {
                EnemyChoiced.Add(EnemyHand[LoopCount]);

                CostCount += CardDataBase.GetCardParameter(EnemyHand[LoopCount]).Cost;
            }

            if (EnemyChoiced.Count>=3 || LoopCount>=EnemyHand.Count-1 || CostCount>=Enemy.NowHaveCost)
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

    //��X��������\��
    private void EndBuffProcess()
    {
        List<BuffBase> Filtered;

        //�܂������̃o�t���s��
        Filtered = MyChara.Buffs.FindAll(Buff=>Buff.UseType==BuffBase.BuffUseType.OnTurnEnd);

        StartCoroutine(BuffProcess(Filtered,false));

        //���ɓG�̃o�t���s��
        Filtered = Enemy.Buffs.FindAll(Buff => Buff.UseType == BuffBase.BuffUseType.OnTurnEnd);

        StartCoroutine(BuffProcess(Filtered,true));

        //�����̃o�t�̃^�[���J�E���g�����炷
        Filtered= MyChara.Buffs.FindAll(Buff => Buff.DecreaseType == BuffBase.BuffDecreaseType.OnTurnEnd||Buff.DecreaseType==BuffBase.BuffDecreaseType.OnUseAndEnd);

        foreach (BuffBase Buff in Filtered)
        {
            Buff.TurnCountDecrease(MyChara, false);
        }

        //�G�̃o�t�̃^�[���J�E���g�����炷
        Filtered = Enemy.Buffs.FindAll(Buff => Buff.DecreaseType == BuffBase.BuffDecreaseType.OnTurnEnd || Buff.DecreaseType == BuffBase.BuffDecreaseType.OnUseAndEnd);

        foreach (BuffBase Buff in Filtered)
        {
            Buff.TurnCountDecrease(Enemy, true);
        }

        TurnChange();
    }

    public IEnumerator BuffProcess(List<BuffBase> Buffs,bool MeOrEnemy)
    {
        foreach (BuffBase Buff in Buffs)
        {
            Buff.BuffProcess(this,MeOrEnemy);

            yield return new WaitForSeconds(Buff.WaitTime);
        }
    }

    public void BuffTurnDecrease(List<BuffBase> Buffs,bool MeOrEnemy)
    {
        BattleStatus Target = MeOrEnemy ? Enemy : MyChara;

        foreach (BuffBase Buff in Buffs)
        {
            Buff.TurnCountDecrease(Target,MeOrEnemy);
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
