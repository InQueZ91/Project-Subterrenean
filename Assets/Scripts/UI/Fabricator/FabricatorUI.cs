using System;
using System.Collections.Generic;
using Runtime.Modules;
using UnityEngine;

namespace UI.Fabricator
{
    public class FabricatorUI : MonoBehaviour
    {
        [SerializeField] private GameObject moduleSlotPrefab;

        public event Action<int, int> OnReceivedItem; // moduleIndex, itemIndex
        public event Action<int> OnModuleDetached; // moduleIndex
        public event Action<int> OnModuleSelected; // moduleIndex

        private readonly List<ModuleSlotUI> moduleSlots = new();

        public void BuildSlot(int slotCount)
        {
            if (moduleSlots.Count == slotCount) return;
            
            ClearSlots();

            for (var i = 0; i < slotCount; i++)
            {
                var go = Instantiate(moduleSlotPrefab, transform);
                if (go == null) continue;

                var slot = go.GetComponent<ModuleSlotUI>();
                if (slot == null) continue;
                
                var moduleIndex = i;
                slot.OnReceivedItem += itemIndex => ReceivedItem(moduleIndex, itemIndex);
                slot.OnModuleDetached += () => OnModuleDetached?.Invoke(moduleIndex);
                slot.OnModuleSelected += () => OnModuleSelected?.Invoke(moduleIndex);
                moduleSlots.Add(slot);
            }
        }
        
        public void UpdateModules(Module[] modules)
        {
            for (var i = 0; i < modules.Length; i++)
            {
                var slot = moduleSlots[i];
                slot.SetModule(modules[i]);
            }
        }

        private void ClearSlots()
        {
            foreach (Transform child in transform)
                Destroy(child.gameObject);
            
            moduleSlots.Clear();
        }

        private void ReceivedItem(int moduleIndex, int itemIndex)
        {
            OnReceivedItem?.Invoke(moduleIndex, itemIndex);
        }
    }
}