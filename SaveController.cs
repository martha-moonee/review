using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[SingularBehaviour(true, true, true)]
public class SaveController : Singleton<SaveController>
{
    private bool isLoaded = false;
    private string saveName = "/PlayerData.json";

    public void SaveDataLocally()
    {
        string playerData = JsonUtility.ToJson(PlayerDataController.Instance.GetPlayerData());
        System.IO.File.WriteAllText(Application.persistentDataPath + "/PlayerData.json", playerData);
        Debug.Log(Application.persistentDataPath + saveName);
    }
    public bool CheckIfDataExist()
    {
        return System.IO.File.Exists(Application.persistentDataPath + saveName);
    }

    public void LoadLocalData()
    {
        if (isLoaded)
            return;

        if (!File.Exists(Application.persistentDataPath + saveName))
            return;

        string playerData = System.IO.File.ReadAllText(Application.persistentDataPath + saveName);
        PlayerDataController.Instance.SetPlayerData((PlayerDataModel) JsonUtility.FromJson(playerData, typeof(PlayerDataModel)));
        isLoaded = true;
    }

    void OnApplicationQuit()
    {
        Debug.Log("Application ending after " + Time.time + " seconds");
        SaveController.Instance.SaveDataLocally();
    }
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
            Debug.Log("Application paused after " + Time.time + " seconds");
        SaveController.Instance.SaveDataLocally();
    }
}
