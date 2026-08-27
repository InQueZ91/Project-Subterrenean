using System.Collections.Generic;
using System.Linq;
using Data.Modules;
using UnityEngine;

namespace Runtime.Modules
{
    public class Fabricator
    {
        private int ModuleLimit { get; set; }
        private readonly Module[] _modules;
        public IReadOnlyList<Module> Modules => _modules;
        public Fabricator(int moduleLimit)
        {
            ModuleLimit = moduleLimit;
            _modules = new Module[moduleLimit];
        }

        public void Run(float deltaTime, ItemBuffer storage)
        {
            // Pass 1 - advance timers
            foreach (var module in _modules)
                module?.Tick(deltaTime);
            
            // Pass 2 - try complete, then try to begin, front to back
            var previousSource = storage;
            for (var i = 0; i < _modules.Length; i++)
            {
                var module = _modules[i];
                if (module == null) continue;
                
                module.CompleteProcess();
                module.TryProcess(BuildInputs(previousSource));

                previousSource = module;
            }
            
            // Pass 3 - pull item from lastest module to crafted storage
            var lastModule = _modules.LastOrDefault(m => m != null);
            if (lastModule == null) return;
            
            foreach (var item in lastModule.PullAll())
            {
                // If storage is full, put it back in last module
                if (!storage.TryAdd(item)) 
                    lastModule.TryAdd(item);
            }
        }
        
        public bool AttachModule(ModuleData moduleData, int slotIndex)
        {
            if (slotIndex >= ModuleLimit) return false;
            if (_modules[slotIndex] != null)
                return false;
            
            _modules[slotIndex] = moduleData.CreateModule();
            return true;
        }

        public ModuleData DetachModule(int slotIndex)
        {
            if (slotIndex >= ModuleLimit) return null;
            if (_modules[slotIndex] == null)
            {
                Debug.Log($"Fabricator slot {slotIndex} is empty.");
                return null;
            }
            
            var module = _modules[slotIndex];
            _modules[slotIndex] = null;
            
            return module.SourceData;
        }

        public bool SwapModule(int slotIndexA, int slotIndexB)
        {
            if (_modules[slotIndexA] == null || _modules[slotIndexB] == null)
                return false;
            
            (_modules[slotIndexA], _modules[slotIndexB]) = (_modules[slotIndexB], _modules[slotIndexA]);
            return true;
        }
        
        // Helpers
        private List<ItemBuffer> BuildInputs(ItemBuffer source)
        {
            var inputs = new List<ItemBuffer>();
            if (source != null) inputs.Add(source);
            return inputs;
        }
    }
}