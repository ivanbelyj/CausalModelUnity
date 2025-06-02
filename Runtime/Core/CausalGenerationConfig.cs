using CausalModel.Common;
using UnityEngine;

[System.Serializable]
public record CausalGenerationConfig<TFactValue>
    where TFactValue : class
{
    [SerializeField]
    private CausalBundleAsset bundleAsset;

    [SerializeField]
    [Tooltip(
        "The name of the model used in generation. Ensure it exists in the causal " +
        "bundle. Leave empty to use default main model")]
    private string modelName = "";

    [SerializeField]
    private bool showLogMessages = true;

    [SerializeField]
    [Tooltip("Generation seed, ensuring reproducibility. Set -1 to use random")]
    private int defaultSeed = -1;

    public CausalBundle<TFactValue> CausalBundle => bundleAsset.GetCausalBundle<TFactValue>();
    public string ModelName
    {
        get
        {
            if (string.IsNullOrWhiteSpace(modelName))
            {
                return null;
            }
            return modelName;
        }
    }
    public bool ShowLogMessages => showLogMessages;
    public int? DefaultSeed
    {
        get
        {
            return defaultSeed == -1 ? null : defaultSeed;
        }
    }
}
