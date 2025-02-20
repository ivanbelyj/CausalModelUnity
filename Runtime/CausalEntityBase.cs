using CausalModel.Fixation.Fixators;
using UnityEngine;

/// <summary>
/// Controls causal model fixation
/// </summary>
public class CausalEntityBase : MonoBehaviour
{
    [SerializeField]
    protected CausalBundleAsset bundleAsset;

    [SerializeField]
    [Tooltip(
        "The name of the model used in generation. Ensure it exists in the causal " +
        "bundle. Leave empty to use default main model")]
    protected string modelName = "";

    [SerializeField]
    protected bool showLogMessages = true;

    [SerializeField]
    [Tooltip("Generation seed, ensuring reproducibility. Set -1 to use random")]
    protected int seed = -1;

    [SerializeField]
    protected bool fixateOnStart = true;

    private CausalGenerationDecorator<string> causalGeneration;

    private void Awake() {
        causalGeneration = CreateCausalGenerationDecorator();
    }

    private void Start() {
        if (fixateOnStart) {
            Fixate();
        }
    }
    
    public void Fixate() {
        causalGeneration.Fixate();
    }

    protected virtual IFixator<string> CreateFixator() => new Fixator<string>();

    private CausalGenerationDecorator<string> CreateCausalGenerationDecorator() {
        return new(
            bundleAsset.CausalBundle,
            CreateFixator(),
            string.IsNullOrWhiteSpace(modelName)
                ? null
                : modelName,
            seed == -1 ? null : seed,
            showLogMessages
        );
    }
}
