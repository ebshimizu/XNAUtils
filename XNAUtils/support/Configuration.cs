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
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace XNAUtils.support
{
    public class Configuration
    {
        private const int DEFAULT_SCREEN_WIDTH = 1280;
        private const int DEFAULT_SCREEN_HEIGHT = 720;
        private const bool DEFAULT_FULL = false;
        private const bool DEFAULT_VSYNC = true;
        private const bool DEFAULT_AA = true;
        private const string DEFAULT_WIN_TITLE = "Pipes";
        private static Color DEFAULT_BACK_COLOR = Color.Black;

        public int screenWidth = DEFAULT_SCREEN_WIDTH;
        public int screenHeight = DEFAULT_SCREEN_HEIGHT;
        public bool isFullScreen = DEFAULT_FULL;
        public bool isVSync = DEFAULT_VSYNC;
        public bool isAntiAliasing = DEFAULT_AA;
        public bool isMouseVisible = false;
        public string windowTitle = DEFAULT_WIN_TITLE;
        public Color defaultBackgroundColor = DEFAULT_BACK_COLOR;

        public float sfxVolume = 1.0f;
        public float musicVolume = 1.0f;
        public float zoomSoundFallof = 100f;


        public Configuration()
        {
            SetDefaults();
        }

        public void SetDefaults()
        {
            this.screenWidth = DEFAULT_SCREEN_WIDTH;
            this.screenHeight = DEFAULT_SCREEN_HEIGHT;
            this.isFullScreen = DEFAULT_FULL;
            this.isAntiAliasing = DEFAULT_AA;
            this.isVSync = DEFAULT_VSYNC;
            this.windowTitle = DEFAULT_WIN_TITLE;
            this.defaultBackgroundColor = DEFAULT_BACK_COLOR;
        }

        public void Apply(Microsoft.Xna.Framework.Game game, GraphicsDeviceManager graphics)
        {
            graphics.PreferredBackBufferWidth = screenWidth;
            graphics.PreferredBackBufferHeight = screenHeight;
            graphics.PreferMultiSampling = isAntiAliasing;
            graphics.IsFullScreen = isFullScreen;
            graphics.SynchronizeWithVerticalRetrace = isVSync;
            graphics.ApplyChanges();
            game.IsMouseVisible = isMouseVisible;
            game.Window.Title = windowTitle;

        }

    }
}
