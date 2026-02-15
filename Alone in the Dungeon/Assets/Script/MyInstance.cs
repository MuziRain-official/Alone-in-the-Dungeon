using UnityEngine;

public class MyInstance<T> : MonoBehaviour where T : MyInstance<T>
{
    public static T Instance { get; private set; }
        
    protected virtual void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = (T)this;
        }
    }

}

// Example usage:
public class GameManager : MyInstance<GameManager>
{
    protected override void Awake()
    {
        base.Awake();
    }
}
