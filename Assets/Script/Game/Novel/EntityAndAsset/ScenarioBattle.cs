using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExcelAsset]
public class ScenarioBattle : ScriptableObject
{
	public List<BattleSentence> Sheet1; // Replace 'EntityType' to an actual type that is serializable.
}
