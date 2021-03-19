using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class ClientTryToConnect : MonoBehaviour
{
	[SerializeField] private NetworkManager networkManager = null;
	[SerializeField] private Text text = null;

	private void Awake()
	{
		text.text = string.Format("Trying to connect to {0}", networkManager.networkAddress);
	}
}
