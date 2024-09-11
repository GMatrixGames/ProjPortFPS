using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager instance;

    [Header("Settings")]
    public Settings settings;

    private string settingsPath;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // This is static across ALL scenes
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
            settings = JsonUtility.FromJson<Settings>(json);
        }
        else
        {
            Save(); // Save default settings for next start
        }
    }
}

[Serializable]
public class Settings
{
    public float volume = .5f;

    public Dictionary<string, KeyCode> keyBindings = new()
    {
        { "Forward", KeyCode.W },
        { "Left", KeyCode.A },
        { "Right", KeyCode.D },
        { "Back", KeyCode.S },
        { "Jump", KeyCode.Space },
        { "Slide", KeyCode.LeftControl }
    };
}