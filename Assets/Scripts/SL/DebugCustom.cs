using System;
using SL;
using TMPro;
using UnityEngine;

public class DebugCustom : MonoBehaviour, IDebug
{
    [SerializeField] private GameObject canvas;
    [SerializeField] private TextMeshProUGUI textDebug;
    private void Awake()
    {
        try
        {
            ServiceLocator.Instance.GetService<IDebug>();
            Destroy(canvas.gameObject);
        }
        catch (Exception e)
        {
            ServiceLocator.Instance.RegisterService<IDebug>(this);
            DontDestroyOnLoad(canvas.gameObject);
        }
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
