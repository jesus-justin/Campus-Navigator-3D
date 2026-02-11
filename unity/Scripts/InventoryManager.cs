using System.Collections.Generic;
using UnityEngine;

namespace CampusNavigator
{
    public class InventoryManager : MonoBehaviour
    {
        public static InventoryManager Instance { get; private set; }

        private readonly HashSet<string> items = new HashSet<string>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void AddItem(string itemId)
        {
            if (string.IsNullOrEmpty(itemId))
            {
                return;
            }
            items.Add(itemId);
            InventoryUI.Instance?.Refresh(items);
        }

        public bool HasItem(string itemId)
        {
            return !string.IsNullOrEmpty(itemId) && items.Contains(itemId);
        }

        public void RemoveItem(string itemId)
        {
            if (string.IsNullOrEmpty(itemId))
            {
                return;
            }
            items.Remove(itemId);
            InventoryUI.Instance?.Refresh(items);
        }

        public IEnumerable<string> GetItems()
        {
            return items;
        }
    }
}
