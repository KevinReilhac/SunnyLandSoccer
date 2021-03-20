using System;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MatchMaker : NetworkBehaviour
{
//-------------------------------[Other classes]------------------------------//
[System.Serializable]
public class Match
{
	[System.Serializable]
	public class SyncListGameObject : SyncList<GameObject> {};

	public string id = null;
	public SyncListGameObject players = new SyncListGameObject();

	public Match(string _id, GameObject _player)
	{
		id = _id;
		players.Add(_player);
	}

	public Match() {}
}

	[System.Serializable]
	public class SyncListMatch : SyncList<Match> {};

//-------------------------------[Statics]----------------------------------//
	private static MatchMaker instance = null;

	public static MatchMaker Instance
	{
		get
		{
			if (instance == null)
				instance = GameObject.FindObjectOfType<MatchMaker>();
			return (instance);
		}
	}

	public static string GetRandomMatchID()
	{
		string _id = "";

		for (int i = 0; i < MATCH_ID_SIZE; i++)
			_id += (char)UnityEngine.Random.Range('A', 'Z');

		Debug.Log("Match ID: " + _id);
		return (_id);
	}

//-------------------------------[Main]----------------------------------//
	private const int MATCH_ID_SIZE = 4;

	private SyncListMatch matches = new SyncListMatch();
	private SyncList<string> matchIds = new SyncList<string>();

	[SerializeField] GameManager gameManagerPrefab = null;

	public bool HostGame(string matchId, GameObject player)
	{
		if (!matchIds.Contains(matchId))
		{
			matches.Add(new Match(matchId, player));
			matchIds.Add(matchId);
			return (true);
		}
		else
		{
			Debug.LogError(matchId + " already exist");
			return (false);
		}
	}

	public void BeginGame(string matchId)
	{
		Match match = GetMatchById(matchId);
		GameManager gameManager = Instantiate(gameManagerPrefab);

		gameManager.GetComponent<NetworkMatchChecker>().matchId = matchId.ToGuid();
		foreach (GameObject player in match.players)
		{
			NetworkPlayer networkPlayer = player.GetComponent<NetworkPlayer>();

			if (networkPlayer)
			{
				networkPlayer.StartGame();
			}
		}
	}

	public bool JoinGame(string matchId, GameObject player)
	{
		Match match = GetMatchById(matchId);

		if (match != null)
		{
			match.players.Add(player);
			if (match.players.Count == 2)
				BeginGame(matchId);
			return (true);
		}
		else
		{
			Debug.LogError(matchId + " not exist");
			return (false);
		}
	}

	private Match GetMatchById(string matchId)
	{
		if (!matchIds.Contains(matchId))
			return (null);
		foreach (Match match in matches)
		{
			if (match.id == matchId)
			{
				return (match);
			}
		}
		return (null);
	}
}
