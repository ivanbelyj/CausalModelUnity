using System.Collections;
using System.Collections.Generic;
using CausalModel.Fixation;
using CausalModel.Fixation.Fixators;
using UnityEngine;

public class InstantEntity : CausalEntity
{
    private void Start() {
        Initialize();
        Fixate();
    }

    protected override IFixator<string> CreateFixator() => new Fixator<string>();
}
