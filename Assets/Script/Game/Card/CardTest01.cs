using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "CardTest01", menuName = "CreateCard/Test01")]
public class CardTest01 : CardBase
{

    public override IEnumerator CardProcess()
    {
        yield return new WaitForSeconds(0.3f);
    }
}