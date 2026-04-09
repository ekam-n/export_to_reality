using UnityEngine;

public class UIActive : MonoBehaviour
{
    [Header("Settings")]
    public GameObject targetUI;
    public bool matchTargetState = false;

    private bool _lastState;
    private CanvasGroup _canvasGroup;

    void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
    }

    void Start()
    {
        if (targetUI != null)
        {
            _lastState = targetUI.activeInHierarchy;
            Apply(matchTargetState ? _lastState : !_lastState);
        }
    }

    void Update()
    {
        if (targetUI == null) return;

        bool currentState = targetUI.activeInHierarchy;
        if (currentState == _lastState) return;

        _lastState = currentState;
        Apply(matchTargetState ? currentState : !currentState);
    }

    void Apply(bool state)
    {
        // CanvasGroup hides/shows this object without deactivating it,
        // so Update() keeps running and can re-activate when needed.
        _canvasGroup.alpha          = state ? 1f : 0f;
        _canvasGroup.interactable   = state;
        _canvasGroup.blocksRaycasts = state;

        foreach (Transform child in transform)
            child.gameObject.SetActive(state);
    }
}
