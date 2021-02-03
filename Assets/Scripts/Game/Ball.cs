using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
	[Header("Components needed")]
	[SerializeField]
	private Rigidbody2D rb = null;
	[SerializeField]
	private TrailRenderer trail = null;
	[SerializeField]
	private Color trailBaseColor = Color.white;
	[Header("Power settings")]
	[SerializeField] private float currentPowerMin = 1;
	[SerializeField] private float currentPowerMax = 10;
	[SerializeField] private float powerMultiplicator = 5;
	[SerializeField] private float startVelUp = 1f;

	private float currentPower = 1;
	private Vector3 originalScale = Vector3.one;
	private SpriteRandomizer spriteRandomizer = null;

	public void Start()
	{
		spriteRandomizer = GetComponent<SpriteRandomizer>();
		originalScale = transform.localScale;
		Init();
	}

	public void Init()
	{
		rb.velocity = new Vector3(0, startVelUp, 0);
		transform.localScale = originalScale;
		trail.startColor = Color.white;
		rb.angularVelocity = 0f;
		spriteRandomizer.Randomize();
		print("uh");
		trail.Clear();
	}

	void Update()
	{
		currentPower = Mathf.Clamp(currentPower, currentPowerMin, currentPowerMax);
		if (transform.localScale != originalScale) {
			float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
		}
	}

	void OnCollisionEnter2D(Collision2D col)
	{
		if (col.gameObject.tag != "Player") {
			transform.localScale = originalScale;
			currentPower = currentPowerMin;
			trail.startColor = Color.white;
		}
	}

	public void Shot(Vector2 direction, float addPower, Color color)
	{
		rb.velocity = direction * powerMultiplicator * currentPower;
		currentPower = currentPower + addPower;
		if (Mathf.Abs(direction.magnitude) > 0 && transform.localScale.y == originalScale.y)
			transform.localScale = new Vector3(originalScale.x, originalScale.y - (originalScale.y * 0.4f), originalScale.z);
		trail.startColor = color;
	}

	public void SetTrailColor(Color color)
	{
		trail.startColor = color;
	}
}
