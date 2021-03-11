using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerColors", menuName = "Data/PlayerColors")]
public class PlayerColors : ScriptableObject
{
	[SerializeField]
	private Color defaultColor = Color.red;
	[SerializeField]
	private List<Color> colors = new List<Color>();

	public Color GetColorByIndex(int index)
	{
		if (index >= colors.Count || index < 0)
			return (defaultColor);
		return (colors[index]);
	}
}
