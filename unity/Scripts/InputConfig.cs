using UnityEngine;

namespace CampusNavigator
{
    [CreateAssetMenu(menuName = "CampusNavigator/InputConfig")]
    public class InputConfig : ScriptableObject
    {
        public KeyCode interactKey = KeyCode.E;
        public KeyCode submitKey = KeyCode.F;
        public KeyCode inventoryKey = KeyCode.I;
        public KeyCode questListKey = KeyCode.Q;
        public KeyCode pauseKey = KeyCode.P;
        public KeyCode startKey = KeyCode.Return;

        public KeyCode dialogueChoice1 = KeyCode.Alpha1;
        public KeyCode dialogueChoice2 = KeyCode.Alpha2;
        public KeyCode dialogueChoice3 = KeyCode.Alpha3;
    }
}
