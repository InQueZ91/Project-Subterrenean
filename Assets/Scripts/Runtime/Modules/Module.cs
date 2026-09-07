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

        // Try to start a processing cycle.
        // Returns true if successfully started.
        public bool TryProcess(ItemBuffer upstream)
        {
            if (State != ModuleState.Starved) return false;
            if (!TryBegin(upstream)) return false;
            
            ElapsedTime = 0;
            State = ModuleState.Processing;
            return true;
        }

        public void Tick(float deltaTime)
        {
            if (State != ModuleState.Processing) return;
            ElapsedTime += deltaTime;
        }
        
        // Try to complete a finished cycle.
        // Blocked → re-attempt each tick until the output buffer has room.
        // Returns true if the output was successfully deposited.
        public bool CompleteProcess()
        {
            // Re-attempt if we were blocked last tick (output was full)
            if (State == ModuleState.Blocked)
            {
                if (!TryComplete()) return false;
                State = ModuleState.Starved;
                return true;
            }
            
            if (State != ModuleState.Processing) return false;
            if (ElapsedTime < ProcessingDuration) return false;
            
            if (!TryComplete())
            {
                // Output buffer full — hold until space clears
                State = ModuleState.Blocked;
                return false;
            }
 
            State = ModuleState.Starved;
            return true;
        }
        
        protected abstract bool TryBegin(ItemBuffer upstream);

        protected abstract bool TryComplete();
    }

    public abstract class Module<TData> : Module where TData : ModuleData
    {
        protected Module(TData data) : base(data)
        {
        }
    }
}