using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

public class Player : NetworkBehaviour
{
	[Header("Components needed")]
	public Rigidbody2D rb = null;
	[SerializeField] private Animator animator = null;
	[SerializeField] private BoxCollider2D boxCollider = null;
	[Header("Player options")]
	[SerializeField] private float speed = 1;
	[SerializeField] private float jumpForce = 1;
	[SerializeField] private float verticalShootOffset = 0.2f;
	[SyncVar] [SerializeField] private Color playerColor = Color.red;
	[SerializeField] private SpriteRenderer marker = null;
	[SerializeField] private float fastFallGravityScale = 1f;
	[Header("Floor check")]
	[SerializeField] private float floorCheckDistance = 0f;
	[Header("Audio")]
	[SerializeField] private AudioSource audioSource = null;
	[SerializeField] private AudioClip jumpSound = null;
	[SerializeField] private AudioClip shootSound = null;
	[Header("Controller")]

	private PlayerInputs playerInputs = null;
	private Vector2 lastInput = Vector2.zero;
	private Vector2 startScale = Vector2.one;
	private float startGravityScale = 0f;
	private Ball ball = null;
	private LeftRight teamSide = LeftRight.None;

//--------------------------[INIT]--------------------------------------------//

	private void Start()
	{
		startScale = transform.localScale;
		startGravityScale = rb.gravityScale;
		RefreshMarkerColor();
	}
//------------------------------[INPUT SYSTEM]--------------------------------//
	private PlayerInputs PlayerInputs
	{
		get
		{
			if (playerInputs == null)
				playerInputs = new PlayerInputs();
			return (playerInputs);
		}
	}

	public override void OnStartAuthority()
	{
		enabled = true;
		SetupInputs();
	}

	[ClientCallback]
	private void OnEnable() =>	PlayerInputs.Enable();
	[ClientCallback]
	private void OnDisable() =>	PlayerInputs.Disable();

	[Client]
	private void SetupInputs()
	{
		PlayerInputs.Game.Move.performed += SetAxisInput;
		PlayerInputs.Game.Move.canceled += SetAxisInput;

		PlayerInputs.Game.Jump.performed += (_) => Jump();
		PlayerInputs.Game.Shot.performed += (_) => Shot();
	}

//---------------------------------[Move & jump]------------------------------//
	[ClientCallback]
	void Update()
	{
		UpdateAnimation();
		Move();
	}

	[Client]
	private void SetAxisInput(InputAction.CallbackContext context = default(InputAction.CallbackContext))
	{
		lastInput = context.ReadValue<Vector2>();
	}

	[ClientCallback]
	private void Move()
	{
		if (lastInput.y < -0.5f)
			rb.gravityScale = fastFallGravityScale;
		else
			rb.gravityScale = startGravityScale;

		rb.velocity = new Vector2(lastInput.x * speed, rb.velocity.y);
	}

//----------------------------[Movement Logic]--------------------------------//

	[Client]
	private void Jump()
	{
		if (IsOnFloor())
		{
			rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
			audioSource?.PlayOneShot(jumpSound);
		}
	}

	[Client]
	private bool IsOnFloor()
	{
		int layerMask = 1 << LayerMask.NameToLayer("Floor");
		RaycastHit2D hit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down, floorCheckDistance, layerMask);

		return (hit.collider != null);
	}


	[ClientCallback]
	private void UpdateAnimation()
	{
		animator.SetFloat("speedX", Mathf.Abs(rb.velocity.x));
		animator.SetFloat("speedY", rb.velocity.y);
		animator.SetBool("crounch", lastInput.y < 0f);
		if (lastInput.x != 0)
			transform.localScale = new Vector2(Mathf.Sign(lastInput.x) * this.startScale.x, this.startScale.y);
	}
//-----------------------------------[Ball]-----------------------------------//

	[Command]
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

	private void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject == this.ball?.gameObject)
			ball.SetTrailColor(playerColor);
	}

//------------------------------[INPUT SYSTEM]--------------------------------//

	public Color PlayerColor
	{
		get => playerColor;
		set
		{
			playerColor = value;
			RefreshMarkerColor();
		}
	}

	private void RefreshMarkerColor()
	{
		if (marker)
			marker.color = playerColor;
	}

	//------------------------------[ACCESSORS]--------------------------------//
	public LeftRight TeamSide
	{
		set => teamSide = value;
		get => teamSide;
	}
	//------------------------------[DEBUG]--------------------------------//
	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawLine(boxCollider.bounds.center, boxCollider.bounds.center + (Vector3.down * (boxCollider.bounds.extents.y + floorCheckDistance)));
	}
}
