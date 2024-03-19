using SL;
using TMPro;
using UnityEngine;

public class DebugCustom : MonoBehaviour, IDebug
{
    [SerializeField] private GameObject canvas;
    [SerializeField] private TextMeshProUGUI textDebug;
    private void Awake()
    {
        ServiceLocator.Instance.RegisterService<IDebug>(this);
        DontDestroyOnLoad(canvas);
    }

    public void Log(string log)
    {
        textDebug.text += log + "\n";
        Debug.Log(log);
    }
}

public interface IDebug
{
    void Log(string log);
}
