using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class JoystickCursor : MonoBehaviour
{
    public RectTransform cursorTransform; // Reference to the cursor RectTransform
    public Canvas canvas; // Reference to your Canvas
    public float moveSpeed = 300f; // Speed of the cursor
    public float deadZone = 0.1f; // Dead zone to ignore small joystick drift

    private PlayerControls controls; // Reference to the input actions
    private Vector2 moveInput; // Current input value

    private void Awake()
    {
        controls = new PlayerControls();
        controls.UIControls.MoveCursor.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.UIControls.MoveCursor.canceled += ctx => moveInput = Vector2.zero;
        controls.UIControls.Select.performed += _ => HandleSelect();
    }

    private void OnEnable()
    {
        controls.UIControls.Enable();
    }

    private void OnDisable()
    {
        controls.UIControls.Disable();
    }

    private void Update()
    {
        // Apply dead zone
        if (moveInput.magnitude < deadZone)
            moveInput = Vector2.zero;

        // Move the cursor
        Vector2 position = cursorTransform.anchoredPosition;
        position += moveInput * moveSpeed * Time.deltaTime;

        // Clamp position to stay within the canvas
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        position.x = Mathf.Clamp(position.x, 0, canvasRect.rect.width);
        position.y = Mathf.Clamp(position.y, -canvasRect.rect.height, 0);

        cursorTransform.anchoredPosition = position;
    }

    private void HandleSelect()
    {
        // Check for hover and trigger button press
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = cursorTransform.position;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, results);

        foreach (var result in results)
        {
            if (result.gameObject.CompareTag("Button"))
            {
                // Highlight the button
                ExecuteEvents.Execute(result.gameObject, pointerEventData, ExecuteEvents.pointerEnterHandler);

                // Trigger the button's click event
                ExecuteEvents.Execute(result.gameObject, pointerEventData, ExecuteEvents.submitHandler);
            }
        }
    }
}
