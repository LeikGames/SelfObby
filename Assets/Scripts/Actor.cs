using Sirenix.OdinInspector;
using UnityEngine;
using Random = Utils.Random;

namespace SelfObby
{
    public abstract class Actor : MonoBehaviour
    {
        protected Mover Mover;

        [SerializeField]
        private bool canDamageShake;

        protected bool DamageShake = false;

        private SpriteRenderer sprite;
        private Vector3 spriteOffset;

        [System.Serializable]
        public struct ActorCollisionState
        {
            public bool Left;
            public bool Right;
            public bool Colliding;
            public bool Below;
            public bool Above;

            public void Reset()
            {
                Colliding = Left = Right = Below = Above = false;
            }
        }

        [ReadOnly]
        public ActorCollisionState CollisionState = new();

        protected virtual void Awake()
        {
            Mover = GetComponent<Mover>();
            sprite = GetComponentInChildren<SpriteRenderer>();
            spriteOffset = sprite.transform.localPosition;

            transform.position = new Vector3(
                Mathf.Round(transform.position.x),
                Mathf.Round(transform.position.y),
                Mathf.Round(transform.position.z)
            );
        }

        protected virtual void LateUpdate()
        {
            if (DamageShake && GameManager.WaitTime > 0 && canDamageShake)
            {
                float shakeRange = 2f;
                Vector3 offset = new Vector3(
                    Random.Range(-shakeRange, shakeRange),
                    Random.Range(-shakeRange, shakeRange)
                );

                if (GameManager.WaitTime < 0.2f)
                    offset *= 0.5f;

                sprite.transform.localPosition = offset + spriteOffset;
            }
            else if (canDamageShake)
            {
                DamageShake = false;
                sprite.transform.localPosition = spriteOffset;
            }
        }

        protected void UpdateCollisionState()
        {
            CollisionState.Below = Mover.FirstCollideAt(Vector2.down);
            CollisionState.Above = Mover.FirstCollideAt(Vector2.up);
            CollisionState.Left = Mover.FirstCollideAt(Vector2.left);
            CollisionState.Right = Mover.FirstCollideAt(Vector2.right);
            CollisionState.Colliding =
                CollisionState.Below
                || CollisionState.Above
                || CollisionState.Left
                || CollisionState.Right;
        }
    }
}
