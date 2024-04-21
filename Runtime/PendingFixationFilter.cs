using System.Collections;
using System.Collections.Generic;
using CausalModel.Fixation.Fixators.Pending;
using CausalModel.Model.Instance;
using UnityEngine;

public class PendingFixationFilter : IPendingFixationFilter<string>
{
    public bool ShouldBePending(InstanceFact<string> fact)
    {
        return true;
    }
}
