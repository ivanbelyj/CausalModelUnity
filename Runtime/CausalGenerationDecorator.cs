using UnityEngine;
using CausalModel.Fixation;
using CausalModel.Model.Instance;
using CausalModel.Fixation.Fixators;
using CausalModel.Blocks;
using CausalModel.Common;

/// <summary>
/// Encapsulates causal generation configuration to provide a more convenient way
/// in CausalModelUnity
/// </summary>
public class CausalGenerationDecorator<TFactValue>
    where TFactValue : class
{
    private readonly bool showLogMessages;
    private readonly string modelName;
    private readonly int? seed;
    private readonly CausalBundle<TFactValue> causalBundle;
    private readonly IFixator<TFactValue> fixator;

    private CausalGenerator<TFactValue> generator;
    private bool isInitialized = false;

    public CausalGenerationDecorator(
        CausalBundle<TFactValue> causalBundle,
        IFixator<TFactValue> fixator = null,
        string modelName = null,
        int? seed = null,
        bool showLogMessages = false)
    {
        this.showLogMessages = showLogMessages;
        this.modelName = modelName;
        this.seed = seed;
        this.causalBundle = causalBundle;
        this.fixator = fixator ?? new Fixator<TFactValue>(); // Default fixator
    }

    public void Fixate() {
        EnsureInitialized();
        generator.FixateRootFacts();
    }

    protected virtual FixationFacadeBuilder<TFactValue> CreateFixationFacadeBuilder()
    {
        var builder = new FixationFacadeBuilder<TFactValue>(causalBundle);
        builder.UseFixator(fixator);
        if (showLogMessages) {
            AddLogMessages(builder);
        }

        return builder;
    }

    private void EnsureInitialized() {
        if (!isInitialized) {
            InitializeGeneration();
        }
    }

    private void InitializeGeneration() {
        var facadeBuilder = CreateFixationFacadeBuilder();
        var fixationFacade = facadeBuilder.Build();
        generator = fixationFacade.CreateGenerator(seed, modelName);
    }

    #region Logging
    private FixationFacadeBuilder<TFactValue> AddLogMessages(
        FixationFacadeBuilder<TFactValue> builder)
    {
        builder
            .AddOnFactFixated((sender, fact, isOccurred) => {
                LogFactFixated(fact, isOccurred);
            })
            .AddOnBlockImplemented((sender, block, convention, implementation) =>
            {
                LogBlockImplemented(block);
            })
            .AddOnModelInstanceCreated((sender, modelInstance) =>
            {
                LogModelInstanceCreated(modelInstance);
            });

        return builder;
    }

    private void LogBlockImplemented(DeclaredBlock block)
    {
        Debug.Log($"Block implemented: {block.Id}");
    }

    private void LogModelInstanceCreated(ModelInstance<TFactValue> modelInstance)
    {
        Debug.Log($"Model instantiated: {modelInstance.Model.Name} " +
            $"{modelInstance.InstanceId}");
    }

    private static void LogFactFixated(
        InstanceFact<TFactValue> fixatedFact,
        bool isHappened)
    {
        if (isHappened)
        {
            Debug.Log(
                "Fixated: "
                + (fixatedFact.Fact.IsRootCause() ? "" : "\t")
                + fixatedFact.Fact.FactValue +  $" ({fixatedFact.InstanceFactId})");
        }
    }
    #endregion
}
