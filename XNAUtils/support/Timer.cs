#region File Info
/*
 * Timer.cs
 * Author: Evan Shimizu, 2010-2012
 */
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using XNAUtils.support;

namespace XNAUtils.support
{
    #region Delegates

    /// <summary>
    /// Execute a function that doesn't take arguments.
    /// </summary>
    public delegate void Action();

    /// <summary>
    /// Delegate that takes a single int parameter
    /// </summary>
    /// <param name="param">Integer parameter</param>
    public delegate void IntAct(int param);

    #endregion

    /// <summary>
    /// Generic timer that executes a function that takes no arguments.
    /// </summary>
    public class Timer
    {
        #region Fields

        /// <summary>
        /// How long to wait before executing the timer's action.
        /// </summary>
        public TimeSpan interval;

        /// <summary>
        /// How much time has passed for the timer.
        /// </summary>
        protected TimeSpan elapsed;

        /// <summary>
        /// Indicates if the timer is running.
        /// </summary>
        protected bool stopped;

        /// <summary>
        /// Indicated if this is a periodic timer.
        /// </summary>
        protected bool periodic;

        /// <summary>
        /// Delegate to be executed at the end of the timer.
        /// </summary>
        protected Action action;

        #endregion Fields

        #region Properties

        /// <summary>
        /// A periodic timer is one that repeats its actions on an interval
        /// </summary>
        public bool Periodic
        {
            get { return periodic; }
            set { periodic = value; }
        }

        public float Remaining { get { return (float) (interval - elapsed).TotalSeconds; } }

        #endregion Properties

        /// <summary>
        /// Makes a stopped timer with no action attached
        /// </summary>
        public Timer()
        {
            interval = new TimeSpan();
            stopped = true;
        }

        /// <summary>
        /// Makes a stopped timer with the specified interval with no action attached.
        /// </summary>
        /// <param name="interval"></param>
        public Timer(TimeSpan interval)
        {
            this.interval = interval;
            stopped = true;
        }

        /// <summary>
        /// Makes a stopped timer that will execute an action after a specified period of time.
        /// </summary>
        /// <param name="interval">How long to wait before executing the action.</param>
        /// <param name="actFunc">The function to execute at the end of the timer.</param>
        public Timer(TimeSpan interval, Action actFunc)
        {
            action = actFunc;
            this.interval = interval;
            stopped = true;
        }

        /// <summary>
        /// Updates the timer. If time has run out, exectue the delegate.
        /// If it's a periodic timer, reset for the next iteration.
        /// </summary>
        /// <param name="time">The GameTime object.</param>
        public virtual void update(GameTime time)
        {
            if (!stopped)
            {
                elapsed += time.ElapsedGameTime;
                if (elapsed.CompareTo(interval) >= 0)
                {
                    execute();
                    if (!periodic) stopped = true;
                    elapsed = new TimeSpan();
                }
            }
        }

        /// <summary>
        /// Executes the action in the timer. Override this to allow different delegates.
        /// </summary>
        protected virtual void execute()
        {
            action();
        }

        /// <summary>
        /// Indicates if the selected timer has stopped.
        /// </summary>
        /// <returns>true if timer is done.</returns>
        public bool Stopped() { return stopped; }

        /// <summary>
        /// Disallow timer updating.
        /// </summary>
        public void Stop() { stopped = true; }

        /// <summary>
        /// Allow the timer to be updated.
        /// </summary>
        public void Start() { stopped = false; }

        /// <summary>
        /// Resets the timer's elapsed time to 0.
        /// </summary>
        public void reset() { elapsed = new TimeSpan(); }
    }

    /// <summary>
    /// An int timer executes a function that takes a single integer parameter.
    /// This is an example of how to extend the timer class to use different delegate types
    /// </summary>
    public class IntTimer : Timer
    {
        #region Fields

        /// <summary>
        /// Delegate parameter
        /// </summary>
        private int param;

        #endregion

        /// <summary>
        /// Creates a timer that uses a single int param as the delegate argument
        /// Esentially wraps the parameter into a void anonymous function to reuse as much
        /// code as possible.
        /// </summary>
        public IntTimer(TimeSpan interval, IntAct action, int param) : base(interval)
        {
            this.param = param;
            this.action = () => action(this.param);
        }
    }
}
