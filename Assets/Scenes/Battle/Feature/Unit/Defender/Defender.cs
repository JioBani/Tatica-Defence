using System;
using UnityEngine;

namespace Scenes.Battle.Feature.Unit.Defenders
{
    public class Defender : Units.Unit
    {
        // public Placement Placement { get; private set; }
        public Placement Placement;

        public void OnDrop(Placement placement)
        {
            Placement = placement;
        }
    }
}