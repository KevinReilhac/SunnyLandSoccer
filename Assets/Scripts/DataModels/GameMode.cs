using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameMode", menuName = "Data/GameMode")]
public class GameMode : ScriptableObject
{
	public int MaxPlayer = 2;
	public int MaxScore = 5;
}