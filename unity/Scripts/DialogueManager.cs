using UnityEngine;

namespace CampusNavigator
{
    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager Instance { get; private set; }

        public DialogueUI ui;
        public InputConfig inputConfig;

        private int pendingQuestId;
        private bool isOpen;

        public bool IsOpen => isOpen;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            if (ui != null)
            {
                if (ui.option1 != null) ui.option1.onClick.AddListener(AcceptQuest);
                if (ui.option2 != null) ui.option2.onClick.AddListener(GiveDirections);
                if (ui.option3 != null) ui.option3.onClick.AddListener(Close);
            }
        }

        private void Update()
        {
            if (!isOpen)
            {
                return;
            }

            if (Input.GetKeyDown(GetKey(1)))
            {
                AcceptQuest();
            }
            else if (Input.GetKeyDown(GetKey(2)))
            {
                GiveDirections();
            }
            else if (Input.GetKeyDown(GetKey(3)))
            {
                Close();
            }
        }

        public void ShowQuestDialogue(string greeting, int questId)
        {
            pendingQuestId = questId;
            isOpen = true;
            if (ui != null)
            {
                ui.Show("NPC", greeting, "1) Accept quest", "2) Where next?", "3) Later");
            }
        }

        public void AcceptQuest()
        {
            isOpen = false;
            ui?.Hide();
            QuestManager.Instance?.StartQuestById(pendingQuestId);
        }

        public void GiveDirections()
        {
            isOpen = false;
            ui?.Hide();
            QuestManager.Instance?.ExplainActiveStep();
        }

        public void Close()
        {
            isOpen = false;
            ui?.Hide();
        }

        private KeyCode GetKey(int index)
        {
            if (inputConfig == null)
            {
                return index == 1 ? KeyCode.Alpha1 : index == 2 ? KeyCode.Alpha2 : KeyCode.Alpha3;
            }
            return index == 1 ? inputConfig.dialogueChoice1 : index == 2 ? inputConfig.dialogueChoice2 : inputConfig.dialogueChoice3;
        }
    }
}
