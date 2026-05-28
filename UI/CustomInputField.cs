using GUIFramework;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Events;

namespace SpeedrunManager.UI
{
    public class CustomInputField
    {
        private string initValue;
        private readonly GameObject inputFieldObject;
        private readonly GameObject inputField;
        private readonly TextMeshProUGUI inputFieldValue;
        public TextMeshProUGUI labelDescription;
        private Button resetButton;

        public CustomInputField(string id, 
                            Vector2 sizeDeltaField,
                            Vector2 position,
                            string title,
                            string value,
                            string initValue, 
                            TMP_InputField.ContentType contentType = TMP_InputField.ContentType.Standard,
                            int characterLimit = 0,
                            bool hasResetButton = false
                            )
        {
            // Main container
            inputFieldObject = new GameObject(id, typeof(RectTransform));

            // RectTransform
            RectTransform inputFieldObjectRect = inputFieldObject.GetComponent<RectTransform>();
            inputFieldObjectRect.anchoredPosition = position;

            // InputField
            GameObject textFieldInput = GameObject.Find("_GameMain/LoadingGUI/PixelFix/IngameGui/TextInput/panel/TextField");
            inputField = Object.Instantiate(textFieldInput, inputFieldObject.transform);
            inputField.name = "TextField";
            inputField.GetComponent<RectTransform>().sizeDelta = sizeDeltaField;
            inputField.GetComponent<RectTransform>().anchoredPosition = new Vector2(145, 0);
            
            //Input rules
            inputField.GetComponent<GuiInputField>().contentType = contentType;
            if (characterLimit > 0)
                inputField.GetComponent<GuiInputField>().characterLimit = characterLimit;
            
            //Text
            if (title != null)
            {
                GameObject textObject = new GameObject("InputLabel", typeof(RectTransform), typeof(TextMeshProUGUI));
                textObject.transform.SetParent(inputFieldObject.transform, false);
                //RectTransform textRect = textObject.GetComponent<RectTransform>();
                //textRect.anchoredPosition = new Vector2(posXDescription, 0);
                labelDescription = textObject.GetComponent<TextMeshProUGUI>();
                labelDescription.text = title;
                labelDescription.fontSize = 18;
                labelDescription.alignment = TextAlignmentOptions.Right;
                labelDescription.font = ModUtils.getFontAsset("Valheim-AveriaSansLibre");
            }

            //Value
            this.initValue = initValue;
            inputFieldValue = inputField.GetComponentInChildren<TextMeshProUGUI>();
            inputFieldValue.fontSize = 18;
            inputFieldValue.font = ModUtils.getFontAsset("Valheim-AveriaSansLibre");
            inputFieldValue.alignment = TextAlignmentOptions.Left;
            inputFieldValue.text = value;
            
            //Reset button
            if (hasResetButton) {
                resetButton = Object.Instantiate(InventoryGui.instance.m_takeAllButton, inputFieldObject.transform);
                resetButton.name = "ResetButton";
                ControllerUtils.RemoveHint(resetButton.transform);
                resetButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(140, 0);
                resetButton.GetComponent<RectTransform>().sizeDelta = new Vector2(30, 30);
                resetButton.GetComponentInChildren<TextMeshProUGUI>().text = "R";
                resetButton.onClick = new Button.ButtonClickedEvent();
                resetButton.onClick.AddListener(() =>
                {
                    updateTextValue(this.initValue);
                });
                UITooltip resetTooltip = resetButton.gameObject.AddComponent<UITooltip>();
                resetTooltip.m_tooltipPrefab = Object.Instantiate(
                    InventoryGui.instance.transform.Find("root/Info/Skills").GetComponent<UITooltip>().m_tooltipPrefab);
                resetTooltip.m_text = "Reset";
            }

        }

        public GameObject getGameObject()
        {
            return inputFieldObject;
        }
        public void OnValueChanged(UnityAction<string> call)
        {
            inputField.GetComponentInChildren<GuiInputField>().onValueChanged = new TMP_InputField.OnChangeEvent();
            inputField.GetComponentInChildren<GuiInputField>().onValueChanged.AddListener(call);
        }

        public void updateTextValue(string value)
        {
            inputFieldValue.text = value;
        }

        public string getValue()
        {
            return inputFieldValue.text;
        }
    }
}
