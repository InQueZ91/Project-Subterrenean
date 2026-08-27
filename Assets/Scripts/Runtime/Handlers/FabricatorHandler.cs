using System.Collections.Generic;
using System.Linq;
using Data.Items.Crafting;
using Data.Modules;
using Data.Stats;
using Runtime.Items;
using Runtime.Modules;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.Handlers
{
    public class FabricatorHandler : MonoBehaviour
    {
        [Header("Configuration")] [SerializeField]
        private float pickupRadius = 2f;

        [SerializeField] private int storageCapacity = 21;
        [SerializeField] private ItemPack[] startingItems;
        [SerializeField] private List<FabricatorStats> startingFabricator = new();

        [Header("Events")]
        public UnityEvent<List<Item>, int> onMaterialStorageChanged;
        public UnityEvent<List<Fabricator>> onFabricatorChanged;
        public UnityEvent<Module> onModuleSelected;

        private Storage _itemStorage;
        private readonly List<Fabricator> _fabricators = new();
        private bool _isToggled;
        private Module _selectedModule;

        #region Unity's life cycle

        private void Awake()
        {
            _itemStorage = new Storage(storageCapacity);
            _itemStorage.OnChanged += OnItemStorageChanged;
        }

        private void Start()
        {
            foreach (var item in startingItems)
                _itemStorage.TryAdd(item);

            foreach (var stats in startingFabricator)
                AddFabricator(new Fabricator(stats.moduleSlots));
        }

        private void Update()
        {
            if (!_isToggled) return;
            
            _fabricators.ForEach(fabricator => fabricator
                .Run(Time.deltaTime, _itemStorage));
            
            onFabricatorChanged?.Invoke(_fabricators);
        }

        private void OnDestroy()
        {
            _itemStorage.OnChanged -= OnItemStorageChanged;
        }

        #endregion

        // API
        public void SelectRecipe(int recipeIndex)
        {
            if (_selectedModule is not Converter converter) return;
            converter.TrySetRecipe(recipeIndex);
        }
        
        public void SelectModule(int fabIndex, int moduleIndex)
        {
            var module = _fabricators[fabIndex].Modules[moduleIndex];
            if (module == null) return;
            
            _selectedModule = module;
            onModuleSelected?.Invoke(module);
        }
        
        public void SetFabricatorsState(bool state)
        {
            _isToggled = state;
        }
        
        public void ModuleSlotReceiveItem(int fabIndex, int moduleIndex, int itemIndex)
        {
            if (_itemStorage.Container[itemIndex].Data is not ModuleData moduleData) return;
            
            _itemStorage.RemoveAt(itemIndex);
            
            AttachModule(fabIndex, moduleData, moduleIndex);
        }
        
        public void AddFabricator(Fabricator fabricator)
        {
            if (_fabricators.Count >= startingFabricator.Count)
            {
                Debug.LogWarning("FabricatorHandler: cannot add more fabricators.");
                return;
            }

            _fabricators.Add(fabricator);
            onFabricatorChanged?.Invoke(_fabricators);
        }

        public void RemoveFabricator(int index)
        {
            if (index >= _fabricators.Count)
            {
                Debug.LogWarning("FabricatorHandler: cannot remove fabricator at index " + index);
                return;
            }

            _fabricators.RemoveAt(index);
        }

        public void AttachModule(int fabricatorIndex, ModuleData moduleData, int moduleIndex)
        {
            var fabricator = _fabricators[fabricatorIndex];
            if (fabricator == null) return;

            if (!fabricator.AttachModule(moduleData, moduleIndex))
            {
                Debug.Log($"Attach Module to fabricator:{fabricatorIndex}, slot:{moduleIndex} failed.");
            }
            
            onFabricatorChanged?.Invoke(_fabricators);
        }

        public void DetachModule(int fabricatorIndex, int moduleIndex)
        {
            var fabricator = _fabricators[fabricatorIndex];
            if (fabricator == null) return;

            if (fabricator.Modules[moduleIndex] == _selectedModule)
            {
                _selectedModule = null;
                onModuleSelected?.Invoke(null);
            }
            
            var moduleData = fabricator.DetachModule(moduleIndex);
            if (moduleData == null) return;

            var toItem = new ItemPack(item: moduleData, quantity: 1);
            _itemStorage.TryAdd(toItem);
            
            onFabricatorChanged?.Invoke(_fabricators);
            OnItemStorageChanged();
        }
        
        // Helpers
        private void OnItemStorageChanged()
        {
            onMaterialStorageChanged?.Invoke(_itemStorage.Container.ToList(), storageCapacity);
        }
    }
}