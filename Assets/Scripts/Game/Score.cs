using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

public class Score : NetworkBehaviour
{
	public class ScoreEvent : UnityEvent<int, int> {}

	private int leftScore = 0;
	private int rightScore = 0;

	private ScoreEvent onScore = new ScoreEvent();

	[Server]
	public void ResetScore()
	{
		leftScore = 0;
		rightScore = 0;
		onScore.Invoke(leftScore, rightScore);
	}

	public ScoreEvent OnScore
	{
		get => onScore;
	}

	public int LeftScore
	{
		get => leftScore;
	}

	public int RightScore
	{
		get => rightScore;
	}

	[ClientRpc]
	public void SetScore(int _leftScore, int _rightScore)
	{
		leftScore = _leftScore;
		rightScore = _rightScore;
	}

	public int GetScore(LeftRight side)
	{
		if (side == LeftRight.Left)
			return (leftScore);
		else if (side == LeftRight.Right)
			return (rightScore);
		return (-1);
	}

	[ServerCallback]
	public int AddScore(LeftRight side)
	{
		if (!isServer)
			return (-1);
		if (side == LeftRight.None)
			return (-1);
		if (side == LeftRight.Left)
			rightScore++;
		else if (side == LeftRight.Right)
			leftScore++;
		onScore.Invoke(leftScore, rightScore);
		return (side == LeftRight.Left ? rightScore : leftScore);
	}

}
