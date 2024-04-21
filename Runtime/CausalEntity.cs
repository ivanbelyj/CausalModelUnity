using System.Collections;
using System.Collections.Generic;
using CausalModel.Fixation;
using CausalModel.Fixation.Fixators;
using CausalModel.Model;
using CausalModel.Model.Instance;
using UnityEngine;

public class CausalEntity : MonoBehaviour
{
    [SerializeField]
    private CausalModelAsset modelAsset;
    [SerializeField]
    private bool showLogMessages = false;

    private CausalGenerator<string> generator;
    private CausalModel<string> Model => modelAsset.Model;
    
    private void Awake() {
        if (showLogMessages)
            Debug.Log($"Model: {Model.Name}; Facts count: {Model.Facts.Count}");
    }

    public void Initialize() {
        InitializeGeneration();
    }

    public void Fixate() {
        generator.FixateRootFacts();
    }

    protected virtual IFixator<string> CreateFixator() => new Fixator<string>();

    protected virtual FixationFacadeBuilder<string> CreateFixationFacadeBuilder() {
        var builder = new FixationFacadeBuilder<string>(Model);
        if (showLogMessages) {
            builder
                .UseFixator(CreateFixator())
                .AddOnFactFixated((sender, fact, isOccurred) => {
                    LogFactFixated(fact, isOccurred);
                })
                .AddOnBlockImplemented((sender, block, convention, implementation) =>
                {
                    Debug.Log($"Block implemented: {block.Id}");
                })
                .AddOnModelInstanceCreated((sender, modelInstance) =>
                {
                    Debug.Log($"Model instantiated: {modelInstance.Model.Name} " +
                        $"{modelInstance.InstanceId}");
                });
        }

        return builder;
    }

    private void InitializeGeneration() {
        var facadeBuilder = CreateFixationFacadeBuilder();
        var fixationFacade = facadeBuilder.Build();
        generator = fixationFacade.CreateGenerator();
    }

    private static void LogFactFixated(
        InstanceFact<string> fixatedFact,
        bool isHappened) {
        if (isHappened)
        {
            Debug.Log(
                "Fixated: "
                + (fixatedFact.Fact.IsRootCause() ? "" : "\t")
                + fixatedFact.Fact.FactValue +  $" ({fixatedFact.InstanceFactId})");
        }
    }

    private InstanceFact<string> GetFact(InstanceFactId factId)
        => generator!.ModelProvider.GetFact(factId.ToAddress());
}
