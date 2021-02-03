using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteRandomizer : MonoBehaviour
{
	[SerializeField] private SpriteRenderer spriteRenderer = null;
	[SerializeField] private Sprite[] sprites = null;

	void Start()
	{
		Randomize();
	}

	public void Randomize()
	{
		if (sprites.Length > 0)
			spriteRenderer.sprite = sprites[Random.Range(0, sprites.Length)];
	}
}