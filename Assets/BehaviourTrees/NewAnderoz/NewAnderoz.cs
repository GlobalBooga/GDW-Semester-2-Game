using System;
using UnityEngine;
using Unity.Behavior;

public class NewAnderoz : MonoBehaviour
{
    private PlayerControls playerControls;
    [SerializeField] private BehaviorGraphAgent agent;

    private Vector2 t;

   // private AndarozRoom roomManager;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //roomManager = transform.parent.GetComponent<AndarozRoom>();
        
        //playerControls = new PlayerControls();

        /*
        playerControls.Menus.MouseClick.performed += ctx =>
        {
            agent.BlackboardReference.SetVariableValue("Location", t);
            Debug.Log(t);
        };
        
        playerControls.Menus.MouseMove.performed += context =>
        {
            var value = context.ReadValue<Vector2>();
            t = Camera.main.ScreenToWorldPoint(value);
        };
        
        playerControls.Menus.MouseClick.Enable();
        playerControls.Menus.MouseMove.Enable();*/
    }

    private void OnDestroy()
    {
        //playerControls.Dispose();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
