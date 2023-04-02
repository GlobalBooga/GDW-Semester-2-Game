using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MenuType
{
    HORIZONTAL,
    VERTICAL,
    TwoByTwo
}
public class MenuDefinition : MonoBehaviour
{
    public MenuType _menuType = MenuType.HORIZONTAL;
    public List<GameObject> _menuButtonObjects = new List<GameObject>();

    private List<ButtonDefinition> _menuButtonDefinitions = new List<ButtonDefinition>();
    private List<Button> _menuButtons = new List<Button>();

    public void Start()
    {
        //Searches and grabs components 
        for (int i = 0; i < _menuButtonObjects.Count; i++)
        {
            _menuButtonDefinitions.Add(_menuButtonObjects[i].GetComponent<ButtonDefinition>());
            _menuButtons.Add(_menuButtonObjects[i].GetComponent<Button>());
        }
    }

    public MenuType GetMenuType()
    {
        return _menuType;
    }

    public int GetButtonCount()
    {
        return _menuButtonObjects.Count;
    }

    public List<ButtonDefinition> GetButtonDefinitions()
    {
        return _menuButtonDefinitions;
    }

    public List<Button> GetButtons()
    {
        return _menuButtons;
    }
}
