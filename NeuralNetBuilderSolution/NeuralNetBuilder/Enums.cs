namespace NeuralNetBuilder
{
    public enum LayerType
    {
        Layer, LearningLayer
    }
    public enum ActivationType
    {
        LeakyReLU, NullActivator,
        ReLU, Sigmoid, SoftMax, SoftMaxWithCrossEntropyLoss, Tanh,
        None
    }
    public enum CostType
    {
        SquaredMeanError
    }
    public enum WeightInitType
    {
        None, Xavier,
    }
    public enum NetStatus
    {
        Undefined, Initialized  // Raw instead of Undefined?
    }
    public enum TrainerStatus
    {
        Undefined, Initialized, Running, Paused, Finished   // Raw instead of Undefined?
    }
}