using UnityEngine;

namespace UI
{
    public class Billboard : MonoBehaviour
    {
        private Camera _mainCam;

        private void Awake()
        {
            _mainCam = Camera.main;
        }

        private void LateUpdate()
        {
            transform.rotation = _mainCam.transform.rotation;
        }
    }
}
