using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace NeuralNetBuilder
{
    public class InitializerAssistant : INotifyPropertyChanged, INotifyStatusChanged
    {
        #region INotifyPropertyChanged

        private event PropertyChangedEventHandler propertyChanged;
        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                if (propertyChanged == null || !propertyChanged.GetInvocationList().Contains(value))
                    propertyChanged += value;
                // else Log when debugging.

            }
            remove { propertyChanged -= value; }
        }
        public bool IsPropertyChangedNull => propertyChanged == null;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            propertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region INotifyStatusChanged

        private event StatusChangedEventHandler statusChanged;
        public event StatusChangedEventHandler StatusChanged
        {
            add
            {
                if (statusChanged == null || !statusChanged.GetInvocationList().Contains(value))
                    statusChanged += value;
                // else Log when debugging.

            }
            remove { statusChanged -= value; }
        }
        public bool IsStatusChangedNull => statusChanged == null;
        protected virtual void OnStatusChanged([CallerMemberName] string info = null)
        {
            statusChanged?.Invoke(this, new StatusChangedEventArgs(info));
        }

        #endregion
    }
}
