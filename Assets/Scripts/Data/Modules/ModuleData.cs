using Data.Items;
using Runtime.Modules;
using UnityEngine;

namespace Data.Modules
{
    public abstract class ModuleData : ItemData
    {
        public int capacity;
        public float processingDuration;
        public abstract Module CreateModule();
        
        [Header("UI")]
        public GameObject moduleUIPrefab;
    }

    public abstract class ModuleData<TModule> : ModuleData where TModule : Module
    {
        public override Module CreateModule() => Build();
        protected abstract TModule Build();
    }
}