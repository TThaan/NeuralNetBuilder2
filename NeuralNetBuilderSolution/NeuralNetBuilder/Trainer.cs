using CustomLogger;
using DeepLearningDataProvider;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace NeuralNetBuilder
{
    public interface ITrainer : INotifyPropertyChanged, IDisposable
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
        TrainerStatus TrainerStatus { get; set; }
        public string Message { get; set; }
        Task Train(string logName, int epochs);
        Task TestAsync(Sample[] testingSamples, ILogger logger = default);
        Task Reset();
        event TrainerStatusChangedEventHandler TrainerStatusChanged;
    }

    public class Trainer : ITrainer, INotifyPropertyChanged 
    {
        #region fields
        
        ILearningNet learningNet;
        INet originalNet, trainedNet;
        ISampleSet sampleSet;
        int samplesTotal, epochs, currentEpoch = 0, currentSample = 0;
        float learningRate, learningRateChange, lastEpochsAccuracy, currentTotalCost;
        TrainerStatus trainerStatus;
        string message;

        #endregion

        #region public

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
            get { return sampleSet; }
            set
            {
                if (sampleSet != value)
                {
                    sampleSet = value;
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
        public TrainerStatus TrainerStatus
        {
            get { return trainerStatus; }
            set
            {
                if (trainerStatus != value)
                {
                    trainerStatus = value;
                    OnPropertyChanged();
                    OnTrainerStatusChanged($"TrainerStatus = {value}");
                }
            }
        }
        public string Message
        {
            get { return message; }
            set
            {
                if (message != value)
                {
                    message = value;
                    OnPropertyChanged();
                }
            }
        }

        public async Task Train(string logName, int epochs)
        {
            SamplesTotal = sampleSet.TrainingSamples.Length;
            Epochs = epochs;
            TrainerStatus = TrainerStatus.Running;
            Message = "Training";

            using (ILogger logger = string.IsNullOrEmpty(logName)
                ? null
                : new Logger(logName))
            {
                for (currentEpoch = CurrentEpoch; currentEpoch < Epochs; CurrentEpoch++)
                {
                    for (currentSample = CurrentSample; currentSample < samplesTotal; CurrentSample++)
                    {
                        await LearningNet.FeedForwardAsync(sampleSet.TrainingSamples[currentSample].Input);
                        await learningNet.PropagateBackAsync(sampleSet.TrainingSamples[currentSample].ExpectedOutput);
                        CurrentTotalCost = learningNet.CurrentTotalCost;
                        await learningNet.AdjustWeightsAndBiasesAsync(LearningRate);

                        LogTraining(currentSample, logger);

                        if (TrainerStatus == TrainerStatus.Paused)
                        {
                            Message = "Training Paused";
                            CurrentSample++;
                            CurrentEpoch = currentSample == samplesTotal ? currentEpoch + 1 : currentEpoch;
                            await FinalizeEpoch(logger);
                            return;
                        }
                    }

                    await FinalizeEpoch(logger);
                }
            }

            TrainerStatus = TrainerStatus.Finished;
            Message = "Training Finished";
        }
        public async Task TestAsync(Sample[] testingSamples, ILogger logger)
        {
            await Task.Run(async () =>
            {
                int correct = 0, wrong = 0;

                for (int i = 0;  i < testingSamples.Length; i++)
                {
                    await LearningNet.FeedForwardAsync(testingSamples[i].Input);
                    bool isOutputCorrect = testingSamples[i].IsOutputApproximatelyCorrect(learningNet.Output);
                    LastEpochsAccuracy = isOutputCorrect
                    ? (float)++correct / (correct + wrong)
                    : (float)correct / (correct + ++wrong);

                    LogTesting(i, isOutputCorrect, correct, wrong, logger);
                }
            });
        }
        public async Task Reset()
        {
            await Task.Run(() =>
            {
                // OriginalNet = null;  // Can be kept?
                LearningNet = null;
                TrainedNet = null;
                // SampleSet = null;    // Can be kept?
                Epochs = 0;
                currentEpoch = 0;
                lastEpochsAccuracy = 0;
                currentSample = 0;
                LearningRate = 0;
                LearningRateChange = 0;
                TrainerStatus = TrainerStatus.Undefined;
            });
        }

        #region helpers

        private async Task FinalizeEpoch(ILogger logger)
        {
            TrainedNet = LearningNet.GetNet();

            if(TrainerStatus != TrainerStatus.Paused)
                OnTrainerStatusChanged($"Epoch {currentEpoch} finished. (Accuracy: {lastEpochsAccuracy})");

            if (currentSample == 1000)
            {
                LearningRate *= LearningRateChange;
                await TestAsync(sampleSet.TestingSamples, logger);
                currentSample = 0;
                await sampleSet.TrainingSamples.ShuffleAsync();
            }
        }

        #endregion

        #region Logging

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
                logger?.Log(layer.Weights);
                if (layer.Biases != null) logger?.Log(layer.Biases);
            }
        }
        private void LogTesting(int sampleNr, bool isOutputCorrect, int correct, int wrong, ILogger logger)
        {
            if (logger == null) return;

            if (sampleNr == 0)
            {
                logger?.Log("\n                        * * * T E S T I N G * * *\n\n");
            }

            logger?.Log(sampleSet.TestingSamples[sampleNr].Input);
            logger?.Log(sampleSet.TestingSamples[sampleNr].ExpectedOutput);
            logger?.Log(learningNet.Output);
            logger?.Log($"\nTestResult: {(isOutputCorrect ? "Correct" : "Wrong")}\n\n");

            if (sampleNr == sampleSet.TestingSamples.Length - 1)
            {
                logger?.Log($"CurrentAccuracy: {(float)correct / (correct + wrong)})\n\n");
            }
        }

        #endregion

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;
        public event TrainerStatusChangedEventHandler TrainerStatusChanged;

        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        void OnTrainerStatusChanged(string info)
        {
            TrainerStatusChanged?.Invoke(this, new TrainerStatusChangedEventArgs(info));
        }

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
                    // Free managed resources if Dispose(true) has been called.
                    
                }
                // Free unmanaged resources.
                isDisposed = true;
            }
        }

        #endregion
    }
}
