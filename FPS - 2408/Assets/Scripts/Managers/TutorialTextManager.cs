using TMPro;
using UnityEngine;

namespace Managers
{
    public class TutorialTextManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text grapplingText;
        [SerializeField] private TMP_Text slideText;

        private void Start()
        {
            grapplingText.text = grapplingText.text.Replace("{GRAPPLE}", SettingsManager.instance.settings.keyBindings["Grapple"].ToString().Replace("Mouse1", "Right Click"));
            slideText.text = slideText.text.Replace("{SLIDE}", SettingsManager.instance.settings.keyBindings["Slide"].ToString()).Replace("{SPRINT}", SettingsManager.instance.settings.keyBindings["Sprint"].ToString());
        }
    }
}