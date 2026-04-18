using System;

// Простая шина событий. Другие системы (sense bar, день/ночь, глюки) подписываются отсюда.
// Не нужно тащить ссылки между компонентами.
public static class GameEvents
{
    public static event Action OnCorrectAnswer;    
    public static event Action OnWrongAnswer;       
    public static event Action OnWorkDayComplete;  
    public static event Action OnEnterComputer;    
    public static event Action OnExitComputer;      

    public static void RaiseCorrect() => OnCorrectAnswer?.Invoke();
    public static void RaiseWrong() => OnWrongAnswer?.Invoke();
    public static void RaiseWorkDayComplete() => OnWorkDayComplete?.Invoke();
    public static void RaiseEnterComputer() => OnEnterComputer?.Invoke();
    public static void RaiseExitComputer() => OnExitComputer?.Invoke();
}