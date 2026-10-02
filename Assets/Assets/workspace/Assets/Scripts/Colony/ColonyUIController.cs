using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class ColonyUIController : MonoBehaviour
{
    public Font uiFont;
    public Font headingFont;
    VisualElement load, start, how, pause, result, hud, spawnDock;
    Label loadLabel, hpLabel, foodLabel, hillLabel, nestLabel, scoreLabel, resultTitle, resultScore;
    VisualElement loadFill, hpFill, hillFill, nestFill;
    Button spawnCollectorBtn, spawnSoldierBtn;
    bool howFromPause;

    void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        root.Query<TextElement>().ForEach(text =>
        {
            var font = text.ClassListContains("title") ? headingFont : uiFont;
            if (font != null) text.style.unityFontDefinition = FontDefinition.FromFont(font);
        });
        load = root.Q("load-screen");
        start = root.Q("start-screen");
        how = root.Q("how-screen");
        pause = root.Q("pause-screen");
        result = root.Q("result-screen");
        hud = root.Q("hud");
        spawnDock = root.Q("spawn-dock");
        loadLabel = root.Q<Label>("load-label");
        hpLabel = root.Q<Label>("hp-label");
        foodLabel = root.Q<Label>("food-label");
        hillLabel = root.Q<Label>("hill-label");
        nestLabel = root.Q<Label>("nest-label");
        scoreLabel = root.Q<Label>("score-label");
        resultTitle = root.Q<Label>("result-title");
        resultScore = root.Q<Label>("result-score");
        loadFill = root.Q("load-fill");
        hpFill = root.Q("hp-fill");
        hillFill = root.Q("hill-fill");
        nestFill = root.Q("nest-fill");
        spawnCollectorBtn = root.Q<Button>("spawn-collector-btn");
        spawnSoldierBtn = root.Q<Button>("spawn-soldier-btn");
        if (spawnDock != null) spawnDock.pickingMode = PickingMode.Position;

        root.Q<Button>("play-btn").RegisterCallback<ClickEvent>(_ => EventDispatcher.SendEventImmediate(GameEventTypes.ColonyEventType.RequestStart));
        root.Q<Button>("how-btn").RegisterCallback<ClickEvent>(_ =>
        {
            howFromPause = false;
            Show(how);
            Hide(start);
        });
        root.Q<Button>("how-back-btn").RegisterCallback<ClickEvent>(_ =>
        {
            Hide(how);
            Show(howFromPause ? pause : start);
        });
        root.Q<Button>("resume-btn").RegisterCallback<ClickEvent>(_ => EventDispatcher.SendEventImmediate(GameEventTypes.ColonyEventType.RequestResume));
        root.Q<Button>("pause-how-btn").RegisterCallback<ClickEvent>(_ =>
        {
            howFromPause = true;
            Hide(pause);
            Show(how);
        });
        root.Q<Button>("quit-btn").RegisterCallback<ClickEvent>(_ => EventDispatcher.SendEventImmediate(GameEventTypes.ColonyEventType.RequestMenu));
        root.Q<Button>("restart-btn").RegisterCallback<ClickEvent>(_ => EventDispatcher.SendEventImmediate(GameEventTypes.ColonyEventType.RequestRestart));
        root.Q<Button>("result-menu-btn").RegisterCallback<ClickEvent>(_ => EventDispatcher.SendEventImmediate(GameEventTypes.ColonyEventType.RequestMenu));
        if (spawnCollectorBtn != null) spawnCollectorBtn.RegisterCallback<ClickEvent>(_ => EventDispatcher.SendEventImmediate(GameEventTypes.ColonyEventType.RequestSummonCollector));
        if (spawnSoldierBtn != null) spawnSoldierBtn.RegisterCallback<ClickEvent>(_ => EventDispatcher.SendEventImmediate(GameEventTypes.ColonyEventType.RequestSummonSoldier));

        EventDispatcher.Register(GameEventTypes.ColonyEventType.LoadingProgress, OnLoad);
        EventDispatcher.Register(GameEventTypes.ColonyEventType.GameStarted, OnStarted);
        EventDispatcher.Register(GameEventTypes.ColonyEventType.GamePaused, OnPaused);
        EventDispatcher.Register(GameEventTypes.ColonyEventType.GameResumed, OnResumed);
        EventDispatcher.Register(GameEventTypes.ColonyEventType.FoodChanged, OnFood);
        EventDispatcher.Register(GameEventTypes.ColonyEventType.SoldierCountChanged, OnSoldiers);
        EventDispatcher.Register(GameEventTypes.ColonyEventType.CollectorCountChanged, OnCollectors);
        EventDispatcher.Register(GameEventTypes.ColonyEventType.AnthillHealthChanged, OnHill);
        EventDispatcher.Register(GameEventTypes.ColonyEventType.NestHealthChanged, OnNest);
        EventDispatcher.Register(GameEventTypes.ColonyEventType.ScoreChanged, OnScore);
        EventDispatcher.Register(GameEventTypes.ColonyEventType.GameWon, OnWon);
        EventDispatcher.Register(GameEventTypes.ColonyEventType.GameLost, OnLost);
        EventDispatcher.Register(GameEventTypes.UIEventType.PlayerHUDUpdated, OnHp);
        EventDispatcher.Register(GameEventTypes.ColonyEventType.MenuShown, OnMenu);
        EventDispatcher.Register(GameEventTypes.ColonyEventType.Feedback, OnFeedback);
    }

    void OnDisable()
    {
        EventDispatcher.Unregister(GameEventTypes.ColonyEventType.LoadingProgress, OnLoad);
        EventDispatcher.Unregister(GameEventTypes.ColonyEventType.GameStarted, OnStarted);
        EventDispatcher.Unregister(GameEventTypes.ColonyEventType.GamePaused, OnPaused);
        EventDispatcher.Unregister(GameEventTypes.ColonyEventType.GameResumed, OnResumed);
        EventDispatcher.Unregister(GameEventTypes.ColonyEventType.FoodChanged, OnFood);
        EventDispatcher.Unregister(GameEventTypes.ColonyEventType.SoldierCountChanged, OnSoldiers);
        EventDispatcher.Unregister(GameEventTypes.ColonyEventType.CollectorCountChanged, OnCollectors);
        EventDispatcher.Unregister(GameEventTypes.ColonyEventType.AnthillHealthChanged, OnHill);
        EventDispatcher.Unregister(GameEventTypes.ColonyEventType.NestHealthChanged, OnNest);
        EventDispatcher.Unregister(GameEventTypes.ColonyEventType.ScoreChanged, OnScore);
        EventDispatcher.Unregister(GameEventTypes.ColonyEventType.GameWon, OnWon);
        EventDispatcher.Unregister(GameEventTypes.ColonyEventType.GameLost, OnLost);
        EventDispatcher.Unregister(GameEventTypes.UIEventType.PlayerHUDUpdated, OnHp);
        EventDispatcher.Unregister(GameEventTypes.ColonyEventType.MenuShown, OnMenu);
        EventDispatcher.Unregister(GameEventTypes.ColonyEventType.Feedback, OnFeedback);
    }

    void OnFeedback(EventDispatcher.EventData data)
    {
        var label = GetComponent<UIDocument>().rootVisualElement.Q<Label>("hint-label");
        if (label != null && data.parameters.Length > 0) label.text = (string)data.parameters[0];
    }

    void OnLoad(EventDispatcher.EventData data)
    {
        int n = data.parameters.Length > 0 ? (int)data.parameters[0] : 0;
        int m = data.parameters.Length > 1 ? (int)data.parameters[1] : 1;
        if (loadLabel != null)
        {
            loadLabel.text = $"Gathering crumbs {n}/{m}";
        }

        if (loadFill != null)
        {
            loadFill.style.width = Length.Percent(m <= 0 ? 100 : 100f * n / m);
        }

        if (n >= m)
        {
            Hide(load);
            Show(start);
        }
    }

    void OnStarted(EventDispatcher.EventData _)
    {
        Hide(load);
        Hide(start);
        Hide(how);
        Hide(pause);
        Hide(result);
        ShowHud(true);
    }

    void OnPaused(EventDispatcher.EventData _)
    {
        Show(pause);
        ShowHud(false);
    }

    void OnResumed(EventDispatcher.EventData _)
    {
        Hide(pause);
        Hide(how);
        ShowHud(true);
    }

    void OnMenu(EventDispatcher.EventData _)
    {
        Hide(pause);
        Hide(result);
        Hide(how);
        ShowHud(false);
        Hide(load);
        Show(start);
    }

    void OnFood(EventDispatcher.EventData data)
    {
        int food = data.parameters.Length > 0 ? (int)data.parameters[0] : 0;
        if (foodLabel != null)
        {
            foodLabel.text = SquadText(ColonyGameDirector.Instance, food);
        }
    }

    void OnSoldiers(EventDispatcher.EventData data)
    {
        var d = ColonyGameDirector.Instance;
        if (foodLabel != null && d != null)
        {
            foodLabel.text = SquadText(d, d.food);
        }
    }

    void OnCollectors(EventDispatcher.EventData _)
    {
        var d = ColonyGameDirector.Instance;
        if (foodLabel != null && d != null)
        {
            foodLabel.text = SquadText(d, d.food);
        }
    }

    void OnHill(EventDispatcher.EventData data)
    {
        float cur = data.parameters.Length > 0 ? System.Convert.ToSingle(data.parameters[0]) : 0f;
        float max = data.parameters.Length > 1 ? System.Convert.ToSingle(data.parameters[1]) : 420f;
        if (hillLabel != null)
        {
            hillLabel.text = $"Enemy Hill {Mathf.CeilToInt(cur)}";
        }

        if (hillFill != null)
        {
            hillFill.style.width = Length.Percent(max <= 0 ? 0 : 100f * cur / max);
        }
    }

    void OnNest(EventDispatcher.EventData data)
    {
        float cur = data.parameters.Length > 0 ? System.Convert.ToSingle(data.parameters[0]) : 0f;
        float max = data.parameters.Length > 1 ? System.Convert.ToSingle(data.parameters[1]) : 360f;
        if (nestLabel != null)
        {
            nestLabel.text = $"Nest {Mathf.CeilToInt(cur)}";
        }

        if (nestFill != null)
        {
            nestFill.style.width = Length.Percent(max <= 0 ? 0 : 100f * cur / max);
        }
    }

    void OnScore(EventDispatcher.EventData data)
    {
        int s = data.parameters.Length > 0 ? System.Convert.ToInt32(data.parameters[0]) : 0;
        if (scoreLabel != null)
        {
            scoreLabel.text = $"Score {s}";
        }
    }

    void OnHp(EventDispatcher.EventData data)
    {
        float cur = data.parameters.Length > 1 ? System.Convert.ToSingle(data.parameters[1]) : 0f;
        float max = data.parameters.Length > 2 ? System.Convert.ToSingle(data.parameters[2]) : 100f;
        if (hpLabel != null)
        {
            hpLabel.text = $"Health {Mathf.CeilToInt(cur)} / {Mathf.CeilToInt(max)}";
        }

        if (hpFill != null)
        {
            hpFill.style.width = Length.Percent(max <= 0 ? 0 : 100f * cur / max);
        }
    }

    void OnWon(EventDispatcher.EventData data)
    {
        ShowHud(false);
        Show(result);
        if (resultTitle != null)
        {
            resultTitle.text = "Hill Destroyed";
        }

        int s = data.parameters.Length > 0 ? System.Convert.ToInt32(data.parameters[0]) : 0;
        if (resultScore != null)
        {
            resultScore.text = $"Score {s}";
        }
    }

    void OnLost(EventDispatcher.EventData data)
    {
        ShowHud(false);
        Show(result);
        if (resultTitle != null)
        {
            resultTitle.text = "Nest Destroyed";
        }

        int s = data.parameters.Length > 0 ? System.Convert.ToInt32(data.parameters[0]) : 0;
        if (resultScore != null)
        {
            resultScore.text = $"Score {s}";
        }
    }

    void ShowHud(bool on)
    {
        if (hud == null)
        {
            return;
        }

        hud.style.display = on ? DisplayStyle.Flex : DisplayStyle.None;
    }

    static void Show(VisualElement v)
    {
        if (v != null)
        {
            v.style.display = DisplayStyle.Flex;
        }
    }

    static void Hide(VisualElement v)
    {
        if (v != null)
        {
            v.style.display = DisplayStyle.None;
        }
    }

    static string SquadText(ColonyGameDirector d, int food)
    {
        int soldiers = d != null ? d.SoldierCount : 0;
        int collectors = d != null ? d.CollectorCount : 0;
        return $"Food {food} · S {soldiers}/4 · C {collectors}/3";
    }
}
