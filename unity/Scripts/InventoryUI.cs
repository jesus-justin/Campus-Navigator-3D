using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CampusNavigator
{
    public class InventoryUI : MonoBehaviour
    {
        public static InventoryUI Instance { get; private set; }

        public GameObject panel;
        public Text listText;
        public InputConfig inputConfig;

        public bool IsOpen => panel != null && panel.activeSelf;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Update()
        {
            if (Input.GetKeyDown(GetToggleKey()))
            {
                Toggle();
            }
        }

        public void Toggle()
        {
            if (panel == null)
            {
                return;
            }
            panel.SetActive(!panel.activeSelf);
            Refresh(InventoryManager.Instance?.GetItems() ?? new string[0]);
        }

        public void Refresh(IEnumerable<string> items)
        {
            if (listText == null)
            {
                return;
            }
            var list = items != null ? items.ToList() : new List<string>();
            listText.text = list.Count == 0 ? "Inventory empty" : string.Join("\n", list);
        }

        private KeyCode GetToggleKey()
        {
            return inputConfig != null ? inputConfig.inventoryKey : KeyCode.I;
        }
    }
}
