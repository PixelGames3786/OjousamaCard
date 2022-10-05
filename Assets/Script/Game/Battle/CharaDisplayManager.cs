using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharaDisplayManager : MonoBehaviour
{
    public static CharaDisplayManager CDM;

    public enum MoveType
    {
        Normal,
        PhysicAttack,
        MagicAttack,
        Damage
    }

    public List<CharaImage> Charas;

    public Image MyChara, Enemy;

    // Start is called before the first frame update
    void Awake()
    {
        CDM = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CharaInstantiate(BattleInformation Info)
    {
        MyChara.name = Info.MyCharaImageName;
        Enemy.name = Info.EnemyImageName;

        CharaImage CharaImage = Charas.Find(Image => Image.name == MyChara.name);
        CharaImage EnemyImage = Charas.Find(Image => Image.name == Enemy.name);

        MyChara.sprite = CharaImage.NormalSprite;
        Enemy.sprite = EnemyImage.NormalSprite;
    }

    public void CharaMove(MoveType CharaType,MoveType EnemyType)
    {
        CharaImage CharaImage = Charas.Find(Image=>Image.name==MyChara.name);
        CharaImage EnemyImage = Charas.Find(Image=>Image.name==Enemy.name);

        switch (CharaType)
        {
            case MoveType.Normal:

                {
                    MyChara.sprite = CharaImage.NormalSprite;

                    MyChara.GetComponent<RectTransform>().localPosition = new Vector3(-550,250);
                }

                break;

            case MoveType.PhysicAttack:

                {
                    MyChara.sprite = CharaImage.PATKSprite;

                    MyChara.GetComponent<RectTransform>().localPosition = new Vector3(250, 250);

                }

                break;

            case MoveType.MagicAttack:

                {
                    MyChara.sprite = CharaImage.MATKSprite;

                    MyChara.GetComponent<RectTransform>().localPosition = new Vector3(-550, 250);

                }

                break;

            case MoveType.Damage:

                {
                    MyChara.sprite = CharaImage.DamageSprite;

                    MyChara.GetComponent<RectTransform>().localPosition = new Vector3(-550, 250);

                }

                break;
        }

        switch (EnemyType)
        {
            case MoveType.Normal:

                {
                    Enemy.sprite = EnemyImage.NormalSprite;

                    Enemy.GetComponent<RectTransform>().localPosition = new Vector3(550, 250);

                }

                break;

            case MoveType.PhysicAttack:

                {
                    Enemy.sprite = EnemyImage.PATKSprite;

                    Enemy.GetComponent<RectTransform>().localPosition = new Vector3(-250, 250);

                }

                break;

            case MoveType.MagicAttack:

                {
                    Enemy.sprite = EnemyImage.MATKSprite;

                    Enemy.GetComponent<RectTransform>().localPosition = new Vector3(550, 250);

                }

                break;

            case MoveType.Damage:

                {
                    Enemy.sprite = EnemyImage.DamageSprite;

                    Enemy.GetComponent<RectTransform>().localPosition = new Vector3(550, 250);

                }

                break;
        }
    }

    public void CharaReset()
    {
        CharaImage CharaImage = Charas.Find(Image => Image.name == MyChara.name);
        CharaImage EnemyImage = Charas.Find(Image => Image.name == Enemy.name);

        MyChara.sprite = CharaImage.NormalSprite;

        MyChara.GetComponent<RectTransform>().localPosition = new Vector3(-550, 250);

        Enemy.sprite = EnemyImage.NormalSprite;

        Enemy.GetComponent<RectTransform>().localPosition = new Vector3(550, 250);
    }

}
