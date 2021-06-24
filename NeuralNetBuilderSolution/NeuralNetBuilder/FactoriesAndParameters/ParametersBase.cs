using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NeuralNetBuilder.FactoriesAndParameters
{
    public interface IParametersBase : INotifyPropertyChanged
    {
    }

    [Serializable]
    public class ParametersBase : IParametersBase
    {
        #region INotifyPropertyChanged

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
