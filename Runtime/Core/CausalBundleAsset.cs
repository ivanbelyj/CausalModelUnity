using CausalModel.Common;
using CausalModel.Serialization;
using UnityEngine;

[CreateAssetMenu(
    fileName = "New Causal Bundle asset",
    menuName = "Causal Bundle/Causal Bundle asset",
    order = 52)]
public class CausalBundleAsset : ScriptableObject
{
    [SerializeField]
    private TextAsset causalBundleTextAsset;

    private object causalBundle;

    public CausalBundle<TFactValue> GetCausalBundle<TFactValue>()
        where TFactValue : class
    {
        causalBundle ??= SerializationUtils.FromJson<TFactValue>(causalBundleTextAsset.text);
        return (CausalBundle<TFactValue>)causalBundle;
    }
}
