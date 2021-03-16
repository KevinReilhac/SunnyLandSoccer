using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

public class Goal : MonoBehaviour
{
	[SerializeField] private LeftRight side = LeftRight.Left;
	[SerializeField] private UnityEvent onGoal = null;
	[Header("Audio")]
	[SerializeField] private AudioSource audioSource = null;
	[SerializeField] private AudioClip onGoalSound = null;
	[Header("Shockwave")]
	[SerializeField] private float shockwaveForce = 1f;
	[Header("Slowmo")]
	private Player[] players;

	private void Start()
	{
		players = GameObject.FindObjectsOfType<Player>();
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		Ball ball = other.gameObject.GetComponent<Ball>();
		if (!ball)
			return;
		audioSource.PlayOneShot(onGoalSound);
		onGoal.Invoke();
		GameManager.Instance.Score.AddScore(side);
		Shockwave();
	}

	private void Shockwave()
	{
		foreach (Player player in players)
		{
			Vector2 direction = (Vector2)(player.transform.position - transform.position);
			float distance = Vector2.Distance(transform.position, player.transform.position);

			player.rb.AddForce(direction.normalized * 1000 * shockwaveForce * (1 / (distance * 1000)), ForceMode2D.Force);
		}
	}

}
