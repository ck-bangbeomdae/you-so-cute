using System.IO;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    public const string SETTINGS_FILE_NAME = "settings.json";

    public const float DEFAULT_SFX_VOLUME = 1.0f;
    public const float DEFAULT_BGM_VOLUME = 1.0f;

    public PlayerProfile playerProfile = new PlayerProfile();

    private void Awake()
    {
        if (Instance != null)
        {
            return;
        }

        Instance = this;

        LoadSettings();
    }

    public void SaveSettings()
    {
        string filePath = Path.Combine(Application.persistentDataPath, SETTINGS_FILE_NAME);
        string json = JsonUtility.ToJson(playerProfile, true);
        File.WriteAllText(filePath, json);
    }

    public void LoadSettings()
    {
        string filePath = Path.Combine(Application.persistentDataPath, SETTINGS_FILE_NAME);

        if (SettingsFileExists())
        {
            string json = File.ReadAllText(filePath);
            playerProfile = JsonUtility.FromJson<PlayerProfile>(json);
        }
    }

    private bool SettingsFileExists()
    {
        return File.Exists(Path.Combine(Application.persistentDataPath, SETTINGS_FILE_NAME));
    }
}

[System.Serializable]
public struct PlayerProfile
{
    public PlayerSpawnpoint PlayerSpawnpoint;
    public float sfxVolume;
    public float bgmVolume;
}
