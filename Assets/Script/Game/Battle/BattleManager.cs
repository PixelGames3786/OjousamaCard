using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    private int TurnCount = 1;

    public Transform HandCardParent;

    private CardEntityList CardDataBase;
    private EnemyDecks EnemyDecks;

    public Transform HandField;
    public GameObject CardPrefab;
    public TextMeshProUGUI TurnText, MyCostText,EnemyCostText, MyHPText, EnemyHPText;

    public BattleStatus MyChara = new BattleStatus(),
                        Enemy = new BattleStatus();

    //�J�[�h����D�ŊǗ�����
    public List<int> Deck, HandCard, ChoicedCard = new List<int>();
    private List<int> EnemyDeck,EnemyHand,EnemyChoiced = new List<int>();

    private Transform[] HandCardTrans=new Transform[5];


    // Start is called before the first frame update
    void Start()
    {
        //�����_���̃V�[�h�l�ݒ�
        Random.InitState(System.DateTime.Now.Millisecond);

        CardDataBase = (CardEntityList)Resources.Load("CardEntityList");
        EnemyDecks = (EnemyDecks)Resources.Load("EnemyDecks");

        BattleStart();
    }

    // Update is called once per frame
    void Update()
    {

    }

    //�f���G�����n�܂�����
    private void BattleStart()
    {
        Deck = SaveLoadManager.instance.Data.MyDecks;
        EnemyDeck= EnemyDecks.GetDeck(0);

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
        CardProcess(true);

        //��D����폜
        foreach (int Num in ChoicedCard)
        {
            HandCard.RemoveAt(Num);
            Destroy(HandCardTrans[Num].gameObject);
        }
        ChoicedCard.Clear();

        yield return new WaitForSeconds(0.3f);

        //�G�̍s���Ɉڂ�
        StartCoroutine("EnemyCharaMove");

        yield return null;
    }

    //�^�[���I�����̑���̓���
    private IEnumerator EnemyCharaMove()
    {
        print("�G��������");

        EnemyChoiced.Clear();
        int LoopCount = 0,CostCount = 0;

        //�G���g���J�[�h��I���A����������AI�͌�Ŏ�������\��
        while (true)
        {
            //�����ǉ����悤�Ƃ��Ă���J�[�h�̃R�X�g������𒴂��Ă�����ǉ����Ȃ�
            if ((CardDataBase.GetCardData(EnemyHand[LoopCount]).Cost+CostCount)<=Enemy.NowHaveCost)
            {
                EnemyChoiced.Add(EnemyHand[LoopCount]);

                CostCount += CardDataBase.GetCardData(EnemyHand[LoopCount]).Cost;
            }

            if (EnemyChoiced.Count>=3 || LoopCount>EnemyHand.Count || CostCount>=Enemy.NowHaveCost)
            {
                break;
            }

            LoopCount++;
        }

        CardProcess(false);


        //�������g��HP���Ȃ��Ȃ�����Q�[���I�[�o�[
        if (MyChara.HP<=0)
        {
            //��Œ���
            SceneManager.LoadScene("Title");
        }

        //�g�p�\�R�X�g�����炷
        Enemy.NowHaveCost -= CostCount;
        EnemyCostText.text = Enemy.NowHaveCost.ToString();

        yield return new WaitForSeconds(0.3f);

        TurnChange();

        yield return null;
    }

    //�J�[�h�g�p���̋���
    private IEnumerator CardProcess(bool MeOrEnemy)
    {
        //MeOrEnemy true���Ǝ��� false���ƓG

        if (MeOrEnemy)
        {
            foreach (int Num in ChoicedCard)
            {
                //�J�[�h�f�[�^�擾
                CardEntity Card = CardDataBase.GetCardData(HandCard[Num]);

                //�J�[�h�̃^�C�v���Ƃɏ���������
                foreach (CardEntity.CardType Type in Card.Types)
                {
                    switch (Type)
                    {
                        case CardEntity.CardType.Attack :
                            {
                                Enemy.HP -= Card.Power;

                                EnemyHPText.text = Enemy.HP.ToString() + "<size=45>/" + Enemy.MaxHP.ToString() + "</size>";

                                break;
                            }

                        case CardEntity.CardType.Difence:
                            {
                                break;
                            }

                        case CardEntity.CardType.MyBuff:
                            {
                                break;
                            }

                        case CardEntity.CardType.EnemyBuff:
                            {
                                break;
                            }
                    }

                    yield return new WaitForSeconds(0.3f);
                }
            }
        }
        else
        {
            //�I�������J�[�h�̏���
            foreach (int Num in EnemyChoiced)
            {
                //�J�[�h�f�[�^�擾
                CardEntity Card = CardDataBase.GetCardData(EnemyHand[Num]);

                //�J�[�h�̃^�C�v���Ƃɏ���������
                foreach (CardEntity.CardType Type in Card.Types)
                {
                    switch (Type)
                    {
                        case CardEntity.CardType.Attack:
                            {
                                MyChara.HP -= Card.Power;

                                MyHPText.text = MyChara.HP.ToString() + "<size=45>/" + MyChara.MaxHP.ToString() + "</size>";

                                break;
                            }

                        case CardEntity.CardType.Difence:
                            {
                                break;
                            }

                        case CardEntity.CardType.MyBuff:
                            {
                                break;
                            }

                        case CardEntity.CardType.EnemyBuff:
                            {
                                break;
                            }
                    }

                    yield return new WaitForSeconds(0.3f);
                }
            }
        }
    }
}
