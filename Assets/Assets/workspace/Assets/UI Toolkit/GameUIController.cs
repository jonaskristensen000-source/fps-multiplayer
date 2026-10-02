using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Inspector-driven controller for the GameUI.uss layout.
///
/// Add this component to the same GameObject as the UIDocument, assign the UIDocument,
/// then tune the serialized values in the Inspector. The controller applies the values
/// to the existing UI Toolkit elements by USS class/name.
///
/// The original GameUI.uss can remain in the UIDocument's Style Sheets list for layout,
/// text alignment, and any selectors you still want to keep. This script overrides the
/// visual values exposed below at runtime.
/// </summary>
[DisallowMultipleComponent]
public class GameUIController : MonoBehaviour
{
    [Header("UI Document")]
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private bool applyOnEnable = true;
    [SerializeField] private bool reapplyWhenPanelChanges = true;
    [SerializeField] private Font uiFont;

    [Header("Theme Colors")]
    [SerializeField] private Color backgroundColor = new Color(28f / 255f, 19f / 255f, 12f / 255f, 0.82f);
    [SerializeField] private Color panelColor = new Color(42f / 255f, 27f / 255f, 17f / 255f, 0.97f);
    [SerializeField] private Color textColor = new Color(1f, 240f / 255f, 214f / 255f, 1f);
    [SerializeField] private Color mutedColor = new Color(214f / 255f, 186f / 255f, 150f / 255f, 1f);
    [SerializeField] private Color accentColor = new Color(242f / 255f, 196f / 255f, 74f / 255f, 1f);
    [SerializeField] private Color dangerColor = new Color(214f / 255f, 92f / 255f, 42f / 255f, 1f);
    [SerializeField] private Color allyColor = new Color(166f / 255f, 214f / 255f, 106f / 255f, 1f);
    [SerializeField] private Color darkButtonTextColor = new Color(42f / 255f, 27f / 255f, 17f / 255f, 1f);
    [SerializeField] private Color soldierButtonTextColor = new Color(28f / 255f, 19f / 255f, 12f / 255f, 1f);
    [SerializeField] private Color barBackgroundColor = new Color(83f / 255f, 60f / 255f, 41f / 255f, 1f);
    [SerializeField] private Color secondaryButtonColor = new Color(91f / 255f, 64f / 255f, 40f / 255f, 1f);

    [Header("Screen")]
    [Min(0f)] [SerializeField] private float sharedSpacing = 12f;
    [Min(0f)] [SerializeField] private float screenBackgroundAlpha = 0.82f;
    [Min(0f)] [SerializeField] private float startScreenBackgroundAlpha = 0.35f;
    [Min(0f)] [SerializeField] private float screenHorizontalPaddingPercent = 6f;

    [Header("Main Panel")]
    [Min(0f)] [SerializeField] private float panelWidth = 610f;
    [Range(0f, 100f)] [SerializeField] private float panelMaxWidthPercent = 88f;
    [Min(0f)] [SerializeField] private float panelBorderLeftWidth = 5f;
    [Min(0f)] [SerializeField] private float panelPaddingTop = 28f;
    [Min(0f)] [SerializeField] private float panelPaddingRight = 34f;
    [Min(0f)] [SerializeField] private float panelPaddingBottom = 28f;
    [Min(0f)] [SerializeField] private float panelPaddingLeft = 34f;
    [Min(0f)] [SerializeField] private float panelRadius = 4f;
    [Min(0f)] [SerializeField] private float startPanelWidth = 520f;

    [Header("Title")]
    [Min(0f)] [SerializeField] private float titleFontSize = 44f;
    [Min(0f)] [SerializeField] private float titleMarginBottom = 12f;

    [Header("Subtitle")]
    [Min(0f)] [SerializeField] private float subtitleFontSize = 20f;
    [Min(0f)] [SerializeField] private float subtitleMarginBottom = 20f;

    [Header("Body")]
    [Min(0f)] [SerializeField] private float bodyFontSize = 19f;
    [Min(0f)] [SerializeField] private float bodyMarginBottom = 16f;

    [Header("Standard Buttons")]
    [Min(0f)] [SerializeField] private float buttonHeight = 52f;
    [Min(0f)] [SerializeField] private float buttonFontSize = 22f;
    [Min(0f)] [SerializeField] private float buttonMarginTop = 12f;
    [Min(0f)] [SerializeField] private float buttonRadius = 4f;
    [Min(0f)] [SerializeField] private float buttonHoverScale = 1.02f;
    [Min(0f)] [SerializeField] private float buttonPressedScale = 0.98f;
    [Min(0f)] [SerializeField] private float buttonTransitionDuration = 0.12f;
    [SerializeField] private Color buttonHoverColor = new Color(1f, 240f / 255f, 214f / 255f, 1f);
    [SerializeField] private Color buttonPressedColor = new Color(166f / 255f, 214f / 255f, 106f / 255f, 1f);

    [Header("HUD")]
    [Min(0f)] [SerializeField] private float hudTop = 22f;
    [Min(0f)] [SerializeField] private float hudSideOffset = 24f;
    [Min(0f)] [SerializeField] private float hudPanelWidth = 290f;
    [Min(0f)] [SerializeField] private float hudPaddingVertical = 12f;
    [Min(0f)] [SerializeField] private float hudPaddingHorizontal = 16f;
    [Min(0f)] [SerializeField] private float hudRadius = 4f;
    [Min(0f)] [SerializeField] private float hudBackgroundAlpha = 0.80f;

    [Header("Spawn Panel")]
    [Min(0f)] [SerializeField] private float spawnRight = 24f;
    [Min(0f)] [SerializeField] private float spawnBottom = 86f;
    [Min(0f)] [SerializeField] private float spawnWidth = 250f;
    [Min(0f)] [SerializeField] private float spawnPaddingVertical = 12f;
    [Min(0f)] [SerializeField] private float spawnPaddingHorizontal = 14f;
    [Min(0f)] [SerializeField] private float spawnBorderLeftWidth = 4f;
    [Min(0f)] [SerializeField] private float spawnRadius = 4f;
    [Min(0f)] [SerializeField] private float spawnBackgroundAlpha = 0.86f;

    [Header("Spawn Caption")]
    [Min(0f)] [SerializeField] private float spawnCaptionFontSize = 15f;
    [Min(0f)] [SerializeField] private float spawnCaptionMarginBottom = 8f;
    [Min(0f)] [SerializeField] private float spawnCaptionHeight = 20f;

    [Header("Spawn Buttons")]
    [Min(0f)] [SerializeField] private float spawnButtonHeight = 42f;
    [Min(0f)] [SerializeField] private float spawnButtonMarginTop = 8f;
    [Min(0f)] [SerializeField] private float spawnButtonFontSize = 18f;
    [Min(0f)] [SerializeField] private float spawnButtonRadius = 4f;
    [Min(0f)] [SerializeField] private float spawnButtonHoverScale = 1.03f;
    [Min(0f)] [SerializeField] private float spawnButtonPressedScale = 0.97f;
    [Min(0f)] [SerializeField] private float spawnButtonTransitionDuration = 0.12f;

    [Header("Bars")]
    [Min(0f)] [SerializeField] private float barLabelFontSize = 19f;
    [Min(0f)] [SerializeField] private float barLabelMarginBottom = 5f;
    [Min(0f)] [SerializeField] private float barLabelHeight = 25f;
    [Min(0f)] [SerializeField] private float barWidth = 258f;
    [Min(0f)] [SerializeField] private float barHeight = 8f;
    [Min(0f)] [SerializeField] private float barMarginBottom = 10f;
    [Min(0f)] [SerializeField] private float fillHeight = 8f;

    [Header("Crosshair")]
    [Min(0f)] [SerializeField] private float crosshairWidth = 10f;
    [Min(0f)] [SerializeField] private float crosshairHeight = 10f;
    [Min(0f)] [SerializeField] private float crosshairBorderWidth = 2f;
    [Min(0f)] [SerializeField] private float crosshairRadius = 5f;

    [Header("Hint")]
    [Range(0f, 50f)] [SerializeField] private float hintHorizontalPercent = 16f;
    [Min(0f)] [SerializeField] private float hintBottom = 24f;
    [Min(0f)] [SerializeField] private float hintPaddingVertical = 10f;
    [Min(0f)] [SerializeField] private float hintPaddingHorizontal = 16f;
    [Min(0f)] [SerializeField] private float hintFontSize = 18f;
    [Min(0f)] [SerializeField] private float hintBackgroundAlpha = 0.78f;

    [Header("Loading Bar")]
    [Min(0f)] [SerializeField] private float loadWrapWidth = 540f;
    [Min(0f)] [SerializeField] private float loadBarHeight = 12f;
    [Min(0f)] [SerializeField] private float loadBarMarginTop = 16f;

    [Header("Advanced")]
    [SerializeField] private bool preserveUSSFontAndTextAlignment = true;
    [SerializeField] private bool includeInactiveElements = true;

    private VisualElement root;
    private readonly List<VisualElement> hookedButtons = new List<VisualElement>();
    private IVisualElementScheduledItem delayedApply;

    private static Color WithAlpha(Color color, float alpha)
    {
        color.a = Mathf.Clamp01(alpha);
        return color;
    }

    private void OnEnable()
    {
        if (applyOnEnable)
            ApplySettings();
    }

    private void OnDisable()
    {
        delayedApply?.Pause();
        UnhookButtonEvents();
    }

    /// <summary>Call this after changing values at runtime.</summary>
    [ContextMenu("Apply UI Settings")]
    public void ApplySettings()
    {
        if (uiDocument == null)
            uiDocument = GetComponent<UIDocument>();

        if (uiDocument == null)
        {
            Debug.LogWarning("GameUIController: No UIDocument assigned or found on this GameObject.", this);
            return;
        }

        root = uiDocument.rootVisualElement;
        if (root == null)
            return;

        ApplyRootAndScreens();
        ApplyPanels();
        ApplyText();
        ApplyStandardButtons();
        ApplyHUD();
        ApplySpawnUI();
        ApplyBars();
        ApplyCrosshair();
        ApplyHint();
        ApplyLoadingBar();
        ApplyFont();
        HookButtonEvents();

        if (reapplyWhenPanelChanges)
        {
            delayedApply?.Pause();
            delayedApply = root.schedule.Execute(ApplyDynamicValuesOnce).StartingIn(1);
        }
    }

    private void ApplyDynamicValuesOnce()
    {
        if (root == null)
            return;

        ApplyRootAndScreens();
        ApplyPanels();
        ApplyText();
        ApplyStandardButtons();
        ApplyHUD();
        ApplySpawnUI();
        ApplyBars();
        ApplyCrosshair();
        ApplyHint();
        ApplyLoadingBar();
        HookButtonEvents();
    }

    private IEnumerable<VisualElement> AllElements()
    {
        if (root == null)
            yield break;

        var stack = new Stack<VisualElement>();
        stack.Push(root);

        while (stack.Count > 0)
        {
            VisualElement current = stack.Pop();
            yield return current;

            for (int i = current.childCount - 1; i >= 0; i--)
            {
                VisualElement child = current.ElementAt(i);
                if (includeInactiveElements || child.resolvedStyle.display != DisplayStyle.None)
                    stack.Push(child);
            }
        }
    }

    private VisualElement First(string selector)
    {
        return root?.Q(selector);
    }

    private IEnumerable<VisualElement> ByClass(string className)
    {
        if (root == null)
            yield break;

        foreach (VisualElement element in root.Query<VisualElement>(className).ToList())
            yield return element;
    }

    private void ApplyRootAndScreens()
    {
        root.style.flexGrow = 1;

        foreach (VisualElement screen in ByClass("screen"))
        {
            screen.style.backgroundColor = WithAlpha(backgroundColor, screenBackgroundAlpha);
            screen.style.alignItems = Align.Center;
            screen.style.justifyContent = Justify.Center;
        }

        VisualElement startScreen = First("#start-screen");
        if (startScreen != null)
        {
            startScreen.style.backgroundColor = WithAlpha(backgroundColor, startScreenBackgroundAlpha);
            startScreen.style.alignItems = Align.FlexStart;
            startScreen.style.paddingLeft = Length.Percent(screenHorizontalPaddingPercent);
        }
    }

    private void ApplyPanels()
    {
        foreach (VisualElement panel in ByClass("panel"))
        {
            panel.style.width = panelWidth;
            panel.style.maxWidth = Length.Percent(panelMaxWidthPercent);
            panel.style.backgroundColor = panelColor;
            panel.style.borderLeftWidth = panelBorderLeftWidth;
            panel.style.borderLeftColor = accentColor;
            panel.style.borderTopLeftRadius = panelRadius;
            panel.style.borderTopRightRadius = panelRadius;
            panel.style.borderBottomLeftRadius = panelRadius;
            panel.style.borderBottomRightRadius = panelRadius;
            panel.style.paddingTop = panelPaddingTop;
            panel.style.paddingRight = panelPaddingRight;
            panel.style.paddingBottom = panelPaddingBottom;
            panel.style.paddingLeft = panelPaddingLeft;
            panel.style.alignItems = Align.Stretch;
        }

        VisualElement startScreen = First("start-screen");
        VisualElement startPanel = startScreen?.Q(className: "panel");
        if (startPanel != null)
            startPanel.style.width = startPanelWidth;
    }

    private void ApplyText()
    {
        foreach (VisualElement title in ByClass("title"))
        {
            title.style.fontSize = titleFontSize;
            title.style.color = accentColor;
            title.style.marginBottom = titleMarginBottom;
            title.style.whiteSpace = WhiteSpace.Normal;
            title.style.unityFontStyleAndWeight = FontStyle.Bold;
        }

        foreach (VisualElement subtitle in ByClass("subtitle"))
        {
            subtitle.style.fontSize = subtitleFontSize;
            subtitle.style.color = mutedColor;
            subtitle.style.marginBottom = subtitleMarginBottom;
            subtitle.style.whiteSpace = WhiteSpace.Normal;
        }

        foreach (VisualElement body in ByClass("body"))
        {
            body.style.fontSize = bodyFontSize;
            body.style.color = textColor;
            body.style.marginBottom = bodyMarginBottom;
            body.style.whiteSpace = WhiteSpace.Normal;
        }
    }

    private void ApplyStandardButtons()
    {
        foreach (VisualElement button in ByClass("btn"))
        {
            button.style.height = buttonHeight;
            button.style.backgroundColor = accentColor;
            button.style.color = darkButtonTextColor;
            button.style.fontSize = buttonFontSize;
            button.style.unityFontStyleAndWeight = FontStyle.Bold;
            button.style.borderTopLeftRadius = buttonRadius;
            button.style.borderTopRightRadius = buttonRadius;
            button.style.borderBottomLeftRadius = buttonRadius;
            button.style.borderBottomRightRadius = buttonRadius;
            button.style.marginTop = buttonMarginTop;
            button.style.borderTopWidth = 0;
            button.style.borderRightWidth = 0;
            button.style.borderBottomWidth = 0;
            button.style.borderLeftWidth = 0;
            button.style.transitionDuration = new List<TimeValue> { new TimeValue(buttonTransitionDuration, TimeUnit.Second) };
        }

        SetButtonStyle("#how-btn", secondaryButtonColor, textColor);
        SetButtonStyle("#quit-btn", secondaryButtonColor, textColor);
        SetButtonStyle("#result-menu-btn", secondaryButtonColor, textColor);
    }

    private void SetButtonStyle(string selector, Color background, Color foreground)
    {
        VisualElement button = First(selector);
        if (button == null)
            return;

        button.style.backgroundColor = background;
        button.style.color = foreground;
    }

    private void ApplyHUD()
    {
        foreach (VisualElement hud in ByClass("hud"))
        {
            hud.style.position = Position.Absolute;
            hud.style.top = 0;
            hud.style.left = 0;
            hud.style.right = 0;
            hud.style.bottom = 0;
        }

        foreach (VisualElement element in ByClass("hud-tl"))
            ApplyHUDPanel(element, true);

        foreach (VisualElement element in ByClass("hud-tr"))
            ApplyHUDPanel(element, false);
    }

    private void ApplyHUDPanel(VisualElement element, bool left)
    {
        element.style.position = Position.Absolute;
        element.style.top = hudTop;
        if (left)
        {
            element.style.left = hudSideOffset;
            element.style.right = StyleKeyword.Auto;
        }
        else
        {
            element.style.right = hudSideOffset;
            element.style.left = StyleKeyword.Auto;
        }
        element.style.width = hudPanelWidth;
        element.style.backgroundColor = WithAlpha(backgroundColor, hudBackgroundAlpha);
        element.style.paddingTop = hudPaddingVertical;
        element.style.paddingBottom = hudPaddingVertical;
        element.style.paddingLeft = hudPaddingHorizontal;
        element.style.paddingRight = hudPaddingHorizontal;
        SetRadius(element, hudRadius);
    }

    private void ApplySpawnUI()
    {
        foreach (VisualElement spawn in ByClass("hud-spawn"))
        {
            spawn.style.position = Position.Absolute;
            spawn.style.right = spawnRight;
            spawn.style.bottom = spawnBottom;
            spawn.style.width = spawnWidth;
            spawn.style.backgroundColor = WithAlpha(backgroundColor, spawnBackgroundAlpha);
            spawn.style.paddingTop = spawnPaddingVertical;
            spawn.style.paddingBottom = spawnPaddingVertical;
            spawn.style.paddingLeft = spawnPaddingHorizontal;
            spawn.style.paddingRight = spawnPaddingHorizontal;
            spawn.style.borderLeftWidth = spawnBorderLeftWidth;
            spawn.style.borderLeftColor = accentColor;
            SetRadius(spawn, spawnRadius);
        }

        foreach (VisualElement caption in ByClass("spawn-caption"))
        {
            caption.style.fontSize = spawnCaptionFontSize;
            caption.style.color = mutedColor;
            caption.style.marginBottom = spawnCaptionMarginBottom;
            caption.style.height = spawnCaptionHeight;
            caption.style.whiteSpace = WhiteSpace.NoWrap;
        }

        foreach (VisualElement button in ByClass("spawn-btn"))
        {
            button.style.height = spawnButtonHeight;
            button.style.marginTop = spawnButtonMarginTop;
            button.style.borderTopWidth = 0;
            button.style.borderRightWidth = 0;
            button.style.borderBottomWidth = 0;
            button.style.borderLeftWidth = 0;
            button.style.fontSize = spawnButtonFontSize;
            button.style.unityFontStyleAndWeight = FontStyle.Bold;
            SetRadius(button, spawnButtonRadius);
            button.style.transitionDuration = new List<TimeValue> { new TimeValue(spawnButtonTransitionDuration, TimeUnit.Second) };
        }

        foreach (VisualElement button in ByClass("collector-btn"))
        {
            button.style.backgroundColor = accentColor;
            button.style.color = darkButtonTextColor;
        }

        foreach (VisualElement button in ByClass("soldier-btn"))
        {
            button.style.backgroundColor = allyColor;
            button.style.color = soldierButtonTextColor;
        }
    }

    private void ApplyBars()
    {
        foreach (VisualElement label in ByClass("bar-label"))
        {
            label.style.fontSize = barLabelFontSize;
            label.style.unityFontStyleAndWeight = FontStyle.Bold;
            label.style.color = textColor;
            label.style.marginBottom = barLabelMarginBottom;
            label.style.height = barLabelHeight;
            label.style.whiteSpace = WhiteSpace.NoWrap;
        }

        foreach (VisualElement bar in ByClass("bar"))
        {
            bar.style.width = barWidth;
            bar.style.height = barHeight;
            bar.style.backgroundColor = barBackgroundColor;
            bar.style.marginBottom = barMarginBottom;
            bar.style.overflow = Overflow.Hidden;
        }

        foreach (VisualElement fill in ByClass("bar-fill"))
        {
            fill.style.height = fillHeight;
            fill.style.backgroundColor = allyColor;
            fill.style.width = Length.Percent(100f);
        }

        foreach (VisualElement fill in ByClass("bar-fill-hill"))
        {
            fill.style.height = fillHeight;
            fill.style.backgroundColor = dangerColor;
            fill.style.width = Length.Percent(100f);
        }
    }

    private void ApplyCrosshair()
    {
        foreach (VisualElement crosshair in ByClass("crosshair"))
        {
            crosshair.style.position = Position.Absolute;
            crosshair.style.left = Length.Percent(50f);
            crosshair.style.top = Length.Percent(50f);
            crosshair.style.width = crosshairWidth;
            crosshair.style.height = crosshairHeight;
            crosshair.style.marginLeft = -crosshairWidth * 0.5f;
            crosshair.style.marginTop = -crosshairHeight * 0.5f;
            crosshair.style.borderTopWidth = crosshairBorderWidth;
            crosshair.style.borderRightWidth = crosshairBorderWidth;
            crosshair.style.borderBottomWidth = crosshairBorderWidth;
            crosshair.style.borderLeftWidth = crosshairBorderWidth;
            crosshair.style.borderTopColor = textColor;
            crosshair.style.borderRightColor = textColor;
            crosshair.style.borderBottomColor = textColor;
            crosshair.style.borderLeftColor = textColor;
            SetRadius(crosshair, crosshairRadius);
        }
    }

    private void ApplyHint()
    {
        foreach (VisualElement hint in ByClass("hint"))
        {
            hint.style.position = Position.Absolute;
            hint.style.bottom = hintBottom;
            hint.style.left = Length.Percent(hintHorizontalPercent);
            hint.style.right = Length.Percent(hintHorizontalPercent);
            hint.style.paddingTop = hintPaddingVertical;
            hint.style.paddingBottom = hintPaddingVertical;
            hint.style.paddingLeft = hintPaddingHorizontal;
            hint.style.paddingRight = hintPaddingHorizontal;
            hint.style.color = textColor;
            hint.style.backgroundColor = WithAlpha(backgroundColor, hintBackgroundAlpha);
            hint.style.fontSize = hintFontSize;
            hint.style.whiteSpace = WhiteSpace.Normal;
            hint.style.unityTextAlign = TextAnchor.MiddleCenter;
        }
    }

    private void ApplyLoadingBar()
    {
        foreach (VisualElement wrap in ByClass("load-wrap"))
            wrap.style.width = loadWrapWidth;

        foreach (VisualElement bar in ByClass("load-bar"))
        {
            bar.style.height = loadBarHeight;
            bar.style.backgroundColor = barBackgroundColor;
            bar.style.marginTop = loadBarMarginTop;
            bar.style.overflow = Overflow.Hidden;
        }

        foreach (VisualElement fill in ByClass("load-fill"))
        {
            fill.style.height = loadBarHeight;
            fill.style.width = Length.Percent(0f);
            fill.style.backgroundColor = accentColor;
        }
    }

    private void ApplyFont()
    {
        if (!preserveUSSFontAndTextAlignment || uiFont == null)
            return;

        FontDefinition definition = FontDefinition.FromFont(uiFont);
        foreach (VisualElement element in AllElements())
            element.style.unityFontDefinition = new StyleFontDefinition(definition);
    }

    private static void SetRadius(VisualElement element, float radius)
    {
        element.style.borderTopLeftRadius = radius;
        element.style.borderTopRightRadius = radius;
        element.style.borderBottomLeftRadius = radius;
        element.style.borderBottomRightRadius = radius;
    }

    private void HookButtonEvents()
    {
        UnhookButtonEvents();

        foreach (VisualElement button in ByClass("btn"))
        {
            Color baseBackground = IsSecondaryButton(button) ? secondaryButtonColor : accentColor;
            Color baseText = IsSecondaryButton(button) ? textColor : darkButtonTextColor;
            HookButton(button, buttonHoverScale, buttonPressedScale, baseBackground, baseText, buttonHoverColor, buttonPressedColor, true);
        }

        foreach (VisualElement button in ByClass("spawn-btn"))
        {
            Color baseBackground = button.ClassListContains("soldier-btn") ? allyColor : accentColor;
            Color baseText = button.ClassListContains("soldier-btn") ? soldierButtonTextColor : darkButtonTextColor;
            HookButton(button, spawnButtonHoverScale, spawnButtonPressedScale, baseBackground, baseText, baseBackground, baseBackground, false);
        }
    }

    private bool IsSecondaryButton(VisualElement button)
    {
        return button.name == "how-btn" || button.name == "quit-btn" || button.name == "result-menu-btn";
    }

    private void HookButton(
        VisualElement button,
        float hoverScale,
        float pressedScale,
        Color normalBackground,
        Color normalText,
        Color hoverBackground,
        Color pressedBackground,
        bool changeColors)
    {
        EventCallback<MouseEnterEvent> enter = _ =>
        {
            button.style.scale = new Scale(new Vector3(hoverScale, hoverScale, 1f));
            if (changeColors)
                button.style.backgroundColor = hoverBackground;
        };

        EventCallback<MouseLeaveEvent> leave = _ =>
        {
            button.style.scale = new Scale(Vector3.one);
            if (changeColors)
            {
                button.style.backgroundColor = normalBackground;
                button.style.color = normalText;
            }
        };

        EventCallback<MouseDownEvent> down = _ =>
        {
            button.style.scale = new Scale(new Vector3(pressedScale, pressedScale, 1f));
            if (changeColors)
                button.style.backgroundColor = pressedBackground;
        };

        EventCallback<MouseUpEvent> up = _ =>
        {
            button.style.scale = new Scale(new Vector3(hoverScale, hoverScale, 1f));
            if (changeColors)
                button.style.backgroundColor = hoverBackground;
        };

        button.RegisterCallback(enter);
        button.RegisterCallback(leave);
        button.RegisterCallback(down);
        button.RegisterCallback(up);

        hookedButtons.Add(button);
        button.userData = new ButtonCallbacks(enter, leave, down, up);
    }

    private void UnhookButtonEvents()
    {
        for (int i = 0; i < hookedButtons.Count; i++)
        {
            VisualElement button = hookedButtons[i];
            if (button == null || !(button.userData is ButtonCallbacks callbacks))
                continue;

            button.UnregisterCallback(callbacks.Enter);
            button.UnregisterCallback(callbacks.Leave);
            button.UnregisterCallback(callbacks.Down);
            button.UnregisterCallback(callbacks.Up);
            button.style.scale = new Scale(Vector3.one);
            button.userData = null;
        }

        hookedButtons.Clear();
    }

    private sealed class ButtonCallbacks
    {
        public readonly EventCallback<MouseEnterEvent> Enter;
        public readonly EventCallback<MouseLeaveEvent> Leave;
        public readonly EventCallback<MouseDownEvent> Down;
        public readonly EventCallback<MouseUpEvent> Up;

        public ButtonCallbacks(
            EventCallback<MouseEnterEvent> enter,
            EventCallback<MouseLeaveEvent> leave,
            EventCallback<MouseDownEvent> down,
            EventCallback<MouseUpEvent> up)
        {
            Enter = enter;
            Leave = leave;
            Down = down;
            Up = up;
        }
    }
}
