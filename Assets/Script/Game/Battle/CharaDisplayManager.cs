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
    public CharaImage CharaImages, EnemyImages;

    // Start is called before the first frame update
    void Awake()
    {
        CDM = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CharaInstantiate()
    {
        CharaImages = BattleManager.BM.Chara.Para.Image;
        EnemyImages = BattleManager.BM.Enemy.Para.Image;

        MyChara.sprite = CharaImages.NormalSprite;
        Enemy.sprite = EnemyImages.NormalSprite;
    }

    public void CharaMove(MoveType CharaType,MoveType EnemyType)
    {
        switch (CharaType)
        {
            case MoveType.Normal:

                {
                    MyChara.sprite = CharaImages.NormalSprite;

                    MyChara.GetComponent<RectTransform>().localPosition = new Vector3(-550,250);
                }

                break;

            case MoveType.PhysicAttack:

                {
                    MyChara.sprite = CharaImages.PATKSprite;

                    MyChara.GetComponent<RectTransform>().localPosition = new Vector3(250, 250);

                }

                break;

            case MoveType.MagicAttack:

                {
                    MyChara.sprite = CharaImages.MATKSprite;

                    MyChara.GetComponent<RectTransform>().localPosition = new Vector3(-550, 250);

                }

                break;

            case MoveType.Damage:

                {
                    MyChara.sprite = CharaImages.DamageSprite;

                    MyChara.GetComponent<RectTransform>().localPosition = new Vector3(-550, 250);

                }

                break;
        }

        switch (EnemyType)
        {
            case MoveType.Normal:

                {
                    Enemy.sprite = EnemyImages.NormalSprite;

                    Enemy.GetComponent<RectTransform>().localPosition = new Vector3(550, 250);

                }

                break;

            case MoveType.PhysicAttack:

                {
                    Enemy.sprite = EnemyImages.PATKSprite;

                    Enemy.GetComponent<RectTransform>().localPosition = new Vector3(-250, 250);

                }

                break;

            case MoveType.MagicAttack:

                {
                    Enemy.sprite = EnemyImages.MATKSprite;

                    Enemy.GetComponent<RectTransform>().localPosition = new Vector3(550, 250);

                }

                break;

            case MoveType.Damage:

                {
                    Enemy.sprite = EnemyImages.DamageSprite;

                    Enemy.GetComponent<RectTransform>().localPosition = new Vector3(550, 250);

                }

                break;
        }
    }

    public void CharaReset()
    {
        MyChara.sprite = CharaImages.NormalSprite;

        MyChara.GetComponent<RectTransform>().localPosition = new Vector3(-550, 250);

        Enemy.sprite = EnemyImages.NormalSprite;

        Enemy.GetComponent<RectTransform>().localPosition = new Vector3(550, 250);
    }

    public void CharaAwake(bool MeOrEnemy)
    {
        CharaImages = BattleManager.BM.Chara.Para.Image;
        EnemyImages = BattleManager.BM.Enemy.Para.Image;

        Image Target = MeOrEnemy ? MyChara : Enemy;
        CharaImage TargetImage = MeOrEnemy ? CharaImages : EnemyImages;

        Target.sprite = TargetImage.NormalSprite;
        
    }

}
