namespace NeuralNetBuilder
{
    public enum LayerType
    {
        Layer, LearningLayer
    }
    public enum ActivationType
    {
        None,
        LeakyReLU, NullActivator,
        ReLU, Sigmoid, SoftMax, SoftMaxWithCrossEntropyLoss, Tanh
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
    public enum PresetValue
    {
        undefined,
        shuffle,    // Makes the trainer shuffle the training samples before the first training
        append,     // Appends a neuronal layer to the net automatically designed to fit the labels/targets of the sample set.
        indented,   // Tells the Json serializer to save with parameter Formatting.Indented.
        no
    }
}