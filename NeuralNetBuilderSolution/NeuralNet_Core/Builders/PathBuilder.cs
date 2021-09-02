using System;
using System.IO;

namespace NeuralNet_Core.Builders
{
    // Builders provide methods to create/load and interact with the data classes.
    // (I.e. they're a bit more than a factory.)
    // You can access them from the ConsoleApi, AIDemoUI or use them as Wpf's 'Command-Executes'.
    // They provide a Status property incl an event to notify about the (succeeded) data changes.
    // Failed attempts throwing an exception can be caught in the client.

    public class PathBuilder : NotificationChangedBase
    {
        #region fields & ctor

        private string netParameters, trainerParameters, log, initializedNet, sampleSet, trainedNet,
            basicName_InitializedNet = "InitializedNet.txt",
            basicName_TrainedNet = "TrainedNet.txt",
            basicName_SampleSet = "Samples.csv",
            basicName_NetParameters = "NetParameters.txt",
            basicName_TrainerParameters = "TrainerParameters.txt",
            basicName_Log = "Log.txt",
            basicName_Prefix = string.Empty,
            basicName_Suffix = string.Empty,
            general = @"C:\Users\Jan_PC\Documents\_NeuralNetApp\Saves\";

        #endregion

        #region properties

        // No INPC for basic names needed?
        public string BasicName_InitializedNet
        {
            get { return basicName_InitializedNet; }
            set
            {
                if (basicName_InitializedNet != value)
                {
                    basicName_InitializedNet = value;
                    OnPropertyChanged();
                }
            }
        }
        public string BasicName_TrainedNet
        {
            get { return basicName_TrainedNet; }
            set
            {
                if (basicName_TrainedNet != value)
                {
                    basicName_TrainedNet = value;
                    OnPropertyChanged();
                }
            }
        }
        public string BasicName_SampleSet
        {
            get { return basicName_SampleSet; }
            set
            {
                if (basicName_SampleSet != value)
                {
                    basicName_SampleSet = value;
                    OnPropertyChanged();
                }
            }
        }
        public string BasicName_NetParameters
        {
            get { return basicName_NetParameters; }
            set
            {
                if (basicName_NetParameters != value)
                {
                    basicName_NetParameters = value;
                    OnPropertyChanged();
                }
            }
        }
        public string BasicName_TrainerParameters
        {
            get { return basicName_TrainerParameters; }
            set
            {
                if (basicName_TrainerParameters != value)
                {
                    basicName_TrainerParameters = value;
                    OnPropertyChanged();
                }
            }
        }
        public string BasicName_Log
        {
            get { return basicName_Log; }
            set
            {
                if (basicName_Log != value)
                {
                    basicName_Log = value;
                    OnPropertyChanged();
                }
            }
        }
        public string BasicName_Prefix
        {
            get { return basicName_Prefix; }
            set
            {
                if (basicName_Prefix != value)
                {
                    basicName_Prefix = value;
                    OnPropertyChanged();
                }
            }
        }
        public string BasicName_Suffix
        {
            get { return basicName_Suffix; }
            set
            {
                if (basicName_Suffix != value)
                {
                    basicName_Suffix = value;
                    OnPropertyChanged();
                }
            }
        }

        // Only actual file names need INPC?
        public string General
        {
            get { return general; }
            set
            {
                if (general != value)
                {
                    general = value;
                    OnPropertyChanged();
                }
            }
        }
        public string NetParameters
        {
            get
            {
                if (string.IsNullOrEmpty(netParameters))
                    return netParameters = Path.Combine(General, BasicName_Prefix, BasicName_NetParameters + BasicName_Suffix);
                else return netParameters;
            }
            set 
            {
                if (netParameters != value)
                {
                    netParameters = value;
                    OnPropertyChanged();
                }
            }
        }
        public string TrainerParameters
        {
            get
            {
                if (string.IsNullOrEmpty(trainerParameters))
                    return trainerParameters = Path.Combine(General, BasicName_Prefix, BasicName_TrainerParameters + BasicName_Suffix);
                else return trainerParameters;
            }
            set 
            {
                if (trainerParameters != value)
                {
                    trainerParameters = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Log
        {
            get
            {
                if (string.IsNullOrEmpty(log))
                    return log = Path.Combine(General, BasicName_Prefix, BasicName_Log + BasicName_Suffix);
                else return log;
            }
            set 
            {
                if (log != value)
                {
                    log = value;
                    OnPropertyChanged();
                }
            }
        }
        public string SampleSet
        {
            get
            {
                if (string.IsNullOrEmpty(sampleSet))
                    return sampleSet = Path.Combine(General, BasicName_Prefix, BasicName_SampleSet + BasicName_Suffix);
                else return sampleSet;
            }
            set 
            {
                if (sampleSet != value)
                {
                    sampleSet = value;
                    OnPropertyChanged();
                }
            }
        }
        public string InitializedNet
        {
            get
            {
                if (string.IsNullOrEmpty(initializedNet))
                    return initializedNet = Path.Combine(General, BasicName_Prefix, BasicName_InitializedNet + BasicName_Suffix);
                else return initializedNet;
            }
            set 
            {
                if (initializedNet != value)
                {
                    initializedNet = value;
                    OnPropertyChanged();
                }
            }
        }
        public string TrainedNet
        {
            get
            {
                if (string.IsNullOrEmpty(trainedNet))
                    return trainedNet = Path.Combine(General, BasicName_Prefix, BasicName_TrainedNet + BasicName_Suffix);
                else return trainedNet;
            }
            set 
            {
                if (trainedNet != value)
                {
                    trainedNet = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region methods

        public void SetGeneralPath(string path)
        {
            if (!Directory.Exists(path))
                throw new ArgumentException($"Path {path} not found.");

            General = path;
            Notification = "General path is set.";
            UseGeneralPathAndDefaultNames();    // no default names here?
        }
        public void SetFileNamePrefix(string prefix)
        {
            BasicName_Prefix = prefix;
            Notification = $"The file name has prefix {prefix} now.";
        }
        public void SetFileNameSuffix(string suffix)
        {
            BasicName_Suffix = suffix;
            Notification = $"The file name has suffix {suffix} now.";
        }
        public void ResetPaths()
        {
            General = @"C:\Users\Jan_PC\Documents\_NeuralNetApp\Saves\";    // Path.GetTempPath();

            netParameters = string.Empty;
            trainerParameters = string.Empty;
            log = string.Empty;
            initializedNet = string.Empty;
            trainedNet = string.Empty;

            sampleSet = string.Empty;

            Notification = $"Path for all files has been reset.";
        }
        public void UseGeneralPathAndDefaultNames()
        {
            SetNetParametersPath(Path.Combine(General, BasicName_Prefix, BasicName_NetParameters + BasicName_Suffix));
            SetTrainerParametersPath(Path.Combine(General, BasicName_Prefix, BasicName_TrainerParameters + BasicName_Suffix));
            SetLogPath(Path.Combine(General, BasicName_Prefix, BasicName_Log + BasicName_Suffix));
            SetInitializedNetPath(Path.Combine(General, BasicName_Prefix, BasicName_InitializedNet + BasicName_Suffix));
            SetTrainedNetPath(Path.Combine(General, BasicName_Prefix, BasicName_TrainedNet + BasicName_Suffix));

            SetSampleSetPath(Path.Combine(General, BasicName_Prefix, BasicName_SampleSet + BasicName_Suffix));
        }

        #region redundant??

        public void SetInitializedNetPath(string path)
        {
            InitializedNet = path;
            Notification = $"Path to the initialized net has been set to\n{path}";
        }
        public void SetTrainedNetPath(string path)
        {
            TrainedNet = path;
            Notification = $"Path to the trained net has been set to\n{path}";
        }
        public void SetSampleSetPath(string path)
        {
            SampleSet = path;
            Notification = $"Path to the sample set has been set to\n{path}";
        }
        public void SetNetParametersPath(string path)
        {
            NetParameters = path;
            Notification = $"Path to net parameters has been set to\n{path}";
        }
        public void SetTrainerParametersPath(string path)
        {
            TrainerParameters = path;
            Notification = $"Path to trainer parameters has been set to\n{path}";
        }
        public void SetLogPath(string path)
        {
            Log = path;
            Notification = $"Path to the log file has been set to\n{path}";
        }

        #endregion

        #endregion
    }
}
