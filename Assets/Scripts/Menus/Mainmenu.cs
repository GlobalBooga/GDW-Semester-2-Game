using UnityEngine;
using UnityEngine.UI;

public class Mainmenu : MonoBehaviour
{
    public Image cursor;

    private void Awake()
    {
        if (cursor) cursor.color = UnityEngine.Color.yellow;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (cursor)
        {
            cursor.transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f);
        }
    }

    public void PlayGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(2);
    }

    public void QuitGame()
    {
        ResetProgressFull();
        Application.Quit();
    }

    public void Tutorial()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }


    public void Save(GameData playerData)
    {
        string data = JsonUtility.ToJson(playerData);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/GameData.json", data);
    }

    GameData LoadGameData()
    {
        string path = Application.persistentDataPath + "/GameData.json";

        if (!System.IO.File.Exists(path))
        {
            Save(new GameData());
        }


        string savedData = System.IO.File.ReadAllText(Application.persistentDataPath + "/GameData.json");

        GameData data = JsonUtility.FromJson<GameData>(savedData);

        return data;
    }

    void ResetProgressFull()
    {
        GameData fg = LoadGameData();
        fg.beatValkyrie = false;
        fg.firstTimeInHub = true;
        fg.beatGluttony = false;
        fg.beatAndaroz = false;
        fg.foundAndarozGun = false;
        fg.weaponID = 1;
        Save(fg);
    }
}
