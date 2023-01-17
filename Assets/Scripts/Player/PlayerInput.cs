using System.Linq;
using UnityEditor.Animations;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private Player player;

    private PlayerControls controls;

    public Vector2 MoveDirection => controls.General.Move.ReadValue<Vector2>();

    public bool IsCrouching => controls.General.Crouch.IsPressed();

    public bool IsJumping => controls.General.Jump.IsPressed();


    private void OnEnable()
    {
        controls.General.Enable();
        //controls.Menus.Disable();
    }

    private void OnDisable()
    {
        controls.General.Disable();
        //controls.Menus.Enable();
    }

    private void OnDestroy()
    {
        controls.Dispose();
    }

    private void Awake()
    {
        controls = new PlayerControls();
        player = gameObject.GetComponent<Player>();

    }

    private void Start()
    {

        // JUMP
        controls.General.Jump.started += ctx => player.movement.TryJump();
       

        // CROUCH
        controls.General.Crouch.started += ctx => player.movement.Crouch();


        // UN-CROUCH
        controls.General.Crouch.canceled += ctx => player.movement.UnCrouch();
    }

    public void Resume()
    {
        //controls.General.Enable();
        //controls.Menus.Disable();
    }

    
}
