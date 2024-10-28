using System;
using System.Collections.Generic;
using UnityEngine;

namespace SelfObby
{
    public class Mover : MonoBehaviour
    {
        private float xRemainder;
        private float yRemainder;

        [SerializeField]
        private Vector2 colliderOffset;

        [SerializeField]
        private Vector2 colliderSize;

        public Vector2 ColliderOffset
        {
            get { return colliderOffset; }
        }
        public Vector2 ColliderSize
        {
            get { return colliderSize; }
        }

        [HideInInspector]
        public List<Collider2D> Ignore = new();

        private Bounds bounds;
        public Bounds Bounds
        {
            get
            {
                if (bounds == null)
                {
                    bounds = new Bounds((Vector2)transform.position + colliderOffset, colliderSize);
                }
                return bounds;
            }
        }

        public LayerMask SolidLayer;

        public void Move(Vector2 amount, Action onCollideX = null, Action onCollideY = null)
        {
            MoveX(amount.x, onCollideX);
            MoveY(amount.y, onCollideY);
        }

        public void MoveX(float amount, Action onCollide = null)
        {
            xRemainder += amount;
            int move = Mathf.RoundToInt(xRemainder);

            if (move != 0)
            {
                xRemainder -= move;
                int sign = (int)Mathf.Sign(move);

                while (move != 0)
                {
                    if (!CollideAt(new Vector3(sign, 0)))
                    {
                        Vector2 pos = transform.position;
                        pos.x += sign;
                        move -= sign;
                        transform.position = pos;
                    }
                    else
                    {
                        if (onCollide != null)
                            onCollide();
                        break;
                    }
                }
            }
        }

        public void MoveY(float amount, Action onCollide = null)
        {
            yRemainder += amount;
            int move = Mathf.RoundToInt(yRemainder);

            if (move != 0)
            {
                yRemainder -= move;
                int sign = (int)Mathf.Sign(move);

                while (move != 0)
                {
                    if (!CollideAt(new Vector3(0, sign)))
                    {
                        Vector2 pos = transform.position;
                        pos.y += sign;
                        move -= sign;
                        transform.position = pos;
                    }
                    else
                    {
                        if (onCollide != null)
                            onCollide();
                        break;
                    }
                }
            }
        }

        public Collider2D[] CollideAtAll(Vector2 offset)
        {
            var position = (Vector2)transform.position + offset;

            return Physics2D.OverlapBoxAll(
                position + colliderOffset,
                colliderSize - (Vector2.one * 0.1f),
                0,
                SolidLayer
            );
        }

        public Collider2D[] CollideAtAll(Vector2 offset, LayerMask layer)
        {
            var position = (Vector2)transform.position + offset;

            return Physics2D.OverlapBoxAll(
                position + colliderOffset,
                colliderSize - (Vector2.one * 0.1f),
                0,
                layer
            );
        }

        public bool CollideAt(Vector2 offset)
        {
            var position = (Vector2)transform.position + offset;

            Collider2D[] overlapping = Physics2D.OverlapBoxAll(
                position + colliderOffset,
                colliderSize - (Vector2.one * 0.1f),
                0,
                SolidLayer
            );
            foreach (Collider2D col in overlapping)
            {
                if (Ignore.Contains(col))
                    continue;
                return !col.isTrigger;
            }

            return false;
        }

        public bool FirstCollideAt(Vector2 offset)
        {
            return !CollideAt(Vector2.zero) && CollideAt(offset);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.color = new Color(0, 1, 0, 0.25f);
            Gizmos.matrix = Matrix4x4.TRS(transform.position, Quaternion.identity, Vector3.one);
            Gizmos.DrawWireCube(Vector2.zero + colliderOffset, colliderSize);
        }
    }
}
