using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace NeuralNetBuilder
{
    public class PropertyChangedBase : INotifyPropertyChanged
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
    }
}
