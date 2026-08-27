using System;
using Data.Modules;
using Runtime.Modules;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Fabricator
{
    [RequireComponent(typeof(HoldClickButtonUI))]
    public class ModuleSlotUI : MonoBehaviour, IDropHandler
    {
        [Header("References")] 
        [SerializeField] private ModuleItemUI moduleUI;
        [SerializeField] private Image holdingFill;

        [Header("Configuration")] 
        [SerializeField] private float holdingTime = 2f;

        // Events
        public event Action OnModuleSelected;
        public event Action<int> OnReceivedItem; // slot index
        public event Action OnModuleDetached;

        private HoldClickButtonUI _holdClickHandler;
        private float _elapsedTime;
        private bool _isHolding;
        private bool _isEmpty;

        private void Awake()
        {
            _holdClickHandler = GetComponent<HoldClickButtonUI>();
        }

        private void Start()
        {
            _holdClickHandler.holdThreshold = holdingTime;
            _holdClickHandler.repeatWhileHeld = false;

            _holdClickHandler.OnHoldStarted += DetachModule;
            _holdClickHandler.OnPressStarted += HoldingStarted;
            _holdClickHandler.OnPressReleased += HoldingReleased;
            
            holdingFill.fillAmount = 0f;
        }

        private void Update()
        {
            if (!_isHolding) return;
            
            _elapsedTime += Time.deltaTime;
            if (_elapsedTime <= holdingTime)
                holdingFill.fillAmount = Mathf.Clamp01(_elapsedTime / holdingTime);
        }

        private void OnDestroy()
        {
            _holdClickHandler.OnHoldStarted -= DetachModule;
            _holdClickHandler.OnPressStarted -= HoldingStarted;
            _holdClickHandler.OnPressReleased -= HoldingReleased;
        }

        public void SetModule(Module module)
        {
            if (module == null)
            {
                moduleUI.Disable();
                _isEmpty = true;
                return;
            }

            _isEmpty = false;
            moduleUI.SetModuleIcon(module.SourceData.icon);
            moduleUI.SetState(module.State, module.ElapsedTime, module.ProcessingDuration);
            moduleUI.Enable();
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (!_isEmpty) return;
            
            var dropObj = eventData.pointerDrag;
            if (dropObj == null) return;

            var itemUI = dropObj.GetComponent<ItemUI>();
            if (itemUI == null) return;

            OnReceivedItem?.Invoke(itemUI.SlotIndex);
        }

        public void SelectModule()
        {
            if (_isEmpty) return;
            if (_isHolding) return;
            
            OnModuleSelected?.Invoke();
        }
        
        private void HoldingStarted()
        {
            _elapsedTime = 0f;
            holdingFill.fillAmount = 0f;
            holdingFill.color = Color.red;
            _isHolding = true;
        }

        private void HoldingReleased()
        {
            _isHolding = false;
            _elapsedTime = 0f;
            holdingFill.fillAmount = 0f;
        }

        private void DetachModule()
        {
            if (_isEmpty) return;
            OnModuleDetached?.Invoke();
        }
    }
}