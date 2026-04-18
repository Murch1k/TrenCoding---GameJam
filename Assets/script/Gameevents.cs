using System;

// Простая шина событий. Другие системы (sense bar, день/ночь, глюки) подписываются отсюда.
// Не нужно тащить ссылки между компонентами.
public static class GameEvents
{
    public static event Action OnCorrectAnswer;     // Правильно промодерировал
    public static event Action OnWrongAnswer;       // Ошибся (отнимать sense bar тут)
    public static event Action OnWorkDayComplete;   // 8 постов обработано
    public static event Action OnEnterComputer;     // Сел за комп
    public static event Action OnExitComputer;      // Встал из-за компа

    public static void RaiseCorrect() => OnCorrectAnswer?.Invoke();
    public static void RaiseWrong() => OnWrongAnswer?.Invoke();
    public static void RaiseWorkDayComplete() => OnWorkDayComplete?.Invoke();
    public static void RaiseEnterComputer() => OnEnterComputer?.Invoke();
    public static void RaiseExitComputer() => OnExitComputer?.Invoke();
}