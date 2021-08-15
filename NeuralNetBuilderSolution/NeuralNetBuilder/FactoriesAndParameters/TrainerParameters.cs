using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.ComponentModel;

namespace NeuralNetBuilder.FactoriesAndParameters
{
    public interface ITrainerParameters : INotifyPropertyChanged
    {
        int Epochs { get; set; }
        float LearningRate { get; set; }
        float LearningRateChange { get; set; }
        CostType CostType { get; set; }
    }

    [Serializable]
    public class TrainerParameters : NotifierBase, ITrainerParameters
    {
        #region fields & ctor

        private int epochs;
        private float learningRateChange;
        private float learningRate;
        private CostType costType;

        public TrainerParameters()
        {
            //Epochs = 10;
            //LearningRate = .1f;
            //LearningRateChange = .9f;
            //CostType = CostType.SquaredMeanError;
        }
        //public TrainerParameters(int epochs, float learningRate, float learningRateChange, CostType costType)
        //{
        //    Epochs = epochs;
        //    LearningRate = learningRate;
        //    LearningRateChange = learningRateChange;
        //    CostType = costType;
        //}

        #endregion

        #region ITrainerParameters

        public int Epochs
        {
            get { return epochs; }
            set
            {
                if (epochs != value)
                {
                    epochs = value;
                    OnPropertyChanged();
                }
            }
        }
        public float LearningRate
        {
            get { return learningRate; }
            set
            {
                if (learningRate != value)
                {
                    learningRate = value;
                    OnPropertyChanged();
                }
            }
        }
        public float LearningRateChange
        {
            get { return learningRateChange; }
            set
            {
                if (learningRateChange != value)
                {
                    learningRateChange = value;
                    OnPropertyChanged();
                }
            }
        }
        [JsonConverter(typeof(StringEnumConverter))]
        public CostType CostType
        {
            get { return costType; }
            set
            {
                if (costType != value)
                {
                    costType = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion
    }
}
