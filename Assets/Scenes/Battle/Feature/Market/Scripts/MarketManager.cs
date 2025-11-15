using System;
using Common.Scripts.Rxs;
using Common.Scripts.SceneSingleton;
using UnityEngine;

namespace Scenes.Battle.Feature.Markets
{
    public class MarketManager : SceneSingleton<MarketManager>
    {
        public RxValue<int> Gold = new RxValue<int>(0);
    }
}
