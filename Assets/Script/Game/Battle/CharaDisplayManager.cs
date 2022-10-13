using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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
        RectTransform CharaRect = MyChara.GetComponent<RectTransform>();
        RectTransform EnemyRect = Enemy.GetComponent<RectTransform>();

        switch (CharaType)
        {
            case MoveType.Normal:

                {
                    MyChara.sprite = CharaImages.NormalSprite;

                    CharaRect.localPosition = new Vector3(-550,250);
                }

                break;

            case MoveType.PhysicAttack:

                {
                    MyChara.sprite = CharaImages.PATKSprite;

                    CharaRect.localPosition = new Vector3(200, 250);
                    CharaRect.DOLocalMoveX(250, 0.5f);
                }

                break;

            case MoveType.MagicAttack:

                {
                    MyChara.sprite = CharaImages.MATKSprite;

                    CharaRect.localPosition = new Vector3(-550, 250);

                }

                break;

            case MoveType.Damage:

                {
                    MyChara.sprite = CharaImages.DamageSprite;

                    CharaRect.localPosition = new Vector3(-500, 250);
                    CharaRect.DOLocalMoveX(-550, 0.5f);
                }

                break;
        }

        switch (EnemyType)
        {
            case MoveType.Normal:

                {
                    Enemy.sprite = EnemyImages.NormalSprite;

                    EnemyRect.localPosition = new Vector3(550, 250);

                }

                break;

            case MoveType.PhysicAttack:

                {
                    Enemy.sprite = EnemyImages.PATKSprite;

                    EnemyRect.localPosition = new Vector3(-200, 250);
                    EnemyRect.DOLocalMoveX(-250, 0.5f);
                }

                break;

            case MoveType.MagicAttack:

                {
                    Enemy.sprite = EnemyImages.MATKSprite;

                    EnemyRect.localPosition = new Vector3(550, 250);
                }

                break;

            case MoveType.Damage:

                {
                    Enemy.sprite = EnemyImages.DamageSprite;

                    EnemyRect.localPosition = new Vector3(500, 250);
                    EnemyRect.DOLocalMoveX(550, 0.5f);

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
