using System.Collections.Generic;
using Sirenix.OdinInspector;
using Thuby.SimpleAnimator2D;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SelfObby
{
    public class PlayerController : Actor
    {
        [Header("Variables")]
        [SerializeField]
        private float groundSpeed;

        [SerializeField]
        private float groundAcceleration;

        [SerializeField]
        private float airAcceleration;

        [SerializeField]
        private float maxFallSpeed;

        [SerializeField]
        private float gravity;

        [SerializeField]
        private float jumpForce;

        [SerializeField]
        private float lowGravThreshold;

        [SerializeField]
        private Vector2 deathObjectOffset;

        [SerializeField]
        private Vector2 respawnOffset;

        [SerializeField]
        private GameObject deadBodyPrefab;

        private Animator2D animator;

        [PropertySpace]
        [SerializeField]
        private TextMeshProUGUI lifeLabel;

        [SerializeField]
        private CanvasGroup gameOverGroup;

        [SerializeField]
        private int lives;

        [PropertySpace]
        [SerializeField]
        private AnimationClip2D idleClip;

        [SerializeField]
        private AnimationClip2D runClip;

        [SerializeField]
        private AnimationClip2D jumpClip;

        [SerializeField]
        private AnimationClip2D fallClip;

        [SerializeField]
        private AnimationClip2D deathClip;

        [ShowInInspector, ReadOnly]
        private float waitTime = 0;

        private bool isDead;

        private bool gameOver;

        private bool grounded;

        private SpriteRenderer spriteRenderer;

        private Vector2 squish = Vector2.one;

        private EFacing facing = EFacing.Right;
        private EFacing prevFacing = EFacing.Right;

        // Inputs
        private PlayerInputManager input = new PlayerInputManager();
        public PlayerInputManager Input => input;

        [SerializeField, ReadOnly]
        private Vector3 velocity;
        public Vector3 Velocity => velocity;

        private bool firstInput = false;
        public bool FirstInput => firstInput;

        private Vector2 moveDir;
        private Vector2 wishDir;
        private Vector2 lastMoveDir = Vector2.down;

        private CameraBehavior cam;

        protected override void Awake()
        {
            base.Awake();

            input.InitialiseInput();

            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

            animator = GetComponentInChildren<Animator2D>();

            lifeLabel.text = lives.ToString();

            gameOverGroup.alpha = 0;
            gameOver = false;
        }

        private void Start()
        {
            cam = CameraBehavior.Instance;
        }

        private void Update()
        {
            if (waitTime > 0)
            {
                waitTime = Mathf.MoveTowards(waitTime, 0, Time.deltaTime);
                spriteRenderer.enabled = false;

                if (waitTime == 0)
                {
                    spriteRenderer.enabled = true;
                    isDead = false;
                }
                else
                {
                    return;
                }
            }

            if (gameOver)
            {
                if (input.anyDown)
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                }

                return;
            }

            input.UpdateInput();

            if (input.anyDown)
                firstInput = true;

            UpdateCollisionState();

            // Update grounded
            grounded = Mover.FirstCollideAt(Vector2.down);

            float accel = groundAcceleration;

            if (!CollisionState.Below)
            {
                accel = airAcceleration;
            }
            else
            {
                if (Mathf.Sign(velocity.x) != input.MoveX && input.MoveX != 0)
                {
                    accel *= 2;
                }
            }

            velocity.x = Mathf.MoveTowards(
                velocity.x,
                groundSpeed * input.MoveX,
                Time.deltaTime * accel
            );

            float fallSpeed = maxFallSpeed;
            float mult = 1;

            if (Mathf.Abs(velocity.y) < lowGravThreshold)
            {
                mult = 0.25f;
            }
            else if (velocity.y < lowGravThreshold)
            {
                mult = 1.75f;
            }

            if (!grounded)
            {
                velocity.y = Mathf.MoveTowards(
                    velocity.y,
                    fallSpeed,
                    Time.deltaTime * gravity * mult
                );
            }
            else
            {
                velocity.y = 0;
            }

            if (input.jump.held && grounded)
            {
                velocity.y = jumpForce;
            }

            if (input.jump.up && velocity.y > 0)
            {
                velocity.y /= 5.0f;
            }

            if (velocity.x < 0)
                facing = EFacing.Left;
            else
                facing = EFacing.Right;

            if (facing == EFacing.Left)
                spriteRenderer.flipX = true;
            else
                spriteRenderer.flipX = false;

            Mover.Move(velocity * Time.deltaTime, ResolveX, ResolveY);

            if (
                (
                    UnityEngine.Input.GetKeyDown(KeyCode.F)
                    || Physics2D.OverlapCircle(
                        transform.position,
                        10,
                        1 << LayerMask.NameToLayer("Hazards")
                    )
                ) && !isDead
            )
            {
                isDead = true;
                HandleDeath();
            }

            prevFacing = facing;

            if (grounded)
            {
                if (velocity.x != 0)
                    animator.Play(runClip);
                else
                    animator.Play(idleClip);
            }
            else
            {
                if (velocity.y < 0)
                    animator.Play(fallClip);
                else
                    animator.Play(jumpClip);
            }

            float time = 12;
            squish = Vector2.MoveTowards(squish, Vector2.one, Time.deltaTime * time);
            spriteRenderer.transform.localScale = squish;
        }

        private void ResolveX()
        {
            velocity.x = 0;
        }

        private void ResolveY()
        {
            if (velocity.y > 0)
                velocity.y = 0;
        }

        private void HandleDeath()
        {
            if (lives <= 0)
            {
                gameOver = true;
                gameOverGroup.alpha = 1;
            }

            Instantiate(
                deadBodyPrefab,
                transform.position + (Vector3)deathObjectOffset,
                Quaternion.identity
            );

            transform.position = cam.transform.position + (Vector3)respawnOffset;

            waitTime = 1f;
            lives--;
            lifeLabel.text = lives.ToString();
        }

        private void OnDrawGizmosSelected()
        {
            if (cam != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(cam.transform.position + (Vector3)respawnOffset, 32);
            }
        }
    }
}
