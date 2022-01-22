using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class PlayerInfoRepresenter : MonoBehaviour
    {
        [SerializeField] private List<HealthCounter> healthCounters;

        private void Start()
        {
            foreach (var counter in healthCounters)
            {
                counter.Init();
            }
        }
    }
}
