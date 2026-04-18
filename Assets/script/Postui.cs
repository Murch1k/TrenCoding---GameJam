using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Вешается на панель поста внутри Canvas рабочего стола.
// В инспекторе привязываешь Text-поля и кнопки.
public class PostUI : MonoBehaviour
{
    [Header("Поля поста")]
    [SerializeField] private TMP_Text authorText;
    [SerializeField] private TMP_Text dateText;
    [SerializeField] private TMP_Text contentText;

    [Header("Кнопки")]
    [SerializeField] private Button deleteButton;
    [SerializeField] private Button approveButton;

    public event Action OnDeleteClicked;
    public event Action OnApproveClicked;

    private void Awake()
    {
        if (deleteButton != null) deleteButton.onClick.AddListener(() => OnDeleteClicked?.Invoke());
        if (approveButton != null) approveButton.onClick.AddListener(() => OnApproveClicked?.Invoke());
    }

    public void Display(PostData post)
    {
        if (post == null) { Clear(); return; }
        if (authorText != null) authorText.text = post.author;
        if (dateText != null) dateText.text = post.date;
        if (contentText != null) contentText.text = post.content;
        SetButtonsInteractable(true);
    }

    public void Clear()
    {
        if (authorText != null) authorText.text = "";
        if (dateText != null) dateText.text = "";
        if (contentText != null) contentText.text = "";
        SetButtonsInteractable(false);
    }

    private void SetButtonsInteractable(bool v)
    {
        if (deleteButton != null) deleteButton.interactable = v;
        if (approveButton != null) approveButton.interactable = v;
    }
}