using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Mirror;

public class UIOffline : MonoBehaviour
{
	[SerializeField] private CustomNetworkManager networkManager = null;
	[SerializeField] private InputField inputNetworkIp = null;

	public void Host()
	{
		networkManager.StartHost();
	}

	public void Join()
	{
		networkManager.networkAddress = inputNetworkIp.text;
		networkManager.StartClient();
	}

}
