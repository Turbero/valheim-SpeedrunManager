using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SpeedrunManager.UI
{
    public class SpeedrunConfigPanel
    {
        public static GameObject panel;

        public static void Create()
        {
            Transform skillsFrameTransform =
                InventoryGui.instance.m_skillsDialog.transform.Find("SkillsFrame").transform;

            // Panel
            panel = new GameObject("SpeedrunConfigPanel", typeof(RectTransform));
            panel.SetActive(false);
            panel.transform.SetParent(Menu.instance.transform, false);
            //panel.transform.SetAsFirstSibling();

            RectTransform panelRect = panel.GetComponent<RectTransform>();
            panelRect.sizeDelta = new Vector2(600, 600);
            panelRect.anchoredPosition = new Vector2(0, 0); // (0,0) = centered on screen

            // Background
            Image original = skillsFrameTransform.Find("bkg").GetComponent<Image>();
            Image clone = GameObject.Instantiate(original, panel.transform);
            clone.name = "bkg";

            // Title
            GameObject originalTitle = skillsFrameTransform.Find("topic").gameObject;
            GameObject titleClone = GameObject.Instantiate(originalTitle, panel.transform);
            titleClone.name = "Title";
            TextMeshProUGUI titleText = titleClone.GetComponent<TextMeshProUGUI>();
            titleText.text = "SPEEDRUN CONFIGURATION";

            RectTransform titleRect = titleClone.GetComponent<RectTransform>();
            titleRect.anchoredPosition = new Vector2(0, -30);

            // Close button
            Transform closeButtonTransform =
                InventoryGui.instance.m_skillsDialog.transform.Find("SkillsFrame/Closebutton");
            GameObject buttonTextObject = GameObject.Instantiate(closeButtonTransform.gameObject, panel.transform);
            buttonTextObject.name = "CloseButton";
            buttonTextObject.transform.SetParent(panel.transform, false);

            RectTransform buttonTextRect = buttonTextObject.GetComponent<RectTransform>();
            buttonTextRect.anchoredPosition = new Vector2(0, 40);

            TMP_Text buttonText = buttonTextObject.GetComponentInChildren<TMP_Text>();
            buttonText.text = Localization.instance.Localize("$menu_close");

            Button closeButton = buttonTextObject.GetComponent<Button>();
            closeButton.onClick = new Button.ButtonClickedEvent();
            closeButton.onClick.AddListener(() => { Hide(); });
            
            //Test InputField
            /*TMP_InputField tmpInputField = panel.AddComponent<TMP_InputField>();
            tmpInputField.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
            tmpInputField.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);*/
        }

        public static bool IsCreated()
        {
            return panel != null;
        }

        public static bool IsVisible()
        {
            return panel != null && panel.activeSelf;
        }

        public static void Show()
        {
            panel.SetActive(true);
            Hud.instance.transform.Find("hudroot").gameObject.SetActive(false);
            Menu.instance.Show();
        }

        public static void Hide(bool hideMenu = true)
        {
            panel.SetActive(false);
            Hud.instance.transform.Find("hudroot").gameObject.SetActive(true);
            if (hideMenu)
                Menu.instance.Hide();
        }
    }
}