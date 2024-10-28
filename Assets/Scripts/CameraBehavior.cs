using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace SelfObby
{
    public class CameraBehavior : MonoBehaviour
    {
        private int width;
        private int height;

        [HideInInspector]
        public Bounds ViewBounds;

        [SerializeField]
        private Bounds cameraLimits;

        private PlayerController player;

        private Vector2 currentTargetPos;

        private Mover mover;

        private Vector2 velocity;

        private bool inTransition;
        public bool InTransition
        {
            get { return inTransition; }
        }

        private static CameraBehavior instance;
        public static CameraBehavior Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindFirstObjectByType<CameraBehavior>();

                    if (instance == null)
                    {
                        Debug.LogError("No camera behavior in scene");
                        return null;
                    }
                }

                return instance;
            }
        }

        private void Awake()
        {
            width = GetComponent<PixelPerfectCamera>().refResolutionX;
            height = GetComponent<PixelPerfectCamera>().refResolutionY;

            mover = GetComponent<Mover>();

            player = FindFirstObjectByType<PlayerController>();
        }

        private void LateUpdate()
        {
            ViewBounds = new Bounds(transform.position, new Vector3(width, height, 999));

            if (!inTransition)
            {
                Vector3 cameraPos = transform.position;
                Vector2 playerCenterOffset = player.transform.position - ViewBounds.center;
                if (playerCenterOffset.x > 0)
                    cameraPos.x += playerCenterOffset.x;
                transform.position = cameraPos;

                // Clamp position
                transform.position = GetClampedPosition();
            }

            int childCount = transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                float offset = (transform.position.x / 2.0f) % 320;
                transform.GetChild(i).localPosition = new Vector3(i * 320 - offset, 0, 10);
            }
        }

        private Vector3 GetClampedPosition()
        {
            return new Vector3(
                Mathf.Clamp(
                    transform.position.x,
                    cameraLimits.min.x + ViewBounds.extents.x,
                    cameraLimits.max.x - ViewBounds.extents.x
                ),
                Mathf.Clamp(
                    transform.position.y,
                    cameraLimits.min.y + ViewBounds.extents.y,
                    cameraLimits.max.y - ViewBounds.extents.y
                ),
                -10
            );
        }

        public bool IsVisible(Vector2 other, int margin = 16)
        {
            Bounds bounds = ViewBounds;
            bounds.Expand(margin);

            Debug.DrawLine(other, other + Vector2.up, Color.red);

            if (bounds.Contains(other))
            {
                return true;
            }

            return false;
        }

        public bool IsVisible(Bounds other, int margin = 16)
        {
            Vector2 topRight = new Vector2(other.min.x, other.max.y);
            Vector2 topLeft = new Vector2(other.max.x, other.max.y);
            Vector2 bottomRight = new Vector2(other.min.x, other.min.y);
            Vector2 bottomLeft = new Vector2(other.max.x, other.min.y);

            if (
                IsVisible(topRight, margin)
                || IsVisible(topLeft, margin)
                || IsVisible(bottomRight, margin)
                || IsVisible(bottomLeft, margin)
            )
            {
                return true;
            }

            return false;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(ViewBounds.center, new Vector3(0, ViewBounds.size.y));

            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(ViewBounds.center, ViewBounds.size);

            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(cameraLimits.center, cameraLimits.extents * 2f);
        }
    }
}
