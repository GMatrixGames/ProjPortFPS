using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsButtonHandler : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private TMP_Text forward, backward, left, right, jump, slide;
    private GameObject currKey;

    /// <summary>
    /// Setup the strings from the settings
    /// </summary>
    private void Start()
    {
        forward.text = SettingsManager.instance.settings.keyBindings?["Forward"].ToString();
        backward.text = SettingsManager.instance.settings.keyBindings?["Back"].ToString();
        left.text = SettingsManager.instance.settings.keyBindings?["Left"].ToString();
        right.text = SettingsManager.instance.settings.keyBindings?["Right"].ToString();
        jump.text = SettingsManager.instance.settings.keyBindings?["Jump"].ToString();
        slide.text = SettingsManager.instance.settings.keyBindings?["Slide"].ToString();

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

            if (e.isKey)
            {
                SettingsManager.instance.settings.keyBindings![currKey.name] = e.keyCode;
                currKey.GetComponentInChildren<TMP_Text>().text = e.keyCode.ToString();
                currKey = null;
            }
        }
    }

    public void ExitWithoutSave()
    {
        var percent = Mathf.RoundToInt(SettingsManager.instance.settings.GetVolume() * 100);
        volumeSlider.GetComponentInChildren<TMP_Text>().text = $"Volume ({percent}%)";
        volumeSlider.value = SettingsManager.instance.settings.GetVolume();

        forward.text = SettingsManager.instance.settings.keyBindings?["Forward"].ToString();
        backward.text = SettingsManager.instance.settings.keyBindings?["Back"].ToString();
        left.text = SettingsManager.instance.settings.keyBindings?["Left"].ToString();
        right.text = SettingsManager.instance.settings.keyBindings?["Right"].ToString();
        jump.text = SettingsManager.instance.settings.keyBindings?["Jump"].ToString();
        slide.text = SettingsManager.instance.settings.keyBindings?["Slide"].ToString();
    }

    public void ChangeKey(GameObject clicked)
    {
        currKey = clicked;
    }
}