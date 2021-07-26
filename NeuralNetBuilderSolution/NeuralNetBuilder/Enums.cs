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
    public enum MainCommand
    {
        Undefined,
        path, show, create, load, save, logon, logoff, train, test, param, layer
    }
    public enum ShowCommand
    {
        Undefined,
        help, settings, par, netpar, trainerpar,
    }
    public enum PathCommand
    {
        Undefined,
        prefix, suffix, reset, general, net0, net1, samples, netpar, trainerpar, log
    }
    public enum LoadAndSaveCommand
    {
        Undefined,
        all, net0, net1, samples, par, netpar, trainerpar,
    }
    public enum CreateCommand
    {
        Undefined,
        all, net, trainer, samples, par, netpar, trainerpar,
    }
    public enum ParameterCommand
    {
        Undefined, set
    }
    public enum LayerCommand
    {
        Undefined, add, del, left, right
    }
    public enum InputHelper
    {
        L, // layer
    }
    public enum ParameterName
    {
        Undefined,
        // net parameters
        wInit,
        // layer parameters
        act, wMax, wMin, bMax, bMin, N,
        // trainer parameters
        cost, epochs, Eta, dEta,
    }
}