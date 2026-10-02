using UnityEngine;

public class ColonyCursor : MonoBehaviour
{
    public bool hideDuringPlay = true;
    bool playing;

    void OnEnable()
    {
        EventDispatcher.Register(GameEventTypes.ColonyEventType.GameStarted, OnPlay);
        EventDispatcher.Register(GameEventTypes.ColonyEventType.GameResumed, OnPlay);
        EventDispatcher.Register(GameEventTypes.ColonyEventType.GamePaused, OnUi);
        EventDispatcher.Register(GameEventTypes.ColonyEventType.GameWon, OnUi);
        EventDispatcher.Register(GameEventTypes.ColonyEventType.GameLost, OnUi);
        EventDispatcher.Register(GameEventTypes.ColonyEventType.MenuShown, OnUi);
        playing = false;
        ShowCursor();
    }

    void OnDisable()
    {
        EventDispatcher.Unregister(GameEventTypes.ColonyEventType.GameStarted, OnPlay);
        EventDispatcher.Unregister(GameEventTypes.ColonyEventType.GameResumed, OnPlay);
        EventDispatcher.Unregister(GameEventTypes.ColonyEventType.GamePaused, OnUi);
        EventDispatcher.Unregister(GameEventTypes.ColonyEventType.GameWon, OnUi);
        EventDispatcher.Unregister(GameEventTypes.ColonyEventType.GameLost, OnUi);
        EventDispatcher.Unregister(GameEventTypes.ColonyEventType.MenuShown, OnUi);
    }

    void Update()
    {
        if (!playing || !hideDuringPlay)
        {
            return;
        }

        if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
        {
            ShowCursor();
        }
        else
        {
            HideCursor();
        }
    }

    void OnPlay(EventDispatcher.EventData _)
    {
        playing = true;
        if (hideDuringPlay)
        {
            HideCursor();
        }
    }

    void OnUi(EventDispatcher.EventData _)
    {
        playing = false;
        ShowCursor();
    }

    static void HideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    static void ShowCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
