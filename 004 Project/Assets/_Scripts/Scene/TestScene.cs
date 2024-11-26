using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScene : BaseScene
{
    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Test;
        GameManager.Instance.CreatePlayerManager();

        if (!GameObject.Find("Arrows"))
        {
            new GameObject
            {
                name = "Arrows"
            }.AddComponent<CoroutineHandler>();
        }
    }
}
