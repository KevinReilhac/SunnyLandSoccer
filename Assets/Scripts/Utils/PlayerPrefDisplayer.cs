using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPrefDisplayer : MonoBehaviour
{
	[SerializeField] private string variableName = null;
	[SerializeField] private Text text = null;

    void Start()
    {
        string pref = PlayerPrefs.GetString(variableName);

		if (pref == "")
			pref = null;
		text.text = pref == null ? "ERROR" : pref;
    }

}
