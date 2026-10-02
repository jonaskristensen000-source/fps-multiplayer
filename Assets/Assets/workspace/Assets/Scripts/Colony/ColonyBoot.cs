using System.Collections;
using UnityEngine;

public class ColonyBoot : MonoBehaviour
{
    public float timeoutSeconds = 30f;
    void OnEnable()
    {
        EventDispatcher.Register(GameEventTypes.ColonyEventType.RequestStart, OnStart);
        EventDispatcher.Register(GameEventTypes.ColonyEventType.RequestRestart, OnStart);
        EventDispatcher.Register(GameEventTypes.ColonyEventType.RequestMenu, OnMenu);
        EventDispatcher.Register(GameEventTypes.ColonyEventType.RequestResume, OnResume);
        EventDispatcher.Register(GameEventTypes.ColonyEventType.RequestSummonSoldier, OnSoldier);
        EventDispatcher.Register(GameEventTypes.ColonyEventType.RequestSummonCollector, OnCollector);
    }
    void OnDisable()
    {
        EventDispatcher.Unregister(GameEventTypes.ColonyEventType.RequestStart, OnStart);
        EventDispatcher.Unregister(GameEventTypes.ColonyEventType.RequestRestart, OnStart);
        EventDispatcher.Unregister(GameEventTypes.ColonyEventType.RequestMenu, OnMenu);
        EventDispatcher.Unregister(GameEventTypes.ColonyEventType.RequestResume, OnResume);
        EventDispatcher.Unregister(GameEventTypes.ColonyEventType.RequestSummonSoldier, OnSoldier);
        EventDispatcher.Unregister(GameEventTypes.ColonyEventType.RequestSummonCollector, OnCollector);
    }
    IEnumerator Start()
    {
        yield return null;
        var d = ColonyGameDirector.Instance;
        Object[] required = { d.Player, d.PlayerHealth, d.Anthill, d.Nest, d.soldierPrefab, d.collectorPrefab, d.globPrefab, d.Anthill.raiderPrefab };
        for (int i = 0; i < required.Length; i++)
        {
            if (required[i] == null)
            {
                Debug.LogError("Missing required colony asset at slot " + i);
                yield break;
            }
            EventDispatcher.SendEventImmediate(GameEventTypes.ColonyEventType.LoadingProgress, i + 1, required.Length + 1);
            yield return null;
        }
        var music = GetComponent<AudioSource>();
        float started = Time.realtimeSinceStartup;
        if (music != null && music.clip != null)
        {
            music.clip.LoadAudioData();
            while (music.clip.loadState == AudioDataLoadState.Loading && Time.realtimeSinceStartup - started < timeoutSeconds)
                yield return null;
        }
        EventDispatcher.SendEventImmediate(GameEventTypes.ColonyEventType.LoadingProgress, required.Length + 1, required.Length + 1);
        d.ReturnToMenu();
    }
    void OnStart(EventDispatcher.EventData _) => ColonyGameDirector.Instance.BeginPlay();
    void OnMenu(EventDispatcher.EventData _) => ColonyGameDirector.Instance.ReturnToMenu();
    void OnResume(EventDispatcher.EventData _) => ColonyGameDirector.Instance.ResumeGame();
    void OnSoldier(EventDispatcher.EventData _) => ColonyGameDirector.Instance.TrySummonSoldier();
    void OnCollector(EventDispatcher.EventData _) => ColonyGameDirector.Instance.TrySummonCollector();
}
