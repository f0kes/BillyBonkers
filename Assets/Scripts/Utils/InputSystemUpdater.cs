using UnityEngine;
using UnityEngine.InputSystem;

namespace Utils
{
    public class InputSystemUpdater : MonoBehaviour
    {
        void Update()
        {
            InputSystem.Update();
        }
    }
}
