using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using Mirror;

public class NetworkPlayer : NetworkBehaviour
{

	private NetworkMatchChecker matchChecker = null;
	private static NetworkPlayer local = null;
	[SyncVar] public string MatchId = null;

	public static NetworkPlayer Local
	{
		get => local;
	}

	private void Awake()
	{
		matchChecker = GetComponent<NetworkMatchChecker>();
		DontDestroyOnLoad(gameObject);
	}

	public void Start()
	{
		if (isLocalPlayer)
			local = this;
	}

//-----------------------------[Host]-----------------------------------------//
	public void Host()
	{
		string matchId = MatchMaker.GetRandomMatchID();
		CmdHost(matchId);
	}

	[Command]
	public void CmdHost(string matchId)
	{
		MatchId = matchId;
		if (MatchMaker.Instance.HostGame(matchId, gameObject))
		{
			Debug.Log("Host success");
			matchChecker.matchId = matchId.ToGuid();
			TargetHost(true, matchId);
		}
		else
		{
			Debug.LogError("Host fail");
			TargetHost(false, matchId);
		}
		
	}

	[TargetRpc]
	void TargetHost(bool success, string matchID)
	{
		Debug.Log($"MatchID: {MatchId}");
		UILobby.Instance.HostSuccess(success);
	}
//-----------------------------[Join]-----------------------------------------//

	public void JoinGame(string _inputId)
	{
		CmdJoinGame(_inputId);
	}

	[Command]
	public void CmdJoinGame(string matchId)
	{
		MatchId = matchId;
		if (MatchMaker.Instance.JoinGame(matchId, gameObject))
		{
			Debug.Log("Join success");
			matchChecker.matchId = matchId.ToGuid();
			TargetJoinGame(true, matchId);
		}
		else
		{
			Debug.LogError("Join fail");
			TargetJoinGame(false, matchId);
		}
		
	}

	[TargetRpc]
	void TargetJoinGame(bool success, string matchID)
	{
		Debug.Log($"MatchID: {MatchId}");
		UILobby.Instance.JoinSuccess(success);
	}
//-----------------------------[Join]-----------------------------------------//
	public void BeginGame(string _inputId)
	{
		CmdBeginGame(_inputId);
	}

	[Command]
	public void CmdBeginGame(string matchId)
	{
		TargetBeginGame();
		MatchMaker.Instance.BeginGame(matchId);
		Debug.Log("Begin game");
	}

	[TargetRpc]
	public void TargetBeginGame()
	{
		Debug.Log($"Match {MatchId} begin");
		SceneManager.LoadScene("Game");
	}

	public void StartGame()
	{
		TargetBeginGame();
	}
}
