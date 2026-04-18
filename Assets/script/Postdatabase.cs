using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Контейнер всех постов. Один ассет на проект.
[CreateAssetMenu(fileName = "PostDatabase", menuName = "Moderator/PostDatabase", order = 1)]
public class PostDatabase : ScriptableObject
{
    public List<PostData> allPosts = new List<PostData>();

    // Берём N случайных постов для рабочего дня
    // currentDay — если нужно фильтровать по дню (0 в посте = доступен всегда)
    public List<PostData> GetRandomPosts(int count, int currentDay = 0)
    {
        var pool = allPosts
            .Where(p => p != null && (p.dayAvailable == 0 || p.dayAvailable == currentDay))
            .OrderBy(_ => Random.value)
            .Take(count)
            .ToList();

        if (pool.Count < count)
            Debug.LogWarning($"[PostDatabase] Запрошено {count} постов, доступно только {pool.Count}");

        return pool;
    }
}