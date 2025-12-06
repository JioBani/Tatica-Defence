using System;
using Common.Scripts.SceneSingleton;
using Scenes.Battle.Feature.Markets;
using UnityEngine;

namespace Scenes.Battle.Feature.Ui
{
    public class AlertManager : SceneSingleton<AlertManager>
    {
        public void Alert(string message)
        {
            Debug.Log(message);   
        }
    }
}
