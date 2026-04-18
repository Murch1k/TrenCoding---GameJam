using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Вешается на панель почты внутри WorkstationCanvas.
// Панель выключена по умолчанию, открывается кнопкой Email.
public class EmailUI : MonoBehaviour
{
    [Header("Список писем")]
    [SerializeField] private Transform emailListParent;   // Layout Group для кнопок-писем
    [SerializeField] private GameObject emailButtonPrefab; // Префаб кнопки письма (Button + TMP_Text)

    [Header("Просмотр письма")]
    [SerializeField] private GameObject emailViewPanel;    // Панель просмотра (выключена по умолчанию)
    [SerializeField] private TMP_Text senderText;
    [SerializeField] private TMP_Text subjectText;
    [SerializeField] private TMP_Text bodyText;
    [SerializeField] private Button closeViewButton;       // Назад к списку

    [Header("Уведомление")]
    [SerializeField] private GameObject unreadDot;         // Красная точка на кнопке Email

    private List<EmailData> currentEmails = new List<EmailData>();

    private void Awake()
    {
        if (closeViewButton != null)
            closeViewButton.onClick.AddListener(CloseEmailView);

        if (emailViewPanel != null)
            emailViewPanel.SetActive(false);
    }

    // Вызывается из ComputerWorkstation при входе в компьютер
    public void SetEmails(List<EmailData> emails)
    {
        currentEmails = emails;
        RefreshList();
        UpdateUnreadDot();
    }

    private void RefreshList()
    {
        // Очищаем старые кнопки
        foreach (Transform child in emailListParent)
            Destroy(child.gameObject);

        foreach (var email in currentEmails)
        {
            var btn = Instantiate(emailButtonPrefab, emailListParent);
            var label = btn.GetComponentInChildren<TMP_Text>();
            if (label != null)
                label.text = $"{email.senderName}  |  {email.subject}";

            var captured = email;
            btn.GetComponent<Button>().onClick.AddListener(() => OpenEmail(captured));
        }
    }

    private void OpenEmail(EmailData email)
    {
        if (emailViewPanel == null) return;
        emailViewPanel.SetActive(true);

        if (senderText != null)
            senderText.text = $"{email.senderName} <{email.senderAddress}>";
        if (subjectText != null)
            subjectText.text = email.subject;
        if (bodyText != null)
            bodyText.text = email.body;
    }

    private void CloseEmailView()
    {
        if (emailViewPanel != null)
            emailViewPanel.SetActive(false);
    }

    private void UpdateUnreadDot()
    {
        if (unreadDot != null)
            unreadDot.SetActive(currentEmails.Count > 0);
    }
}