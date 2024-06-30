using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pacman
{
    public class PacmanCharacter : Character
    {
        [SerializeField] private Animator animator = default;
        private Vector3 direction = default;
        private Vector3 defaultPos = default;
        protected override void Awake()
        {
            base.Awake();
            defaultPos = transform.position;
            direction = validateDirection;
        }

        public void ResetGameCharacter()
        {
            transform.position = defaultPos;
            transform.rotation = Quaternion.identity;
            animator.Rebind();
            animator.SetTrigger("alive");
        }

        protected void Update()
        {
            if (!isCharacterAlive)
                return;

            if (Input.GetKeyDown(KeyCode.A))
                direction = Vector2.left;
            else if (Input.GetKeyDown(KeyCode.D))
                direction = Vector2.right;
            else if (Input.GetKeyDown(KeyCode.S))
                direction = Vector2.down;
            else if (Input.GetKeyDown(KeyCode.W))
                direction = Vector2.up;

            if (IsSwitchableDirection(direction))
            {
                validateDirection = direction;
            }
        }

        private void FixedUpdate()
        {
            if (PacmanGameController.Instance.hasLose || !PacmanGameController.Instance.hasStartGame)
                return;

            if (IsSwitchableDirection(validateDirection))
            {
                transform.position += validateDirection * PacmanGameController.MOVEMENT_SPEED_MULTIPLIER * Time.fixedDeltaTime;
                float angle = Mathf.Atan2(validateDirection.y, validateDirection.x);
                transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.forward);
            }
        }

        private void Start()
        {
            Init();
        }


        public void Init()
        {
            isCharacterAlive = true;
            animator.Rebind();
        }

        public Vector3 GetCurrentDirection()
        {
            return validateDirection;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.tag == "Ghost")
            {
                PacmanGameController.Instance.UpdatePlayerGameStatus();
                animator.SetTrigger("dead");
                validateDirection = Vector3.zero;
                direction = Vector3.zero;
            }
        }
    }
}