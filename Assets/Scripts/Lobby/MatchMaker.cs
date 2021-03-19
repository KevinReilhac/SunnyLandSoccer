using System;
using System.Text;
using System.Security.Cryptography;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public static class MatchExtentions
{
	public static Guid ToGuid(this string id)
	{
		MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();

		byte[] inputBytes = Encoding.Default.GetBytes(id);
		byte[] hashBytes = provider.ComputeHash(inputBytes);

		return (new Guid(hashBytes));
	}
}

public class MatchMaker : NetworkBehaviour
{
//-------------------------------[Other classes]------------------------------//
	[System.Serializable]
	public class SyncListGameObject : SyncList<GameObject> {};

	[System.Serializable]
	public class Match
	{
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

	public bool JoinGame(string matchId, GameObject player)
	{
		if (matchIds.Contains(matchId))
		{
			foreach (Match match in matches)
			{
				if (match.id == matchId)
				{
					match.players.Add(player);
					break;
				}
			}
			Debug.Log("Match joined");
			return (true);
		}
		else
		{
			Debug.LogError(matchId + " not exist");
			return (false);
		}

		return (true);
	}

}
