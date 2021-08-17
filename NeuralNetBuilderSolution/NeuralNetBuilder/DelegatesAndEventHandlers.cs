using System;

namespace NeuralNetBuilder
{
    // public delegate void OnPropertyChangedDelegate(string propertyName = "");

    public delegate void StatusChangedEventHandler(object sender, StatusChangedEventArgs e);
    public delegate void TrainerStatusChangedEventHandler(object sender, TrainerStatusChangedEventArgs e);
    public delegate void InitializerStatusChangedEventHandler(object sender, InitializerStatusChangedEventArgs e);

    public class StatusChangedEventArgs : EventArgs
    {
        #region fields & ctor

        private readonly string _info;

        public StatusChangedEventArgs(string info)
        {
            _info = info;
        }

        #endregion

        #region public

        public string Info => _info;

        #endregion
    }
    public class TrainerStatusChangedEventArgs : EventArgs
    {
        #region fields & ctor

        private readonly string _info;

        public TrainerStatusChangedEventArgs(string info)
        {
            _info = info;
        }

        #endregion

        #region public

        public string Info => _info;

        #endregion
    }
    public class InitializerStatusChangedEventArgs : EventArgs
    {
        #region fields & ctor

        private readonly string _info;

        public InitializerStatusChangedEventArgs(string info)
        {
            _info = info;
        }

        #endregion

        #region public

        public string Info => _info;

        #endregion
    }
}