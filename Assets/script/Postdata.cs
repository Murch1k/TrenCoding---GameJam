using UnityEngine;

[CreateAssetMenu(fileName = "NewPost", menuName = "Moderator/Post", order = 0)]
public class PostData : ScriptableObject
{
    [TextArea(2, 6)]
    public string content;        // Текст поста

    public string author;         // Автор (ник)
    public string date;           // Дата (строкой — для хакатона проще)

    [Tooltip("True = пост вредный (мат/спам/запрещёнка), правильный ответ DELETE. False = нормальный, правильный ответ APPROVE.")]
    public bool isHarmful;

    [Tooltip("На каком дне этот пост может выпасть. 0 = любой день.")]
    public int dayAvailable = 0;
}