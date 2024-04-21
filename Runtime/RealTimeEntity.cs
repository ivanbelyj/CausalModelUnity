using System.Collections;
using System.Collections.Generic;
using CausalModel.Fixation.Fixators;
using CausalModel.Fixation.Fixators.Pending;
using UnityEngine;

public class RealTimeEntity : CausalEntity
{
    private PendingFixator<string> pendingFixator;
    protected override IFixator<string> CreateFixator() {
        pendingFixator = new PendingFixator<string>(new PendingFixationFilter());
        return pendingFixator;
    }

    private void Start() {
        Initialize();
        Fixate();
    }

    public void ApprovePendingFacts() {
        pendingFixator.ApprovePendingFacts();
    }
}
