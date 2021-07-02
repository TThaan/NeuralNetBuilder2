using System;

namespace NeuralNetBuilder
{
    public delegate void TrainerStatusChangedEventHandler(object sender, TrainerStatusChangedEventArgs e);
    public delegate void InitializerStatusChangedEventHandler(object sender, InitializerStatusChangedEventArgs e);

    public class TrainerStatusChangedEventArgs : EventArgs
    {
        #region fields & ctor

        private string _info;

        public TrainerStatusChangedEventArgs(string info)
        {
            _info = info;
        }

        #endregion

        #region public

        public string Info
        {
            get { return _info; }
        }

        #endregion
    }
    public class InitializerStatusChangedEventArgs : EventArgs
    {
        #region fields & ctor

        private string _info;

        public InitializerStatusChangedEventArgs(string info)
        {
            _info = info;
        }

        #endregion

        #region public

        public string Info
        {
            get { return _info; }
        }

        #endregion
    }
}