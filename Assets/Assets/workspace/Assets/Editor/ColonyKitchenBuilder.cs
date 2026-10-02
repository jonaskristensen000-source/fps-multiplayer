using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public static class ColonyKitchenBuilder
{
    const string ScenePath = "Assets/Scenes/ColonyKitchen.unity";

    public static string Build()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        DirectoryEnsure("Assets/Prefabs");
        EnsureLayer("Ground");
        var tileAlbedo = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/image/Color/Color.jpg");
        var tileNormal = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/image/Normal/Normal.jpg");
        var woodAlbedo = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/image/Color_1/Color_1.jpg");
        var tileMat = MakeMat("KitchenTile", tileAlbedo, tileNormal, new Color(0.82f, 0.74f, 0.62f), 8f);
        var woodMat = MakeMat("KitchenWood", woodAlbedo, null, new Color(0.45f, 0.28f, 0.14f), 4f);
        var paintMat = MakeMat("CabinetPaint", null, null, new Color(0.78f, 0.42f, 0.22f), 1f);
        var crumbMat = MakeMat("Crumb", null, null, new Color(0.86f, 0.62f, 0.28f), 1f);
        var acidMat = MakeUnlit("AcidGlob", new Color(1f, 0.75f, 0.12f, 1f));

        var floor = Box("KitchenFloor", new Vector3(0f, -0.2f, 0f), new Vector3(48f, 0.4f, 36f), tileMat, "Ground", true);
        Box("WallNorth", new Vector3(0f, 4f, 18.1f), new Vector3(48.4f, 8.4f, 0.4f), paintMat, "Ground", true);
        Box("WallSouth", new Vector3(0f, 4f, -18.1f), new Vector3(48.4f, 8.4f, 0.4f), paintMat, "Ground", true);
        Box("WallEast", new Vector3(24.1f, 4f, 0f), new Vector3(0.4f, 8.4f, 36.4f), paintMat, "Ground", true);
        Box("WallWest", new Vector3(-24.1f, 4f, 0f), new Vector3(0.4f, 8.4f, 36.4f), paintMat, "Ground", true);
        Box("BaseboardN", new Vector3(0f, 0.35f, 17.85f), new Vector3(47.6f, 0.7f, 0.18f), woodMat, "Ground", true);
        Box("Fridge", new Vector3(20.5f, 4.2f, 12.5f), new Vector3(5.2f, 8.4f, 8.5f), paintMat, "Ground", true);
        Box("CabinetsW", new Vector3(-21.6f, 2.1f, 2f), new Vector3(3.6f, 4.2f, 22f), woodMat, "Ground", true);
        Box("TableTop", new Vector3(-2f, 6.15f, -1f), new Vector3(14f, 0.35f, 8.5f), woodMat, "Ground", true);
        Box("LegA", new Vector3(-7.5f, 3f, 2.4f), new Vector3(0.55f, 6f, 0.55f), woodMat, "Ground", true);
        Box("LegB", new Vector3(3.4f, 3f, 2.4f), new Vector3(0.55f, 6f, 0.55f), woodMat, "Ground", true);
        Box("LegC", new Vector3(-7.5f, 3f, -4.4f), new Vector3(0.55f, 6f, 0.55f), woodMat, "Ground", true);
        Box("LegD", new Vector3(3.4f, 3f, -4.4f), new Vector3(0.55f, 6f, 0.55f), woodMat, "Ground", true);
        var safety = Box("SafetyFloor", new Vector3(0f, -2.4f, 0f), new Vector3(60f, 0.2f, 48f), paintMat, "Default", true);
        safety.GetComponent<Collider>().isTrigger = true;

        PlacePrefab("Assets/object/anthill_unity/anthill_unity_Prefab.prefab", "EnemyAnthill", new Vector3(16.5f, 0f, 12.2f), Vector3.one, Quaternion.Euler(0f, -35f, 0f));
        PlacePrefab("Assets/object/soda_bottle_unity/soda_bottle_unity_Prefab.prefab", "GiantBottle", new Vector3(8.5f, 0f, -8.5f), Vector3.one, Quaternion.identity);
        PlacePrefab("Assets/object/kitchen_chair/kitchen_chair_Prefab.prefab", "GiantChair", new Vector3(-12f, 0f, 6.5f), Vector3.one * 8.8f, Quaternion.Euler(0f, 25f, 0f));

        PlaceFood(new Vector3(-16f, 0.35f, -12f), crumbMat);
        PlaceFood(new Vector3(-8f, 0.35f, -6f), crumbMat);
        PlaceFood(new Vector3(-3f, 0.35f, 1.5f), crumbMat);
        PlaceFood(new Vector3(4f, 0.35f, -4f), crumbMat);
        PlaceFood(new Vector3(9f, 0.35f, 3f), crumbMat);
        PlaceFood(new Vector3(-14f, 0.35f, 8f), crumbMat);
        PlaceFood(new Vector3(2f, 0.35f, 8f), crumbMat);

        var globPrefab = MakeGlobPrefab(acidMat);
        var soldierPrefab = MakeAntPrefab("SoldierAnt", ColonyHealth.Team.Ally, 55f, 10f, 0.28f, new Color(0.72f, 0.12f, 0.08f));
        var raiderPrefab = MakeAntPrefab("RaiderAnt", ColonyHealth.Team.Enemy, 28f, 8f, 0.42f, new Color(0.12f, 0.05f, 0.04f));

        var player = new GameObject("Player");
        player.tag = "Player";
        var cc = player.AddComponent<CharacterController>();
        cc.height = 0.9f;
        cc.radius = 0.28f;
        cc.center = new Vector3(0f, 0.45f, 0f);
        player.transform.position = new Vector3(-18.5f, 0.95f, -13.5f);
        player.transform.rotation = Quaternion.LookRotation(new Vector3(16.5f, 0f, 12.2f) - player.transform.position, Vector3.up);
        var camGo = new GameObject("PlayerCamera");
        camGo.transform.SetParent(player.transform, false);
        camGo.transform.localPosition = new Vector3(0f, 0.72f, 0.12f);
        var cam = camGo.AddComponent<Camera>();
        cam.nearClipPlane = 0.05f;
        cam.fieldOfView = 72f;
        camGo.AddComponent<AudioListener>();
        var muzzle = new GameObject("Muzzle");
        muzzle.transform.SetParent(camGo.transform, false);
        muzzle.transform.localPosition = new Vector3(0.12f, -0.08f, 0.35f);
        var motor = player.AddComponent<PlayerController>();
        motor.BindCamera(camGo.transform);
        var php = player.AddComponent<ColonyHealth>();
        php.team = ColonyHealth.Team.Player;
        php.maxHealth = 100f;
        php.current = 100f;
        php.regenPerSecond = 4f;
        var pgun = player.AddComponent<ColonyAcidGun>();
        pgun.muzzle = muzzle.transform;
        pgun.playerOwned = true;
        pgun.team = ColonyHealth.Team.Player;
        pgun.damage = 18f;
        ColonyAcidGun.BindPrefab(globPrefab.GetComponent<ColonyAcidGlob>());

        var hill = GameObject.Find("EnemyAnthill");
        var hillHp = hill.AddComponent<ColonyHealth>();
        hillHp.team = ColonyHealth.Team.Base;
        hillHp.maxHealth = 420f;
        hillHp.current = 420f;
        var spawn = new GameObject("Spawn");
        spawn.transform.SetParent(hill.transform, false);
        spawn.transform.localPosition = new Vector3(0f, 0.4f, 1.6f);
        var anthill = hill.AddComponent<ColonyAnthill>();
        anthill.spawnPoint = spawn.transform;
        anthill.raiderPrefab = raiderPrefab;

        var systems = new GameObject("ColonySystems");
        var director = systems.AddComponent<ColonyGameDirector>();
        director.Player = motor;
        director.PlayerHealth = php;
        director.Anthill = anthill;
        director.soldierPrefab = soldierPrefab;
        systems.AddComponent<ColonyBoot>();
        systems.AddComponent<ColonyCursor>();
        systems.AddComponent<ColonyQA>();
        var bgm = systems.AddComponent<AudioSource>();
        bgm.clip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/music/bgm_154627_3003240/bgm_154627_3003240.mp3");
        bgm.loop = true;
        bgm.playOnAwake = true;
        bgm.volume = 0.35f;

        var ui = new GameObject("GameUI");
        var doc = ui.AddComponent<UIDocument>();
        doc.panelSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>("Assets/UI Toolkit/SeeleDefaultPanelSettings.asset");
        doc.visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI Toolkit/GameUI.uxml");
        ui.AddComponent<ColonyUIController>();

        var sun = new GameObject("Sun");
        var light = sun.AddComponent<Light>();
        light.type = LightType.Directional;
        light.color = new Color(1f, 0.84f, 0.62f);
        light.intensity = 1.15f;
        sun.transform.rotation = Quaternion.Euler(38f, -40f, 0f);
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.28f, 0.2f, 0.14f);
        var window = new GameObject("WindowLight");
        var area = window.AddComponent<Light>();
        area.type = LightType.Point;
        area.color = new Color(1f, 0.92f, 0.75f);
        area.intensity = 8f;
        area.range = 22f;
        window.transform.position = new Vector3(-18f, 5.5f, -16f);

        EditorSceneManager.SaveScene(scene, ScenePath);
        var build = new EditorBuildSettingsScene(ScenePath, true);
        EditorBuildSettings.scenes = new[] { build };
        AssetDatabase.SaveAssets();
        return "built " + ScenePath + " floor=" + floor.name;
    }

    static Material MakeMat(string name, Texture2D albedo, Texture2D normal, Color fallback, float tiling)
    {
        var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.name = name;
        if (albedo != null)
        {
            mat.SetTexture("_BaseMap", albedo);
            mat.SetTextureScale("_BaseMap", new Vector2(tiling, tiling));
        }
        else
        {
            mat.SetColor("_BaseColor", fallback);
        }

        if (normal != null)
        {
            mat.SetTexture("_BumpMap", normal);
            mat.EnableKeyword("_NORMALMAP");
        }

        AssetDatabase.CreateAsset(mat, "Assets/Materials/" + name + ".mat");
        return mat;
    }

    static Material MakeUnlit(string name, Color c)
    {
        var shader = Shader.Find("Universal Render Pipeline/Unlit");
        if (shader == null)
        {
            shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
        }

        var mat = new Material(shader);
        mat.name = name;
        mat.SetColor("_BaseColor", c);
        AssetDatabase.CreateAsset(mat, "Assets/Materials/" + name + ".mat");
        return mat;
    }

    static GameObject Box(string name, Vector3 pos, Vector3 size, Material mat, string layer, bool collider)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = name;
        go.transform.position = pos;
        go.transform.localScale = size;
        go.GetComponent<MeshRenderer>().sharedMaterial = mat;
        if (!string.IsNullOrEmpty(layer))
        {
            int l = LayerMask.NameToLayer(layer);
            if (l >= 0)
            {
                go.layer = l;
            }
        }

        if (!collider)
        {
            Object.DestroyImmediate(go.GetComponent<Collider>());
        }

        return go;
    }

    static void PlacePrefab(string path, string name, Vector3 pos, Vector3 scale, Quaternion rot)
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (prefab == null)
        {
            return;
        }

        var inst = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        inst.name = name;
        inst.transform.position = pos;
        inst.transform.rotation = rot;
        inst.transform.localScale = scale;
        SnapToGround(inst);
    }

    static void SnapToGround(GameObject go)
    {
        var ray = new Ray(go.transform.position + Vector3.up * 12f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 40f))
        {
            var bounds = GetBounds(go);
            float bottom = bounds.min.y;
            go.transform.position += Vector3.up * (hit.point.y - bottom);
        }
    }

    static Bounds GetBounds(GameObject go)
    {
        var rs = go.GetComponentsInChildren<Renderer>();
        if (rs.Length == 0)
        {
            return new Bounds(go.transform.position, Vector3.one);
        }

        var b = rs[0].bounds;
        for (int i = 1; i < rs.Length; i++)
        {
            b.Encapsulate(rs[i].bounds);
        }

        return b;
    }

    static void PlaceFood(Vector3 pos, Material mat)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.name = "FoodCrumb";
        go.transform.position = pos;
        go.transform.localScale = Vector3.one * 0.55f;
        go.GetComponent<MeshRenderer>().sharedMaterial = mat;
        var col = go.GetComponent<SphereCollider>();
        col.isTrigger = true;
        col.radius = 0.7f;
        go.AddComponent<ColonyFoodPickup>();
    }

    static GameObject MakeGlobPrefab(Material mat)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.name = "AcidGlob";
        go.transform.localScale = Vector3.one * 0.16f;
        Object.DestroyImmediate(go.GetComponent<Collider>());
        go.GetComponent<MeshRenderer>().sharedMaterial = mat;
        var trail = go.AddComponent<TrailRenderer>();
        trail.time = 0.18f;
        trail.startWidth = 0.12f;
        trail.endWidth = 0.01f;
        trail.material = mat;
        var glob = go.AddComponent<ColonyAcidGlob>();
        glob.trail = trail;
        glob.radius = 0.08f;
        DirectoryEnsure("Assets/Prefabs");
        var path = "Assets/Prefabs/AcidGlob.prefab";
        var prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
        Object.DestroyImmediate(go);
        return prefab;
    }

    static GameObject MakeAntPrefab(string name, ColonyHealth.Team team, float hp, float dmg, float interval, Color tint)
    {
        var root = new GameObject(name);
        var cc = root.AddComponent<CharacterController>();
        cc.height = 0.7f;
        cc.radius = 0.22f;
        cc.center = new Vector3(0f, 0.35f, 0f);
        var visualRoot = new GameObject("Visual");
        visualRoot.transform.SetParent(root.transform, false);
        var src = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/avatar/ant_worker_unity/ant_worker_unity_Avatar.prefab");
        if (src != null)
        {
            var vis = (GameObject)PrefabUtility.InstantiatePrefab(src);
            vis.transform.SetParent(visualRoot.transform, false);
            vis.transform.localPosition = Vector3.zero;
        }

        var health = root.AddComponent<ColonyHealth>();
        health.team = team;
        health.maxHealth = hp;
        health.current = hp;
        var gun = root.AddComponent<ColonyAcidGun>();
        gun.playerOwned = false;
        gun.team = team;
        gun.damage = dmg;
        gun.interval = interval;
        gun.muzzle = visualRoot.transform;
        root.AddComponent<ColonyAntMotion>().visual = visualRoot.transform;
        if (team == ColonyHealth.Team.Ally)
        {
            root.AddComponent<ColonySoldierAI>();
        }
        else
        {
            root.AddComponent<ColonyRaiderAI>();
        }

        DirectoryEnsure("Assets/Prefabs");
        var path = "Assets/Prefabs/" + name + ".prefab";
        var prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        Object.DestroyImmediate(root);
        return prefab;
    }

    static void DirectoryEnsure(string path)
    {
        if (!AssetDatabase.IsValidFolder(path))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        }

        if (!AssetDatabase.IsValidFolder("Assets/Materials"))
        {
            AssetDatabase.CreateFolder("Assets", "Materials");
        }
    }

    static void EnsureLayer(string name)
    {
        var tagManager = new SerializedObject(AssetDatabase.LoadAssetAtPath<Object>("ProjectSettings/TagManager.asset"));
        var layers = tagManager.FindProperty("layers");
        for (int i = 8; i < layers.arraySize; i++)
        {
            var p = layers.GetArrayElementAtIndex(i);
            if (p.stringValue == name)
            {
                return;
            }

            if (string.IsNullOrEmpty(p.stringValue))
            {
                p.stringValue = name;
                tagManager.ApplyModifiedProperties();
                return;
            }
        }
    }
}
