using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement), typeof(PlayerInput))]
public class Player : MonoBehaviour
{
    public static List<Action> onPlayerRespawn = new List<Action>();


    //internal PlayerAnimator animator;
    internal PlayerMovement movement;
    //internal PlayerCollision collision;
    internal PlayerInput input;
    //[SerializeField] internal Health health;
    //[SerializeField] internal PlayerAudio sound;
    // pause menu object


    // general
    [SerializeField] internal float health = 100f;
    [SerializeField] internal float attackRange = 2f;
    [SerializeField] internal float interactRange = 1f;
    [SerializeField] internal float damage = 34f;


    internal Quaternion originalRot;
    internal Vector3 originalPos;
    internal Vector3 mousePos;



    [MenuItem("GameObject/Characters/Player")]
    public static void MakeNewPlayer()
    {
        GameObject player = new();
        player.name = "New Player";
        player.transform.position = Vector3.zero;

        // ADDING A SPRITE
        SpriteRenderer sr = player.AddComponent<SpriteRenderer>();
        sr.sprite = Resources.Load<Sprite>("character_default");

        // ADDING A SCRIPT
        player.AddComponent<Player>();
    }




    private void Awake()
    {
        movement = gameObject.GetComponent<PlayerMovement>();
        input = gameObject.GetComponent<PlayerInput>();

        originalPos = transform.position;
        originalRot = transform.rotation;
    }
}