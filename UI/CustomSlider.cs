using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Events;
using System.Reflection;

namespace SpeedrunManager.UI
{
    public class CustomSlider
    {
        private int initValue;
        private readonly GameObject sliderObject;
        private readonly Slider slider;
        private readonly TextMeshProUGUI sliderValue;
        public TextMeshProUGUI sliderLabelDescription;
        private Button resetButton;

        public CustomSlider(string name, int minValue, int maxValue,
                            Vector2 sizeDelta, Vector2 position,
                            int posXIcon, string spriteName,
                            int posXDescription, string description,
                            int posXValue, int initValue, string valueDesc, bool hasResetButton = false
                            )
        {
            // Main container
            sliderObject = new GameObject(name, typeof(RectTransform));

            // RectTransform
            RectTransform sliderRect = sliderObject.GetComponent<RectTransform>();
            sliderRect.sizeDelta = sizeDelta;
            sliderRect.anchoredPosition = position;

            // Slider
            slider = sliderObject.AddComponent<Slider>();
            slider.name = name;
            slider.minValue = minValue;
            slider.maxValue = maxValue;
            slider.value = initValue;
            this.initValue = initValue;
            //m_WholeNumbers = 1 makes automatically stepSize=1
            typeof(Slider).GetField("m_WholeNumbers", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(slider, true);

            //Background
            GameObject background = new GameObject("Background", typeof(RectTransform), typeof(Image));
            background.transform.SetParent(sliderObject.transform, false);
            RectTransform backgroundRect = background.GetComponent<RectTransform>();
            backgroundRect.anchorMin = new Vector2(0, 0);
            backgroundRect.anchorMax = new Vector2(1, 1);
            backgroundRect.sizeDelta = new Vector2(0, 0);
            background.GetComponent<Image>().color = Color.gray;  // Background color

            //Fill Area
            GameObject fillArea = new GameObject("Fill Area", typeof(RectTransform));
            fillArea.transform.SetParent(sliderObject.transform, false);
            RectTransform fillAreaRect = fillArea.GetComponent<RectTransform>();
            fillAreaRect.anchorMin = new Vector2(0, 0.25f);
            fillAreaRect.anchorMax = new Vector2(1, 0.75f);
            fillAreaRect.sizeDelta = new Vector2(0, 0);

            //Fill up to current value
            GameObject fill = new GameObject("Fill", typeof(RectTransform), typeof(Image));
            fill.transform.SetParent(fillArea.transform, false);
            RectTransform fillRect = fill.GetComponent<RectTransform>();
            fillRect.anchorMin = new Vector2(0, 0);
            fillRect.anchorMax = new Vector2(1, 1);
            fillRect.sizeDelta = new Vector2(0, 0);
            fill.GetComponent<Image>().color = Color.green;  // Fill color
            slider.fillRect = fillRect;

            //Handle to manipulate value
            GameObject handle = new GameObject("Handle", typeof(RectTransform), typeof(Image));
            handle.transform.SetParent(sliderObject.transform, false);
            RectTransform handleRect = handle.GetComponent<RectTransform>();
            handleRect.sizeDelta = new Vector2(10, 10);
            handle.GetComponent<Image>().color = Color.white;  // Handle color
            slider.targetGraphic = handle.GetComponent<Image>();
            slider.handleRect = handleRect;

            //Icon
            if (spriteName != null)
            {
                GameObject iconObject = new GameObject("Icon");
                Image iconImage = iconObject.AddComponent<Image>();
                iconImage.sprite = ModUtils.getSprite(spriteName);
                RectTransform iconRect = iconObject.GetComponent<RectTransform>();
                iconRect.SetParent(sliderObject.transform, false);
                iconRect.sizeDelta = new Vector2(25, 25);
                iconRect.anchoredPosition = new Vector2(posXIcon, 0);
            }

            //Text
            if (description != null)
            {
                GameObject textObject = new GameObject("SliderLabel", typeof(RectTransform), typeof(TextMeshProUGUI));
                textObject.transform.SetParent(sliderObject.transform, false);
                RectTransform textRect = textObject.GetComponent<RectTransform>();
                textRect.anchoredPosition = new Vector2(posXDescription, 0);
                sliderLabelDescription = textObject.GetComponent<TextMeshProUGUI>();
                sliderLabelDescription.text = description;
                sliderLabelDescription.fontSize = 18;
                sliderLabelDescription.alignment = TextAlignmentOptions.Right;
                sliderLabelDescription.font = ModUtils.getFontAsset("Valheim-AveriaSansLibre");
            }

            //Value
            if (initValue >= 0)
            {
                GameObject textObject = new GameObject("SliderValue", typeof(RectTransform), typeof(TextMeshProUGUI));
                textObject.transform.SetParent(sliderObject.transform, false);
                RectTransform textRect = textObject.GetComponent<RectTransform>();
                textRect.anchoredPosition = new Vector2(posXValue, 0);
                sliderValue = textObject.GetComponent<TextMeshProUGUI>();
                sliderValue.fontSize = 18;
                sliderValue.font = ModUtils.getFontAsset("Valheim-AveriaSansLibre");
                sliderValue.alignment = TextAlignmentOptions.Left;
                sliderValue.text = valueDesc;
            }
            
            //Reset button
            if (hasResetButton) {
                resetButton = Object.Instantiate(InventoryGui.instance.m_takeAllButton, sliderObject.transform);
                resetButton.name = "ResetButton";
                ControllerUtils.HideGamePad(resetButton.transform);
                resetButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(140, 0);
                resetButton.GetComponent<RectTransform>().sizeDelta = new Vector2(30, 30);
                resetButton.GetComponentInChildren<TextMeshProUGUI>().text = "R";
                resetButton.onClick = new Button.ButtonClickedEvent();
                resetButton.onClick.AddListener(() =>
                {
                    updateValue(this.initValue);
                    updateTextValue(this.initValue.ToString());
                });
                UITooltip resetTooltip = resetButton.gameObject.AddComponent<UITooltip>();
                resetTooltip.m_tooltipPrefab = Object.Instantiate(
                    InventoryGui.instance.transform.Find("root/Info/Skills").GetComponent<UITooltip>().m_tooltipPrefab);
                resetTooltip.m_text = "Reset";
            }

        }

        public GameObject getGameObject()
        {
            return sliderObject;
        }
        public void OnValueChanged(UnityAction<float> call)
        {
            slider.onValueChanged = new Slider.SliderEvent();
            slider.onValueChanged.AddListener(call);
        }

        public void updateTextValue(string value)
        {
            sliderValue.text = value;
        }
        
        public void updateValue(float value)
        {
            slider.value = value;
        }

        public float getValue()
        {
            return slider.value;
        }
    }
}
