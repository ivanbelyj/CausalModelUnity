using System.Collections;
using System.Collections.Generic;
using CausalModel.Model;
using CausalModel.Model.Serialization;
using UnityEngine;

[CreateAssetMenu(fileName = "New Causal Model asset",
    menuName = "Causal Model/Causal Model asset", order = 52)]
public class CausalModelAsset : ScriptableObject
{
    [SerializeField]
    private TextAsset causalModelFile;

    private CausalModel<string> causalModel;

    public CausalModel<string> Model {
        get {
            causalModel = CausalModelSerialization
                .FromJson<string>(causalModelFile.text);
            return causalModel;
        }
    }
}
