using System.Collections.Generic;
using Data.Modules;

namespace Runtime.Modules
{
    public abstract class Module : ItemBuffer
    {
        public ModuleData SourceData { get; internal set; }
        public float ProcessingDuration { get; protected set;}
        public ModuleState State { get; private set; } = ModuleState.Starved;
        public float ElapsedTime { get; private set; }

        protected Module(ModuleData data) : base(data.capacity) 
        {
            ProcessingDuration = data.processingDuration;
            SourceData = data;
        }

        public bool TryProcess(List<ItemBuffer> connectedModules)
        {
            if (State != ModuleState.Starved) return false;
            if (!TryBegin(connectedModules)) return false;
            
            ElapsedTime = 0;
            State = ModuleState.Processing;
            return true;
        }

        public void Tick(float deltaTime)
        {
            if (State != ModuleState.Processing) return;
            ElapsedTime += deltaTime;
        }
        
        private bool IsReadyToComplete => State == ModuleState.Processing && ElapsedTime >= ProcessingDuration;
        
        public bool CompleteProcess()
        {
            if (!IsReadyToComplete) return false;
            if (!TryComplete())
            {
                State = ModuleState.Blocked;
                return false;
            }
            State = ModuleState.Starved;
            return true;
        }
        
        protected abstract bool TryBegin(List<ItemBuffer> connectedModules);

        protected abstract bool TryComplete();
    }

    public abstract class Module<TData> : Module where TData : ModuleData
    {
        protected Module(TData data) : base(data)
        {
        }
    }
}