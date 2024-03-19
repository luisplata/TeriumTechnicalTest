using System.Collections;
using SL;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RulesOfTitleRoom : MonoBehaviour
{

    [SerializeField] private GameObject canvas;

    public void SaveNickName(TMP_InputField text)
    {
        ServiceLocator.Instance.GetService<ISaveData>().SaveNickName(text.text, true);
        Debug.Log("Nick Name Saved");
        LoadScene();
    }
    
    [ContextMenu("Get Nick Name")]
    public void GetNickName()
    {
        var nickName = ServiceLocator.Instance.GetService<ISaveData>().GetNickName();
        Debug.Log(nickName);
    }

    public GameObject GetCanvas()
    {
        return canvas;
    }
    public void LoadScene()
    {
        StartCoroutine(UpdateTableCurruting());
    }
    
    

    private IEnumerator UpdateTableCurruting()
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(sceneBuildIndex: 1);
    }
}
