using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pacman
{
    [RequireComponent (typeof(Collider2D))]
    public abstract class Character : MonoBehaviour
    {
        [SerializeField] protected Sprite[] characterSprites;
        [SerializeField] protected SpriteRenderer characterSpriteRenderer = default;
        [SerializeField] protected Rigidbody2D characterRigidBody = default;
        [SerializeField] protected CircleCollider2D characterCollider = default;
      
        private LayerMask layerMask = default;
        protected bool isCharacterAlive = false;
        private Coroutine playCharacterAnimation = default;
        private float characterColliderSize = default;
        protected Vector3 validateDirection = default;

        protected virtual void Awake()
        {
            characterColliderSize = characterCollider.radius;
            validateDirection = transform.position;
            layerMask = LayerMask.GetMask("Obstacle");
        }

        protected void ResetCharacterState()
        {
            isCharacterAlive = true;

            KillCharacterAnimation();
        }

        protected void OnCharacterDied()
        {
            isCharacterAlive = false;

            KillCharacterAnimation();
        }

        private void KillCharacterAnimation()
        {
            if (playCharacterAnimation != null)
                StopCoroutine(playCharacterAnimation);
        }

        protected bool IsSwitchableDirection(Vector2 validateDirection)
        {
            RaycastHit2D raycastHit = Physics2D.BoxCast(transform.position, Vector2.one * characterColliderSize, 0f, validateDirection, characterColliderSize, layerMask);
            return raycastHit.collider == null;
        }

        private void OnDestroy()
        {
            KillCharacterAnimation();
        }
    }
}
