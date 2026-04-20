using UnityEngine;

public class CustomCursorManager : MonoBehaviour
{
    public static CustomCursorManager Instance { get; private set; }

    [Header("Cursor Textures")]
    [SerializeField] private Texture2D idleCursor;
    [SerializeField] private Texture2D selectionCursor;
    [SerializeField] private Vector2 cursorHotspot = Vector2.zero;

    private bool isHoveringButton = false;
    private bool menuCursorVisible = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        HideCursor();
    }

    public void ShowMenuCursor()
    {
        if (idleCursor != null)
            Cursor.SetCursor(idleCursor, cursorHotspot, CursorMode.Auto);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        menuCursorVisible = true;
        isHoveringButton = false;
    }

    public void HideCursor()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.None;
        menuCursorVisible = false;
        isHoveringButton = false;
    }

    public void SetIdleCursor()
    {
        if (!menuCursorVisible)
            return;

        if (idleCursor != null)
        {
            Cursor.SetCursor(idleCursor, cursorHotspot, CursorMode.Auto);
            isHoveringButton = false;
        }
    }

    public void SetSelectionCursor()
    {
        if (!menuCursorVisible)
            return;

        if (selectionCursor != null)
        {
            Cursor.SetCursor(selectionCursor, cursorHotspot, CursorMode.Auto);
            isHoveringButton = true;
        }
    }

    public bool IsHoveringButton => isHoveringButton;
}
