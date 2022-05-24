namespace System.CommandLine.Reflection;

public interface ICliHandler
{
    #region Set Overloads
    
    void Set<T1>(
        Func<T1, Task<int>> handler);
    
    void Set<T1, T2>( 
        Func<T1, T2, Task<int>> handler);

    void Set<T1, T2, T3>( 
        Func<T1, T2, T3, Task<int>> handler);

    void Set<T1, T2, T3, T4>( 
        Func<T1, T2, T3, T4, Task<int>> handler);

    void Set<T1, T2, T3, T4, T5>(
        Func<T1, T2, T3, T4, T5, Task<int>> handler);

    void Set<T1, T2, T3, T4, T5, T6>(
        Func<T1, T2, T3, T4, T5, T6, Task<int>> handler);

    void Set<T1, T2, T3, T4, T5, T6, T7>(
        Func<T1, T2, T3, T4, T5, T6, T7, Task<int>> handler);

    void Set<T1, T2, T3, T4, T5, T6, T7, T8>(
        Func<T1, T2, T3, T4, T5, T6, T7, T8, Task<int>> handler);

    void Set<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
        Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, Task<int>> handler);

    void Set<T1, T2, T3, T4, T5, T6, T7, T8, T9, T0>(
        Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T0, Task<int>> handler);
    
    #endregion
}