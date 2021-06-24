using DeepLearningDataProvider;
using System;

namespace NeuralNetBuilder.FactoriesAndParameters
{
    internal class TrainerFactory
    {
        #region public/internal

        internal static ITrainer GetRawTrainer()
        {
            return new Trainer() { TrainerStatus = TrainerStatus.Undefined };
        }
        internal static ITrainer InitializeTrainer(ITrainer rawTrainer, INet net, ITrainerParameters trainerParameters, ISampleSet sampleSet)
        {
            if (trainerParameters == null) throw new NullReferenceException(
                $"{typeof(TrainerFactory)}.{nameof(InitializeTrainer)}: Parameter {nameof(trainerParameters)}.");

            if (trainerParameters.Epochs == 0 || trainerParameters.LearningRate == 0)
                throw new ArgumentException($"{typeof(TrainerFactory)}.{nameof(InitializeTrainer)}: " +
                    $"Parameters 'Epoch' and 'LearningRate' must be greater than 0.");


            rawTrainer.OriginalNet = net ?? throw new NullReferenceException($"{typeof(TrainerFactory)}.{nameof(InitializeTrainer)}: Parameter {nameof(net)}.");
            rawTrainer.SampleSet = sampleSet ?? throw new NullReferenceException($"{typeof(TrainerFactory)}.{nameof(InitializeTrainer)} Parameter {nameof(sampleSet)}.");
            rawTrainer.LearningNet = NetFactory.GetLearningNet(net, trainerParameters.CostType);
            rawTrainer.LearningRate = trainerParameters.LearningRate;
            rawTrainer.LearningRateChange = trainerParameters.LearningRateChange;
            //trainer.Epochs = trainerParameters.Epochs;
            rawTrainer.TrainerStatus = TrainerStatus.Initialized;    // DIC?

            return rawTrainer;
        }

        #endregion
    }
}
