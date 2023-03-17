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
        Application.Quit();
    }

    public void Tutorial()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }
}
