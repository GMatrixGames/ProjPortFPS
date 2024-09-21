using System;
using System.IO;
using JetBrains.Annotations;
using UnityEngine;
using Utils;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager instance;

    [Header("Settings")]
    public GameSettings settings;

    private string settingsPath;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
            // DontDestroyOnLoad(gameObject); // This is static across ALL scenes
            settingsPath = Path.Combine(Application.persistentDataPath, "settings.json");
            Load(); // Load settings on awake
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Save()
    {
        var json = JsonUtility.ToJson(settings, true);
        File.WriteAllText(settingsPath, json);
    }

    public void Load()
    {
        if (File.Exists(settingsPath))
        {
            var json = File.ReadAllText(settingsPath);
            settings = JsonUtility.FromJson<GameSettings>(json);

            var defaultSettings = new GameSettings();
            if (settings.version is not GameSettings.LATEST_VERSION) // Value doesn't exist or version doesn't match
            {
                settings.version = GameSettings.LATEST_VERSION;
                settings.volume = settings.volume == default ? 0.5f : settings.volume;
                settings.keyBindings ??= defaultSettings.keyBindings;

                foreach (var (action, code) in defaultSettings.keyBindings!)
                {
                    settings.keyBindings?.TryAdd(action, code);
                }

                Save(); // Save if default settings were updated
            }
        }
        else
        {
            settings = new GameSettings
            {
                version = GameSettings.LATEST_VERSION,
                volume = 0.5f
            };

            Save(); // Save default settings for next start
        }

        AudioListener.volume = settings.GetVolume();
    }
}

[Serializable]
public class GameSettings
{
    public const int LATEST_VERSION = 1;

    public int version;
    public float volume;

    public SerializableDictionary<string, KeyCode> keyBindings = new()
    {
        { "Forward", KeyCode.W },
        { "Left", KeyCode.A },
        { "Right", KeyCode.D },
        { "Back", KeyCode.S },
        { "Jump", KeyCode.Space },
        { "Slide", KeyCode.LeftControl },

        // Special cases for WebGL, not configurable (for now)
        { "Grapple", Application.platform != RuntimePlatform.WebGLPlayer ? KeyCode.Mouse1 : KeyCode.V },
        { "Pause", Application.platform != RuntimePlatform.WebGLPlayer ? KeyCode.Escape : KeyCode.P }
    };

    public float GetVolume()
    {
        return volume;
    }
}