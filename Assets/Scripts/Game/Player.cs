using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

public class Player : MonoBehaviour
{
	[Header("Components needed")]
	public Rigidbody2D rb = null;
	[SerializeField] private Animator animator = null;
	[Header("Player options")]
	[SerializeField] private float speed = 1;
	[SerializeField] private float jumpForce = 1;
	[SerializeField] private float verticalShootOffset = 0.2f;
	[SerializeField] private Color playerColor = Color.red;
	[SerializeField] private SpriteRenderer marker = null;
	[SerializeField] private float distFromFloor = 0f;
	[SerializeField] private float fastFallGravityScale = 1f;
	public int playerId = 1;
	[Header("Audio")]
	[SerializeField] private AudioSource audioSource = null;
	[SerializeField] private AudioClip jumpSound = null;
	[SerializeField] private AudioClip shootSound = null;
	[Header("Controller")]

	private Vector2 startScale = Vector2.one;
	private float startGravityScale = 0f;
	private Ball ball = null;
	public bool shoked = false;
	public bool canMove = true;

	private const float stickDeadZone = 0.1f;

//------------------------------[Change Move & jump]-------------------------//

	void Update()
	{
		Move();
		UpdateAnimation();
		if (GetShoot())
			Shot();
	}

	private void Move()
	{
		if (!shoked && (GetHorizontal() != 0 || IsOnFloor()))
			canMove = true;
		if (canMove)
			rb.velocity = new Vector2(GetHorizontal() * speed, rb.velocity.y);
		if (GetJump())
			Jump();
		if (GetVertical() < -0.5f)
			rb.gravityScale = fastFallGravityScale;
		else
			rb.gravityScale = startGravityScale;
	}

	private void Jump()
	{
		if (IsOnFloor()) {
			rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
			audioSource?.PlayOneShot(jumpSound);
		}
	}

//--------------------------------[Don't touch]-------------------------------//
	void Start()
	{
		startScale = transform.localScale;
		if (marker)
			marker.color = playerColor;
		startGravityScale = rb.gravityScale;
	}


	private bool IsOnFloor()
	{
		RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position, -Vector2.up);
		RaycastHit2D? floorhit = null;
		float closest = Mathf.Infinity;

		foreach (RaycastHit2D eachHit in hit) {
			if (eachHit.collider.tag == "Floor") {
				if (eachHit.distance < closest) {
					floorhit = eachHit;
					closest = eachHit.distance;
				}
			}
		}

		if (floorhit == null)
			return (false);
		return (floorhit?.distance < distFromFloor);
	}

	private void UpdateAnimation()
	{
		this.animator.SetFloat("speedX", Mathf.Abs(rb.velocity.x));
		this.animator.SetFloat("speedY", rb.velocity.y);
		this.animator.SetBool("crounch", AxisRawer(GetVertical()) == -1);
		if (this.GetHorizontal() != 0)
			this.transform.localScale = new Vector2(Mathf.Sign(GetHorizontal()) * this.startScale.x, this.startScale.y);
	}
//-----------------------------------[Ball]-----------------------------------//
	private void OnTriggerEnter2D(Collider2D other)
	{
		Ball enterBall = other.gameObject.GetComponent<Ball>();

		if (enterBall)
			this.ball = enterBall;
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		if (this.ball == null)
			return;
		if (other.gameObject == ball.gameObject)
			this.ball = null;
	}

	private void Shot()
	{
		Vector2 direction = Vector2.zero;

		this.animator.SetTrigger("punch");
		if (!this.GetShoot() || !ball)
			return;
		this.audioSource?.PlayOneShot(shootSound);
		direction = new Vector2(this.GetHorizontal(), this.GetVertical());
		if (direction.magnitude == 0)
			direction = (ball.transform.position - transform.position);
		direction.y += verticalShootOffset;
		this.ball.Shot(direction.normalized, 0.2f, this.playerColor);
	}

	private void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject == this.ball?.gameObject)
			this.ball.SetTrailColor(playerColor);
	}

//------------------------------[INPUT SYSTEM]--------------------------------//

	private float GetHorizontal()
	{
		if (XCI.IsPluggedIn((XboxController)playerId))
			return (DeadZoner(XCI.GetAxis(XboxAxis.LeftStickX, (XboxController)playerId)));
		return (DeadZoner(GetAxis("Horizontal")));
	}

	private float GetVertical()
	{
		if (XCI.IsPluggedIn((XboxController)playerId))
			return (DeadZoner(XCI.GetAxis(XboxAxis.LeftStickY, (XboxController)playerId)));
		return (DeadZoner(GetAxis("Vertical")));
	}

	private bool GetJump()
	{
		if (XCI.IsPluggedIn((XboxController)playerId))
			return (XCI.GetButtonDown(XboxButton.A, (XboxController)playerId));
		return (GetButtonDown("jump"));
	}


	private bool GetShoot()
	{
		if (XCI.IsPluggedIn((XboxController)playerId))
			return (XCI.GetButtonDown(XboxButton.B, (XboxController)playerId));
		return (GetButtonDown("punch"));
	}

	private float AxisRawer(float axisValue)
	{
		if (axisValue == 0)
			return (0);
		return (Mathf.Sign(axisValue));
	}

	private float GetAxis(string axisName)
	{
		return (Input.GetAxis(axisName + this.playerId.ToString()));
	}

	private float GetAxisRaw(string axisName)
	{
		return (Input.GetAxisRaw(axisName + this.playerId.ToString()));
	}

	private bool GetButtonDown(string buttonName)
	{
		return (Input.GetButtonDown(buttonName + this.playerId.ToString()));
	}

	private float DeadZoner(float axisValue)
	{
		if (Mathf.Abs(axisValue) < stickDeadZone)
			return (0);
		return (axisValue);
	}
	//------------------------------[INPUT SYSTEM]--------------------------------//
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.magenta;
		Gizmos.DrawLine(transform.position, transform.position - new Vector3(0, distFromFloor, 0));
	}
}
