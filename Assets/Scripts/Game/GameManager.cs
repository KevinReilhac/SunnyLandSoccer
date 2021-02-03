using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	[Header("Ball")]
	[SerializeField] private Transform ballSpawnPosition = null;
	[Header("Win")]
	[SerializeField] int maxScore = 5;
	[SerializeField] ScoreBoard score = null;
	[SerializeField] private GameObject centerBend = null;
	[SerializeField] private GameObject redWin = null;
	[SerializeField] private GameObject blueWin = null;
	[Header("Audio")]
	[SerializeField] private AudioSource audioSource = null;
	[SerializeField] private AudioSource musicSource = null;
	[SerializeField] private AudioClip winBlueSound = null;
	[SerializeField] private AudioClip winRedSound = null;
	private Goal[] goals;

	private bool ended = false;
	private Ball ball;


	private void Start()
	{
		ball = GameObject.FindObjectOfType<Ball>();
	}

	private void Update()
	{
		if (ended)
			return;
		if (score?.GetHighestScore() == maxScore)
			Win();
	}

	public void RespawnBall()
	{
		if (score.GetHighestScore() != maxScore) {
			ball.transform.position = ballSpawnPosition.position;
			ball.Init();
		}
		else
			Destroy(ball.gameObject);
	}

	private void Win()
	{
		int highest = score.GetHighestScore();

		musicSource?.Stop();
		centerBend?.SetActive(true);
		ended = true;
		if (score?.GetScore(LeftRight.Left) == highest) {
			redWin?.SetActive(true);
			audioSource?.PlayOneShot(winBlueSound);
		}
		else {
			blueWin?.SetActive(true);
			audioSource?.PlayOneShot(winRedSound);
		}
		StartCoroutine(ChangeScene(3f));
	}

	private IEnumerator ChangeScene(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		SceneManager.LoadScene("Menu");
	}
}
