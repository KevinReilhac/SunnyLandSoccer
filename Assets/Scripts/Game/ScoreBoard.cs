using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoard : MonoBehaviour
{
	[SerializeField] private string separator = ":";
	[SerializeField] private Text text = null;

	private int leftValue = 0;
	private int rightValue = 0;

	private void Start()
	{
		DrawScore();
	}

	public void AddScore(LeftRight val)
	{
		if (val == LeftRight.Left)
			leftValue++;
		else
			rightValue++;
		DrawScore();
	}

	public int GetHighestScore()
	{
		return (Mathf.Max(leftValue, rightValue));
	}

	public int GetScore(LeftRight side)
	{
		return (side == LeftRight.Left ? leftValue : rightValue);
	}

	private void DrawScore()
	{
		text.text = leftValue.ToString() + separator + rightValue.ToString();
	}
}
