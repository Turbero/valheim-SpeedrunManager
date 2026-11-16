using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SpeedrunManager.UI
{
    public class SpeedrunConfigPanel
    {
        public static GameObject panel;
        private static CustomSlider customSliderShowTimer;
        private static CustomInputField customInputTimerPositionX;
        private static CustomInputField customInputTimerPositionY;
        private static CustomInputField customInputFontTimer;
        private static CustomInputField customTimerSplitsPositionX;
        private static CustomInputField customTimerSplitsPositionY;
        private static CustomSlider customSliderSplitsColumnSize;
        private static CustomInputField customSliderSplitsColumnsSpace;
        private static CustomInputField customSliderSplitsRowsSpace;
        private static CustomSlider customSliderFontSplits;

        private static CustomSlider customSliderRunType;
        private static CustomSlider customSliderShowSplits;

        private static CustomInputField effectsPerRow;

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
            
            //Input fields tests
            effectsPerRow = new CustomInputField(
                id: "EffectsPerRowInput",
                title: "Effects Per Row",
                sizeDelta: new Vector2(80, 30),
                position: new Vector2(-376, 50),
                inputPosition: new Vector2(150, -2),
                buttonPosition: new Vector2(326, -1),
                value: ConfigurationFile.effectsPerRow.Value.ToString(),
                initValue: DefaultUIPositions.effectsPerRow.ToString(),
                contentType: TMP_InputField.ContentType.IntegerNumber,
                characterLimit: 1,
                hasResetButton: true
            );
            effectsPerRow.getGameObject().transform.SetParent(panel.transform, false);
            effectsPerRow.OnValueChanged(value =>
            {
                if (int.TryParse(value, out var result))
                {
                    Logger.Log("inputField changed to " + value);
                    ConfigurationFile.effectsPerRow.Value = result;
                }
            });

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
            customInputTimerPositionX = new CustomInputField(
                id: "TimerPositionXInput",
                title: "Timer X-ayis",
                sizeDelta: new Vector2(80, 30),
                position: new Vector2(-376, 140),
                inputPosition: new Vector2(150, -2),
                buttonPosition: new Vector2(326, -1),
                value: ConfigurationFile.positionTimer.Value.x.ToGlobalInvariantString(),
                contentType: TMP_InputField.ContentType.IntegerNumber,
                characterLimit: 5,
                initValue: DefaultUIPositions.positionTimer.x.ToGlobalInvariantString(),
                hasResetButton: true
            );
            customInputTimerPositionX.getGameObject().transform.SetParent(panel.transform, false);
            customInputTimerPositionX.OnValueChanged(value =>
            {
                Logger.Log("input changed to " + value);
                customInputTimerPositionX.updateTextValue(value);
                ConfigurationFile.positionTimer.Value = new Vector2(float.Parse(value), ConfigurationFile.positionTimer.Value.y);
            });
            //Y
            customInputTimerPositionY = new CustomInputField(
                id: "TimerPositionYInput",
                title: "Timer Y-ayis",
                sizeDelta: new Vector2(80, 30),
                position: new Vector2(-376, 110),
                inputPosition: new Vector2(150, -2),
                buttonPosition: new Vector2(326, -1),
                value: ConfigurationFile.positionTimer.Value.y.ToGlobalInvariantString(),
                contentType: TMP_InputField.ContentType.IntegerNumber,
                initValue: DefaultUIPositions.positionTimer.y.ToGlobalInvariantString(),
                hasResetButton: true
            );
            customInputTimerPositionY.getGameObject().transform.SetParent(panel.transform, false);
            customInputTimerPositionY.OnValueChanged(value =>
            {
                Logger.Log("input changed to " + value);
                customInputTimerPositionY.updateTextValue(value);
                ConfigurationFile.positionTimer.Value = new Vector2(ConfigurationFile.positionTimer.Value.x, float.Parse(value));
            });
            customInputFontTimer = new CustomInputField(
                id: "FontTimerSlider",
                title: "Timer Size",
                sizeDelta: new Vector2(80, 30),
                position: new Vector2(-376, 80),
                inputPosition: new Vector2(150, -2),
                buttonPosition: new Vector2(326, -1),
                value: ConfigurationFile.fontSizeTimer.Value.ToString(),
                contentType: TMP_InputField.ContentType.IntegerNumber,
                initValue: DefaultUIPositions.fontSizeTimer.ToString(),
                hasResetButton: true
            );
            customInputFontTimer.getGameObject().transform.SetParent(panel.transform, false);
            customInputFontTimer.OnValueChanged(value =>
            {
                Logger.Log("slider changed to " + value);
                customInputFontTimer.updateTextValue(value);
                ConfigurationFile.fontSizeTimer.Value = int.Parse(value);
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
            customTimerSplitsPositionX = new CustomInputField(
                id: "SplitsPositionXTimer",
                title: "Splits X-ayis",
                sizeDelta: new Vector2(80, 30),
                position: new Vector2(54, 170),
                inputPosition: new Vector2(150, -2),
                buttonPosition: new Vector2(326, -1),
                contentType: TMP_InputField.ContentType.IntegerNumber,
                value: ConfigurationFile.positionSplits.Value.x.ToGlobalInvariantString(),
                initValue: DefaultUIPositions.positionSplits.x.ToGlobalInvariantString(),
                hasResetButton: true
            );
            customTimerSplitsPositionX.getGameObject().transform.SetParent(panel.transform, false);
            customTimerSplitsPositionX.OnValueChanged(value =>
            {
                Logger.Log("input changed to " + value);
                customTimerSplitsPositionX.updateTextValue(value);
                ConfigurationFile.positionSplits.Value = new Vector2(float.Parse(value), ConfigurationFile.positionSplits.Value.y);
            });
            //Y
            customTimerSplitsPositionY = new CustomInputField(
                id: "SplitsPositionYTimer",
                title: "Splits Y-ayis",
                sizeDelta: new Vector2(80, 30),
                position: new Vector2(54, 140),
                inputPosition: new Vector2(150, -2),
                buttonPosition: new Vector2(326, -1),
                contentType: TMP_InputField.ContentType.IntegerNumber,
                initValue: DefaultUIPositions.positionSplits.y.ToGlobalInvariantString(),
                value: ConfigurationFile.positionSplits.Value.y.ToGlobalInvariantString(),
                hasResetButton: true
            );
            customTimerSplitsPositionY.getGameObject().transform.SetParent(panel.transform, false);
            customTimerSplitsPositionY.OnValueChanged(value =>
            {
                Logger.Log("input changed to " + value);
                customTimerSplitsPositionY.updateTextValue(value);
                ConfigurationFile.positionSplits.Value = new Vector2(ConfigurationFile.positionSplits.Value.x, float.Parse(value));
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
                initValue: DefaultUIPositions.splitsColumnSize,
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
            
            customSliderSplitsColumnsSpace = new CustomInputField(
                id: "SplitsColumnsSpace",
                title: "Splits Columns Space",
                sizeDelta: new Vector2(80, 30),
                position: new Vector2(54, 80),
                inputPosition: new Vector2(150, -2),
                buttonPosition: new Vector2(326, -1),
                contentType: TMP_InputField.ContentType.IntegerNumber,
                initValue: DefaultUIPositions.splitsColumnsSpace.ToString(),
                value: ConfigurationFile.splitsColumnsSpace.Value.ToString(),
                hasResetButton: true
            );
            customSliderSplitsColumnsSpace.getGameObject().transform.SetParent(panel.transform, false);
            customSliderSplitsColumnsSpace.OnValueChanged(value =>
            {
                Logger.Log("slider changed to " + value);
                customSliderSplitsColumnsSpace.updateTextValue(value);
                ConfigurationFile.splitsColumnsSpace.Value = int.Parse(value) + 104;
            });
            customSliderSplitsRowsSpace = new CustomInputField(
                id: "SplitsRowsSpace",
                title: "Splits Rows Space",
                sizeDelta: new Vector2(80, 30),
                position: new Vector2(54, 50),
                inputPosition: new Vector2(150, -2),
                buttonPosition: new Vector2(326, -1),
                contentType: TMP_InputField.ContentType.IntegerNumber,
                initValue: DefaultUIPositions.splitsRowsSpace.ToString(),
                value: ConfigurationFile.splitsRowsSpace.Value.ToString(),
                hasResetButton: true
            );
            customSliderSplitsRowsSpace.getGameObject().transform.SetParent(panel.transform, false);
            customSliderSplitsRowsSpace.OnValueChanged(value =>
            {
                Logger.Log("slider changed to " + value);
                customSliderSplitsRowsSpace.updateTextValue(value);
                ConfigurationFile.splitsRowsSpace.Value = int.Parse(value);
            });
            
            customSliderFontSplits = new CustomSlider(
                name: "FontSplitsSlider",
                minValue: 1,
                maxValue: 256,
                sizeDelta: new Vector2(150, 10),
                position: new Vector2(54, 20),
                posXIcon: 0,
                spriteName: null,
                posXDescription: -186,
                description: "Splits Size",
                posXValue: 185,
                initValue: DefaultUIPositions.fontSizeSplits,
                valueDesc: ConfigurationFile.fontSizeSplits.Value.ToString(),
                hasResetButton: true
            );
            //TODO
            //customSliderFontSplits.getGameObject().transform.SetParent(panel.transform, false);
            customSliderFontSplits.OnValueChanged(value =>
            {
                Logger.Log("slider changed to " + value);
                customSliderFontSplits.updateTextValue(value.ToGlobalInvariantString());
                ConfigurationFile.fontSizeSplits.Value = (int)value;
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