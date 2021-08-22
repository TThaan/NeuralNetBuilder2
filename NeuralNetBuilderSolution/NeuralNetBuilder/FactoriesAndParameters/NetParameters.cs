using NeuralNetBuilder.FactoriesAndParameters.JsonConverters;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace NeuralNetBuilder.FactoriesAndParameters
{
    public interface INetParameters : INotifyPropertyChanged//, INotifyStatusChanged
    {
        ObservableCollection<ILayerParameters> LayerParametersCollection { get; set; }
        WeightInitType WeightInitType { get; set; }
    }

    [Serializable]
    public class NetParameters : PropertyChangedBase, INetParameters
    {
        #region fields

        private ObservableCollection<ILayerParameters> layerParametersCollection = new ObservableCollection<ILayerParameters>();
        private WeightInitType weightInitType = WeightInitType.None;

        #endregion

        [JsonProperty(ItemConverterType = typeof(GenericJsonConverter<LayerParameters>))]
        public ObservableCollection<ILayerParameters> LayerParametersCollection
        {
            get { return layerParametersCollection; }
            set
            {
                layerParametersCollection = value;
                OnPropertyChanged();
            }
        }
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
