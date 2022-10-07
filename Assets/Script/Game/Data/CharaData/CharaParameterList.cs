using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "CharaParemeterList", menuName = "CreateAsset/CharaParameterList")]
public class CharaParameterList : ScriptableObject
{
    public List<CharaParameter> ParameterList;
    public CharaParameter GetCharaPara(string CharaName)
    {
        return ParameterList.Find(Chara => Chara.name == CharaName);
    }
}
