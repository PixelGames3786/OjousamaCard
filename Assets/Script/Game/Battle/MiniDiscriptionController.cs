using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class MiniDiscriptionController : MonoBehaviour
{
    public CardController CardCon;

    public CardBase Card;

    public TextMeshProUGUI Name, Cost,Discription;

    private bool Open=false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Rayを発射
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit2d = Physics2D.Raycast((Vector2)Input.mousePosition, (Vector2)ray.direction);

            // Rayで何もヒットしなかったら画面タッチイベント関数を呼ぶ
            if (!hit2d||(hit2d.collider.tag!="Card"&& Open))
            {
                Close();
            }

            ////Sceneビュー確認用
            //Debug.DrawRay(ray.origin, ray.direction);
            //Debug.DrawRay(Input.mousePosition,ray.direction);
        }
    }

    public void Initialize(CardParameter Card)
    {
        GetComponent<RectTransform>().anchoredPosition = new Vector2(0,665);

        CardCon.Initialize(Card.CardID);

        Name.text = Card.Name;
        Cost.text = "使用コスト:"+Card.Cost;
        Discription.text = Card.Description;

        GetComponent<RectTransform>().DOAnchorPos(new Vector2(0,415),0.15f);

        Open = true;
    }

    public void Close()
    {
        GetComponent<RectTransform>().DOAnchorPos(new Vector2(0,665), 0.15f);

        Open = false;
    }
}
