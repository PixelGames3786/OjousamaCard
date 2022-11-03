using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

[CreateAssetMenu(fileName = "Shield", menuName = "CreateBuff/Shield")]

public class Shield : BuffBase
{
    public override void BuffProcess(BattleManager BM,bool MeOrEnemy)
    {
        CharaBase Target = MeOrEnemy ? BM.Enemy : BM.Chara;

        Target.HPChange(1,false);
    }

    public override int BuffEnhanceProcess()
    {
        return 1;
    }

    public override int ShieldPowerChange(int MotoPower)
    {
        if (UseCount<=0)
        {
            CharaDisplayManager.CDM.DifenseFlag = false;
        }
        else
        {
            CharaDisplayManager.CDM.DifenseFlag = true;

        }

        int Return = MotoPower;

        Return -= UseCount;

        if (Return<0)
        {
            Return = 0;
        }

        UseCount += (Return-MotoPower);

        return Return;
    }
}
