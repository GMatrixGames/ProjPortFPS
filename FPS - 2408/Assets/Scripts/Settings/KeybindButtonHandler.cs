using TMPro;
using UnityEngine;

namespace Settings
{
    public class KeybindButtonHandler : MonoBehaviour
    {
        [SerializeField] private TMP_Text forward, backward, left, right, jump, slide;
        private GameObject currKey;

        /// <summary>
        /// Setup the strings from the settings
        /// </summary>
        private void Start()
        {
            forward.text = SettingsManager.instance.settings.keyBindings["Forward"].ToString();
            backward.text = SettingsManager.instance.settings.keyBindings["Back"].ToString();
            left.text = SettingsManager.instance.settings.keyBindings["Left"].ToString();
            right.text = SettingsManager.instance.settings.keyBindings["Right"].ToString();
            jump.text = SettingsManager.instance.settings.keyBindings["Jump"].ToString();
            slide.text = SettingsManager.instance.settings.keyBindings["Slide"].ToString();
        }

        private void OnGUI()
        {
            if (currKey)
            {
                var e = Event.current;

                if (e.isKey)
                {
                    SettingsManager.instance.settings.keyBindings[currKey.name] = e.keyCode;
                    currKey.GetComponentInChildren<TMP_Text>().text = e.keyCode.ToString();
                    currKey = null;
                }
            }
        }

        public void ChangeKey(GameObject clicked)
        {
            currKey = clicked;
        }

        public void SaveAndExit()
        {
            SettingsManager.instance.Save();
            GameManager.instance.OpenOptionsMenu();
        }

        public void Cancel()
        {
            SettingsManager.instance.Load(); // Load previous settings
            GameManager.instance.OpenOptionsMenu();
        }
    }
}