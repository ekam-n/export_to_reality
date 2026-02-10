using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ControlsPopupUI : MonoBehaviour
{
    [Header("Wiring")]
    [SerializeField] private GameObject popupPanel;   // ControlsPopup
    [SerializeField] private Button toggleButton;     // ShowControlsButton
    [SerializeField] private TMP_Text buttonLabel;    // Button Text (TMP)
    [SerializeField] private TMP_Text popupText;      // ControlsText (TMP)

    [Header("Button Text")]
    [SerializeField] private string showLabel = "Show Controls";
    [SerializeField] private string hideLabel = "Hide Controls";

    [TextArea(6, 30)]
    [SerializeField] private string controlsBody =
@"MAKER
- Left Click: Place platform
- Right Click: Delete platform
- Mouse Move: Aim placement
- Scroll / Q,E: Rotate (if applicable)
- 1 / 2: Select platform type
- R: Reset / Clear placed (if applicable)

MOVER
- A/D or Left/Right: Move
- Space: Jump
- (Add other controls here)";

    private bool isOpen;

    private void Awake()
    {
        if (toggleButton != null)
            toggleButton.onClick.AddListener(Toggle);

        // start closed
        SetOpen(false);

        // fill text once
        if (popupText != null)
            popupText.text = controlsBody;
    }

    public void Toggle()
    {
        SetOpen(!isOpen);
    }

    public void SetOpen(bool open)
    {
        isOpen = open;

        if (popupPanel != null)
            popupPanel.SetActive(isOpen);

        if (buttonLabel != null)
            buttonLabel.text = isOpen ? hideLabel : showLabel;
    }
}
