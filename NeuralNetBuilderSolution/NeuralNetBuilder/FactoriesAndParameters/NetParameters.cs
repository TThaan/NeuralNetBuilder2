using NeuralNetBuilder.FactoriesAndParameters.JsonConverters;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.ObjectModel;

namespace NeuralNetBuilder.FactoriesAndParameters
{
    public interface INetParameters : IParametersBase
    {
        ObservableCollection<ILayerParameters> LayerParametersCollection { get; set; }
        WeightInitType WeightInitType { get; set; }
    }

    [Serializable]
    public class NetParameters : ParametersBase, INetParameters
    {
        [JsonProperty(ItemConverterType = typeof(GenericJsonConverter<LayerParameters>))]
        public ObservableCollection<ILayerParameters> LayerParametersCollection { get; set; } = new ObservableCollection<ILayerParameters>();
        [JsonConverter(typeof(StringEnumConverter))]
        public WeightInitType WeightInitType { get; set; } = WeightInitType.None;
    }
}
