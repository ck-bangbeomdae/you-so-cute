using System;
using System.IO;
using UnityEngine;

public class ProfileManager : MonoBehaviour
{
    public static ProfileManager Instance { get; private set; }

    public const string PROFILE_FILE_NAME = "profile.json";

    public const float DEFAULT_SFX_VOLUME = 1f;
    public const float DEFAULT_BGM_VOLUME = 1f;

    public PlayerProfile playerProfile;

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

    public void SaveProfile()
    {
        string filePath = Path.Combine(Application.persistentDataPath, PROFILE_FILE_NAME);
        string json = JsonUtility.ToJson(playerProfile, true);
        File.WriteAllText(filePath, json);
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

    public void UpdateRecord(Record newRecord)
    {
        playerProfile.lastRecord = newRecord;
        UpdateTop10HighRecords(newRecord);
    }

    private void UpdateTop10HighRecords(Record newRecord)
    {
        int emptyIndex = -1;
        int lastIndex = playerProfile.top10HighRecord.Length - 1;

        for (int i = 0; i < playerProfile.top10HighRecord.Length; i++)
        {
            if (playerProfile.top10HighRecord[i].flipCount == 0)
            {
                emptyIndex = i;
                break;
            }
        }

        if (emptyIndex != -1)
        {
            playerProfile.top10HighRecord[emptyIndex] = newRecord;
        }
        else
        {
            if (newRecord.elapsedTime < playerProfile.top10HighRecord[lastIndex].elapsedTime)
            {
                playerProfile.top10HighRecord[lastIndex] = newRecord;
            }
        }

        Array.Sort(playerProfile.top10HighRecord, (x, y) =>
        {
            if (x.flipCount == 0) return 1;
            if (y.flipCount == 0) return -1;
            return x.elapsedTime.CompareTo(y.elapsedTime);
        });
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
    public Record[] top10HighRecord;

    public PlayerProfile(ProgressSave progressSave, string playerName, float sfxVolume, float bgmVolume)
    {
        this.progressSave = progressSave;
        this.playerName = playerName;
        this.sfxVolume = sfxVolume;
        this.bgmVolume = bgmVolume;
        this.lastRecord = new Record();
        this.top10HighRecord = new Record[10];
    }
}

[System.Serializable]
public struct ProgressSave
{
    public PlayerSpawnpoint playerSpawnpoint;
    public float elapsedTime;
    public int coinCount;
    public int flipCount;
    public int deathCount;

    public ProgressSave(PlayerSpawnpoint playerSpawnpoint, float elapsedTime, int coinCount, int flipCount, int deathCount)
    {
        this.playerSpawnpoint = playerSpawnpoint;
        this.elapsedTime = elapsedTime;
        this.coinCount = coinCount;
        this.flipCount = flipCount;
        this.deathCount = deathCount;
    }
}

[System.Serializable]
public struct Record
{
    public string playerName;
    public float elapsedTime;
    public int coinCount;
    public int flipCount;
    public int deathCount;

    public Record(string playerName, float elapsedTime, int coinCount, int flipCount, int deathCount)
    {
        this.playerName = playerName;
        this.elapsedTime = elapsedTime;
        this.coinCount = coinCount;
        this.flipCount = flipCount;
        this.deathCount = deathCount;
    }
}
