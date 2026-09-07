using System.Collections.Generic;
using System.Linq;
using Data;
using Data.Items;
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
        [Header("Configuration")] 
        [SerializeField] private float pickupRadius = 2f;
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
        // Track previous selection to avoid firing onModuleSelected every frame.
        private Module _lastReportedModule;

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

            RunFabricators();
            onModuleSelected?.Invoke(_selectedModule);
        }

        private void OnDestroy()
        {
            _itemStorage.OnChanged -= OnItemStorageChanged;
        }

        #endregion

        #region Public API

        public void ConsumeMagazineItem(MagazineType type)
        {
            var magazineItem = GetItemByMagazineType(type);
            if (magazineItem == null) return;

            var item = magazineItem.ToPack();
            item.quantity = 1;
            _itemStorage.TryPull(item);
        }
        
        public Item GetItemByMagazineType(MagazineType type)
        {
            return _itemStorage.Container.First(i => i.Data is MagazineItem a && a.type == type);
        }
        
        public int AddToStorage(ItemPack itemPack)
        {
            return _itemStorage.ForceAdd(itemPack);
        }
        
        public void SelectRecipe(int recipeIndex)
        {
            if (_selectedModule is not Converter converter) return;
            converter.TrySetRecipe(recipeIndex);
            
            _selectedModule = converter;
            onModuleSelected?.Invoke(converter);
            _lastReportedModule = converter;
        }
        
        public void SelectModule(int fabIndex, int moduleIndex)
        {
            var module = _fabricators[fabIndex].Modules[moduleIndex];
            if (module == null) return;
            
            _selectedModule = module;
            onModuleSelected?.Invoke(module);
            _lastReportedModule = module;
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
                Debug.LogWarning("FabricatorHandler: fabricator slot limit reached.");
                return;
            }

            _fabricators.Add(fabricator);
            RewireCombinators();
            onFabricatorChanged?.Invoke(_fabricators);
        }

        public void RemoveFabricator(int index)
        {
            if (index < 0 || index >= _fabricators.Count)
            {
                Debug.LogWarning($"FabricatorHandler: invalid fabricator index {index}.");
                return;
            }

            _fabricators.RemoveAt(index);
            RewireCombinators();
            onFabricatorChanged?.Invoke(_fabricators);
        }

        public void AttachModule(int fabricatorIndex, ModuleData moduleData, int moduleIndex)
        {
            if (!IsValidFabricatorIndex(fabricatorIndex)) return;

            if (!_fabricators[fabricatorIndex].AttachModule(moduleData, moduleIndex))
            {
                Debug.Log($"Attach Module failed - fabricator:{fabricatorIndex}, slot:{moduleIndex}.");
                return;
            }
            
            // Wiring may have changed if the new module is a Combinator.
            RewireCombinators();
            onFabricatorChanged?.Invoke(_fabricators);
        }

        public void DetachModule(int fabricatorIndex, int moduleIndex)
        {
            if (!IsValidFabricatorIndex(fabricatorIndex)) return;
            
            var fabricator = _fabricators[fabricatorIndex];

            if (fabricator.Modules[moduleIndex] == _selectedModule)
            {
                _selectedModule = null;
                _lastReportedModule = null;
                onModuleSelected?.Invoke(null);
            }
            
            var detachedModule = fabricator.DetachModule(moduleIndex);
            if (detachedModule == null) return;
            
            // Return module contents and the module item itself to storage
            var itemsToReturn = detachedModule.PullAll();
            itemsToReturn.Add(new ItemPack(item: detachedModule.SourceData, quantity: 1));
            foreach (var item in itemsToReturn)
                _itemStorage.TryAdd(item);
            
            // Wiring may have changed if the detached module was a Combinator.
            RewireCombinators();
            onFabricatorChanged?.Invoke(_fabricators);
            OnItemStorageChanged();
        }

        // Called by UI when the user picks which fabricator line to connect to a Combinator.
        // fabricatorIndex: the fabricator that owns the Combinator.
        // connectedFabricatorIndex: the fabricator whose last-module buffer feeds into it.
        public void SetCombinatorConnection(int fabricatorIndex, int connectedFabricatorIndex)
        {
            if (!IsValidFabricatorIndex(fabricatorIndex)) return;
            
            var fabricator = _fabricators[fabricatorIndex];
            var combinator = fabricator.Modules.OfType<Combinator>().FirstOrDefault();

            if (connectedFabricatorIndex < 0 || connectedFabricatorIndex >= _fabricators.Count)
            {
                combinator?.SetConnectedBuffer(null);
                _fabricators[fabricatorIndex].SetBypassing(false);
                return;
            }
            
            var connectedFab = _fabricators[connectedFabricatorIndex];
            combinator?.SetConnectedBuffer(connectedFab.LastModule);
            connectedFab.SetBypassing(true);
            
            onFabricatorChanged?.Invoke(_fabricators);
        }

        #endregion

        #region Private

        private void RunFabricators()
        {
            foreach (var fabricator in _fabricators)
                fabricator?.Run(Time.deltaTime, _itemStorage);
            
            onFabricatorChanged?.Invoke(_fabricators);
        }

        // Rewire is called once whenever the fabricator list or module layout changes,
        // not every frame. Keeps cross-fabricator logic out of the hot path.
        private void RewireCombinators()
        {
            // Reset all bypass states first.
            foreach (var fab in _fabricators)
                fab?.SetBypassing(false);

            for (var i = 0; i < _fabricators.Count; i++)
            {
                var fabricator = _fabricators[i];

                var combinator = fabricator?.Modules.OfType<Combinator>().FirstOrDefault();
                if (combinator == null) continue;
                
                // Only wire to an adjacent line if one exists - no wrap-around;
                // Default: connect to the next fabricator if present.
                // This overridden by SetCombinatorConnection when the user
                // explicitly picks a line from the UI.
                if (combinator.HasConnectedBuffer || i + 1 >= _fabricators.Count) continue;
                var next = _fabricators[i + 1];
                if (next == null) continue;
                
                combinator.SetConnectedBuffer(next.LastModule);
                next.SetBypassing(true);
            }
        }
        
        private void NotifyModuleSelectionIfChanged()
        {
            if (_lastReportedModule == _selectedModule) return;
            onModuleSelected?.Invoke(_selectedModule);
            _lastReportedModule = _selectedModule;
        }
        
        private void OnItemStorageChanged()
        {
            onMaterialStorageChanged?.Invoke(_itemStorage.Container.ToList(), storageCapacity);
        }

        private bool IsValidFabricatorIndex(int index)
        {
            if (index >= 0 && index < _fabricators.Count) return true;
            Debug.LogWarning($"FabricatorHandler: fabricator index {index} out of range.");
            return false;
        }

        #endregion
    }
}