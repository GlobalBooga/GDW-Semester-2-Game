using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialObjectiveListener : MonoBehaviour
{
    bool complete4;
    bool complete3;
    bool complete2;
    bool completeMove;
    bool completeDodge;

    Player player;

    private void Start()
    {
        player = GetComponent<Player>();
    }

    void Update()
    {
        SceneManager t = LevelManager.instance.CurrentScene.manager;
        if (t.gameObject.name == "Room 1")
        {
            if (player.IsMoving && !completeMove)
            {
                completeMove = true;
                LevelManager.instance.hintController.ObjectiveComplete(SceneManager.MOVEMENT_TUTORIAL);
            }
            if (player.IsDodging && !completeDodge)
            {
                completeDodge = true;
                LevelManager.instance.hintController.ObjectiveComplete(SceneManager.DODGE_TUTORIAL);
            }
        }
        else if (t.gameObject.name == "Room 2" && !complete2)
        {
            if (player.IsAttacking && !complete2) 
            {
                complete2 = true;
                LevelManager.instance.hintController.ObjectiveComplete(SceneManager.WEAPONS_TUTORIAL);
            }
        }
        else if (t.gameObject.name == "Room 3" && !complete3)
        {
            if (CameraShake.instance.GetIntensity() > 14f)
            {
                complete3 = true;
                LevelManager.instance.hintController.ObjectiveComplete(SceneManager.DESTRUCTIBLE_OBJECTS_TUTORIAL);
                TutorialManager tm = LevelManager.instance.CurrentScene.manager as TutorialManager;
                tm.ExplosiveLabPart2Dialogue();
            }
        }
    }
}
