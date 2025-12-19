using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A utility class that allows executing code on Unity's main thread from background threads.
/// This is essential when you need to call Unity API methods from non-main threads.
/// </summary>
public class UnityMainThreadDispatcher : MonoBehaviour
{
    private static UnityMainThreadDispatcher _instance;
    private readonly Queue<Action> _executionQueue = new Queue<Action>();
    private readonly object _lock = new object();

    /// <summary>
    /// Gets the singleton instance of the UnityMainThreadDispatcher.
    /// Returns null if not initialized yet.
    /// </summary>
    public static UnityMainThreadDispatcher Instance => _instance;

    /// <summary>
    /// Checks if the dispatcher is ready to use.
    /// </summary>
    public static bool IsReady => _instance != null;

    void Start()
    {
        // Initialize the singleton instance
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("UnityMainThreadDispatcher initialized.");
        }
        else if (_instance != this)
        {
            // Destroy duplicate instances
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // Execute all queued actions on the main thread
        lock (_lock)
        {
            while (_executionQueue.Count > 0)
            {
                try
                {
                    Debug.Log("Executing action on main thread. Remaining queue size: " + (_executionQueue.Count - 1));
                    _executionQueue.Dequeue().Invoke();
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error executing action on main thread: {ex}");
                }
            }
        }
    }

    /// <summary>
    /// Enqueue an action to be executed on the main thread.
    /// Can be called from any thread.
    /// </summary>
    /// <param name="action">The action to execute on the main thread</param>
    public void Enqueue(Action action)
    {
        if (action == null) return;

        lock (_lock)
        {
            _executionQueue.Enqueue(action);
        }
    }

    /// <summary>
    /// Static method to enqueue an action.
    /// Safe to call from background threads if dispatcher is ready.
    /// </summary>
    /// <param name="action">The action to execute on the main thread</param>
    public static void EnqueueAction(Action action)
    {
        if (_instance != null)
        {
            _instance.Enqueue(action);
        }
        else
        {
            Debug.LogWarning("UnityMainThreadDispatcher: Not ready yet. Make sure a GameObject with this component exists in the scene.");
        }
    }

    /// <summary>
    /// Execute an action immediately if on main thread, otherwise enqueue it.
    /// </summary>
    /// <param name="action">The action to execute</param>
    public static void ExecuteOnMainThread(Action action)
    {
        if (IsMainThread())
        {
            action?.Invoke();
        }
        else
        {
            EnqueueAction(action);
        }
    }

    /// <summary>
    /// Checks if we're currently running on the main thread.
    /// </summary>
    public static bool IsMainThread()
    {
        return System.Threading.Thread.CurrentThread.ManagedThreadId == 1;
    }

    /// <summary>
    /// Gets the number of actions currently queued for execution.
    /// </summary>
    public int QueueCount
    {
        get
        {
            lock (_lock)
            {
                return _executionQueue.Count;
            }
        }
    }

    /// <summary>
    /// Clears all queued actions. Use with caution.
    /// </summary>
    public void ClearQueue()
    {
        lock (_lock)
        {
            _executionQueue.Clear();
        }
    }

    void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Show Queue Count")]
    private void ShowQueueCount()
    {
        Debug.Log($"UnityMainThreadDispatcher queue count: {QueueCount}");
    }

    [ContextMenu("Clear Queue")]
    private void ClearQueueFromEditor()
    {
        ClearQueue();
        Debug.Log("UnityMainThreadDispatcher queue cleared.");
    }
#endif
}