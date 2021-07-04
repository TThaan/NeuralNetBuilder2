using System;

namespace NeuralNetBuilder.Builders
{
    public class PathBuilder
    {
        #region fields & ctor

        private readonly Action<string> _onInitializerStatusChanged;

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
        public string FileName_Prefix { get; set; } = default;
        public string FileName_Suffix { get; set; } = ".txt";

        public string General { get; set; } = @"C:\Users\Jan_PC\Documents\_NeuralNetApp\Saves\";
        public string NetParameters { get; set; }
        public string TrainerParameters { get; set; }
        public string Log { get; set; }
        public string SampleSetParameters { get; set; }
        public string SampleSet { get; set; }
        public string InitializedNet { get; set; }
        public string TrainedNet { get; set; }

        #endregion

        #region methods

        public void SetGeneralPath(string path)
        {
            General = path;
            _onInitializerStatusChanged("General path is set.");
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
        public void SetAllPaths()
        {
            SetNetParametersPath(@$"{General}{FileName_Prefix}{FileName_NetParameters}{FileName_Suffix}");
            SetTrainerParametersPath(@$"{General}{FileName_Prefix}{FileName_TrainerParameters}{FileName_Suffix}");
            SetLogPath(@$"{General}{FileName_Prefix}{FileName_Log}{FileName_Suffix}");
            SetSampleSetParametersPath(@$"{General}{FileName_Prefix}{FileName_SampleSetParameters}{FileName_Suffix}");
            SetSampleSetPath(@$"{General}{FileName_Prefix}{FileName_SampleSet}{FileName_Suffix}");
            SetInitializedNetPath(@$"{General}{FileName_Prefix}{FileName_InitializedNet}{FileName_Suffix}");
            SetTrainedNetPath(@$"{General}{FileName_Prefix}{FileName_TrainedNet}{FileName_Suffix}");
        }

        #endregion
    }
}
