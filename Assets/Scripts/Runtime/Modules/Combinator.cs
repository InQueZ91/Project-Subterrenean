using System.Linq;
using Data.Items.Crafting;
using Data.Modules;

namespace Runtime.Modules
{
    // Combinator owns the reference to the buffer it pulls its second input from.
    // FabricatorHandler no longer needs to wire to SubStorage every frame -
    // it calls SetConnectedBuffer once when the layout changes.
    public class Combinator : Module<CombinatorData>
    {
        private readonly CombinationRecipe[] _recipes;
        private CombinationRecipe _currentRecipe;
        
        // The external buffer this combinator pulls its second input from.
        // Set once by FabricatorHandler when a line is connected or disconnected.
        private ItemBuffer _connectedBuffer;
        
        public Combinator(CombinatorData data) : base(data)
        {
            _recipes = data.combinationRecipes.ToArray();
        }

        // Called by FabricatorHandler when the user connects or disconnects a line.
        // Null disconnects.
        public void SetConnectedBuffer(ItemBuffer buffer)
        {
            _connectedBuffer = buffer;
        }
        public bool HasConnectedBuffer => _connectedBuffer != null;
        
        // TryBegin receives connected Buffers from Fabricator.Run -
        // the first entry is the previous module in the own line
        // The combinator appends its own _connectedBuffer as Input B.
        protected override bool TryBegin(ItemBuffer upstream)
        {
            if (_recipes.Length == 0) return false;
            if (_connectedBuffer == null) return false;

            // Build the full two-buffer list: own-line previous + connected line
            var a = upstream.Container.FirstOrDefault();
            var b = _connectedBuffer.Container.FirstOrDefault();
            if (a == null || b == null) return false;

            var match = _recipes
                .FirstOrDefault(r => CanCombine(a.ToPack(), b.ToPack(), r));
            if (match == null)
            {
                _currentRecipe = null;
                return false;
            }
            
            _currentRecipe = match;
            ProcessingDuration = _currentRecipe.combinationTime;
            
            upstream.ForcePull(_currentRecipe.inputA);
            _connectedBuffer.ForcePull(_currentRecipe.inputB);
            
            return true;
        }

        protected override bool TryComplete()
        {
            return _currentRecipe is not null && TryAdd(_currentRecipe.output);
        }
        
        private bool CanCombine(ItemPack a, ItemPack b, CombinationRecipe recipe)
        {
            var inA = recipe.inputA;
            var inB = recipe.inputB;

            var directMatch = inA.item == a.item && inA.quantity <= a.quantity &&
                          inB.item == b.item && inB.quantity <= b.quantity;
            
            var flippedMatch = inA.item == b.item && inA.quantity <= b.quantity &&
                          inB.item == a.item && inB.quantity <= a.quantity;

            return directMatch || flippedMatch;
        }
    }
}