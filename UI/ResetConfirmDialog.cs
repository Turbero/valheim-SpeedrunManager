using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SpeedrunManager.UI
{
    public class ResetConfirmDialog
    {
        public GameObject resetConfirmDialog;

        public ResetConfirmDialog()
        {
            Logger.Log("Creating resetConfirmDialog...");
            resetConfirmDialog = Object.Instantiate(
                GameObject.Find("_GameMain/LoadingGUI/PixelFix/IngameGui/Menu/MenuRoot/ExitConfirm"),
                Menu.instance.transform);
            resetConfirmDialog.name = "ResetConfirmDialog";

            UIGroupHandler dialog = resetConfirmDialog.GetComponentInChildren<UIGroupHandler>();
            dialog.transform.Find("Exit").GetComponentInChildren<TextMeshProUGUI>().text = "Confirm?";

            Button[] btns = dialog.GetComponentsInChildren<Button>();
            Button btnYes = btns[0];
            btnYes.onClick = new Button.ButtonClickedEvent();
            btnYes.onClick.AddListener(() =>
            {
                Logger.Log("Yes button clicked");
                showResetDialog(false, false);
                SpeedrunTimer.ResetTimer();
            });
            Button btnNo = btns[1];
            btnNo.onClick = new Button.ButtonClickedEvent();
            btnNo.onClick.AddListener(() =>
            {
                Logger.Log("No button clicked");
                showResetDialog(false, false);
            });
            
            //Black background
            GameObject blackBackground = Object.Instantiate(
                InventoryGui.instance.m_skillsDialog.transform.Find("darken").gameObject,
                resetConfirmDialog.transform);
            blackBackground.name = "dark_en";
            blackBackground.transform.SetAsFirstSibling();
            blackBackground.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -140);
        }

        public void showResetDialog(bool show, bool impactMenu = true)
        {
            resetConfirmDialog.SetActive(show);
            if (impactMenu)
            {
                if (show)
                {
                    Menu.instance.Show();
                    Menu.instance.transform.Find("MenuRoot/Menu/MenuEntries").gameObject.SetActive(false);
                    Menu.instance.transform.Find("MenuRoot/Menu/ornament").gameObject.SetActive(false);
                }
                else
                {
                    Menu.instance.transform.Find("MenuRoot/Menu/MenuEntries").gameObject.SetActive(true);
                    Menu.instance.transform.Find("MenuRoot/Menu/ornament").gameObject.SetActive(true);
                    Menu.instance.Hide();
                }
            }
        }
    }
}
