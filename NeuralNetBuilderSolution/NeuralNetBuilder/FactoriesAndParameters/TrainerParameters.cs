using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace NeuralNetBuilder.FactoriesAndParameters
{
    public interface ITrainerParameters : IParametersBase
    {
        int Epochs { get; set; }
        float LearningRate { get; set; }
        float LearningRateChange { get; set; }
        CostType CostType { get; set; }
    }

    [Serializable]
    public class TrainerParameters : ParametersBase, ITrainerParameters
    {
        #region ctors

        public TrainerParameters()
        {
            Epochs = 10;
            LearningRate = .1f;
            LearningRateChange = .9f;
            CostType = CostType.SquaredMeanError;
        }
        public TrainerParameters(int epochs, float learningRate, float learningRateChange, CostType costType)
        {
            Epochs = epochs;
            LearningRate = learningRate;
            LearningRateChange = learningRateChange;
            CostType = costType;
        }

        #endregion

        #region ITrainerParameters

        public int Epochs { get; set; }
        public float LearningRate { get; set; }
        public float LearningRateChange { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public CostType CostType { get; set; }

        #endregion
    }
}
