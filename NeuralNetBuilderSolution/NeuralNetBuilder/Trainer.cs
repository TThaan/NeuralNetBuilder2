using CustomLogger;
using DeepLearningDataProvider;
using MatrixExtensions;
using NeuralNetBuilder.FactoriesAndParameters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
        int Epochs { get; }
        int CurrentEpoch { get; set; }
        int CurrentSample { get; set; }
        float LearningRate { get; set; }
        float LearningRateChange { get; }
        float LastEpochsAccuracy { get; set; }
        float CurrentTotalCost { get; set; }        
        TrainerStatus TrainerStatus { get; set; }
        public string Message { get; set; }
        Task TrainAsync(bool shuffleSamplesBeforeTraining, string logName);
        Task TestAsync(Sample[] testingSamples, ILogger logger = default);
        Task Reset();
        event TrainerStatusChangedEventHandler TrainerStatusChanged;
    }

    public class Trainer : ITrainer, INotifyPropertyChanged 
    {
        #region fields

        private readonly ITrainerParameters _parameters;
        ILearningNet learningNet;
        INet originalNet, trainedNet;

        ISampleSet _sampleSet;
        List<Sample> arrangedTrainSet = new List<Sample>();  // in SampleSet?
        IEnumerable<IGrouping<string, Sample>> groupedSamples;  // in SampleSet?
        Dictionary<string, NullableIntArray> groupedAndRandomizedIndeces, multipliedGroupedAndRandomizedIndeces;
        Dictionary<string, int> appendedSamplesPerLabel = new Dictionary<string, int>();

        int samplesTotal, epochs, currentEpoch = 0, currentSample = 0;
        float learningRate, learningRateChange, lastEpochsAccuracy, currentTotalCost;
        TrainerStatus trainerStatus;
        string message;
        Random rnd;

        #endregion

        #region public

        public Trainer(ITrainerParameters parameters)
        {
            _parameters = parameters ?? throw new NullReferenceException(
                $"{typeof(Trainer)}.ctor: Parameter {nameof(parameters)}.");

            if (_parameters.Epochs == 0 || _parameters.LearningRate == 0)
                throw new ArgumentException($"{typeof(Trainer)}.ctor: " +
                    $"Parameters 'Epoch' and 'LearningRate' must be greater than 0.");

            LearningRate = _parameters.LearningRate;
            TrainerStatus = TrainerStatus.Initialized;    // DIC?
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
        public int Epochs => _parameters.Epochs;
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
        public float LearningRateChange => _parameters.LearningRateChange;
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
        public TrainerStatus TrainerStatus   // redundant?
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

        public async Task TrainAsync(bool shuffleSamplesBeforeTraining, string logName)
        {
            LearningNet = NetFactory.GetLearningNet(originalNet, _parameters.CostType); // CostType as Trainer prop?
            TrainerStatus = TrainerStatus.Running;
            Message = "Training";

            using (ILogger logger = string.IsNullOrEmpty(logName)
                ? null
                : new Logger(logName))
            {
                LogNet(logger);

                for (currentEpoch = CurrentEpoch; currentEpoch < Epochs; CurrentEpoch++)
                {
                    await ArrangeSamplesAsync(shuffleSamplesBeforeTraining);    // Better at start of epochs loop (and remove from TestASync end)!?
                    
                    for (currentSample = CurrentSample; currentSample < samplesTotal; CurrentSample++)
                    {
                        await learningNet.FeedForwardAsync(arrangedTrainSet[currentSample].Features);   // _sampleSet.TrainSet
                        LogFeedForward(currentSample, logger);

                        await learningNet.PropagateBackAsync(_sampleSet.Targets[arrangedTrainSet[currentSample].Label]);    // _sampleSet.TrainSet
                        CurrentTotalCost = learningNet.CurrentTotalCost;
                        LogBackProp(currentSample, logger);

                        await learningNet.AdjustWeightsAndBiasesAsync(LearningRate);
                        LogNet(logger);

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

        private async Task FinalizeEpoch(ILogger logger)
        {
            TrainedNet = LearningNet.GetNet();

            if (currentSample == SamplesTotal)
            {
                LearningRate *= LearningRateChange;
                await TestAsync(_sampleSet.TestSet, logger);
                currentSample = 0;
                await _sampleSet.TrainSet.ShuffleAsync();
            }

            if (TrainerStatus != TrainerStatus.Paused)
                OnTrainerStatusChanged($"Epoch {currentEpoch} finished. (Accuracy: {lastEpochsAccuracy})");
        }
        public async Task TestAsync(Sample[] testSet, ILogger logger)
        {
            await Task.Run(async () =>
            {
                decimal injSetFraction = .5m;   // make dynamic
                int setLength = testSet.Length;
                int totalUnrecognizedSamples = 0;

                Dictionary<string, int> unrecognizedSamplesPerLabel = new Dictionary<string, int>();
                foreach (var label in SampleSet.Targets.Keys)
                    unrecognizedSamplesPerLabel[label] = 0;

                for (int i = 0;  i < testSet.Length; i++)
                {
                    Sample sample = testSet[i];
                    await LearningNet.FeedForwardAsync(sample.Features);                    
                    bool isOutputCorrect = CheckPredictionOfSingleSample(sample, out string predictedLabel);

                    if(!isOutputCorrect)
                    {
                        unrecognizedSamplesPerLabel[sample.Label] += 1;
                        totalUnrecognizedSamples++;
                    }

                    LogTesting(i, isOutputCorrect, predictedLabel, logger);
                }

                LastEpochsAccuracy = (float)(setLength - totalUnrecognizedSamples) / setLength;

                foreach (var item in unrecognizedSamplesPerLabel)
                {
                    decimal fractionOfAllUnrecognizedSamples = totalUnrecognizedSamples == 0
                    ? 0
                    : item.Value / totalUnrecognizedSamples;
                    appendedSamplesPerLabel[item.Key] = (int)(samplesTotal * injSetFraction * fractionOfAllUnrecognizedSamples);
                }
            });
        }
        public bool CheckPredictionOfSingleSample(Sample sample, out string prediction)
        {
            int targetHotIndex = Array.IndexOf(SampleSet.Targets[sample.Label], 1);
            int predictedHotIndex = Array.IndexOf(LearningNet.Output, LearningNet.Output.GetMaximum());
            prediction = SampleSet.Targets.First(x => x.Value[predictedHotIndex] == 1).Key;
            return targetHotIndex == predictedHotIndex;
        }
        public async Task Reset()
        {
            await Task.Run(() =>
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
                TrainerStatus = TrainerStatus.Undefined;
            });
        }

        #region helpers

        private async Task ArrangeSamplesAsync(bool shuffleSamples, bool equalizeGroupSizes = true)
        {
            await Task.Run(() =>
            {
                if (groupedSamples == null)
                    groupedSamples = _sampleSet.TrainSet.GroupBy(x => x.Label);

                groupedAndRandomizedIndeces = groupedSamples
                    .ToDictionary(group => group.Key, group => new NullableIntArray(
                        GetRandomIndeces(group, equalizeGroupSizes),
                        appendedSamplesPerLabel.Keys.Contains(group.Key) ? appendedSamplesPerLabel[group.Key] : 0));
                SetArrangedTrainSet(groupedAndRandomizedIndeces);

                SamplesTotal = arrangedTrainSet.Count;  // in SampleSet? // changes after injection (if not put in first if clause)?
            });
        }
        private IEnumerable<int?> GetRandomIndeces(IGrouping<string, Sample> group, bool equalizeGroupSizes)
        {
            int minGroupLength = groupedSamples.Min(x => x.Count()),
                groupLength = group.Count(),
                intendedGroupLength = equalizeGroupSizes ? minGroupLength : groupLength;

            var result = Enumerable.Cast<int?>(
                Enumerable.Range(0, groupLength))
                .Shuffle()
                .Take(intendedGroupLength);

            return result;
        }
        private void SetArrangedTrainSet(Dictionary<string, NullableIntArray> dict)   // , bool shuffleSamples
        {
            arrangedTrainSet.Clear();

            int lengthOfBiggestGroup = dict.Values.Max(x => x.Length);

            for (int i = 0; i < lengthOfBiggestGroup; i++)
            {
                foreach (var group in dict)
                {
                    if (i == group.Value.Length)
                        dict.Remove(group.Key);
                    else
                        // arrangedTrainSet of indeces only??
                        arrangedTrainSet.Add(groupedSamples.First(x => x.Key == group.Key).ElementAt((int)group.Value.NextItem));
                }
            }
        }

        #endregion

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
                    // Free managed resources.

                    
                }
                // Free unmanaged resources.


                isDisposed = true;
            }
        }

        #endregion
    }
}