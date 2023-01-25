using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    public GameObject _activeMenu;
    public List<KeyCode> _increaseVert;
    public List<KeyCode> _decreaseVert;
    public List<KeyCode> _increaseHoriz;
    public List<KeyCode> _decreaseHoriz;
    public List<KeyCode> _confirmButtons;

    private MenuDefinition _activeMenuDefitnition;
    private int _activeButton = 0;

    public void Start()
    {
        //Update active menu definition at start
        UpdateActiveMenuDefinition();
    }

    public void Update()
    {
        switch (_activeMenuDefitnition.GetMenuType())
        {
            case MenuType.HORIZONTAL:
                MenuInput(_increaseHoriz, _decreaseHoriz);
                break;
            case MenuType.VERTICAL:
                MenuInput(_increaseVert, _decreaseVert);
                break;
        }
    }

    private void MenuInput(List<KeyCode> increase, List<KeyCode> decrease)
    {
        int newActive = _activeButton;

        for (int i = 0; i < increase.Count; i++)
        {
            if (Input.GetKeyDown(increase[i]))
            {
                newActive = SwitchCurrentButton(1);
            }
        }

        for (int i = 0; i < decrease.Count; i++)
        {
            if (Input.GetKeyDown(decrease[i]))
            {
                newActive = SwitchCurrentButton(-1);
            }
        }

        for (int i = 0; i < _confirmButtons.Count; i++)
        {
            if (Input.GetKeyDown(_confirmButtons[i]))
            {
                ClickCurrentButton();
            }
        }

        _activeButton = newActive;
    }

    private int SwitchCurrentButton(int increment)
    {
        if (!_activeMenuDefitnition.GetButtonDefinitions()[_activeButton].GetDisableControls())
        {
            int newActive = Utility.WrapAround(_activeMenuDefitnition.GetButtonCount(), _activeButton, increment);

            _activeMenuDefitnition.GetButtonDefinitions()[_activeButton].SwappedOff();
            _activeMenuDefitnition.GetButtonDefinitions()[newActive].SwappedTo();

            return newActive;
        }
        return _activeButton;
    }

    private void ClickCurrentButton()
    {
        if (!_activeMenuDefitnition.GetButtonDefinitions()[_activeButton].GetDisableControls())
        {
            _activeMenuDefitnition.GetButtonDefinitions()[_activeButton].ClickButton();
        }
    }
    public void UpdateActiveMenuDefinition()
    {
        _activeMenuDefitnition = _activeMenu.GetComponent<MenuDefinition>();
    }

    public void SetActiveMenu(GameObject activeMenu)
    {
        //Set active menu
        _activeMenu = activeMenu;

        //Make sure to update definition
        UpdateActiveMenuDefinition();

    }
}
