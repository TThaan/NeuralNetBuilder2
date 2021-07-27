using System;
using System.IO;

namespace NeuralNetBuilder.Builders
{
    public class PathBuilder
    {
        #region fields & ctor

        private readonly Action<string> _onInitializerStatusChanged;
        private string netParameters, trainerParameters, log, initializedNet, sampleSet, trainedNet;//, sampleSetParameters

        public PathBuilder(Action<string> onInitializerStatusChanged)
        {
            _onInitializerStatusChanged = onInitializerStatusChanged;
        }

        #endregion

        #region properties

        public string FileName_InitializedNet { get; set; } = "InitializedNet.txt";
        public string FileName_TrainedNet { get; set; } = "TrainedNet.txt";
        public string FileName_SampleSet { get; set; } = "Samples.csv";
        public string FileName_NetParameters { get; set; } = "NetParameters.txt";
        public string FileName_TrainerParameters { get; set; } = "TrainerParameters.txt";
        public string FileName_Log { get; set; } = "Log.txt";
        public string FileName_Prefix { get; set; } = string.Empty;
        public string FileName_Suffix { get; set; } = string.Empty;

        public string General { get; set; } = @"C:\Users\Jan_PC\Documents\_NeuralNetApp\Saves\";    // Path.GetTempPath();
        public string NetParameters
        {
            get
            {
                if (string.IsNullOrEmpty(netParameters))
                    return netParameters = Path.Combine(General, FileName_Prefix, FileName_NetParameters + FileName_Suffix);
                else return netParameters;
            }
            set { netParameters = value; }
        }
        public string TrainerParameters
        {
            get
            {
                if (string.IsNullOrEmpty(trainerParameters))
                    return trainerParameters = Path.Combine(General, FileName_Prefix, FileName_TrainerParameters + FileName_Suffix);
                else return trainerParameters;
            }
            set { trainerParameters = value; }
        }
        public string Log
        {
            get
            {
                if (string.IsNullOrEmpty(log))
                    return log = Path.Combine(General, FileName_Prefix, FileName_Log + FileName_Suffix);
                else return log;
            }
            set { log = value; }
        }
        public string SampleSet
        {
            get
            {
                if (string.IsNullOrEmpty(sampleSet))
                    return sampleSet = Path.Combine(General, FileName_Prefix, FileName_SampleSet + FileName_Suffix);
                else return sampleSet;
            }
            set { sampleSet = value; }
        }
        public string InitializedNet
        {
            get
            {
                if (string.IsNullOrEmpty(initializedNet))
                    return initializedNet = Path.Combine(General, FileName_Prefix, FileName_InitializedNet + FileName_Suffix);
                else return initializedNet;
            }
            set { initializedNet = value; }
        }
        public string TrainedNet
        {
            get
            {
                if (string.IsNullOrEmpty(trainedNet))
                    return trainedNet = Path.Combine(General, FileName_Prefix, FileName_TrainedNet + FileName_Suffix);
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
            General = @"C:\Users\Jan_PC\Documents\_NeuralNetApp\Saves\";    // Path.GetTempPath();

            netParameters = string.Empty;
            trainerParameters = string.Empty;
            log = string.Empty;
            initializedNet = string.Empty;
            trainedNet = string.Empty;

            _onInitializerStatusChanged($"Path for all files has been reset.");
        }
        public void UseGeneralPathAndDefaultNames()
        {
            SetNetParametersPath(Path.Combine(General, FileName_Prefix, FileName_NetParameters + FileName_Suffix));
            SetTrainerParametersPath(Path.Combine(General, FileName_Prefix, FileName_TrainerParameters + FileName_Suffix));
            SetLogPath(Path.Combine(General, FileName_Prefix, FileName_Log + FileName_Suffix));
            SetInitializedNetPath(Path.Combine(General, FileName_Prefix, FileName_InitializedNet + FileName_Suffix));
            SetTrainedNetPath(Path.Combine(General, FileName_Prefix, FileName_TrainedNet + FileName_Suffix));
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
        public void SetLogPath(string path)
        {
            Log = path;
            _onInitializerStatusChanged("Path to the log file has been set.");
        }

        #endregion

        #endregion
    }
}
