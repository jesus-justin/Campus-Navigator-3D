using UnityEngine;
using UnityEngine.UI;

namespace CampusNavigator
{
    public class DialogueUI : MonoBehaviour
    {
        public GameObject panel;
        public Text titleText;
        public Text bodyText;
        public Button option1;
        public Button option2;
        public Button option3;

        public void Show(string title, string body, string opt1, string opt2, string opt3)
        {
            if (panel != null)
            {
                panel.SetActive(true);
            }
            if (titleText != null)
            {
                titleText.text = title;
            }
            if (bodyText != null)
            {
                bodyText.text = body;
            }
            SetButton(option1, opt1);
            SetButton(option2, opt2);
            SetButton(option3, opt3);
        }

        public void Hide()
        {
            if (panel != null)
            {
                panel.SetActive(false);
            }
        }

        private void SetButton(Button button, string label)
        {
            if (button == null)
            {
                return;
            }

            button.gameObject.SetActive(!string.IsNullOrEmpty(label));
            var text = button.GetComponentInChildren<Text>();
            if (text != null)
            {
                text.text = label;
            }
        }
    }
}
