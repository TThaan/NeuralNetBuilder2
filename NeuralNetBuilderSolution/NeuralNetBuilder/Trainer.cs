using CustomLogger;
using DeepLearningDataProvider;
using DeepLearningDataProvider.SampleSetHelpers;
using MatrixExtensions;
using NeuralNetBuilder.FactoriesAndParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeuralNetBuilder
{
    public interface ITrainer : INotificationChanged, IDisposable
    {
        INet OriginalNet { get; set; }
        ILearningNet LearningNet { get; set; }
        INet TrainedNet { get; set; }
        ISampleSet SampleSet { get; set; }
        int SamplesTotal { get; set; }
        int Epochs { get; set; }
        int CurrentEpoch { get; set; }
        int CurrentSample { get; set; }
        float LearningRate { get; set; }
        float LearningRateChange { get; set; }
        float LastEpochsAccuracy { get; set; }
        float CurrentTotalCost { get; set; }        
        TrainerStatus Status { get; set; }
        Task TrainAsync(bool shuffleSamplesBeforeTraining, string logName);
        Task TestAsync(Sample[] testingSamples, ILogger logger = default);

        CostType CostType { get; set; }

        void Initialize(ITrainerParameters trainerParameters, INet net, ISampleSet sampleSet);
        void Reset();
        bool IsInitialized { get; }
    }

    public class Trainer : NotificationChangedBase, ITrainer 
    {
        #region fields

        ILearningNet learningNet;
        INet originalNet, trainedNet;
        ISampleSet _sampleSet;

        Dictionary<string, int> lastTestResult = new Dictionary<string, int>();
        int samplesTotal, epochs, currentEpoch = 0, currentSample = 0;
        float learningRateChange, currentLearningRate, lastEpochsAccuracy, currentTotalCost;
        CostType costType;
        TrainerStatus trainerStatus;
        // Random rnd;

        #endregion

        #region public

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

        public INet OriginalNet
        {
            get { return originalNet; }
            set
            {
                if (originalNet != value)
                {
                    originalNet = value;
                    OnPropertyChanged();
                }
            }
        }
        public ILearningNet LearningNet
        {
            get { return learningNet; }
            set
            {
                if (learningNet != value)
                {
                    learningNet = value;
                    OnPropertyChanged();
                }
            }
        }
        public INet TrainedNet
        {
            get { return trainedNet; }
            set
            {
                if (trainedNet != value)
                {
                    trainedNet = value;
                    OnPropertyChanged();
                }
            }
        }
        public ISampleSet SampleSet
        {
            get { return _sampleSet; }
            set
            {
                if (_sampleSet != value)
                {
                    _sampleSet = value;
                    OnPropertyChanged();
                }
            }
        }
        public int SamplesTotal
        {
            get { return samplesTotal; }
            set
            {
                if (samplesTotal != value)
                {
                    samplesTotal = value;
                    OnPropertyChanged();
                }
            }
        }
        public int CurrentEpoch
        {
            get { return currentEpoch; }
            set
            {
                if (currentEpoch != value)
                {
                    currentEpoch = value;
                    OnPropertyChanged();
                }
            }
        }
        public int CurrentSample
        {
            get { return currentSample; }
            set
            {
                if (currentSample != value)
                {
                    currentSample = value;
                    OnPropertyChanged();
                }
            }
        }
        public float LearningRate
        {
            get { return currentLearningRate; }
            set
            {
                if (currentLearningRate != value)
                {
                    currentLearningRate = value;
                    OnPropertyChanged();
                }
            }
        }
        public float LastEpochsAccuracy
        {
            get { return lastEpochsAccuracy; }
            set
            {
                if (lastEpochsAccuracy != value)
                {
                    lastEpochsAccuracy = value;
                    OnPropertyChanged();
                }
            }
        }
        public float CurrentTotalCost
        {
            get { return currentTotalCost; }
            set
            {
                if (currentTotalCost != value)
                {
                    currentTotalCost = value;
                    OnPropertyChanged();
                }
            }
        }
        public TrainerStatus Status
        {
            get { return trainerStatus; }
            set
            {
                if (trainerStatus != value)
                {
                    trainerStatus = value;
                    OnPropertyChanged();
                }
            }
        }

        public async Task TrainAsync(bool shuffleSamplesBeforeTraining, string logName) // Remove logName as parameter and use a field/prop!?
        {
            // Status = TrainerStatus.Running;
            Notification = "\n            Training, please wait...\n";

            // LearningNet = NetFactory.GetLearningNet(originalNet, CostType);

            using (ILogger logger = string.IsNullOrEmpty(logName)
                ? null
                : new Logger(logName))
            {
                LogNet(logger);

                for (currentEpoch = CurrentEpoch; currentEpoch < Epochs; CurrentEpoch++)
                {
                    if (CurrentSample == 0)
                    {
                        _sampleSet.ArrangeSamplesAsync(shuffleSamplesBeforeTraining, lastTestResult);
                        SamplesTotal = _sampleSet.ArrangedTrainSet.Count;
                    }

                    for (currentSample = CurrentSample; currentSample < samplesTotal; CurrentSample++)
                    {
                        await learningNet.FeedForwardAsync(_sampleSet.ArrangedTrainSet[currentSample].Features);   // _sampleSet.TrainSet
                        LogFeedForward(currentSample, logger);

                        await learningNet.PropagateBackAsync(_sampleSet.Targets[_sampleSet.ArrangedTrainSet[currentSample].Label]);    // _sampleSet.TrainSet
                        CurrentTotalCost = learningNet.CurrentTotalCost;
                        LogBackProp(currentSample, logger);

                        await learningNet.AdjustWeightsAndBiasesAsync(LearningRate);
                        LogNet(logger);

                        if (Status == TrainerStatus.Paused)
                        {
                            await FinalizeEpoch(logger);
                            return;
                        }
                        // Only in debugging or optional?!
                        // else await FinalizeEpoch(logger);
                    }

                    await FinalizeEpoch(logger);
                }
            }

            Status = TrainerStatus.Finished;
            Notification = "\n            Finished training.\n";
        }

        private async Task FinalizeEpoch(ILogger logger)
        {
            await Task.Run(() =>
            {
                TrainedNet = (Net)(LearningNet as LearningNet);
            });

            if (currentSample == SamplesTotal)
            {
                LearningRate *= LearningRateChange;
                await TestAsync(_sampleSet.TestSet, logger);
                CurrentSample = 0;
                await _sampleSet.TrainSet.ShuffleAsync();
            }

            if (Status == TrainerStatus.Paused)
            {
                Notification = "Training Paused";
                CurrentSample++;
                CurrentEpoch = currentSample == samplesTotal ? currentEpoch + 1 : currentEpoch;
            }
            else if(Status == TrainerStatus.Running)
                Notification = $"Epoch {currentEpoch} finished. (Accuracy: {lastEpochsAccuracy})";
        }
        public async Task TestAsync(Sample[] testSet, ILogger logger)
        {
            await Task.Run(async () =>
            {
                int setLength = testSet.Length;
                int totalUnrecognizedSamples = 0;

                foreach (var label in SampleSet.Targets.Keys)
                    lastTestResult[label] = 0;

                for (int i = 0; i < testSet.Length; i++)
                {
                    Sample sample = testSet[i];
                    await LearningNet.FeedForwardAsync(sample.Features);
                    bool isOutputCorrect = CheckPredictionOfSingleSample(sample, out string predictedLabel);

                    if (!isOutputCorrect)
                    {
                        lastTestResult[sample.Label] += 1;
                        totalUnrecognizedSamples++;
                    }

                    LogTesting(i, isOutputCorrect, predictedLabel, logger);
                }

                LastEpochsAccuracy = (float)(setLength - totalUnrecognizedSamples) / setLength;
            });
        }
        public bool CheckPredictionOfSingleSample(Sample sample, out string prediction)
        {
            int targetHotIndex = Array.IndexOf(SampleSet.Targets[sample.Label], 1);
            int predictedHotIndex = Array.IndexOf(LearningNet.Output, LearningNet.Output.GetMaximum());
            prediction = SampleSet.Targets.First(x => x.Value[predictedHotIndex] == 1).Key;
            return targetHotIndex == predictedHotIndex;
        }

        #region Logging

        private void LogFeedForward(int sampleNr, ILogger logger)
        {
            if (logger == null) return;

            logger?.Log($"\nFeed Forward (Sample: {sampleNr}/{samplesTotal})\n");

            for (int i = 0; i < LearningNet.Layers.Length; i++)
            {
                logger?.Log(LearningNet.Layers[i], "\n", Details.Little);
            }
        }
        private void LogBackProp(int sampleNr, ILogger logger)
        {
            if (logger == null) return;

            logger?.Log($"\nBack Propagation (Sample: {sampleNr}/{samplesTotal})\n");
            logger?.Log($"Label: {_sampleSet.TrainSet[sampleNr].Label}, Target:\n{_sampleSet.Targets[_sampleSet.TrainSet[sampleNr].Label].ToLog()}");

            int layersCount = LearningNet.Layers.Length;
            for (int i = layersCount - 1; i > 0; i--)
            {
                logger?.Log(LearningNet.Layers[i], "\n", Details.All);
            }
        }
        private void LogNet(ILogger logger)
        {
            if (logger == null) return;

            logger?.Log(learningNet, $"\n              + Current Net +\n\n", Details.Medium);
        }
        // Unused?
        private void LogTraining(int sampleNr, ILogger logger)
        {
            if (logger == null) return;

            if (sampleNr == 0)
            {
                if (currentEpoch == 0)
                {
                    logger?.Log(OriginalNet, $"\n              + Original Neural Net +\n\n");
                }
            }
            logger?.Log(LearningNet, $"\n              + Feed Forward & Backpropagation +\n" +
                $"                (Sample: {sampleNr}/{samplesTotal})\n\n");

            logger?.Log($"\n              + New Weights and Biases: +\n\n");
            foreach (var layer in learningNet.Layers)
            {
                logger?.Log(layer.Weights.ToLog(nameof(layer.Weights)));
                if (layer.Biases != null) logger?.Log(layer.Biases.ToLog(nameof(layer.Biases)));
            }
        }
        private void LogTesting(int sampleNr, bool isOutputCorrect, string predictedLabel, ILogger logger)
        {
            if (logger == null) return;

            if (sampleNr == 0)
            {
                logger?.Log("\n                        * * * T E S T I N G * * *\n\n");
            }

            logger?.Log($"Label     : {_sampleSet.TestSet[sampleNr].Label}");
            logger?.Log($"\nPrediction: {_sampleSet.TestSet[sampleNr].Label}");
            logger?.Log($"\nTestResult: {(isOutputCorrect ? "Correct" : "Wrong")}\n\n");
            logger?.Log(_sampleSet.TestSet[sampleNr].Features.ToLog("Features"));
            logger?.Log(_sampleSet.Targets[_sampleSet.TestSet[sampleNr].Label].ToLog("\nTarget"));
            logger?.Log(learningNet.Output.ToLog(nameof(learningNet.Output)));
            // Task: Show value ('probability')!

            if (sampleNr == _sampleSet.TestSet.Length - 1)
                logger?.Log($"CurrentAccuracy: {LastEpochsAccuracy})\n\n");
        }

        #endregion

        #endregion

        #region Initialization

        public void Initialize(ITrainerParameters trainerParameters, INet net, ISampleSet sampleSet)
        {
            if (!net.IsInitialized)
                throw new ArgumentException($"Net must be initialized to initialize the trainer.");
            if (!sampleSet.IsInitialized)
                throw new ArgumentException($"SampleSet must be initialized to initialize the trainer.");

            Epochs = trainerParameters.Epochs;
            LearningRate = trainerParameters.LearningRate;
            LearningRateChange = trainerParameters.LearningRateChange;
            CostType = trainerParameters.CostType;
            OriginalNet = net.GetCopy();
            SampleSet = sampleSet;
            SamplesTotal = SampleSet.Samples.Length;
            LearningNet = net.GetLearningNet(CostType);

            IsInitialized = true;
        }
        public void Reset()
        {
            // OriginalNet = null;  // Can be kept?
            LearningNet = null;
            TrainedNet = null;
            // SampleSet = null;    // Can be kept?
            //Epochs = 0;
            currentEpoch = 0;
            lastEpochsAccuracy = 0;
            currentSample = 0;
            LearningRate = 0;
            //LearningRateChange = 0;
            Status = TrainerStatus.Undefined;
        }
        public bool IsInitialized { get; private set; }

        #endregion

        #region IDisposable

        private bool isDisposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    // Free managed resources.

                    
                }
                // Free unmanaged resources.


                isDisposed = true;
            }
        }

        #endregion
    }
}