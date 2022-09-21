using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleManager : MonoBehaviour
{
    //���ݎ����Ă���}�i�R�X�g
    public int HavingCost = 3;

    private int TurnCount = 1;

    [SerializeField]
    private CardEntityList CardDataBase;

    public Transform HandField;
    public GameObject CardPrefab;
    public TextMeshProUGUI TurnText, MyCostText, MyHPText, EnemyHPText;

    public BattleStatus MyChara = new BattleStatus(),
                        Enemy = new BattleStatus();

    //�J�[�h����D�ŊǗ�����
    public List<int> Deck, HandCard, ChoicedCard = new List<int>();

    private GameObject[] HandCardTrans=new GameObject[5];

    // Start is called before the first frame update
    void Start()
    {
        //�����_���̃V�[�h�l�ݒ�
        Random.InitState(System.DateTime.Now.Millisecond);

        CardDataBase = (CardEntityList)Resources.Load("CardEntityList");

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

        //�V���b�t������
        {
            // ���� n �̏����l�̓f�b�L�̖���
            int n = 20;

            // n��1��菬�����Ȃ�܂ŌJ��Ԃ�
            while (n > 1)
            {
                n--;

                // k�� 0 �` n+1 �̊Ԃ̃����_���Ȓl
                int k = UnityEngine.Random.Range(0, n + 1);

                // k�Ԗڂ̃J�[�h��temp�ɑ��
                int temp = Deck[k];
                Deck[k] = Deck[n];
                Deck[n] = temp;
            }
        }

        //�h���[����&�J�[�h����
        for (int i = 0; i < 5; i++)
        {
            HandCard.Add(Deck[i]);

            //�J�[�h����
            CardController CreatedCard = Instantiate(CardPrefab, HandField).GetComponent<CardController>();

            HandCardTrans[i]=CreatedCard.gameObject;

            //�ʒu�ݒ聕�J�[�h�ԍ�����
            CreatedCard.CardNumber = Deck[i];
            CreatedCard.HandNumber = i;

            CreatedCard.transform.localPosition = new Vector3(-500 + i * 250, 0, 0);

            CreatedCard.BM = this;
        }
        //�f�b�L����h���[�������̃J�[�h���폜
        Deck.RemoveRange(0, 5);

    }

    //�h���[����&�J�[�h����
    private void Draw(int[] Blank)
    {
        //�h���[����&�J�[�h����
        for (int i = 0; i < 5; i++)
        {
            HandCard.Add(Deck[i]);

            //�J�[�h����
            CardController CreatedCard = Instantiate(CardPrefab, HandField).GetComponent<CardController>();

            HandCardTrans[i] = CreatedCard.gameObject;

            //�ʒu�ݒ聕�J�[�h�ԍ�����
            CreatedCard.CardNumber = Deck[i];
            CreatedCard.HandNumber = i;

            CreatedCard.transform.localPosition = new Vector3(-500 + i * 250, 0, 0);

            CreatedCard.BM = this;
        }
        //�f�b�L����h���[�������̃J�[�h���폜
        Deck.RemoveRange(0, 5);
    }

    //�^�[���G���h����
    public void TurnEnd()
    {
        StartCoroutine("MyCharaMove");
    }

    //�^�[���J�E���g���₵����h���[������
    public void TurnChange()
    {

        TurnCount++;

        TurnText.text = TurnCount.ToString();
    }

    private IEnumerator MyCharaMove()
    {
        print("���g��������");

        //�I�������J�[�h�̏���
        foreach (int Num in ChoicedCard)
        {
            //�J�[�h�f�[�^�擾
            CardEntity Card = CardDataBase.GetCardData(HandCard[Num]);

            //�Ƃ肠�����_���[�W�^���鏈���̂�
            Enemy.HP -= Card.Power;

            EnemyHPText.text = Enemy.HP.ToString()+"<size=45>/"+Enemy.MaxHP.ToString()+"</size>";

            yield return new WaitForSeconds(0.3f);
        }

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

    private IEnumerator EnemyCharaMove()
    {
        print("�G��������");

        TurnChange();

        yield return null;
    }
}
