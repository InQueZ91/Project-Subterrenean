using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace UI.Fabricator
{
    public class FabricatorRigUI : MonoBehaviour
    {
        [SerializeField] private GameObject fabricatorPrefab;

        // Events
        public UnityEvent<int, int, int> onModuleReceivedItem; // fabricatorIndex, moduleIndex, itemIndex 
        public UnityEvent<int, int> onModuleDetached; // fabricatorIndex, moduleIndex
        public UnityEvent<int, int> onModuleSelected; // fabricatorIndex, moduleIndex
        
        private readonly List<FabricatorUI> _fabricatorUIList = new();

        public void BuildFabricator(List<Runtime.Modules.Fabricator> fabricators)
        {
            // Only rebuild structure if capacity changed
            if (_fabricatorUIList.Count != fabricators.Count)
            {
                ClearFabricators();
                
                // Build new fabricators
                for (var i = 0; i < fabricators.Count; i++)
                {
                    var fabGo = Instantiate(fabricatorPrefab, transform);
                    var fabUI = fabGo.GetComponent<FabricatorUI>();
                    if (fabUI == null) return;
                    
                    var fabricatorIndex = i;
                    fabUI.OnReceivedItem += (moduleIndex, itemIndex)
                        => onModuleReceivedItem?.Invoke(fabricatorIndex, moduleIndex, itemIndex);
                    fabUI.OnModuleDetached += moduleIndex
                        => onModuleDetached?.Invoke(fabricatorIndex, moduleIndex);
                    fabUI.OnModuleSelected += moduleIndex
                        => onModuleSelected?.Invoke(fabricatorIndex, moduleIndex);

                    fabUI.BuildSlot(fabricators[i].Modules.Count);
                    _fabricatorUIList.Add(fabUI);
                }
            }

            // Update modules of each fabricator
            for (var i = 0; i < fabricators.Count; i++)
            {
                var fabricator = fabricators[i];
                if (fabricator == null) continue;
                                
                var fabUI = _fabricatorUIList[i];
                if (fabUI == null) continue;
                
                fabUI.UpdateModules(fabricator.Modules.ToArray());
            }
        }
        
        private void ClearFabricators()
                {
                    foreach (Transform child in transform)
                        Destroy(child.gameObject);
                        
                    _fabricatorUIList.Clear();
                }
    }
}