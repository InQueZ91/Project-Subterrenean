using System.Collections.Generic;
using System.Linq;
using Data.Modules;
using UnityEngine;

namespace Runtime.Modules
{
    public class Fabricator
    {
        private readonly Module[] _modules;

        // When true, this fabricator does not push its output back to the main storage.
        // Set internally when another fabricator's Combinator connects to this one.
        private bool _isBypassing;
        
        public IReadOnlyList<Module> Modules => _modules;
        private int ModuleLimit { get; }
        
        public Fabricator(int moduleLimit)
        {
            ModuleLimit = moduleLimit;
            _modules = new Module[moduleLimit];
        }
        
        public Module LastModule => _modules.LastOrDefault(m => m != null);
        public void SetBypassing(bool bypassing) => _isBypassing = bypassing;

        public void Run(float deltaTime, ItemBuffer mainStorage)
        {
            // Pass 1 - advance timers
            foreach (var module in _modules)
                module?.Tick(deltaTime);
            
            // Pass 2 - complete, then begin front to back
            var previousBuffer = mainStorage;
            foreach (var module in _modules)
            {
                if (module == null) continue;
                
                module.CompleteProcess();
                module.TryProcess(previousBuffer);
                
                previousBuffer = module;
            }
            
            // Pass 3 - flush last module output to main storage (unless bypassing)
            if (_isBypassing) return;
            
            var last = LastModule;
            if (last == null) return;
            
            foreach (var item in last.PullAll().Where(item => !mainStorage.TryAdd(item))) 
                last.TryAdd(item);
        }
        
        public bool AttachModule(ModuleData moduleData, int slotIndex)
        {
            if (slotIndex >= ModuleLimit) return false;
            if (_modules[slotIndex] != null)
                return false;
            
            _modules[slotIndex] = moduleData.CreateModule();
            return true;
        }

        public Module DetachModule(int slotIndex)
        {
            if (slotIndex >= ModuleLimit) return null;
            if (_modules[slotIndex] == null)
            {
                Debug.Log($"Fabricator slot {slotIndex} is empty.");
                return null;
            }
            
            var module = _modules[slotIndex];
            _modules[slotIndex] = null;
            
            return module;
        }

        public bool SwapModule(int slotIndexA, int slotIndexB)
        {
            if (_modules[slotIndexA] == null || _modules[slotIndexB] == null)
                return false;
            
            (_modules[slotIndexA], _modules[slotIndexB]) = (_modules[slotIndexB], _modules[slotIndexA]);
            return true;
        }
    }
}