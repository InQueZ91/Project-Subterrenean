using System.Collections.Generic;
using System.Linq;
using Data;
using Data.Items.Crafting;
using Runtime.Items;
using Runtime.Spawners;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.Handlers.Machine
{
    public class ExchangeHandler : MonoBehaviour
    {
        [SerializeField] private List<RewardData> itemPool = new();
        [SerializeField] private int baseCost = 100;
        [SerializeField] private float rerollDiscount = 0.5f;

        private int _currentCost;
        private ItemPack _pendingReward;
        private bool _hasActiveReward;
        
        [Header("Events")]
        public UnityEvent<ItemPack> onRewardChanged; // Drop
        public UnityEvent<int> onCostChanged; // Cost
        public UnityEvent onRollFailed; // not enough Cp

        private StorageHandler _storageHandler;

        private void Awake()
        {
            _storageHandler = GetComponent<StorageHandler>();
            ResetCost();
        }
        
        public void Roll()
        {
            if (!PointManager.Instance.TrySpend(_currentCost))
            {
                onRollFailed?.Invoke();
                return;
            }

            _pendingReward = PickReward();
            _pendingReward.quantity = Random.Range(1, _pendingReward.item.maxStack);
            _hasActiveReward = true;
            
            // Apply reroll discount to next roll cost
            if (_currentCost >= baseCost)
                _currentCost = Mathf.RoundToInt(_currentCost * rerollDiscount);
            
            onRewardChanged?.Invoke(_pendingReward);
            onCostChanged?.Invoke(_currentCost);
        }

        public void TakeReward()
        {
            if (!_hasActiveReward) return;

            var overflow = _storageHandler.TryAddItem(_pendingReward.item, _pendingReward.quantity);

            if (overflow > 0)
            {
                // Storage full - spawn overflow as loot at machine position
                LootSpawner.Instance.Spawn(_pendingReward.item, overflow, transform.position);
            }
            
            _hasActiveReward = false;
            
            ResetCost();
            
            onRewardChanged?.Invoke(null);
            onCostChanged?.Invoke(_currentCost);
        }

        public void ResetCost() => _currentCost = baseCost;

        private ItemPack PickReward()
        {
            var totalWeight = itemPool.Sum(e => e.chance);
            var roll = Random.Range(0f, totalWeight);
            var cumulative = 0f;

            foreach (var entry in itemPool)
            {
                cumulative += entry.chance;
                if (roll <= cumulative)
                    return new ItemPack { item = entry.itemData, quantity = entry.quantity };
            }

            var toReturn = itemPool[^1];
            return new ItemPack { item = toReturn.itemData, quantity = toReturn.quantity }; 
        }
    }
}