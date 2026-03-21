using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SpeedrunManager.UI
{
    public class SpeedrunConfigPanel
    {
        public static GameObject panel;
        private static CustomSlider customSliderTimerPositionX;
        private static CustomSlider customSliderTimerPositionY;
        private static CustomSlider customSliderFontTimer;
        private static CustomSlider customSliderRunType;
        private static CustomSlider customSliderShowSplits;

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
            Image background = GameObject.Instantiate(original, panel.transform);
            background.name = "bkg";

            // Title
            GameObject originalTitle = skillsFrameTransform.Find("topic").gameObject;
            GameObject title = GameObject.Instantiate(originalTitle, panel.transform);
            title.name = "Title";
            title.GetComponent<RectTransform>().sizeDelta = new Vector2(400, 50);
            TextMeshProUGUI titleText = title.GetComponent<TextMeshProUGUI>();
            titleText.text = $"SPEEDRUN CONFIGURATION (v{SpeedrunManager.VERSION})";

            RectTransform titleRect = title.GetComponent<RectTransform>();
            titleRect.anchoredPosition = new Vector2(0, -30);

            // Close button
            Transform closeButtonTransform =
                InventoryGui.instance.m_skillsDialog.transform.Find("SkillsFrame/Closebutton");
            GameObject closebuttonGo = GameObject.Instantiate(closeButtonTransform.gameObject, panel.transform);
            closebuttonGo.name = "CloseButton";
            closebuttonGo.transform.SetParent(panel.transform, false);

            RectTransform buttonTextRect = closebuttonGo.GetComponent<RectTransform>();
            buttonTextRect.anchoredPosition = new Vector2(0, 40);

            TMP_Text buttonText = closebuttonGo.GetComponentInChildren<TMP_Text>();
            buttonText.text = Localization.instance.Localize("$menu_close");

            Button closeButton = closebuttonGo.GetComponent<Button>();
            closeButton.onClick = new Button.ButtonClickedEvent();
            closeButton.onClick.AddListener(() => { Hide(); });
            
            addTimerPositionSliders(panel.transform);
            addFontTimerSlider(panel.transform);
            
            addColorTimerSlider(panel.transform);
            addRunType(panel.transform);
            addShowSplits(panel.transform);
        }

        private static void addTimerPositionSliders(Transform parent)
        {
            //X
            customSliderTimerPositionX = new CustomSlider(
                name: "TimerPositionXSlider",
                minValue: 0,
                maxValue: 2000,
                sizeDelta: new Vector2(150, 10),
                position: new Vector2(-150, 150),
                posXIcon: 0,
                spriteName: null,
                posXDescription: -186,
                description: "X-ayis",
                posXValue: 185,
                initValue: (int)ConfigurationFile.positionTimer.Value.x,
                valueDesc: ConfigurationFile.positionTimer.Value.x.ToGlobalInvariantString()
            );
            customSliderTimerPositionX.getGameObject().transform.SetParent(parent, false);
            customSliderTimerPositionX.OnValueChanged(value =>
            {
                Logger.Log("slider changed to " + value);
                customSliderTimerPositionX.updateTextValue(value.ToGlobalInvariantString());
                ConfigurationFile.positionTimer.Value = new Vector2(value, ConfigurationFile.positionTimer.Value.y);
            });
            //Y
            customSliderTimerPositionY = new CustomSlider(
                name: "TimerPositionYSlider",
                minValue: 0,
                maxValue: 1100,
                sizeDelta: new Vector2(150, 10),
                position: new Vector2(-150, 100),
                posXIcon: 0,
                spriteName: null,
                posXDescription: -186,
                description: "Y-ayis",
                posXValue: 185,
                initValue: (int)ConfigurationFile.positionTimer.Value.y * -1,
                valueDesc: (ConfigurationFile.positionTimer.Value.y * -1).ToGlobalInvariantString()
            );
            customSliderTimerPositionY.getGameObject().transform.SetParent(parent, false);
            customSliderTimerPositionY.OnValueChanged(value =>
            {
                Logger.Log("slider changed to " + value);
                customSliderTimerPositionY.updateTextValue(value.ToGlobalInvariantString());
                ConfigurationFile.positionTimer.Value = new Vector2(ConfigurationFile.positionTimer.Value.x, value * -1);
            });
        }

        private static void addFontTimerSlider(Transform panelTransform)
        {
            customSliderFontTimer = new CustomSlider(
                name: "FontTimerSlider",
                minValue: 1,
                maxValue: 256,
                sizeDelta: new Vector2(150, 10),
                position: new Vector2(-150, 50),
                posXIcon: 0,
                spriteName: null,
                posXDescription: -186,
                description: "Size",
                posXValue: 185,
                initValue: ConfigurationFile.fontSizeTimer.Value,
                valueDesc: ConfigurationFile.fontSizeTimer.Value.ToString()
            );
            customSliderFontTimer.getGameObject().transform.SetParent(panelTransform, false);
            customSliderFontTimer.OnValueChanged(value =>
            {
                Logger.Log("slider changed to " + value);
                customSliderFontTimer.updateTextValue(value.ToGlobalInvariantString());
                ConfigurationFile.fontSizeTimer.Value = (int)value;
            });
        }

        private static void addColorTimerSlider(Transform panelTransform)
        {
            //TODO
        }

        private static void addRunType(Transform panelTransform)
        {
            customSliderRunType = new CustomSlider(
                name: "RunTypeSlider",
                minValue: 0,
                maxValue: 1,
                sizeDelta: new Vector2(25, 10),
                position: new Vector2(120, 150),
                posXIcon: -1,
                spriteName: null,
                posXDescription: -124,
                description: "Run Type",
                posXValue: 123,
                initValue: ConfigurationFile.speedrunType.Value == SpeedrunType.Permadeath ? 1 : 0,
                valueDesc: ConfigurationFile.speedrunType.Value.ToString()
            );
            customSliderRunType.getGameObject().transform.SetParent(panelTransform, false);
            customSliderRunType.OnValueChanged(value =>
            {
                Logger.Log("slider changed to " + value);
                customSliderRunType.updateTextValue(value.Equals(1f) ? nameof(SpeedrunType.Permadeath) : nameof(SpeedrunType.InfiniteLives));
                ConfigurationFile.speedrunType.Value = value.Equals(1f) ? SpeedrunType.Permadeath : SpeedrunType.InfiniteLives;
            });
        }

        private static void addShowSplits(Transform panelTransform)
        {
            customSliderShowSplits = new CustomSlider(
                name: "ShowSplitsSlider",
                minValue: 0,
                maxValue: 1,
                sizeDelta: new Vector2(25, 10),
                position: new Vector2(120, 100),
                posXIcon: -1,
                spriteName: null,
                posXDescription: -124,
                description: "Show Splits",
                posXValue: 123,
                initValue: ConfigurationFile.showSplits.Value ? 1 : 0,
                valueDesc: ConfigurationFile.showSplits.Value.ToString()
            );
            customSliderShowSplits.getGameObject().transform.SetParent(panelTransform, false);
            customSliderShowSplits.OnValueChanged(value =>
            {
                customSliderShowSplits.updateTextValue(value.Equals(1f).ToString());
                ConfigurationFile.showSplits.Value = value.Equals(1f);
            });
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