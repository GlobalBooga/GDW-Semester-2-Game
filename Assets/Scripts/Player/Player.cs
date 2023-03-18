using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CircleCollider2D), typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    [Header("Movement Values"), Space(5f)]
    public float runSpeed = 10f;
    public float moveForce = 15f;
    public float accelerationDrag = 1f;
    public float deccelerationDrag = 5f;
    private bool rotationEnabled = true;

    [Space(10f)]

    [Header("Dodging"), Space(5f)]
    public float dodgeCooldown = 1f;
    public float dodgeDuration = 0.3f;
    public float dodgeForce = 5f;
    private bool isUsingMoveAbility;
    private bool canUseMoveAbility = true;
    public float initialDodgeRechargeDelay = 3f;
    public float secondDodgeRechargeDelay = 1.5f;
    public float dodgeRechargeSpeed = 2f;
    private int dodgesAvailable;
    public bool mustDepleteAllDodgesBeforeRecharging = false;
    private bool rechargingDodge;

    [Space(10f)]

    [Header("Objects"), Space(5f)]
    public Sprite defaultSprite;
    private Rigidbody2D rb;
    private CircleCollider2D cc;
    private HPComponent hpcomp;
    //public ParticleSystem particles;
    private PlayerControls controls;
    public Weapon weapon;
    private GameObject pickupable;
    public Animator screenOverlayAnimator;
    public List<Slider> staminaBars;
    private SpriteRenderer sr;
    public GameObject flashlight;

    [Header("All Weapons"), Space(5f)]
    [SerializeField] private GameObject AdanasDualies;
    [SerializeField] private GameObject EnergyDualies;
    [SerializeField] private GameObject Bow;
    [SerializeField] private GameObject AndarozGun;

    // for animations
    public const string PLAYER_HIT_INDICATOR = "PlayerDamageTaken";


    // other
    private Quaternion originalRot;
    private Vector3 originalPos;
    private Vector2 lastDirection;

    GameData gameData;


    public Vector2 RawDirection => controls.General.Move.ReadValue<Vector2>();
    public Vector2 RotatedRawDirection => transform.up * RawDirection.y + transform.right * RawDirection.x;
    public Vector2 MousePosition => Camera.main.ScreenToWorldPoint(Input.mousePosition);
    public Vector2 MouseDirection => (MousePosition - (Vector2)transform.position).normalized;
    public bool IsMoving => RawDirection != Vector2.zero;

    public bool IsDodging => isUsingMoveAbility;

    public bool IsAttacking { get; private set; } 

    #region Unity Messages

    private void OnEnable()
    {
        controls.General.Enable();
        controls.Menus.Disable();
    }

    private void OnDisable()
    {
        controls.General.Disable();
        controls.Menus.Enable();
    }


    private void OnDestroy()
    {
        controls.Dispose();
    }

    private void Awake()
    {
        controls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
        cc = gameObject.GetComponent<CircleCollider2D>();
        hpcomp = gameObject.GetComponent<HPComponent>();
        sr = GetComponent<SpriteRenderer>();

        if (hpcomp)
        {
            hpcomp.OnHPZero = OnDead;
            hpcomp.OnHit.Add(OnHit);
        }

        SetupInputEvents();

    }

    void Start()
    {
        gameData = LevelManager.instance.LoadGameData();

        originalPos = transform.position;
        originalRot = transform.rotation;
        rb.freezeRotation = true;
        rb.drag = accelerationDrag;

        if (weapon)
        {
            weapon.SetHeld();
            gameData.weaponID = weapon.GetID();
            LevelManager.instance.Save(gameData);
        }
    }

    private void Update()
    {
        // If we are still holding down attack button, continue attacking
        if (weapon)
        {
            if (weapon.readyToUse && weapon.isAutoUse && controls.General.Attack.IsPressed()) weapon.Use();
        }

        if (rotationEnabled) transform.rotation = Quaternion.Euler(0f, 0f, Vector3.SignedAngle(MouseDirection, Vector3.up, Vector3.back));
        Debug.DrawLine(transform.position, transform.position + (Vector3)MouseDirection * 1.5f, Color.red, Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (!isUsingMoveAbility) Move();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log(collision.gameObject.layer);
        //Debug.Log(LayerMask.LayerToName(StaticHelpers.PickupLayer));
        if (collision.gameObject.layer == StaticHelpers.PickupLayer)
        {
            pickupable = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        pickupable = null;
    }


    #endregion

    private void Move()
    {
        //bool movingInSameDir;
        bool isTooFast = Mathf.Abs(rb.velocity.magnitude) > runSpeed;
        rb.AddForce(RawDirection * moveForce * rb.mass, ForceMode2D.Force); //if (!isTooFast) 

        if (isTooFast)
        {
            rb.velocity = rb.velocity.normalized * runSpeed;
        }

        if (RawDirection == Vector2.zero) rb.drag = deccelerationDrag;
    }

    private void SetupInputEvents()
    {
        controls.General.Move.started += ctx => rb.drag = accelerationDrag;

        controls.General.Move.canceled += ctx => rb.drag = deccelerationDrag;

        controls.General.Pickup.started += ctx => 
        {
            if (pickupable)
            {
                Weapon newWeapon = (Weapon)pickupable.GetComponent(typeof(Weapon));

                if (newWeapon)
                {
                    weapon.Drop(transform.up);
                
                    newWeapon.Pickup(transform, weapon);
                    weapon = newWeapon;
                    gameData.weaponID = newWeapon.GetID();

                    TutorialManager tm = LevelManager.instance.CurrentScene.manager as TutorialManager;
                    if (!tm)
                    {
                        if (newWeapon.GetID() == 4)
                        {
                            gameData.foundAndarozGun = true;
                        }
                        LevelManager.instance.Save(gameData);
                    }
                    else
                    {
                        Debug.Log("was tutorial");
                    }
                }
            }
        };

        controls.General.Attack.started += ctx => 
        {
            IsAttacking = true;
            if (weapon) weapon.Use(); 
        };

        controls.General.Attack.canceled += ctx => IsAttacking = false;

        controls.General.MovementAbility.started += ctx =>
        {
            if (!canUseMoveAbility) return;

            if (staminaBars.Count > 0)
            {
                int strikes = 0;
                for (int i = 0; i < staminaBars.Count; i++)
                {
                    if (i == staminaBars.Count - 1 && staminaBars[i].value < 1) return;
                    
                    // is this bar empty or charging
                    if (staminaBars[i].value == 0)
                    {
                        strikes++;
                        continue;
                    }
                    if (rechargingDodge && (i + 1) < staminaBars.Count)
                    {
                        staminaBars[i + 1].value = staminaBars[i].value;
                        strikes++;
                    }

                    staminaBars[i].value = 0;
                    break;
                }

                // 3 strikes mean no bars are full
                // 2 strikes mean we just used the last bar
                
                if (strikes == 3) return;
                else if (!mustDepleteAllDodgesBeforeRecharging)
                {
                    StopCoroutine(nameof(RechargeDodge));
                    StartCoroutine(nameof(RechargeDodge), false);
                }
            }


            canUseMoveAbility = false;
            isUsingMoveAbility = true;


            if (hpcomp) hpcomp.isInvincible = true;
            gameObject.layer = StaticHelpers.PlayerInvincibleLayer;

            Vector2 dir = rb.velocity.normalized;
            rb.velocity = Vector2.zero;
            if (RawDirection == Vector2.zero)
            {
                // dash backwards
                rb.AddForce((dir - (Vector2)transform.up * 2.5f).normalized * dodgeForce * rb.mass, ForceMode2D.Impulse);
            }
            else
            {
                // dash in direction
                rb.AddForce((dir + RawDirection * 2.5f).normalized * dodgeForce * rb.mass, ForceMode2D.Impulse);
            }
            rb.drag = 10f;
            Invoke(nameof(EndDodge), dodgeDuration);
            
        };

        controls.General.WeaponAbility.started += ctx =>
        {
            if (weapon)
            {
                if (weapon.canUseAbility)
                {
                    weapon.UseAbility();
                }
            }
        };

        controls.Menus.AdvanceDialogue.started += ctx =>
        {
            if (LevelManager.instance.dialogueController.isEnabled && !controls.Menus.Unpause.IsPressed())
            {
                LevelManager.instance.dialogueController.NextSentence();
            }
        };

        controls.Menus.Unpause.started += ctx =>
        {
            LevelManager.instance.pauseMenu.ResumeGame();
        };

        controls.General.Pause.started += ctx =>
        {
            LevelManager.instance.pauseMenu.PauseGame();
        };
    }

    public void DisableGeneralControls()
    {
        controls.General.Disable();
        controls.Menus.Enable();
    }

    public void EnableGeneralControls()
    {
        controls.Menus.Disable();
        controls.General.Enable();
    }

    private void EndDodge()
    {
        rb.drag = accelerationDrag;
        isUsingMoveAbility = false;
        if (hpcomp) hpcomp.isInvincible = false;
        gameObject.layer = StaticHelpers.PlayerLayer;

        if (dodgeCooldown - dodgeDuration > 0) Invoke(nameof(ResetMoveAbility), dodgeCooldown - dodgeDuration);
        else ResetMoveAbility();
    }

    private void ResetMoveAbility()
    {
        canUseMoveAbility = true;
    }

    private void OnDead()
    {
        Debug.Log("you died");
    }

    private void OnHit()
    {
        // disable movement until grounded
        //Debug.Log("ouch");
        if (screenOverlayAnimator) screenOverlayAnimator.Play(PLAYER_HIT_INDICATOR);
    }

    private IEnumerator RechargeDodge(bool startImmediately = false)
    {
        if (!startImmediately) yield return new WaitForSeconds(initialDodgeRechargeDelay);
        rechargingDodge = true;

        // get the index of the stamina bar to recharge
        int bar = 0;
        for (int i = staminaBars.Count - 1; i >= 0; i--)
        {
            //rechargingDodge = true;
            bar = i;
            if (staminaBars[i].value < 1)
            {
                // replenish its stamina
                while (staminaBars[bar].value < 1)
                {
                    staminaBars[bar].value += Time.deltaTime * dodgeRechargeSpeed;
                    yield return null;
                }
                staminaBars[bar].value = 1f;
            }
            //if (!mustDepleteAllDodgesBeforeRecharging) rechargingDodge = false;
            yield return new WaitForSeconds(secondDodgeRechargeDelay);
        }

        if (rechargingDodge) rechargingDodge = false;
    }

    public void DisableRotation()
    {
        rotationEnabled = false;
    }

    public void EnableRotation()
    {
        rotationEnabled = true;
    }

    public void SetSprite(Sprite sprite)
    {
        if (sprite) sr.sprite = sprite;
        else sr.sprite = defaultSprite;
    }

    public void FlashlightOn()
    {
        flashlight.SetActive(true);
    }

    public void FlashlightOff()
    {
        flashlight.SetActive(false);
    }

    public void DisableAttacks()
    {
        controls.General.Attack.Disable();
        controls.General.WeaponAbility.Disable();
    }

    public void EnableAttacks()
    {
        controls.General.Attack.Enable();
        controls.General.WeaponAbility.Enable();
    }

    private void OnLevelWasLoaded(int level)
    {
        Destroy(weapon.gameObject);
        Weapon newWeapon = null;
        int id = LevelManager.instance.LoadGameData().weaponID;

        if (id == 1) newWeapon = Instantiate(AdanasDualies, transform).GetComponent<Weapon>();
        else if (id == 2) newWeapon = Instantiate(EnergyDualies, transform).GetComponent<Weapon>();
        else if (id == 3) newWeapon = Instantiate(Bow, transform).GetComponent<Weapon>();
        else if (id == 4) newWeapon = Instantiate(AndarozGun, transform).GetComponent<Weapon>();

        newWeapon.Pickup(transform, weapon);
        weapon = newWeapon;
    }
}
