using UnityEngine;
using CausalModel.Fixation;
using CausalModel.Model.Instance;
using CausalModel.Fixation.Fixators;
using CausalModel.Blocks;
using System;
using CausalModel.Running.FixationRunning;
using CausalModel.Running.Models.FixationResult;

public class CausalGenerationDecorator<TFactValue>
    where TFactValue : class
{
    private readonly CausalGenerationConfig<TFactValue> causalGenerationConfig;
    private readonly Func<IFixator<TFactValue>> createFixator;
    private readonly FixationFacadeBuilder<TFactValue> fixationFacadeBuilder;

    private FixationFacade<TFactValue> fixationFacade;
    private CausalGenerator<TFactValue> generator;
    private int? seed;
    private bool isReset;

    public CausalGenerationDecorator(
        CausalGenerationConfig<TFactValue> causalGenerationConfig,
        Func<IFixator<TFactValue>> createFixator = null)
    {
        this.causalGenerationConfig = causalGenerationConfig;
        this.createFixator = createFixator;
        fixationFacadeBuilder = CreateFixationFacadeBuilder();
    }

    public void Reset(int? seed = null)
    {
        fixationFacade = fixationFacadeBuilder.Build();
        this.seed = seed;

        isReset = true;
    }

    public void Fixate()
    {
        EnsureInitialized();

        // This method requires manually created generator.
        // Create when Fixate is called the first time after Reset
        if (isReset)
        {
            generator = fixationFacade.CreateGenerator(
                GetSeed(),
                causalGenerationConfig.ModelName);
        }
        generator.FixateRootFacts();

        // Should be reset to generate other causal entities
        // (for example, when we generate many characters using single
        // CausalGenerationDecorator)
        isReset = false;
    }

    public FixationResult<TFactValue> RunFixation()
    {
        EnsureInitialized();
        var fixationRunner = new FixationRunner<TFactValue>(fixationFacade);
        return fixationRunner.Run(GetSeed());
    }

    protected virtual FixationFacadeBuilder<TFactValue> CreateFixationFacadeBuilder()
    {
        var builder = new FixationFacadeBuilder<TFactValue>(causalGenerationConfig.CausalBundle);
        builder.UseFixatorFactory(createFixator);

        if (causalGenerationConfig.ShowLogMessages)
        {
            AddLogMessages(builder);
        }

        return builder;
    }

    private int? GetSeed() => seed ?? causalGenerationConfig.DefaultSeed;

    private void EnsureInitialized()
    {
        if (fixationFacadeBuilder == null)
        {
            Reset();
        }
    }

    #region Logging
    private FixationFacadeBuilder<TFactValue> AddLogMessages(
        FixationFacadeBuilder<TFactValue> builder)
    {
        builder
            .AddOnFactFixated((sender, fact, isOccurred) =>
            {
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
                + fixatedFact.Fact.FactValue + $" ({fixatedFact.InstanceFactId})");
        }
    }
    #endregion
}
