using UnityEngine;

namespace UI
{
    public class ScreenSpacePlayerUI : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 worldOffset;
        private RectTransform _rectTransform;
        private Camera _mainCam;

        private void Awake()
        {
            _mainCam = Camera.main;
            _rectTransform = GetComponent<RectTransform>();
        }

        private void LateUpdate()
        {
            // Convert world pos → screen pos
            var screenPos = _mainCam.WorldToScreenPoint(target.position + worldOffset);

            // Hide if behind camera
            if (screenPos.z < 0)
            {
                gameObject.SetActive(false);
                return;
            }

            gameObject.SetActive(true);
            _rectTransform.position = screenPos;
        }
    }
}
