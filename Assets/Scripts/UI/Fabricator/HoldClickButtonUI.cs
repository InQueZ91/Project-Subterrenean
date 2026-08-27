using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Fabricator
{
    public class HoldClickButtonUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        [Header("Configuration")]
        [SerializeField] public float holdThreshold = 0.5f;
        [SerializeField] public bool repeatWhileHeld = false;
        [SerializeField] public float repeatInterval = 0.2f;
        
        public event Action OnHoldStarted; // fires once, when threshold passed
        public event Action OnHoldRepeat; // fires on every repeat interval 
        public event Action OnPressStarted;
        public event Action OnPressReleased;
        
        private Coroutine holdRoutine;
        private bool isPointerDown;

        public void OnPointerDown(PointerEventData eventData)
        {
            isPointerDown = true;
            OnPressStarted?.Invoke();
            holdRoutine = StartCoroutine(HoldCheck());
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            CancelHold();
            OnPressReleased?.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            // Optional: cancel if pointer drags off the button
            if (isPointerDown) CancelHold();
        }

        private IEnumerator HoldCheck()
        {
            yield return new WaitForSeconds(holdThreshold);
            OnHoldStarted?.Invoke();

            if (!repeatWhileHeld) yield break;
            
            while (isPointerDown)
            {
                yield return new WaitForSeconds(repeatInterval);
                OnHoldRepeat?.Invoke();
            }
        }

        private void CancelHold()
        {
            isPointerDown = false;
            if (holdRoutine == null) return;
            
            StopCoroutine(holdRoutine);
            holdRoutine = null;
        }
    }
}