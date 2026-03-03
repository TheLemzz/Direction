using UnityEngine;

/// <summary>
/// Provides base Singleton pattern for MonoBehaviour class.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class MonoBehaviorSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T _instance;

    public static T Instance => _instance;

    public static T GetInstance()
    {
        return _instance;
    }

    /// <summary>
    /// Set an instance for class.
    /// </summary>
    /// <param name="instance"></param>
    /// <param name="dontDestroyOnLoad"></param>
    protected void SetInstance(T instance, bool dontDestroyOnLoad = false)
    {
        if (_instance != null && instance != null) return;

        _instance = instance;
        if (dontDestroyOnLoad) DontDestroyOnLoad(instance);
    }

    private void OnDestroy()
    {
        SetInstance(null);
    }
}