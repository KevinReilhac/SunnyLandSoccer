using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

	private PlayerInputs playerInputs = null;
	private Vector2 axisInput = Vector2.zero;
	private Vector2 startScale = Vector2.one;
	private float startGravityScale = 0f;
	private Ball ball = null;
	public bool shoked = false;
	public bool canMove = true;

	private const float stickDeadZone = 0.1f;

//------------------------------[Change Move & jump]-------------------------//

	void Awake()
	{
		SetupInputs();
	}

	void Update()
	{
		UpdateAnimation();
		Move();
	}

	private void SetAxisInput(InputAction.CallbackContext context = default(InputAction.CallbackContext))
	{
		axisInput = context.ReadValue<Vector2>();
	}

	private void Move()
	{
		if (!shoked && (axisInput.x != 0 || IsOnFloor()))
			canMove = true;
		if (canMove)
			rb.velocity = new Vector2(axisInput.x * speed, rb.velocity.y);
		if (axisInput.y < -0.5f)
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
		animator.SetFloat("speedX", Mathf.Abs(rb.velocity.x));
		animator.SetFloat("speedY", rb.velocity.y);
		animator.SetBool("crounch", axisInput.y < 0f);
		if (axisInput.x != 0)
			transform.localScale = new Vector2(Mathf.Sign(axisInput.x) * this.startScale.x, this.startScale.y);
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
		if (ball == null)
			return;
		if (other.gameObject == ball.gameObject)
			ball = null;
	}

	private void Shot()
	{
		Vector2 direction = Vector2.zero;

		animator.SetTrigger("punch");
		if (!ball)
			return;
		audioSource?.PlayOneShot(shootSound);
		direction = new Vector2(direction.x, direction.y);
		if (direction.magnitude == 0)
			direction = (ball.transform.position - transform.position);
		direction.y += verticalShootOffset;
		ball.Shot(direction.normalized, 0.2f, playerColor);
	}

	private void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject == this.ball?.gameObject)
			ball.SetTrailColor(playerColor);
	}

//------------------------------[INPUT SYSTEM]--------------------------------//

	private void SetupInputs()
	{
		playerInputs = new PlayerInputs();

		playerInputs.Enable();
		playerInputs.Game.Move.performed += SetAxisInput;
		playerInputs.Game.Move.canceled += SetAxisInput;

		playerInputs.Game.Jump.performed += (_) => Jump();
		playerInputs.Game.Shot.performed += (_) => Shot();
	}

	//------------------------------[DEBUG]--------------------------------//
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.magenta;
		Gizmos.DrawLine(transform.position, transform.position - new Vector3(0, distFromFloor, 0));
	}
}
