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

    private CausalBundle<string> causalBundle;

    public CausalBundle<string> CausalBundle
    {
        get
        {
            if (causalBundle == null) {
                causalBundle = SerializationUtils.FromJson<string>(causalBundleTextAsset.text);
            }
            return causalBundle;
        }
    }
}
