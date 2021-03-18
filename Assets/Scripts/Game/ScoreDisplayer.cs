using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class ScoreDisplayer : NetworkBehaviour
{
	[SerializeField] private Text text = null;

	private void Start()
	{
		UpdateScore(0, 0);
	}

	private void Awake()
	{
		GameManager.Instance.Score.OnScore.AddListener(UpdateScore);
	}

	[ClientRpc]
	public void UpdateScore(int leftScore, int rightScore)
	{
		text.text = leftScore.ToString() + " : " + rightScore.ToString();
	}

}
