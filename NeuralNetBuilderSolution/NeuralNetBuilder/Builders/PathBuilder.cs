using System;
using System.IO;

namespace NeuralNetBuilder.Builders
{
    // Builders provide methods to interact with the data classes (all pocos?).
    // You can access them from the ConsoleApi, AIDemoUI or use them as Wpf's 'Command-Executes'.
    // They already do or will (soon) provide an event to notify about the (succeeded) data changes.

    public class PathBuilder : InitializerAssistant
    {
        #region fields & ctor

        private string netParameters, trainerParameters, log, initializedNet, sampleSet, trainedNet;

        #endregion

        #region properties

        public string BasicName_InitializedNet { get; set; } = "InitializedNet.txt";
        public string BasicName_TrainedNet { get; set; } = "TrainedNet.txt";
        public string BasicName_SampleSet { get; set; } = "Samples.csv";
        public string BasicName_NetParameters { get; set; } = "NetParameters.txt";
        public string BasicName_TrainerParameters { get; set; } = "TrainerParameters.txt";
        public string BasicName_Log { get; set; } = "Log.txt";
        public string BasicName_Prefix { get; set; } = string.Empty;
        public string BasicName_Suffix { get; set; } = string.Empty;

        public string General { get; set; } = @"C:\Users\Jan_PC\Documents\_NeuralNetApp\Saves\";    // Path.GetTempPath();
        public string NetParameters
        {
            get
            {
                if (string.IsNullOrEmpty(netParameters))
                    return netParameters = Path.Combine(General, BasicName_Prefix, BasicName_NetParameters + BasicName_Suffix);
                else return netParameters;
            }
            set { netParameters = value; }
        }
        public string TrainerParameters
        {
            get
            {
                if (string.IsNullOrEmpty(trainerParameters))
                    return trainerParameters = Path.Combine(General, BasicName_Prefix, BasicName_TrainerParameters + BasicName_Suffix);
                else return trainerParameters;
            }
            set { trainerParameters = value; }
        }
        public string Log
        {
            get
            {
                if (string.IsNullOrEmpty(log))
                    return log = Path.Combine(General, BasicName_Prefix, BasicName_Log + BasicName_Suffix);
                else return log;
            }
            set { log = value; }
        }
        public string SampleSet
        {
            get
            {
                if (string.IsNullOrEmpty(sampleSet))
                    return sampleSet = Path.Combine(General, BasicName_Prefix, BasicName_SampleSet + BasicName_Suffix);
                else return sampleSet;
            }
            set { sampleSet = value; }
        }
        public string InitializedNet
        {
            get
            {
                if (string.IsNullOrEmpty(initializedNet))
                    return initializedNet = Path.Combine(General, BasicName_Prefix, BasicName_InitializedNet + BasicName_Suffix);
                else return initializedNet;
            }
            set { initializedNet = value; }
        }
        public string TrainedNet
        {
            get
            {
                if (string.IsNullOrEmpty(trainedNet))
                    return trainedNet = Path.Combine(General, BasicName_Prefix, BasicName_TrainedNet + BasicName_Suffix);
                else return trainedNet;
            }
            set { trainedNet = value; }
        }

        #endregion

        #region methods

        public bool SetGeneralPath(string path)
        {
            if (!Directory.Exists(path))
            {
                OnStatusChanged("Path not found!");
                return false;
            }

            General = path;
            OnStatusChanged("General path is set.");
            UseGeneralPathAndDefaultNames();    // no default names here?
            return true;
        }
        public void SetFileNamePrefix(string prefix)
        {
            BasicName_Prefix = prefix;
            OnStatusChanged($"The file name has prefix {prefix} now.");
        }
        public void SetFileNameSuffix(string suffix)
        {
            BasicName_Suffix = suffix;
            OnStatusChanged($"The file name has suffix {suffix} now.");
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

            OnStatusChanged($"Path for all files has been reset.");
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

        #region redundant?

        public void SetInitializedNetPath(string path)
        {
            InitializedNet = path;
            OnStatusChanged("Path to the initialized net has been set.");
        }
        public void SetTrainedNetPath(string path)
        {
            TrainedNet = path;
            OnStatusChanged("Path to the trained net has been set.");
        }
        public void SetSampleSetPath(string path)
        {
            SampleSet = path;
            OnStatusChanged("Path to the sample set has been set.");
        }
        public void SetNetParametersPath(string path)
        {
            NetParameters = path;
            OnStatusChanged("Path to net parameters has been set.");
        }
        public void SetTrainerParametersPath(string path)
        {
            TrainerParameters = path;
            OnStatusChanged("Path to trainer parameters has been set.");
        }
        public void SetLogPath(string path)
        {
            Log = path;
            OnStatusChanged("Path to the log file has been set.");
        }

        #endregion

        #endregion
    }
}
