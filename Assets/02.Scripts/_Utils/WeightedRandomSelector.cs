using UnityEngine;
using UnityEngine.Assertions;

public static class WeightedRandomSelector
{
    // 가중치에 따라 랜덤으로 프리팹을 선택함
    public static GameObject GetRandomWeightedPrefab(WeightedPrefab[] weightedPrefabs)
    {
        int totalWeight = 0;
        foreach (var weightedPrefab in weightedPrefabs)
        {
            totalWeight += weightedPrefab.Weight;
        }

        int randomValue = Random.Range(0, totalWeight);
        int cumulativeWeight = 0;
        foreach (var weightedPrefab in weightedPrefabs)
        {
            cumulativeWeight += weightedPrefab.Weight;
            if (randomValue < cumulativeWeight)
            {
                return GetRandomPrefab(weightedPrefab.Prefabs);
            }
        }

        return null;
    }

    // 프리팹 배열에서 랜덤으로 하나의 프리팹을 선택함
    public static GameObject GetRandomPrefab(GameObject[] prefabs)
    {
        Assert.IsNotNull(prefabs, "프리팹 배열이 null입니다.");
        Assert.IsTrue(prefabs.Length > 0, "프리팹 배열이 비어 있습니다.");

        int index = Random.Range(0, prefabs.Length);
        return prefabs[index];
    }
}

public struct WeightedPrefab
{
    public int Weight;
    public GameObject[] Prefabs;

    public WeightedPrefab(int weight, GameObject[] prefabs)
    {
        Weight = weight;
        Prefabs = prefabs;
    }

    public WeightedPrefab(int weight, GameObject prefab)
    {
        Weight = weight;
        Prefabs = new GameObject[] { prefab };
    }
}
