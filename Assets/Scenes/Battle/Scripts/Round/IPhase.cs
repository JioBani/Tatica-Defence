using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scenes.Battle.Scripts.Round
{
    public interface IPhase
    {
        string Name { get; }
        void Enter();
        void Run();
        void Exit();
    }
}
