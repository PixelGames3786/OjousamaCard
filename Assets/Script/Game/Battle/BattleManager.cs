using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleManager : MonoBehaviour
{
    private int TurnCount = 1;

    public Transform HandField;
    public GameObject CardPrefab;
    public TextMeshProUGUI TurnText;

    //�J�[�h����D�ŊǗ�����
    public List<int> Deck,HandCard = new List<int>();

    // Start is called before the first frame update
    void Start()
    {
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
        for (int i=0;i<5;i++)
        {
            HandCard.Add(Deck[i]);

            Transform CreatedCard = Instantiate(CardPrefab, HandField).transform;

            CreatedCard.localPosition = new Vector3(-500+i*250,0,0);
        }
        //�f�b�L����J�[�h���폜
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

        StartCoroutine("EnemyCharaMove");

        return null;
    }

    private IEnumerator EnemyCharaMove()
    {
        print("�G��������");

        TurnChange();

        return null;
    }
}
