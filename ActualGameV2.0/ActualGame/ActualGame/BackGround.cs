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
    class BackGround
    {   
        
        public Vector2 position = new Vector2(0, 0);//The current position of the Sprite
        public Texture2D backGroundTexture;//The texture object used when drawing the sprite
        public string AssetName; //The asset name for the Sprite's Texture
        public Rectangle size;//The Size of the Sprite (with scale applied)
        
        //The amount to increase/decrease the size of the original sprite. When
        //modified throught he property, the Size of the sprite is recalculated
        //with the new scale applied.
        private float scale = 1.0f;
        public float Scale
        {
            get { return scale; }
            set
            {
                scale = value;
                //Recalculate the Size of the Sprite with the new scale
                size = new Rectangle(0, 0, (int)(backGroundTexture.Width * scale), (int)(backGroundTexture.Height * scale));

            }
        }
        BackGround rightMostBackground;//The Background at the right end of the chain
        BackGround leftMostBackground;//The Background at the left end of the chain

        //The backgrounds that make up the images to be scrolled
        //across the screen.
        List<BackGround> listOfBackgrounds;
        Viewport viewport; //The viewing area for drawing the Scrolling background images within

        public BackGround(Viewport theViewport)//Constructor
        {
            listOfBackgrounds = new List<BackGround>();
            rightMostBackground = null;
            leftMostBackground = null;
            viewport = theViewport;
        
        }

        public BackGround()//Second const
        { }

        public void LoadContent(ContentManager theContentManager)
        {
            //Clear the Sprites currently stored as the left and right ends of the chain
            rightMostBackground = null;
            leftMostBackground = null;

            //The total width of all the sprites in the chain
            float width = 0.0f;

            //Cycle through all of the Background sprites that have been added
            //and load their content and position them.
            foreach (BackGround oBackgroundSprite in listOfBackgrounds)
            {
                //Load the background's content and apply it's scale, the scale is calculated by figuring
                //out how far the sprite needs to be stretech to make it fill the height of the viewport
                oBackgroundSprite.LoadContent(theContentManager, oBackgroundSprite.AssetName);
                oBackgroundSprite.Scale = viewport.Height / oBackgroundSprite.size.Height;

                
                if (rightMostBackground == null)
                {
                    //Position the first Background sprite in line at the (0,0) position
                    oBackgroundSprite.position = new Vector2(viewport.X, viewport.Y);
                    leftMostBackground = oBackgroundSprite;
                }
                else
                {
                    //Position the sprite after the last sprite in line
                    oBackgroundSprite.position = new Vector2(rightMostBackground.position.X + rightMostBackground.size.Width, viewport.Y);
                }

                //Set the sprite as the last one in line
                rightMostBackground = oBackgroundSprite;

                //Increment the width of all the sprites combined in the chain
                width += oBackgroundSprite.size.Width;
            
            }

            //If the Width of all the sprites in the chain does not fill the twice the Viewport width
            //then cycle through the images over and over until we have added
            //enough background images to fill the twice the width. 
            int index = 0;
            if (listOfBackgrounds.Count > 0 && width < viewport.Width * 2)
            {
                do
                {
                    //Add another background image to the chain
                    BackGround oBackgroundSprite = new BackGround();
                    oBackgroundSprite.AssetName = listOfBackgrounds[index].AssetName;
                    oBackgroundSprite.LoadContent(theContentManager, oBackgroundSprite.AssetName);
                    oBackgroundSprite.Scale = viewport.Height / oBackgroundSprite.size.Height;
                    oBackgroundSprite.position = new Vector2(rightMostBackground.position.X + rightMostBackground.size.Width, viewport.Y);
                    listOfBackgrounds.Add(oBackgroundSprite);
                    rightMostBackground = oBackgroundSprite;

                    //Add the new background Image's width to the total width of the chain
                    width += oBackgroundSprite.size.Width;

                    //Move to the next image in the background images
                    //If we've moved to the end of the indexes, start over
                    index += 1;
                    if (index > listOfBackgrounds.Count - 1)
                    {
                        index = 0;
                    }

                } while (width < viewport.Width * 2);
            }

        }

        //Update the Sprite and change it's position based on the passed in speed, direction and elapsed time.
        public void Update(GameTime theGameTime, Vector2 theSpeed, Vector2 theDirection)
        {
            position += theDirection * theSpeed * (float)theGameTime.ElapsedGameTime.TotalSeconds;
        }

        //Adds a background sprite to be scrolled through the screen
        public void AddBackground(string theAssetName)
        {
            BackGround oBackgroundSprite = new BackGround();
            oBackgroundSprite.AssetName = theAssetName;

            listOfBackgrounds.Add(oBackgroundSprite);
        
        }

        //Update the positon of the background images
        public void Update(GameTime theGameTime, int theSpeed)
        {
            //Check to see if any of the Background sprites have moved off the screen
            //if they have, then move them to the right of the chain of scrolling backgrounds
            foreach (BackGround oBackgroundSprite in listOfBackgrounds)
            {
                if (oBackgroundSprite.position.X < viewport.X - oBackgroundSprite.size.Width)
                {
                    oBackgroundSprite.position = new Vector2(rightMostBackground.position.X + rightMostBackground.size.Width, viewport.Y);
                    rightMostBackground = oBackgroundSprite;
                }
            }

            //Set the Direction based on movement to the left or right that was passed in (-1 goes right, 1 goes left)
            Vector2 direction = new Vector2(-1, 0);

            foreach (BackGround oBackgroundSprite in listOfBackgrounds)
            {
                oBackgroundSprite.Update(theGameTime, new Vector2(theSpeed, 0), direction);
            }
        
        }
        //Load the texture for the sprite using the Content Pipeline
        public void LoadContent(ContentManager theContentManager, string theAssetName)
        {
           
            { 
                backGroundTexture = theContentManager.Load<Texture2D>(theAssetName);
                AssetName = theAssetName;
                size = new Rectangle(0, 0, (int)(backGroundTexture.Width * scale), (int)(backGroundTexture.Height * scale));
            }
      
        }

        //Draw the background images to the screen
        public void Draw2(SpriteBatch theSpriteBatch)
        {
            foreach (BackGround oBackgroundSprite in listOfBackgrounds)
            {
                oBackgroundSprite.Draw(theSpriteBatch);
            }
        }

        public void Draw(SpriteBatch theSpriteBatch)
        {
                 
            theSpriteBatch.Draw(backGroundTexture, position,
                new Rectangle(0, 0, backGroundTexture.Width, backGroundTexture.Height), Color.White,
                0.0f, Vector2.Zero, scale, SpriteEffects.None, 0);
        }
    }
}
