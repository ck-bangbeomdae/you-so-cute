using System.IO;
using UnityEngine;

public class ProfileManager : MonoBehaviour
{
    public static ProfileManager Instance { get; private set; }

    public const string PROFILE_FILE_NAME = "profile.json";

    public const float DEFAULT_SFX_VOLUME = 1f;
    public const float DEFAULT_BGM_VOLUME = 1f;

    public PlayerProfile playerProfile;

    private FMOD.Studio.Bus sfxBus;
    private FMOD.Studio.Bus bgmBus;

    private void Awake()
    {
        if (Instance != null)
        {
            return;
        }

        Instance = this;

        playerProfile = new PlayerProfile(new ProgressSave(), "Player", DEFAULT_SFX_VOLUME, DEFAULT_BGM_VOLUME);
        LoadProfile();
    }

    private void Start()
    {
        // Bus 인스턴스 가져오기 (FMOD Studio에서 설정한 Bus 이름 사용)
        sfxBus = FMODUnity.RuntimeManager.GetBus("bus:/SFX_Bus");
        bgmBus = FMODUnity.RuntimeManager.GetBus("bus:/BGM_Bus");

        sfxBus.setVolume(playerProfile.sfxVolume);
        bgmBus.setVolume(playerProfile.bgmVolume);
    }

    public void SaveProfile()
    {
        string filePath = Path.Combine(Application.persistentDataPath, PROFILE_FILE_NAME);
        string json = JsonUtility.ToJson(playerProfile, true);
        File.WriteAllText(filePath, json);

        sfxBus.setVolume(playerProfile.sfxVolume);
        bgmBus.setVolume(playerProfile.bgmVolume);
    }

    public void LoadProfile()
    {
        string filePath = Path.Combine(Application.persistentDataPath, PROFILE_FILE_NAME);

        if (ProfileFileExist())
        {
            string json = File.ReadAllText(filePath);
            playerProfile = JsonUtility.FromJson<PlayerProfile>(json);
        }
        else
        {
            SaveProfile();
        }
    }

    private bool ProfileFileExist()
    {
        return File.Exists(Path.Combine(Application.persistentDataPath, PROFILE_FILE_NAME));
    }
}

[System.Serializable]
public struct PlayerProfile
{
    public ProgressSave progressSave;
    public string playerName;
    public float sfxVolume;
    public float bgmVolume;
    public Record lastRecord;

    public PlayerProfile(ProgressSave progressSave, string playerName, float sfxVolume, float bgmVolume)
    {
        this.progressSave = progressSave;
        this.playerName = playerName;
        this.sfxVolume = sfxVolume;
        this.bgmVolume = bgmVolume;
        this.lastRecord = new Record();
    }
}

[System.Serializable]
public struct ProgressSave
{
    public PlayerSpawnpoint playerSpawnpoint;
    public int lastSavepointId;
    public int lastSavepointProgressPortalCount;
    public float elapsedTime;
    public int progressPortalCount;
    public int flipCount;
    public int deathCount;

    public ProgressSave(PlayerSpawnpoint playerSpawnpoint, int lastSavepointId, int lastSavepointProgressPortalCount, float elapsedTime, int progressPortalCount, int flipCount, int deathCount)
    {
        this.playerSpawnpoint = playerSpawnpoint;
        this.lastSavepointId = lastSavepointId;
        this.lastSavepointProgressPortalCount = lastSavepointProgressPortalCount;
        this.elapsedTime = elapsedTime;
        this.progressPortalCount = progressPortalCount;
        this.flipCount = flipCount;
        this.deathCount = deathCount;
    }
}

[System.Serializable]
public struct Record
{
    public string playerName;
    public float elapsedTime;
    public int flipCount;
    public int deathCount;

    public Record(string playerName, float elapsedTime, int flipCount, int deathCount)
    {
        this.playerName = playerName;
        this.elapsedTime = elapsedTime;
        this.flipCount = flipCount;
        this.deathCount = deathCount;
    }
}
