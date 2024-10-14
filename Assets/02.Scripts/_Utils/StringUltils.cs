using UnityEngine;

public static class StringUtils
{
    public static string FormatElapsedTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60F);
        int seconds = Mathf.FloorToInt(time % 60F);
        float fraction = time * 100 % 100;
        return string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, fraction);
    }

    public static string FormatRecord(Record record)
    {
        return $"{record.playerName} | {FormatElapsedTime(record.elapsedTime)} | {record.deathCount}";
    }
}
