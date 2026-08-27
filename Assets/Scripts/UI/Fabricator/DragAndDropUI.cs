using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Fabricator
{
    [RequireComponent(typeof(RectTransform))]
    public class DragAndDropUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public event Action OnDropped;
        public event Action<PointerEventData> DragStarted;
        public event Action<PointerEventData> Dragged;
        public event Action<PointerEventData> DragEnded;
        
        public bool IsDragging { get; private set; }
        
        private RectTransform _rectTransform;
        private Canvas _rootCanvas;
        private CanvasGroup _canvasGroup;

        private Transform _originParent;
        private int _originSiblingIndex;

        private void Awake()
        {
            _rectTransform = (RectTransform)transform;

            var canvas = GetComponentInParent<Canvas>();
            _rootCanvas = canvas != null ? canvas.rootCanvas : null;
            
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            IsDragging = true;
            
            _originParent = _rectTransform.parent;
            _originSiblingIndex = _rectTransform.GetSiblingIndex();
            
            if (_rootCanvas != null)
                _rectTransform.SetParent(_rootCanvas.transform, true);
            
            if (_canvasGroup != null)
                _canvasGroup.blocksRaycasts = false;
            
            DragStarted?.Invoke(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            var scaleFactor = _rootCanvas != null ? _rootCanvas.scaleFactor : 1f;
            _rectTransform.anchoredPosition += eventData.delta / scaleFactor;
            Dragged?.Invoke(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            IsDragging = false;
            
            if (_canvasGroup != null)
                _canvasGroup.blocksRaycasts = true;
            
            ReturnToOrigin();
            
            DragEnded?.Invoke(eventData);
        }
        
        public void CommitNewParent(RectTransform newParent)
        {
            _originParent = newParent;
            _rectTransform.anchoredPosition = Vector2.zero;
        }

        public void Drop()
        {
            OnDropped?.Invoke();
        }
        
        private void ReturnToOrigin()
        {
            if (_originParent != null)
            {
                _rectTransform.SetParent(_originParent, true);
                _rectTransform.SetSiblingIndex(_originSiblingIndex);
            }
            
            _rectTransform.anchoredPosition = Vector2.zero;
        }
    }
}