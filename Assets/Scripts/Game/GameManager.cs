using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class GameManager : NetworkManager
{
	private static GameManager instance = null;

	[SerializeField] private Score score = null;
	[Header("Ball")]
	[SerializeField] private Transform ballSpawnPosition = null;
	[Header("Win")]
	[SerializeField] private GameObject centerBend = null;
	[SerializeField] private GameObject winBand = null;
	[SerializeField] private GameObject loseBand = null;
	[Header("Options")]
	[SerializeField] private PlayerColors playerColors;
	[SerializeField] private GameMode gameMode = null;

	[SerializeField] private List<Transform> leftPlayerPositions = new List<Transform>();
	[SerializeField] private List<Transform> rightPlayerPositions = new List<Transform>();

	private bool _ended = false;
	private int leftTeamPlayerSize = 0;
	private int rightTeamPlayerSize = 0;

	public Score Score
	{
		get => score;
	}

	public static GameManager Instance
	{
		get
		{
			if (instance == null)
				instance = GameObject.FindObjectOfType<GameManager>();
			return (instance);
		}
	}

	public override void Awake()
	{
		base.Awake();
		maxConnections = gameMode.MaxPlayer;
		score.OnScore.AddListener(CheckWin);
	}

	public override void OnServerAddPlayer(NetworkConnection conn)
	{
		Transform start = numPlayers % 2 == 0 ? leftPlayerPositions[leftTeamPlayerSize++] : rightPlayerPositions[rightTeamPlayerSize++];
		Player player = Instantiate(playerPrefab, start.position, start.rotation).GetComponent<Player>();

		player.TeamSide = numPlayers % 2 == 0 ? LeftRight.Left : LeftRight.Right;
		player.PlayerColor = playerColors.GetColorByIndex(numPlayers);
		NetworkServer.AddPlayerForConnection(conn, player.gameObject);
	}

	public override void OnServerConnect(NetworkConnection conn)
	{
		score.SetScore(score.LeftScore, score.RightScore);

		base.OnServerConnect(conn);
	}

	private void CheckWin(int leftScore, int rightScore)
	{
		if (leftScore >= gameMode.MaxScore || rightScore >= gameMode.MaxScore)
			EndScreen();
		StartCoroutine(__loadMenu(3f));
	}

	private void EndScreen()
	{
		bool hasWin = true;

		centerBend.SetActive(true);
		if (hasWin)
			winBand.SetActive(true);
		else
			loseBand.SetActive(false);
	}

	private IEnumerator __loadMenu(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		SceneManager.LoadScene("Menu");
	}

}
