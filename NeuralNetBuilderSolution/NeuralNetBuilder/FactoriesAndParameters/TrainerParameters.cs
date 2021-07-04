using System;

namespace NeuralNetBuilder.FactoriesAndParameters
{
    public interface ITrainerParameters : IParametersBase
    {
        int Epochs { get; set; }
        float LearningRate { get; set; }
        float LearningRateChange { get; set; }
        CostType CostType { get; set; }
        //bool IsLoggingActivated { get; set; }
    }

    [Serializable]
    public class TrainerParameters : ParametersBase, ITrainerParameters
    {
        public int Epochs { get; set; }
        public float LearningRate { get; set; }
        public float LearningRateChange { get; set; }
        public CostType CostType { get; set; }
        //public bool IsLoggingActivated { get; set; }
    }
}
