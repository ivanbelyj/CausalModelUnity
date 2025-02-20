using CausalModel.Fixation.Fixators;
using CausalModel.Fixation.Fixators.Pending;
using UnityEngine;

public class DynamicCausalEntity : CausalEntityBase
{
    private PendingFixator<string> pendingFixator;
    protected override IFixator<string> CreateFixator() {
        pendingFixator = new PendingFixator<string>(new PendingFixationFilter());
        return pendingFixator;
    }

    public void ApprovePendingFacts() {
        pendingFixator.ApprovePendingFacts();
    }
}
