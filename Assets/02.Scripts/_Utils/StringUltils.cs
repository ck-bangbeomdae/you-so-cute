using UnityEngine;

public static class StringUtils
{
    public static string FormatRecord(Record record)
    {
        return $"{(string.IsNullOrEmpty(record.playerName) ? "noname" : record.playerName)} | {FormatElapsedTime(record.elapsedTime)} | {record.flipCount} | {record.deathCount}";
    }

    public static string FormatElapsedTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60F);
        int seconds = Mathf.FloorToInt(time % 60F);
        float fraction = time * 100 % 100;
        return string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, fraction);
    }
}
