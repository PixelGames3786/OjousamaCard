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
        Damage,
        Difense
    }

    public List<CharaImage> Charas;

    public Image MyChara, Enemy;
    public CharaImage CharaImages, EnemyImages;

    public bool DifenseFlag;

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

        MyChara.GetComponent<RectTransform>().sizeDelta = CharaImages.NormalSize;
        Enemy.GetComponent<RectTransform>().sizeDelta = EnemyImages.NormalSize;
    }

    public void CharaMove(MoveType CharaType,MoveType EnemyType,float MoveTime)
    {
        RectTransform CharaRect = MyChara.GetComponent<RectTransform>();
        RectTransform EnemyRect = Enemy.GetComponent<RectTransform>();

        switch (CharaType)
        {
            case MoveType.Normal:

                {
                    MyChara.sprite = CharaImages.NormalSprite;

                    CharaRect.localPosition = CharaImages.NormalPosi;
                    CharaRect.sizeDelta = CharaImages.NormalSize;
                }

                break;

            case MoveType.PhysicAttack:

                {
                    MyChara.sprite = CharaImages.PATKSprite;

                    CharaRect.localPosition = CharaImages.PATKPosi;
                    CharaRect.DOLocalMoveX(CharaImages.PATKPosi.x+50, MoveTime);
                    CharaRect.sizeDelta = CharaImages.PATKSize;
                }

                break;

            case MoveType.MagicAttack:

                {
                    MyChara.sprite = CharaImages.MATKSprite;

                    CharaRect.localPosition = new Vector3(-550, 280);
                    CharaRect.sizeDelta = CharaImages.MATKSize;

                }

                break;

            case MoveType.Damage:

                {

                    if (DifenseFlag)
                    {
                        MyChara.sprite = CharaImages.DifenseSprite;

                        CharaRect.localPosition = CharaImages.DamagePosi;
                        CharaRect.DOLocalMoveX(CharaImages.DamagePosi.x-50, MoveTime);
                        CharaRect.sizeDelta = CharaImages.DifenseSize;

                        DifenseFlag = false;

                        break;
                    }

                    MyChara.sprite = CharaImages.DamageSprite;

                    CharaRect.localPosition = CharaImages.DamagePosi;
                    CharaRect.DOLocalMoveX(CharaImages.DamagePosi.x - 50, MoveTime);
                    CharaRect.sizeDelta = CharaImages.DamageSize;
                }

                break;

            case MoveType.Difense:

                {
                    
                }

                break;
        }

        switch (EnemyType)
        {
            case MoveType.Normal:

                {
                    Enemy.sprite = EnemyImages.NormalSprite;

                    EnemyRect.localPosition = EnemyImages.NormalPosi;
                    EnemyRect.sizeDelta = EnemyImages.NormalSize;

                }

                break;

            case MoveType.PhysicAttack:

                {
                    Enemy.sprite = EnemyImages.PATKSprite;

                    EnemyRect.localPosition = EnemyImages.PATKPosi;
                    EnemyRect.DOLocalMoveX(EnemyImages.PATKPosi.x-50, MoveTime);
                    EnemyRect.sizeDelta = EnemyImages.PATKSize;

                }

                break;

            case MoveType.MagicAttack:

                {
                    Enemy.sprite = EnemyImages.MATKSprite;

                    EnemyRect.localPosition = new Vector3(550, 250);
                    EnemyRect.sizeDelta = EnemyImages.MATKSize;

                }

                break;

            case MoveType.Damage:

                {
                    if (DifenseFlag)
                    {
                        Enemy.sprite = EnemyImages.DifenseSprite;

                        EnemyRect.localPosition = EnemyImages.DamagePosi;
                        EnemyRect.DOLocalMoveX(EnemyImages.DamagePosi.x+50, MoveTime);
                        EnemyRect.sizeDelta = EnemyImages.DifenseSize;

                        DifenseFlag = false;

                        break;
                    }

                    Enemy.sprite = EnemyImages.DamageSprite;

                    EnemyRect.localPosition = EnemyImages.DamagePosi;
                    EnemyRect.DOLocalMoveX(EnemyImages.DamagePosi.x + 50, MoveTime);
                    EnemyRect.sizeDelta = EnemyImages.DamageSize;


                }

                break;

            case MoveType.Difense:

                Enemy.sprite = EnemyImages.DifenseSprite;

                EnemyRect.localPosition = new Vector3(-500, 280);
                EnemyRect.DOLocalMoveX(-550, MoveTime);
                EnemyRect.sizeDelta = EnemyImages.DifenseSize;

                break;
        }
    }

    public void CharaReset()
    {
        MyChara.sprite = CharaImages.NormalSprite;

        MyChara.GetComponent<RectTransform>().localPosition = new Vector3(-550, 250);

        Enemy.sprite = EnemyImages.NormalSprite;

        Enemy.GetComponent<RectTransform>().localPosition = new Vector3(550, 250);

        MyChara.GetComponent<RectTransform>().sizeDelta = CharaImages.NormalSize;
        Enemy.GetComponent<RectTransform>().sizeDelta = EnemyImages.NormalSize;
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
