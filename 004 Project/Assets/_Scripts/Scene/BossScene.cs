using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossScene : BaseScene
{
    public Transform playerSpawnTransform;
    public GameObject bossPrefab;
    public Transform bossSpawnTransform;
    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Boss;
        GameManager.Instance.CreatePlayerManager(playerSpawnTransform.position);
        if (!GameObject.Find("Arrows"))
        {
            new GameObject
            {
                name = "Arrows"
            }.AddComponent<CoroutineHandler>();
        }
        Instantiate(bossPrefab, bossSpawnTransform.position, Quaternion.identity);
       
    }
}
