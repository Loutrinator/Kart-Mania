using System;
using UnityEngine;

namespace Handlers
{
    public class CircuitLoader : MonoBehaviour
    {
        private void Awake()
        {
            GameManager.Instance.InitLevel();
        }
    }
}
