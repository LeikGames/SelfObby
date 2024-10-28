using Rewired;
using UnityEngine;

namespace SelfObby
{
    public class PlayerInputManager
    {
        public struct ButtonInput
        {
            public bool down;
            public bool up;
            public bool held;
            public float heldTime;

            public void Update()
            {
                if (down)
                    heldTime += Time.deltaTime;
                else
                    heldTime = 0;
            }
        }

        private Player player;

        public Vector2 move = Vector2.zero;
        public Vector2 normalMove = Vector2.zero;
        public Vector2 prevNormalMove = Vector2.zero;

        public int MoveX => (int)normalMove.x;
        public int MoveY => (int)normalMove.y;

        public ButtonInput jump;

        public bool anyDown;

        public void InitialiseInput()
        {
            player = ReInput.players.GetPlayer(0);
        }

        public void UpdateInput()
        {
            prevNormalMove = normalMove;

            move.x = player.GetAxis("Horizontal");
            move.y = player.GetAxis("Vertical");

            jump.down = player.GetButtonDown("Jump");
            jump.up = player.GetButtonUp("Jump");
            jump.held = player.GetButton("Jump");
            jump.Update();

            anyDown = (jump.down || move.magnitude > 0);

            // Normalized horizontal input
            if (move.x > 0)
            {
                normalMove.x = 1;
            }
            else if (move.x < 0)
            {
                normalMove.x = -1;
            }
            else
            {
                normalMove.x = 0;
            }

            // Normalized vertical input
            if (move.y > 0)
            {
                normalMove.y = 1;
            }
            else if (move.y < -0)
            {
                normalMove.y = -1;
            }
            else
            {
                normalMove.y = 0;
            }
        }
    }
}
