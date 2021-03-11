using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class GameManager : NetworkManager
{
	[Header("Ball")]
	[SerializeField] private Transform ballSpawnPosition = null;
	[Header("Win")]
	[SerializeField] private GameObject centerBend = null;
	[SerializeField] private GameObject redWin = null;
	[SerializeField] private GameObject blueWin = null;
	[Header("Options")]
	[SerializeField] private PlayerColors playerColors;
	[SerializeField] private GameMode gameMode = null;

	[SerializeField] private List<Transform> leftPlayerPositions = new List<Transform>();
	[SerializeField] private List<Transform> rightPlayerPositions = new List<Transform>();

	private bool _ended = false;
	private int leftTeamPlayerSize = 0;
	private int rightTeamPlayerSize = 0;

	public override void Awake()
	{
		base.Awake();
		maxConnections = gameMode.MaxPlayer;
	}

	public override void OnServerAddPlayer(NetworkConnection conn)
	{
		Transform start = numPlayers % 2 == 0 ? leftPlayerPositions[leftTeamPlayerSize++] : rightPlayerPositions[rightTeamPlayerSize++];
		Player player = Instantiate(playerPrefab, start.position, start.rotation).GetComponent<Player>();

		player.PlayerColor = playerColors.GetColorByIndex(numPlayers);
		NetworkServer.AddPlayerForConnection(conn, player.gameObject);
	}

	private void Update()
	{
	}

	private IEnumerator ChangeScene(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		SceneManager.LoadScene("Menu");
	}
}
