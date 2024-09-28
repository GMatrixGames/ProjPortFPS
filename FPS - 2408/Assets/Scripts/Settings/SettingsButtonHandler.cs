using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsButtonHandler : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private TMP_Text forward, backward, left, right, jump, slide, sprint, grapple;
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
        grapple.text = SettingsManager.instance.settings.keyBindings["Grapple"].ToString();
        sprint.text = SettingsManager.instance.settings.keyBindings["Sprint"].ToString();

        var percent = Mathf.RoundToInt(SettingsManager.instance.settings.GetVolume() * 100);
        volumeSlider.GetComponentInChildren<TMP_Text>().text = $"Volume ({percent}%)";
        volumeSlider.value = SettingsManager.instance.settings.GetVolume();
        AudioListener.volume = SettingsManager.instance.settings.GetVolume();
        volumeSlider.onValueChanged.AddListener(val =>
        {
            percent = Mathf.RoundToInt(val * 100);
            volumeSlider.GetComponentInChildren<TMP_Text>().text = $"Volume ({percent}%)";
            SettingsManager.instance.settings.volume = val;
            Debug.Log(SettingsManager.instance.settings.volume);
            AudioListener.volume = val;
        });
    }

    private void OnGUI()
    {
        if (currKey)
        {
            var e = Event.current;

            if (e.isKey || (e.isMouse && e.type == EventType.MouseDown && e.button == 1))
            {
                // You cannot (use) escape in WebGL
                if (e.keyCode == KeyCode.Escape && Application.platform == RuntimePlatform.WebGLPlayer)
                {
                    currKey = null;
                    return;
                }

                var keyCode = e.keyCode;
                if (e.isMouse && e.button == 1) keyCode = KeyCode.Mouse1;

                // Keybinds cannot be duplicated
                if (SettingsManager.instance.settings.keyBindings.Any(kb => kb.Value == keyCode))
                {
                    currKey = null;
                    return;
                }

                SettingsManager.instance.settings.keyBindings[currKey.name] = keyCode;
                currKey.GetComponentInChildren<TMP_Text>().text = keyCode.ToString();
                currKey = null;
            }
        }
    }

    public void ExitWithoutSave()
    {
        var percent = Mathf.RoundToInt(SettingsManager.instance.settings.GetVolume() * 100);
        volumeSlider.GetComponentInChildren<TMP_Text>().text = $"Volume ({percent}%)";
        volumeSlider.value = SettingsManager.instance.settings.GetVolume();

        forward.text = SettingsManager.instance.settings.keyBindings["Forward"].ToString();
        backward.text = SettingsManager.instance.settings.keyBindings["Back"].ToString();
        left.text = SettingsManager.instance.settings.keyBindings["Left"].ToString();
        right.text = SettingsManager.instance.settings.keyBindings["Right"].ToString();
        jump.text = SettingsManager.instance.settings.keyBindings["Jump"].ToString();
        slide.text = SettingsManager.instance.settings.keyBindings["Slide"].ToString();
        grapple.text = SettingsManager.instance.settings.keyBindings["Grapple"].ToString();
        sprint.text = SettingsManager.instance.settings.keyBindings["Sprint"].ToString();
    }

    public void ChangeKey(GameObject clicked)
    {
        currKey = clicked;
    }
}