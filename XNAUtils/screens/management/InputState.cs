#region File Description
//-----------------------------------------------------------------------------
// Adapted from InputState.cs from Microsoft's GSM sample code
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
#endregion

namespace XNAUtils.screens.management
{
    /// <summary>
    /// Helper for reading input from keyboard, gamepad, and touch input. This class 
    /// tracks both the current and previous state of the input devices, and implements 
    /// query methods for high level input actions.
    /// </summary>
    public class InputState
    {
        #region Fields

        public const int MaxInputs = 4;

        public readonly KeyboardState[] CurrentKeyboardStates;
        public readonly GamePadState[] CurrentGamePadStates;
        private MouseState currentMouseState;

        public readonly KeyboardState[] LastKeyboardStates;
        public readonly GamePadState[] LastGamePadStates;
        private MouseState lastMouseState;

        public readonly bool[] GamePadWasConnected;

        #endregion

        #region Properties

        public MouseState CurrentMouseState { get { return currentMouseState; } }
        public MouseState LastMouseState { get { return lastMouseState; } }

        public Vector2 MousePosition { get { return new Vector2(currentMouseState.X, currentMouseState.Y); } }

        #endregion

        #region Initialization

        /// <summary>
        /// Constructs a new input state.
        /// </summary>
        public InputState()
        {
            CurrentKeyboardStates = new KeyboardState[MaxInputs];
            CurrentGamePadStates = new GamePadState[MaxInputs];
            currentMouseState = new MouseState();

            LastKeyboardStates = new KeyboardState[MaxInputs];
            LastGamePadStates = new GamePadState[MaxInputs];
            lastMouseState = new MouseState();

            GamePadWasConnected = new bool[MaxInputs];
        }

        #endregion

        #region Update

        /// <summary>
        /// Reads the latest state of the keyboard and gamepad.
        /// </summary>
        public void Update()
        {
            for (int i = 0; i < MaxInputs; i++)
            {
                LastKeyboardStates[i] = CurrentKeyboardStates[i];
                LastGamePadStates[i] = CurrentGamePadStates[i];

                CurrentKeyboardStates[i] = Keyboard.GetState((PlayerIndex)i);
                CurrentGamePadStates[i] = GamePad.GetState((PlayerIndex)i);

                // Keep track of whether a gamepad has ever been
                // connected, so we can detect if it is unplugged.
                if (CurrentGamePadStates[i].IsConnected)
                {
                    GamePadWasConnected[i] = true;
                }
            }

            lastMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();
        }

        #endregion

        #region Keyboard

        /// <summary>
        /// Helper for checking if a key was newly pressed during this update. The
        /// controllingPlayer parameter specifies which player to read input for.
        /// If this is null, it will accept input from any player. When a keypress
        /// is detected, the output playerIndex reports which player pressed it.
        /// </summary>
        public bool IsNewKeyPress(Keys key, PlayerIndex? controllingPlayer,
                                            out PlayerIndex playerIndex)
        {
            if (controllingPlayer.HasValue)
            {
                // Read input from the specified player.
                playerIndex = controllingPlayer.Value;

                int i = (int)playerIndex;

                return (CurrentKeyboardStates[i].IsKeyDown(key) &&
                        LastKeyboardStates[i].IsKeyUp(key));
            }
            else
            {
                // Accept input from any player.
                return (IsNewKeyPress(key, PlayerIndex.One, out playerIndex) ||
                        IsNewKeyPress(key, PlayerIndex.Two, out playerIndex) ||
                        IsNewKeyPress(key, PlayerIndex.Three, out playerIndex) ||
                        IsNewKeyPress(key, PlayerIndex.Four, out playerIndex));
            }
        }

        /// <summary>
        /// Returns true if the player has released a key that was down previously
        /// </summary>
        public bool IsKeyRelease(Keys key, PlayerIndex? controllingPlayer,
                                           out PlayerIndex playerIndex)
        {
            if (controllingPlayer.HasValue)
            {
                // Read input from the specified player.
                playerIndex = controllingPlayer.Value;

                int i = (int)playerIndex;

                return (CurrentKeyboardStates[i].IsKeyUp(key) &&
                        LastKeyboardStates[i].IsKeyDown(key));
            }
            else
            {
                // Accept input from any player.
                return (IsKeyRelease(key, PlayerIndex.One, out playerIndex) ||
                        IsKeyRelease(key, PlayerIndex.Two, out playerIndex) ||
                        IsKeyRelease(key, PlayerIndex.Three, out playerIndex) ||
                        IsKeyRelease(key, PlayerIndex.Four, out playerIndex));
            }
        }

        /// <summary>
        /// Checks if a key is down regardless of previous state.
        /// </summary>
        public bool IsKeyDown(Keys key, PlayerIndex? controllingPlayer,
                                        out PlayerIndex playerIndex)
        {
            if (controllingPlayer.HasValue)
            {
                // Read input from the specified player.
                playerIndex = controllingPlayer.Value;

                int i = (int)playerIndex;

                return (CurrentKeyboardStates[i].IsKeyDown(key));
            }
            else
            {
                // Accept input from any player.
                return (IsKeyDown(key, PlayerIndex.One, out playerIndex) ||
                        IsKeyDown(key, PlayerIndex.Two, out playerIndex) ||
                        IsKeyDown(key, PlayerIndex.Three, out playerIndex) ||
                        IsKeyDown(key, PlayerIndex.Four, out playerIndex));
            }
        }

        /// <summary>
        /// Checks if a key is up regardless of previous state.
        /// </summary>
        public bool IsKeyUp(Keys key, PlayerIndex? controllingPlayer,
                                      out PlayerIndex playerIndex)
        {
            if (controllingPlayer.HasValue)
            {
                // Read input from the specified player.
                playerIndex = controllingPlayer.Value;

                int i = (int)playerIndex;

                return (CurrentKeyboardStates[i].IsKeyUp(key));
            }
            else
            {
                // Accept input from any player.
                return (IsKeyUp(key, PlayerIndex.One, out playerIndex) &&
                        IsKeyUp(key, PlayerIndex.Two, out playerIndex) &&
                        IsKeyUp(key, PlayerIndex.Three, out playerIndex) &&
                        IsKeyUp(key, PlayerIndex.Four, out playerIndex));
            }
        }

        #endregion

        #region Controller

        /// <summary>
        /// Helper for checking if a button was newly pressed during this update.
        /// The controllingPlayer parameter specifies which player to read input for.
        /// If this is null, it will accept input from any player. When a button press
        /// is detected, the output playerIndex reports which player pressed it.
        /// </summary>
        public bool IsNewButtonPress(Buttons button, PlayerIndex? controllingPlayer,
                                                     out PlayerIndex playerIndex)
        {
            if (controllingPlayer.HasValue)
            {
                // Read input from the specified player.
                playerIndex = controllingPlayer.Value;

                int i = (int)playerIndex;

                return (CurrentGamePadStates[i].IsButtonDown(button) &&
                        LastGamePadStates[i].IsButtonUp(button));
            }
            else
            {
                // Accept input from any player.
                return (IsNewButtonPress(button, PlayerIndex.One, out playerIndex) ||
                        IsNewButtonPress(button, PlayerIndex.Two, out playerIndex) ||
                        IsNewButtonPress(button, PlayerIndex.Three, out playerIndex) ||
                        IsNewButtonPress(button, PlayerIndex.Four, out playerIndex));
            }
        }


        /// <summary>
        /// Returns true if the player has released a button that was down previously
        /// </summary>
        public bool IsButtonRelease(Buttons button, PlayerIndex? controllingPlayer,
                                                    out PlayerIndex playerIndex)
        {
            if (controllingPlayer.HasValue)
            {
                // Read input from the specified player.
                playerIndex = controllingPlayer.Value;

                int i = (int)playerIndex;

                return (CurrentGamePadStates[i].IsButtonUp(button) &&
                        LastGamePadStates[i].IsButtonDown(button));
            }
            else
            {
                // Accept input from any player.
                return (IsButtonRelease(button, PlayerIndex.One, out playerIndex) ||
                        IsButtonRelease(button, PlayerIndex.Two, out playerIndex) ||
                        IsButtonRelease(button, PlayerIndex.Three, out playerIndex) ||
                        IsButtonRelease(button, PlayerIndex.Four, out playerIndex));
            }
        }

        

        /// <summary>
        /// Checks if a button is down regardless of previous state.
        /// </summary>
        public bool IsButtonDown(Buttons button, PlayerIndex? controllingPlayer,
                                                 out PlayerIndex playerIndex)
        {
            if (controllingPlayer.HasValue)
            {
                // Read input from the specified player.
                playerIndex = controllingPlayer.Value;

                int i = (int)playerIndex;

                return (CurrentGamePadStates[i].IsButtonDown(button));
            }
            else
            {
                // Accept input from any player.
                return (IsButtonDown(button, PlayerIndex.One, out playerIndex) ||
                        IsButtonDown(button, PlayerIndex.Two, out playerIndex) ||
                        IsButtonDown(button, PlayerIndex.Three, out playerIndex) ||
                        IsButtonDown(button, PlayerIndex.Four, out playerIndex));
            }
        }

        

        /// <summary>
        /// Checks if a button is up regardless of previous state.
        /// </summary>
        public bool IsButtonUp(Buttons button, PlayerIndex? controllingPlayer,
                                                 out PlayerIndex playerIndex)
        {
            if (controllingPlayer.HasValue)
            {
                // Read input from the specified player.
                playerIndex = controllingPlayer.Value;

                int i = (int)playerIndex;

                return (CurrentGamePadStates[i].IsButtonUp(button));
            }
            else
            {
                // Accept input from any player.
                return (IsButtonUp(button, PlayerIndex.One, out playerIndex) &&
                        IsButtonUp(button, PlayerIndex.Two, out playerIndex) &&
                        IsButtonUp(button, PlayerIndex.Three, out playerIndex) &&
                        IsButtonUp(button, PlayerIndex.Four, out playerIndex));
            }
        }

        /// <summary>
        /// Returns true if a player's left trigger is strictly greater than a certain threshold.
        /// </summary>
        /// <param name="val">Trigger threshold.</param>
        public bool IsLeftTriggerAbove(float val, PlayerIndex? controllingPlayer,
                                                  out PlayerIndex playerIndex)
        {
            if (controllingPlayer.HasValue)
            {
                // Read input from the specified player.
                playerIndex = controllingPlayer.Value;

                int i = (int)playerIndex;

                bool ret = CurrentGamePadStates[i].Triggers.Left > val;

                return ret;
            }
            else
            {
                // Accept input from any player.
                return (IsLeftTriggerAbove(val, PlayerIndex.One, out playerIndex) ||
                        IsLeftTriggerAbove(val, PlayerIndex.Two, out playerIndex) ||
                        IsLeftTriggerAbove(val, PlayerIndex.Three, out playerIndex) ||
                        IsLeftTriggerAbove(val, PlayerIndex.Four, out playerIndex));
            }
        }

        /// <summary>
        /// Returns true if a player's right trigger is strictly greater than a certain threshold.
        /// </summary>
        /// <param name="val">Trigger threshold.</param>
        public bool IsRightTriggerAbove(float val, PlayerIndex? controllingPlayer,
                                                   out PlayerIndex playerIndex)
        {
            if (controllingPlayer.HasValue)
            {
                // Read input from the specified player.
                playerIndex = controllingPlayer.Value;

                int i = (int)playerIndex;

                return (CurrentGamePadStates[i].Triggers.Right > val);
            }
            else
            {
                // Accept input from any player.
                return (IsRightTriggerAbove(val, PlayerIndex.One, out playerIndex) ||
                        IsRightTriggerAbove(val, PlayerIndex.Two, out playerIndex) ||
                        IsRightTriggerAbove(val, PlayerIndex.Three, out playerIndex) ||
                        IsRightTriggerAbove(val, PlayerIndex.Four, out playerIndex));
            }
        }

        #endregion

        #region Mouse

        /// <summary>
        /// Returns true if the left mouse button has been clicked.
        /// Not an indicator of if the mouse button is being held down.
        /// </summary>
        public bool IsLeftMouseClick()
        {
            return (currentMouseState.LeftButton == ButtonState.Pressed &&
                    lastMouseState.LeftButton == ButtonState.Released);
        }

        /// <summary>
        /// Returns true if the right mouse button has been clicked.
        /// Not an indicator of if the mouse button is being held down
        /// </summary>
        /// <returns></returns>
        public bool IsRightMouseClick()
        {
            return (currentMouseState.RightButton == ButtonState.Pressed &&
                    lastMouseState.RightButton == ButtonState.Released);
        }

        #endregion

        #region Events

        /// <summary>
        /// Checks for a "confirm" input action.
        /// The controllingPlayer parameter specifies which player to read input for.
        /// If this is null, it will accept input from any player. When the action
        /// is detected, the output playerIndex reports which player pressed it.
        /// </summary>
        public virtual bool IsConfirm(PlayerIndex? controllingPlayer,
                                 out PlayerIndex playerIndex)
        {
            return IsNewKeyPress(Keys.Space, controllingPlayer, out playerIndex) ||
                   IsNewKeyPress(Keys.Enter, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.A, controllingPlayer, out playerIndex);
        }

        /// <summary>
        /// Overload that doesn't need a player index out given as an argument.
        /// </summary>
        public virtual bool IsConfirm(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;

            return IsNewKeyPress(Keys.Space, controllingPlayer, out playerIndex) ||
                   IsNewKeyPress(Keys.Enter, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.A, controllingPlayer, out playerIndex);
        }

        /// <summary>
        /// Checks for a "menu cancel" input action.
        /// The controllingPlayer parameter specifies which player to read input for.
        /// If this is null, it will accept input from any player. When the action
        /// is detected, the output playerIndex reports which player pressed it.
        /// </summary>
        public bool IsMenuCancel(PlayerIndex? controllingPlayer,
                                 out PlayerIndex playerIndex)
        {
            return IsNewKeyPress(Keys.Escape, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.B, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.Back, controllingPlayer, out playerIndex);
        }

        /// <summary>
        /// Checks for a "menu up" input action.
        /// The controllingPlayer parameter specifies which player to read
        /// input for. If this is null, it will accept input from any player.
        /// </summary>
        public bool IsMenuUp(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;

            return IsNewKeyPress(Keys.Up, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.DPadUp, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.LeftThumbstickUp, controllingPlayer, out playerIndex);
        }

        /// <summary>
        /// Checks for a "menu down" input action.
        /// The controllingPlayer parameter specifies which player to read
        /// input for. If this is null, it will accept input from any player.
        /// </summary>
        public bool IsMenuDown(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;

            return IsNewKeyPress(Keys.Down, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.DPadDown, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.LeftThumbstickDown, controllingPlayer, out playerIndex);
        }

        /// <summary>
        /// Checks for a "pause the game" input action.
        /// The controllingPlayer parameter specifies which player to read
        /// input for. If this is null, it will accept input from any player.
        /// </summary>
        public bool IsPauseGame(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;

            return IsNewKeyPress(Keys.Escape, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.Back, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.Start, controllingPlayer, out playerIndex);
        }

        #endregion
    }
}
