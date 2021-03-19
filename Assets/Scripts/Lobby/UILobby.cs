using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class UILobby : MonoBehaviour
{
	private static UILobby instance = null;
	[SerializeField] private GameObject hostWaitScreen = null;
	[SerializeField] private Text matchIdText = null;
	[SerializeField] private GameObject baseScreen = null;
	[SerializeField] private InputField inputMatchId = null;

	public static UILobby Instance
	{
		get
		{
			if (instance == null)
				instance = GameObject.FindObjectOfType<UILobby>();
			return (instance);
		}
	}

	[SerializeField] private InputField matchInput = null;

	private NetworkManager networkManager = null;

	private void Awake()
	{
		networkManager = GameObject.FindObjectOfType<NetworkManager>();
	}

	public void Host()
	{
		NetworkPlayer.Local.Host();
		baseScreen.gameObject.SetActive(false);
		hostWaitScreen.gameObject.SetActive(true);
		matchIdText.text = "Match id: " + NetworkPlayer.Local.MatchId;
	}

	public void HostSuccess(bool success)
	{
		if (success)
		{
		}
		else
		{
			Debug.LogError("Host failed");
		}
	}

	public void JoinSuccess(bool success)
	{
		if (success)
		{
			Debug.Log("ehhhh");
		}
		else
		{
			Debug.LogError("Join failed");
		}
	}

	public void Join()
	{
		NetworkPlayer.Local.JoinGame(inputMatchId.text);
	}

}
