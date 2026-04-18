using UnityEngine;

// Assets > Create > Moderator > Email
[CreateAssetMenu(fileName = "NewEmail", menuName = "Moderator/Email", order = 2)]
public class EmailData : ScriptableObject
{
    public string senderName;      // Имя отправителя
    public string senderAddress;   // Адрес (например, hr@content-corp.net)
    public string subject;         // Тема письма
    [TextArea(4, 12)]
    public string body;            // Текст письма
    public int showOnDay = 1;      // В какой день показывать
}