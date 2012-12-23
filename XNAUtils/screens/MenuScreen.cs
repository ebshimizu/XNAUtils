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
using XNAUtils.screens.management;
using XNAUtils.support;

namespace XNAUtils.screens
{
    /// <summary>
    /// A MenuScreen is a screen that is able to have MenuEntries on it.
    /// Menu entries are essentially a list of objects that, when selected, perform a particular action (a callback).
    /// </summary>
    abstract class MenuScreen : GameScreen
    {
        #region Fields

        List<MenuEntry> menuEntries = new List<MenuEntry>();
        int selectedEntry = 0;
        string menuTitle;

        protected Texture2D menuTexture;
        float alpha;

        // Y value of first menu entry
        protected Vector2 position;

        /// <summary>
        /// Line spacing between menu entries. Defaults to 1.2f
        /// </summary>
        protected float lineHeight = 1.2f;

        /// <summary>
        /// Mouse cursor position
        /// </summary>
        protected Vector2 cursorPosition;

        /// <summary>
        /// Use mouse instead of keyboard input
        /// </summary>
        protected bool useMouse;

        #endregion

        #region Properties


        /// <summary>
        /// Gets the list of menu entries, so derived classes can add
        /// or change the menu contents.
        /// </summary>
        protected IList<MenuEntry> MenuEntries
        {
            get { return menuEntries; }
        }


        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public MenuScreen(string menuTitle, Vector2 pos, bool useMouse = false)
        {
            this.menuTitle = menuTitle;
            position = pos;

            this.useMouse = useMouse;

            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }


        #endregion

        #region Handle Input


        /// <summary>
        /// Responds to user input, changing the selected entry and accepting
        /// or cancelling the menu.
        /// </summary>
        public override void HandleInput(InputState input)
        { 
            Vector2 screenPos = input.MousePosition;
            cursorPosition = ScreenManager.camera.screenToWorldUI(screenPos);

            // Accept or cancel the menu? We pass in our ControllingPlayer, which may
            // either be null (to accept input from any player) or a specific index.
            // If we pass a null controlling player, the InputState helper returns to
            // us which player actually provided the input. We pass that through to
            // OnSelectEntry and OnCancel, so they can tell which player triggered them.
            PlayerIndex playerIndex = PlayerIndex.One;

            if (!useMouse)
            {
                // Move to the previous menu entry?
                if (input.IsMenuUp(ControllingPlayer))
                {
                    selectedEntry--;

                    if (selectedEntry < 0)
                        selectedEntry = menuEntries.Count - 1;
                }

                // Move to the next menu entry?
                if (input.IsMenuDown(ControllingPlayer))
                {
                    selectedEntry++;

                    if (selectedEntry >= menuEntries.Count)
                        selectedEntry = 0;
                }
            }
            else
            {
                int index = 0;
                Vector2 pos = new Vector2(0f, position.Y);

                foreach (MenuEntry entry in MenuEntries)
                {
                    pos.X = position.X - entry.GetWidth(this) / 2;
                    Rectangle menuBounds = new Rectangle((int)pos.X, (int)pos.Y, entry.GetWidth(this), entry.GetHeight(this));
                    if (menuBounds.Contains((int)cursorPosition.X, (int)cursorPosition.Y))
                    {
                        selectedEntry = index;

                        if (input.IsLeftMouseClick())
                        {
                            OnSelectEntry(selectedEntry, playerIndex);
                            break;
                        }
                    }

                    index++;
                    pos.Y += entry.GetHeight(this) * lineHeight;
                }
            }

            if (input.IsConfirm(ControllingPlayer, out playerIndex))
            {
                OnSelectEntry(selectedEntry, playerIndex);
            }
            else if (input.IsMenuCancel(ControllingPlayer, out playerIndex))
            {
                OnCancel(playerIndex);
            }
        }


        /// <summary>
        /// Handler for when the user has chosen a menu entry.
        /// </summary>
        protected virtual void OnSelectEntry(int entryIndex, PlayerIndex playerIndex)
        {
            menuEntries[entryIndex].OnSelectEntry(playerIndex);
        }


        /// <summary>
        /// Handler for when the user has cancelled the menu.
        /// </summary>
        protected virtual void OnCancel(PlayerIndex playerIndex)
        {
            ExitScreen();
        }


        /// <summary>
        /// Helper overload makes it easy to use OnCancel as a MenuEntry event handler.
        /// </summary>
        protected void OnCancel(object sender, PlayerIndexEventArgs e)
        {
            OnCancel(e.PlayerIndex);
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Allows the screen the chance to position the menu entries. By default
        /// all menu entries are lined up in a vertical list, centered on the screen.
        /// </summary>
        protected virtual void UpdateMenuEntryLocations()
        {
            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            // start at Y = 175; each X value is generated per entry
            Vector2 pos = new Vector2(0f, position.Y);

            // update each menu entry's location in turn
            for (int i = 0; i < menuEntries.Count; i++)
            {
                MenuEntry menuEntry = menuEntries[i];

                // each entry is to be centered horizontally under max camera resolution
                //pos.X = ScreenManager.GraphicsDevice.Viewport.Width / 2 - menuEntry.GetWidth(this) / 2;
                pos.X = position.X - menuEntry.GetWidth(this) / 2;

                if (ScreenState == ScreenState.TransitionOn)
                    pos.X -= transitionOffset * 256;
                else
                    pos.X += transitionOffset * 512;

                // set the entry's position
                menuEntry.Position = pos;

                // move down for the next entry the size of this entry
                pos.Y += menuEntry.GetHeight(this) * lineHeight;
            }
        }


        /// <summary>
        /// Updates the menu.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            // Update each nested MenuEntry object.
            for (int i = 0; i < menuEntries.Count; i++)
            {
                bool isSelected = IsActive && (i == selectedEntry);

                menuEntries[i].Update(this, isSelected, gameTime);
            }
        }

        /// <summary>
        /// Draws the menu.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // make sure our entries are in the right place before we draw them
            UpdateMenuEntryLocations();

            GraphicsDevice graphics = ScreenManager.GraphicsDevice;
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.DefaultFont;

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend,
                     null, null, null, null, ScreenManager.camera.UITransformation());

            // draw logo texture
            alpha = 1f - TransitionPosition;
            if (menuTexture != null)
                spriteBatch.Draw(menuTexture, Vector2.Zero, Color.White * alpha);

            // Draw each menu entry in turn.
            for (int i = 0; i < menuEntries.Count; i++)
            {
                MenuEntry menuEntry = menuEntries[i];

                bool isSelected = IsActive && (i == selectedEntry);

                menuEntry.Draw(this, isSelected, gameTime);
            }

            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            // Draw the menu title centered on the screen
            Vector2 titlePosition = new Vector2(1920 / 2, 80);
            Vector2 titleOrigin = font.MeasureString(menuTitle) / 2;
            Color titleColor = Color.White * TransitionAlpha;
            float titleScale = 1.25f;

            titlePosition.Y -= transitionOffset * 100;

            spriteBatch.DrawString(font, menuTitle, titlePosition, titleColor, 0,
                                   titleOrigin, titleScale, SpriteEffects.None, 0);

            spriteBatch.End();
        }


        #endregion
    }
}
