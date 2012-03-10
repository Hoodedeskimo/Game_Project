using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
namespace ActualGame
{
    class Character
    {
        Texture2D playerTexture;

        Rectangle playerBounds = new Rectangle(300,337,40,48);
        Rectangle frameBounds = new Rectangle(0,0,40,48);

        int initial_YJumpPosition;
        int frameCount = 0;
        int frameCountDelay = 4;
        int loopCount = 0;
        int upVelocity = 3;
        int downVelocity = 2;

        enum PlayerState
        { 
            Flying,
            Falling
        
        }

        PlayerState currentPlayerState = PlayerState.Falling;

        public void Input()
        {
            KeyboardState currentKeyBoardState = Keyboard.GetState();

            if (currentKeyBoardState.IsKeyDown(Keys.Space))
            {
                currentPlayerState = PlayerState.Flying;

                if (currentPlayerState == PlayerState.Flying)
                {
                    Flying();
                }

                if (currentPlayerState == PlayerState.Falling)
                {
                    Falling();
                }
            }


            if (currentKeyBoardState.IsKeyUp(Keys.Space))
            {
                loopCount = 0;
                currentPlayerState = PlayerState.Falling;
                Falling();
            }
        }

        private void Flying()
        {
            if (loopCount <= 10)
            {

                initial_YJumpPosition = (int)playerBounds.Y;
                playerBounds.Y -= upVelocity;

            }

            else currentPlayerState = PlayerState.Falling;
            loopCount++;
        }
        
        private void Falling()
        {
            playerBounds.Y += downVelocity;
        }

        public void MovementAnimation()
        {
            if (frameCount % frameCountDelay == 0)
            {
                switch (currentPlayerState)
                {
                    case PlayerState.Flying:

                        if (frameCount / frameCountDelay >= 4)

                            frameCount = 0;
                        frameBounds = new Rectangle(frameCount / frameCountDelay * 40, 96, 40, 48);
                        break;

                    case PlayerState.Falling:

                        if (frameCount / frameCountDelay >= 4)

                            frameCount = 0;
                        frameBounds = new Rectangle(frameCount / frameCountDelay * 40, 0, 40, 48);
                        break;

                }
            }
            frameCount++;
        }

        public void LoadContent(ContentManager theContentManager)
        {
            playerTexture = theContentManager.Load<Texture2D>("lucas");
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (currentPlayerState == PlayerState.Flying)
            {
                spriteBatch.Draw(playerTexture, playerBounds, frameBounds, Color.White);
            }

            if (currentPlayerState == PlayerState.Falling)
            {
                spriteBatch.Draw(playerTexture, playerBounds, frameBounds, Color.White);
            }
        }


    }
}
