using UnityEngine;

/// <summary>
/// 启动器：进入 Play 时自动搭建整个游戏场景（相机、地面、背景、玩家、管理器），
/// 因此你不需要手动拖拽任何组件，新建工程后直接按 Play 即可。
/// </summary>
public static class GameBootstrap
{
    const string RootName = "__RunnerRoot";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Boot()
    {
        // 防止重复生成
        if (GameObject.Find(RootName) != null) return;

        var root = new GameObject(RootName);

        SetupCamera(root.transform);
        ViewBounds.Refresh();

        BuildBackground(root.transform);
        BuildGround(root.transform);
        var player = BuildPlayer(root.transform);

        var managerGo = new GameObject("GameManager");
        managerGo.transform.SetParent(root.transform, false);

        var obstaclesRoot = new GameObject("Obstacles");
        obstaclesRoot.transform.SetParent(root.transform, false);

        var audio = managerGo.AddComponent<AudioSource>();
        audio.playOnAwake = false;
        audio.volume = 0.6f;
        SoundFX.Init(audio);

        var manager = managerGo.AddComponent<GameManager>();
        manager.Bind(player, obstaclesRoot.transform);

        managerGo.AddComponent<ObstacleSpawner>();
        managerGo.AddComponent<GameUI>();
    }

    static Camera SetupCamera(Transform parent)
    {
        Camera cam = Camera.main;

        if (cam == null)
        {
            var camGo = new GameObject("MainCamera");
            camGo.tag = "MainCamera";
            camGo.transform.SetParent(parent, false);
            cam = camGo.AddComponent<Camera>();
            camGo.AddComponent<AudioListener>();
        }

        cam.orthographic = true;
        cam.orthographicSize = GameConfig.CameraSize;
        cam.transform.position = new Vector3(0f, 0f, -10f);
        cam.transform.rotation = Quaternion.identity;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.07f, 0.09f, 0.16f);

        return cam;
    }

    /// <summary>远景建筑（视差滚动）</summary>
    static void BuildBackground(Transform parent)
    {
        float spacing = 2.6f;
        int count = Mathf.CeilToInt((ViewBounds.Width + 12f) / spacing);
        float span = count * spacing;
        float wrapLeft = ViewBounds.Left - 6f;

        var bg = new GameObject("Background");
        bg.transform.SetParent(parent, false);

        for (int i = 0; i < count; i++)
        {
            float height = Random.Range(1.2f, 3.8f);

            var building = new GameObject("Building");
            building.transform.SetParent(bg.transform, false);

            var sr = building.AddComponent<SpriteRenderer>();
            sr.sprite = SpriteFactory.Solid(new Color(0.12f, 0.16f, 0.30f));
            sr.sortingOrder = -5;

            building.transform.localScale = new Vector3(spacing * 0.72f, height, 1f);
            building.transform.position = new Vector3(
                wrapLeft + i * spacing + Random.Range(-0.3f, 0.3f),
                GameConfig.GroundTop + height * 0.5f,
                0f);

            var scroll = building.AddComponent<ScrollingObject>();
            scroll.speedFactor = 0.35f;
            scroll.wrapSpan = span;
            scroll.wrapLeft = wrapLeft;
        }
    }

    /// <summary>地面：一个静态碰撞体 + 视觉地表 + 滚动的接缝</summary>
    static void BuildGround(Transform parent)
    {
        var ground = new GameObject("Ground");
        ground.transform.SetParent(parent, false);
        ground.transform.position = new Vector3(0f, GameConfig.GroundTop, 0f);

        var col = ground.AddComponent<BoxCollider2D>();
        col.size = new Vector2(300f, 2f);
        col.offset = new Vector2(0f, -1f);
        ground.AddComponent<GroundMarker>();

        // 地面本体（纯视觉）
        var slab = new GameObject("Slab");
        slab.transform.SetParent(ground.transform, false);
        var slabSr = slab.AddComponent<SpriteRenderer>();
        slabSr.sprite = SpriteFactory.Solid(new Color(0.10f, 0.12f, 0.20f));
        slabSr.sortingOrder = 1;
        slab.transform.localScale = new Vector3(120f, 2f, 1f);
        slab.transform.localPosition = new Vector3(0f, -1f, 0f);

        // 地表亮边
        var topLine = new GameObject("TopLine");
        topLine.transform.SetParent(ground.transform, false);
        var topSr = topLine.AddComponent<SpriteRenderer>();
        topSr.sprite = SpriteFactory.Solid(new Color(0.24f, 0.55f, 0.85f));
        topSr.sortingOrder = 2;
        topLine.transform.localScale = new Vector3(120f, 0.1f, 1f);
        topLine.transform.localPosition = new Vector3(0f, -0.05f, 0f);

        // 滚动接缝：提供移动速度感
        float spacing = 2f;
        int count = Mathf.CeilToInt((ViewBounds.Width + 10f) / spacing);
        float span = count * spacing;
        float wrapLeft = ViewBounds.Left - 4f;

        var seams = new GameObject("Seams");
        seams.transform.SetParent(ground.transform, false);

        for (int i = 0; i < count; i++)
        {
            var seam = new GameObject("Seam");
            seam.transform.SetParent(seams.transform, false);

            var sr = seam.AddComponent<SpriteRenderer>();
            sr.sprite = SpriteFactory.Solid(new Color(0.18f, 0.22f, 0.34f));
            sr.sortingOrder = 2;

            seam.transform.localScale = new Vector3(0.08f, 0.5f, 1f);
            seam.transform.position = new Vector3(wrapLeft + i * spacing, GameConfig.GroundTop - 0.25f, 0f);

            var scroll = seam.AddComponent<ScrollingObject>();
            scroll.speedFactor = 1f;
            scroll.wrapSpan = span;
            scroll.wrapLeft = wrapLeft;
        }
    }

    /// <summary>玩家：根节点负责物理与碰撞，子节点负责显示（便于做挤压动画）</summary>
    static PlayerController BuildPlayer(Transform parent)
    {
        var go = new GameObject("Player");
        go.transform.SetParent(parent, false);
        go.transform.position = new Vector3(
            GameConfig.PlayerStartX,
            GameConfig.GroundTop + GameConfig.PlayerHeight * 0.55f,
            0f);

        var visual = new GameObject("Visual");
        visual.transform.SetParent(go.transform, false);
        var sr = visual.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteFactory.PlayerSprite(GameConfig.PlayerWidth, GameConfig.PlayerHeight);
        sr.sortingOrder = 10;

        var rb = go.AddComponent<Rigidbody2D>();
        rb.gravityScale = GameConfig.GravityScale;
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        var col = go.AddComponent<BoxCollider2D>();
        col.size = new Vector2(GameConfig.PlayerWidth * 0.8f, GameConfig.PlayerHeight * 0.96f);

        var controller = go.AddComponent<PlayerController>();
        controller.Visual = visual.transform;

        return controller;
    }
}
