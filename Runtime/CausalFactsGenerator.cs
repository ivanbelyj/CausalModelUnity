using System.Linq;
using CausalModel.Running.Models.FixationResult;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class CausalFactsGenerator<TFact> : MonoBehaviour
{
    [SerializeField]
    protected CausalGenerationConfig<string> causalGenerationConfig;

    protected CausalGenerationDecorator<string> causalGenerationDecorator;

    protected void Initialize()
    {
        causalGenerationDecorator = new(causalGenerationConfig);
    }

    protected TFact[] GenerateFacts(int? seed = null)
    {
        causalGenerationDecorator.Reset(seed);
        var fixationResult = causalGenerationDecorator.RunFixation();

        var elementsByModelInstance = fixationResult
            .ModelInstanceInfoById
            .Values
            .First(x => x.ModelInstance.ModelName == causalGenerationConfig.ModelName);
        return elementsByModelInstance
            .OccurredFacts
            .Select(ParseFactValue)
            .Where(x => x != null)
            .SelectMany(x => x)
            .ToArray();
    }

    private TFact[] ParseFactValue(InstanceFactInfo<string> factInfo)
    {
        if (string.IsNullOrWhiteSpace(factInfo.FactValue))
        {
            return null;
        }

        string trimmedValue = factInfo.FactValue.Trim();

        if (!(trimmedValue.StartsWith("{") || trimmedValue.StartsWith("[")))
        {
            return null;
        }

        try
        {
            var token = JToken.Parse(factInfo.FactValue);
            return token.Type switch
            {
                JTokenType.Array => ((JArray)token).ToObject<TFact[]>(),
                JTokenType.Object => new[] { ((JObject)token).ToObject<TFact>() },
                _ => null,
            };
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning(
                $"Failed to parse fact value. Instance fact id: '{factInfo.InstanceFactId}'; " +
                $"Fact value: '{factInfo.FactValue}'. Error: {ex.Message}");
            return null;
        }
    }
}
