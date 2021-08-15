using NeuralNetBuilder.FactoriesAndParameters.JsonConverters;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace NeuralNetBuilder.FactoriesAndParameters
{
    public interface INetParameters : INotifyPropertyChanged
    {
        ObservableCollection<ILayerParameters> LayerParametersCollection { get; }
        WeightInitType WeightInitType { get; set; }
    }

    [Serializable]
    public class NetParameters : NotifierBase, INetParameters
    {
        #region fields

        // private ObservableCollection<ILayerParameters> layerParametersCollection;
        private WeightInitType weightInitType = WeightInitType.None;

        public NetParameters()
        {
            // Defined this way a potentially registered INPC event will be fired
            LayerParametersCollection = new ObservableCollection<ILayerParameters>();
            OnPropertyChanged(nameof(LayerParametersCollection));
        }

        #endregion

        [JsonProperty(ItemConverterType = typeof(GenericJsonConverter<LayerParameters>))]
        public ObservableCollection<ILayerParameters> LayerParametersCollection { get; }
        [JsonConverter(typeof(StringEnumConverter))]
        public WeightInitType WeightInitType
        {
            get { return weightInitType; }
            set 
            {
                if (weightInitType != value)
                {
                    weightInitType = value;
                    OnPropertyChanged();
}
            }
        }
    }
}
