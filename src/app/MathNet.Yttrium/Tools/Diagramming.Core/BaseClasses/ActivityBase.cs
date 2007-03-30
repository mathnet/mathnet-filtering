using System;
using System.Collections.Generic;
using System.Text;

namespace Netron.Diagramming.Core
{
    abstract class ActivityBase : IActivity
    {
        #region Events
        public event EventHandler OnActivityRun;
        #endregion

        #region Fields
        private bool mEnabled;
        private TimeSpan mStepTime = new TimeSpan(0,0,80);
        private DateTime mStartTime = DateTime.Now;
        private string mName;
        #endregion

        #region Properties
        public string Name
        {
            get { return mName; }
        }
        public bool Enabled
        {
            get
            {
                return mEnabled;
            }
            set
            {
                mEnabled = value;
            }
        }

        public TimeSpan StepTime
        {
            get
            {
                return mStepTime;   
            }
            set
            {
                mStepTime = value;
            }
        }

        public DateTime StartTime
        {
            get
            {
                return mStartTime;
            }
            set
            {
               mStartTime = value;
            }
        }

        public TimeSpan Duration
        {
            get
            {
                
                return mStepTime;
            }
        }
        #endregion
        protected ActivityBase(string name){ mName = name;}

        public abstract void Run();

        public abstract void Run(int time);

        public void RunAfter(IActivity firstActivity)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        public abstract void Stop();

        protected void RaiseOnActivityRun()
        {
            if (OnActivityRun != null)
                OnActivityRun(this, EventArgs.Empty);
        }
    }
}
