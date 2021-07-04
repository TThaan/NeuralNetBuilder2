using System;
using System.IO;

namespace NeuralNetBuilder.Builders
{
    public class PathBuilder
    {
        #region fields & ctor

        private readonly Action<string> _onInitializerStatusChanged;
        string netParameters, trainerParameters, log, sampleSetParameters, sampleSet, initializedNet, trainedNet;

        public PathBuilder(Action<string> onInitializerStatusChanged)
        {
            _onInitializerStatusChanged = onInitializerStatusChanged;
        }

        #endregion

        #region properties

        public string FileName_InitializedNet { get; set; } = "InitializedNet";
        public string FileName_TrainedNet { get; set; } = "TrainedNet";
        public string FileName_SampleSet { get; set; } = "SampleSet";
        public string FileName_SampleSetParameters { get; set; } = "SampleSetParameters";
        public string FileName_NetParameters { get; set; } = "NetParameters";
        public string FileName_TrainerParameters { get; set; } = "TrainerParameters";
        public string FileName_Log { get; set; } = "Log";
        public string FileName_Prefix { get; set; } = string.Empty;
        public string FileName_Suffix { get; set; } = ".txt";

        public string General { get; set; } = Path.GetTempPath();   // @"C:\Users\Jan_PC\Documents\_NeuralNetApp\Saves\";
        public string NetParameters
        {
            get
            {
                if (string.IsNullOrEmpty(netParameters))
                    return netParameters = Path.Combine(General, FileName_Prefix, FileName_NetParameters, FileName_Suffix);
                else return netParameters;
            }
            set { netParameters = value; }
        }
        public string TrainerParameters
        {
            get
            {
                if (string.IsNullOrEmpty(trainerParameters))
                    return trainerParameters = Path.Combine(General + FileName_Prefix + FileName_TrainerParameters + FileName_Suffix);
                else return trainerParameters;
            }
            set { trainerParameters = value; }
        }
        public string Log
        {
            get
            {
                if (string.IsNullOrEmpty(log))
                    return log = Path.Combine(General, FileName_Prefix, FileName_Log, FileName_Suffix);
                else return log;
            }
            set { log = value; }
        }
        public string SampleSetParameters
        {
            get
            {
                if (string.IsNullOrEmpty(sampleSetParameters))
                    return sampleSetParameters = Path.Combine(General, FileName_Prefix, FileName_SampleSetParameters, FileName_Suffix);
                else return sampleSetParameters;
            }
            set { sampleSetParameters = value; }
        }
        public string SampleSet
        {
            get
            {
                if (string.IsNullOrEmpty(sampleSet))
                    return sampleSet = Path.Combine(General, FileName_Prefix, FileName_SampleSet, FileName_Suffix);
                else return sampleSet;
            }
            set { sampleSet = value; }
        }
        public string InitializedNet
        {
            get
            {
                if (string.IsNullOrEmpty(initializedNet))
                    return initializedNet = Path.Combine(General, FileName_Prefix, FileName_InitializedNet, FileName_Suffix);
                else return initializedNet;
            }
            set { initializedNet = value; }
        }
        public string TrainedNet
        {
            get
            {
                if (string.IsNullOrEmpty(trainedNet))
                    return trainedNet = Path.Combine(General, FileName_Prefix, FileName_TrainedNet, FileName_Suffix);
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
                _onInitializerStatusChanged("Path not found!");
                return false;
            }

            General = path;
            _onInitializerStatusChanged("General path is set.");
            UseGeneralPathAndDefaultNames();    // no default names here?
            return true;
        }
        public void SetFileNamePrefix(string prefix)
        {
            FileName_Prefix = prefix;
            _onInitializerStatusChanged($"The file name has prefix {prefix} now.");
        }
        public void SetFileNameSuffix(string suffix)
        {
            FileName_Suffix = suffix;
            _onInitializerStatusChanged($"The file name has suffix {suffix} now.");
        }
        public void ResetPaths()
        {
            General = Path.GetTempPath();

            netParameters = string.Empty;
            trainerParameters = string.Empty;
            log = string.Empty;
            sampleSetParameters = string.Empty;
            sampleSet = string.Empty;
            initializedNet = string.Empty;
            trainedNet = string.Empty;

            _onInitializerStatusChanged($"Path for all files has been reset.");
        }
        public void UseGeneralPathAndDefaultNames()
        {
            SetNetParametersPath(Path.Combine(General, FileName_Prefix, FileName_NetParameters, FileName_Suffix));
            SetTrainerParametersPath(Path.Combine(General, FileName_Prefix, FileName_TrainerParameters, FileName_Suffix));
            SetLogPath(Path.Combine(General, FileName_Prefix, FileName_Log, FileName_Suffix));
            SetSampleSetParametersPath(Path.Combine(General, FileName_Prefix, FileName_SampleSetParameters, FileName_Suffix));
            SetSampleSetPath(Path.Combine(General, FileName_Prefix, FileName_SampleSet, FileName_Suffix));
            SetInitializedNetPath(Path.Combine(General, FileName_Prefix, FileName_InitializedNet, FileName_Suffix));
            SetTrainedNetPath(Path.Combine(General, FileName_Prefix, FileName_TrainedNet, FileName_Suffix));
        }

        #region redundant?

        public void SetInitializedNetPath(string path)
        {
            InitializedNet = path;
            _onInitializerStatusChanged("Path to the initialized net has been set.");
        }
        public void SetTrainedNetPath(string path)
        {
            TrainedNet = path;
            _onInitializerStatusChanged("Path to the trained net has been set.");
        }
        public void SetSampleSetPath(string path)
        {
            SampleSet = path;
            _onInitializerStatusChanged("Path to the sample set has been set.");
        }
        public void SetNetParametersPath(string path)
        {
            NetParameters = path;
            _onInitializerStatusChanged("Path to net parameters has been set.");
        }
        public void SetTrainerParametersPath(string path)
        {
            TrainerParameters = path;
            _onInitializerStatusChanged("Path to trainer parameters has been set.");
        }
        public void SetSampleSetParametersPath(string path)
        {
            SampleSetParameters = path;
            _onInitializerStatusChanged("Path to parameters for the sample set has been set.");
        }
        public void SetLogPath(string path)
        {
            Log = path;
            _onInitializerStatusChanged("Path to the log file has been set.");
        }

        #endregion

        #endregion
    }
}
