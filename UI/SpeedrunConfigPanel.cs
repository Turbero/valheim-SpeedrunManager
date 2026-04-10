using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SpeedrunManager.UI
{
    public class SpeedrunConfigPanel
    {
        public static GameObject panel;
        private static CustomSlider customSliderShowTimer;
        private static CustomSlider customSliderTimerPositionX;
        private static CustomSlider customSliderTimerPositionY;
        private static CustomSlider customSliderFontTimer;
        private static CustomSlider customSliderSplitsPositionX;
        private static CustomSlider customSliderSplitsPositionY;
        private static CustomSlider customSliderSplitsColumnSize;
        private static CustomSlider customSliderSplitsColumnsSpace;
        private static CustomSlider customSliderSplitsRowsSpace;
        //private static CustomSlider customSliderFontSplits;

        private static CustomSlider customSliderRunType;
        private static CustomSlider customSliderShowSplits;

        private static ResetConfirmDialog resetConfirmDialog;

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
            panelRect.sizeDelta = new Vector2(820, 550);
            panelRect.anchoredPosition = new Vector2(0, 0); // (0,0) = centered on screen

            // Background
            Image original = skillsFrameTransform.Find("bkg").GetComponent<Image>();
            Image background = Object.Instantiate(original, panel.transform);
            background.name = "bkg";

            // Title
            GameObject originalTitle = skillsFrameTransform.Find("topic").gameObject;
            GameObject title = Object.Instantiate(originalTitle, panel.transform);
            title.name = "Title";
            title.GetComponent<RectTransform>().sizeDelta = new Vector2(400, 50);
            TextMeshProUGUI titleText = title.GetComponent<TextMeshProUGUI>();
            titleText.text = $"SPEEDRUN CONFIGURATION (v{SpeedrunManager.VERSION})";

            RectTransform titleRect = title.GetComponent<RectTransform>();
            titleRect.anchoredPosition = new Vector2(0, -30);

            // Close button
            Transform closeButtonTransform =
                InventoryGui.instance.m_skillsDialog.transform.Find("SkillsFrame/Closebutton");
            GameObject closeButtonGo = Object.Instantiate(closeButtonTransform.gameObject, panel.transform);
            closeButtonGo.name = "CloseButton";
            closeButtonGo.transform.SetParent(panel.transform, false);
            ControllerUtils.BindGamePad(closeButtonGo.transform, KeyCode.JoystickButton1);

            RectTransform buttonTextRect = closeButtonGo.GetComponent<RectTransform>();
            buttonTextRect.anchoredPosition = new Vector2(200, 40);

            TMP_Text buttonText = closeButtonGo.GetComponentInChildren<TMP_Text>();
            buttonText.text = Localization.instance.Localize("$menu_close");

            Button closeButton = closeButtonGo.GetComponent<Button>();
            closeButton.onClick = new Button.ButtonClickedEvent();
            closeButton.onClick.AddListener(() => { resetConfirmDialog.showResetDialog(false); Hide(); });
            
            // Reset button
            GameObject resetButtonGo = Object.Instantiate(closeButtonGo.gameObject, panel.transform);
            resetButtonGo.name = "ResetButton";
            resetButtonGo.transform.SetParent(panel.transform, false);
            ControllerUtils.BindGamePad(resetButtonGo.transform, KeyCode.JoystickButton2);

            RectTransform resetButtonTextRect = resetButtonGo.GetComponent<RectTransform>();
            resetButtonTextRect.anchoredPosition = new Vector2(-200, 40);

            TMP_Text resetButtonText = resetButtonGo.GetComponentInChildren<TMP_Text>();
            resetButtonText.text = "Reset";

            Button resetButton = resetButtonGo.GetComponent<Button>();
            resetButton.onClick = new Button.ButtonClickedEvent();
            resetButton.onClick.AddListener(() => { resetConfirmDialog.showResetDialog(true, false); });
            
            addTimerSliders();
            addSplitsSliders();

            //Reset dialog
            resetConfirmDialog = new ResetConfirmDialog();
        }

        private static void addTimerSliders()
        {
            customSliderShowTimer = new CustomSlider(
                name: "ShowTimerSlider",
                minValue: 0,
                maxValue: 1,
                sizeDelta: new Vector2(25, 10),
                position: new Vector2(-272, 200),
                posXIcon: -1,
                spriteName: null,
                posXDescription: -124,
                description: "Show Timer",
                posXValue: 123,
                initValue: ConfigurationFile.showTimer.Value ? 1 : 0,
                valueDesc: ConfigurationFile.showTimer.Value.ToString()
            );
            customSliderShowTimer.getGameObject().transform.SetParent(panel.transform, false);
            customSliderShowTimer.OnValueChanged(value =>
            {
                customSliderShowTimer.updateTextValue(value.Equals(1f).ToString());
                ConfigurationFile.showTimer.Value = value.Equals(1f);
            });
            customSliderRunType = new CustomSlider(
                name: "RunTypeSlider",
                minValue: 0,
                maxValue: 1,
                sizeDelta: new Vector2(25, 10),
                position: new Vector2(-272, 170),
                posXIcon: -1,
                spriteName: null,
                posXDescription: -124,
                description: "Run Type",
                posXValue: 123,
                initValue: ConfigurationFile.speedrunType.Value == SpeedrunType.Permadeath ? 1 : 0,
                valueDesc: ConfigurationFile.speedrunType.Value.ToString()
            );
            customSliderRunType.getGameObject().transform.SetParent(panel.transform, false);
            customSliderRunType.OnValueChanged(value =>
            {
                Logger.Log("slider changed to " + value);
                customSliderRunType.updateTextValue(value.Equals(1f) ? nameof(SpeedrunType.Permadeath) : nameof(SpeedrunType.InfiniteLives));
                ConfigurationFile.speedrunType.Value = value.Equals(1f) ? SpeedrunType.Permadeath : SpeedrunType.InfiniteLives;
            });
            //X
            customSliderTimerPositionX = new CustomSlider(
                name: "TimerPositionXSlider",
                minValue: 0,
                maxValue: 2000,
                sizeDelta: new Vector2(150, 10),
                position: new Vector2(-210, 140),
                posXIcon: 0,
                spriteName: null,
                posXDescription: -186,
                description: "Timer X-ayis",
                posXValue: 185,
                initValue: (int)ConfigurationFile.positionTimer.Value.x,
                valueDesc: ConfigurationFile.positionTimer.Value.x.ToGlobalInvariantString(),
                hasResetButton: true
            );
            customSliderTimerPositionX.getGameObject().transform.SetParent(panel.transform, false);
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
                position: new Vector2(-210, 110),
                posXIcon: 0,
                spriteName: null,
                posXDescription: -186,
                description: "Timer Y-ayis",
                posXValue: 185,
                initValue: (int)ConfigurationFile.positionTimer.Value.y,
                valueDesc: ConfigurationFile.positionTimer.Value.y.ToGlobalInvariantString(),
                hasResetButton: true
            );
            customSliderTimerPositionY.getGameObject().transform.SetParent(panel.transform, false);
            customSliderTimerPositionY.OnValueChanged(value =>
            {
                Logger.Log("slider changed to " + value);
                customSliderTimerPositionY.updateTextValue(value.ToGlobalInvariantString());
                ConfigurationFile.positionTimer.Value = new Vector2(ConfigurationFile.positionTimer.Value.x, value);
            });
            customSliderFontTimer = new CustomSlider(
                name: "FontTimerSlider",
                minValue: 1,
                maxValue: 256,
                sizeDelta: new Vector2(150, 10),
                position: new Vector2(-210, 80),
                posXIcon: 0,
                spriteName: null,
                posXDescription: -186,
                description: "Timer Size",
                posXValue: 185,
                initValue: ConfigurationFile.fontSizeTimer.Value,
                valueDesc: ConfigurationFile.fontSizeTimer.Value.ToString(),
                hasResetButton: true
            );
            customSliderFontTimer.getGameObject().transform.SetParent(panel.transform, false);
            customSliderFontTimer.OnValueChanged(value =>
            {
                Logger.Log("slider changed to " + value);
                customSliderFontTimer.updateTextValue(value.ToGlobalInvariantString());
                ConfigurationFile.fontSizeTimer.Value = (int)value;
            });
            
            //TODO ColorTimerSlider
            
        }

        private static void addSplitsSliders()
        {
            customSliderShowSplits = new CustomSlider(
                name: "ShowSplitsSlider",
                minValue: 0,
                maxValue: 1,
                sizeDelta: new Vector2(25, 10),
                position: new Vector2(178, 200),
                posXIcon: -1,
                spriteName: null,
                posXDescription: -124,
                description: "Show Splits",
                posXValue: 123,
                initValue: ConfigurationFile.showSplits.Value ? 1 : 0,
                valueDesc: ConfigurationFile.showSplits.Value.ToString()
            );
            customSliderShowSplits.getGameObject().transform.SetParent(panel.transform, false);
            customSliderShowSplits.OnValueChanged(value =>
            {
                customSliderShowSplits.updateTextValue(value.Equals(1f).ToString());
                ConfigurationFile.showSplits.Value = value.Equals(1f);
            });
            //X
            customSliderSplitsPositionX = new CustomSlider(
                name: "SplitsPositionXSlider",
                minValue: 0,
                maxValue: 1800,
                sizeDelta: new Vector2(150, 10),
                position: new Vector2(240, 170),
                posXIcon: 0,
                spriteName: null,
                posXDescription: -186,
                description: "Splits X-ayis",
                posXValue: 185,
                initValue: (int)ConfigurationFile.positionSplits.Value.x,
                valueDesc: ConfigurationFile.positionSplits.Value.x.ToGlobalInvariantString(),
                hasResetButton: true
            );
            customSliderSplitsPositionX.getGameObject().transform.SetParent(panel.transform, false);
            customSliderSplitsPositionX.OnValueChanged(value =>
            {
                Logger.Log("slider changed to " + value);
                customSliderSplitsPositionX.updateTextValue(value.ToGlobalInvariantString());
                ConfigurationFile.positionSplits.Value = new Vector2(value, ConfigurationFile.positionSplits.Value.y);
            });
            //Y
            customSliderSplitsPositionY = new CustomSlider(
                name: "SplitsPositionYSlider",
                minValue: 0,
                maxValue: 960,
                sizeDelta: new Vector2(150, 10),
                position: new Vector2(240, 140),
                posXIcon: 0,
                spriteName: null,
                posXDescription: -186,
                description: "Splits Y-ayis",
                posXValue: 185,
                initValue: (int)ConfigurationFile.positionSplits.Value.y,
                valueDesc: ConfigurationFile.positionSplits.Value.y.ToGlobalInvariantString(),
                hasResetButton: true
            );
            customSliderSplitsPositionY.getGameObject().transform.SetParent(panel.transform, false);
            customSliderSplitsPositionY.OnValueChanged(value =>
            {
                Logger.Log("slider changed to " + value);
                customSliderSplitsPositionY.updateTextValue(value.ToGlobalInvariantString());
                ConfigurationFile.positionSplits.Value = new Vector2(ConfigurationFile.positionSplits.Value.x, value);
            });
            
            customSliderSplitsColumnSize = new CustomSlider(
                name: "SplitsColumnSize",
                minValue: 4,
                maxValue: 8,
                sizeDelta: new Vector2(150, 10),
                position: new Vector2(240, 110),
                posXIcon: 0,
                spriteName: null,
                posXDescription: -186,
                description: "Splits Column Size",
                posXValue: 185,
                initValue: ConfigurationFile.splitsColumnSize.Value,
                valueDesc: ConfigurationFile.splitsColumnSize.Value.ToString(),
                hasResetButton: true
            );
            customSliderSplitsColumnSize.getGameObject().transform.SetParent(panel.transform, false);
            customSliderSplitsColumnSize.OnValueChanged(value =>
            {
                Logger.Log("slider changed to " + value);
                customSliderSplitsColumnSize.updateTextValue(value.ToGlobalInvariantString());
                ConfigurationFile.splitsColumnSize.Value = (int)value;
            });
            
            customSliderSplitsColumnsSpace = new CustomSlider(
                name: "SplitsColumnsSpace",
                minValue: 0,
                maxValue: 2000,
                sizeDelta: new Vector2(150, 10),
                position: new Vector2(240, 80),
                posXIcon: 0,
                spriteName: null,
                posXDescription: -186,
                description: "Splits Columns Space",
                posXValue: 185,
                initValue: ConfigurationFile.splitsColumnsSpace.Value,
                valueDesc: ConfigurationFile.splitsColumnsSpace.Value.ToString(),
                hasResetButton: true
            );
            customSliderSplitsColumnsSpace.getGameObject().transform.SetParent(panel.transform, false);
            customSliderSplitsColumnsSpace.OnValueChanged(value =>
            {
                Logger.Log("slider changed to " + value);
                customSliderSplitsColumnsSpace.updateTextValue(value.ToGlobalInvariantString());
                ConfigurationFile.splitsColumnsSpace.Value = (int) (value + 104);
            });
            customSliderSplitsRowsSpace = new CustomSlider(
                name: "SplitsRowsSpace",
                minValue: 40,
                maxValue: 256,
                sizeDelta: new Vector2(150, 10),
                position: new Vector2(240, 50),
                posXIcon: 0,
                spriteName: null,
                posXDescription: -186,
                description: "Splits Rows Space",
                posXValue: 185,
                initValue: ConfigurationFile.splitsRowsSpace.Value,
                valueDesc: ConfigurationFile.splitsRowsSpace.Value.ToString(),
                hasResetButton: true
            );
            customSliderSplitsRowsSpace.getGameObject().transform.SetParent(panel.transform, false);
            customSliderSplitsRowsSpace.OnValueChanged(value =>
            {
                Logger.Log("slider changed to " + value);
                customSliderSplitsRowsSpace.updateTextValue(value.ToGlobalInvariantString());
                ConfigurationFile.splitsRowsSpace.Value = (int)value;
            });
            
            //TODO
            /*customSliderFontSplits = new CustomSlider(
                name: "FontSplitsSlider",
                minValue: 1,
                maxValue: 256,
                sizeDelta: new Vector2(150, 10),
                position: new Vector2(240, 20),
                posXIcon: 0,
                spriteName: null,
                posXDescription: -186,
                description: "Splits Size",
                posXValue: 185,
                initValue: ConfigurationFile.fontSizeSplits.Value,
                valueDesc: ConfigurationFile.fontSizeSplits.Value.ToString(),
                hasResetButton: true
            );
            customSliderFontSplits.getGameObject().transform.SetParent(panel.transform, false);
            customSliderFontSplits.OnValueChanged(value =>
            {
                Logger.Log("slider changed to " + value);
                customSliderFontSplits.updateTextValue(value.ToGlobalInvariantString());
                ConfigurationFile.fontSizeSplits.Value = (int)value;
            });*/
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
            resetConfirmDialog.resetConfirmDialog.SetActive(false);
        }

        public static void Hide(bool hideMenu = true)
        {
            panel.SetActive(false);
            Hud.instance.transform.Find("hudroot").gameObject.SetActive(true);
            resetConfirmDialog.showResetDialog(false);
            if (hideMenu)
                Menu.instance.Hide();
        }
    }
}