using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace NeuralNetBuilder.FactoriesAndParameters
{
    public interface ILayerParameters : IParametersBase
    {
        int Id { get; set; }
        int NeuronsPerLayer { get; set; }
        float WeightMin { get; set; }
        float WeightMax { get; set; }
        float BiasMin { get; set; }
        float BiasMax { get; set; }
        ActivationType ActivationType { get; set; }
    }

    [Serializable]
    public class LayerParameters : ParametersBase, ILayerParameters
    {
        #region fields

        private int id, neuronsPerLayer;
        float weightMin, weightMax, biasMin, biasMax;
        ActivationType activationType;

        #endregion

        #region public

        public int Id
        {
            get { return id; }
            set
            {
                if (id != value)
                {
                    id = value;
                    OnPropertyChanged();
                }
            }
        }
        public int NeuronsPerLayer
        {
            get { return neuronsPerLayer; }
            set
            {
                if (neuronsPerLayer != value)
                {
                    neuronsPerLayer = value;
                    OnPropertyChanged();
                }
            }
        }
        public float WeightMin
        {
            get { return weightMin; }
            set
            {
                if (weightMin != value)
                {
                    weightMin = value;
                    OnPropertyChanged();
                }
            }
        }
        public float WeightMax
        {
            get { return weightMax; }
            set
            {
                if (weightMax != value)
                {
                    weightMax = value;
                    OnPropertyChanged();
                }
            }
        }
        public float BiasMin
        {
            get { return biasMin; }
            set
            {
                if (biasMin != value)
                {
                    biasMin = value;
                    OnPropertyChanged();
                }
            }
        }
        public float BiasMax
        {
            get { return biasMax; }
            set
            {
                if (biasMax != value)
                {
                    biasMax = value;
                    OnPropertyChanged();
                }
            }
        }
        [JsonConverter(typeof(StringEnumConverter))]
        public ActivationType ActivationType
        {
            get { return activationType; }
            set
            {
                if (activationType != value)
                {
                    activationType = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion
    }
}