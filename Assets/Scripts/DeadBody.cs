using Sirenix.OdinInspector;
using UnityEngine;

namespace SelfObby
{
    public class DeadBody : Actor
    {
        [SerializeField]
        private float maxFallSpeed;

        [SerializeField]
        private float gravity;

        [SerializeField]
        private float lowGravThreshold;

        private bool grounded;

        [SerializeField, ReadOnly]
        private Vector3 velocity;
        public Vector3 Velocity => velocity;

        private void Start()
        {
            velocity.y = 100;
        }

        private void Update()
        {
            UpdateCollisionState();

            // Update grounded
            grounded = Mover.FirstCollideAt(Vector2.down);

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

            Mover.Move(velocity * Time.deltaTime, ResolveX, ResolveY);
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
    }
}
