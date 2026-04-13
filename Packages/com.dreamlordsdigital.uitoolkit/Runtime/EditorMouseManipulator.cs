// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using DLD.Utility;
using UnityEngine;
using UnityEngine.UIElements;
using static DLD.UIToolkit.UITkUtil;

namespace DLD.UIToolkit
{

public interface IDragStatus
{
	bool IsDragging { get; }
}

/// <summary>
///    Allows responding to when user holds ctrl while hovering over a link, to show a special cursor.
/// </summary>
public interface ICtrlLinkRegister
{
	void OnHoverEnterCtrlLink(PointerEnterEvent e);
	void OnHoverExitCtrlLink(PointerLeaveEvent e);
}

/// <summary>
///    Initiates box selection.
/// </summary>
public interface IBoxSelection
{
	void StartBoxSelection(PointerDownEvent e, VisualElement startElement);
}

public interface IShortcutListener
{
	void OnSaveShortcutPressed();
	void OnSaveAsShortcutPressed();
	void OnUndoShortcutPressed();
	void OnRedoShortcutPressed();
}

public class EditorMouseManipulator : PointerManipulator, IDragStatus, ICtrlLinkRegister, IBoxSelection
{
	// ==================================================================================
	// Dependencies

	/// <summary>
	///    Used for showing tooltips while dragging.
	/// </summary>
	protected ITooltip Tooltip;

	public interface ICustomAllowControls
	{
		bool AllowPanning(VisualElement elementPanningOn);
		bool AllowZooming(VisualElement elementZoomingOn);
		bool AllowMouseWheelZooming(VisualElement elementZoomingOn);
		bool AllowDragAndDrop(VisualElement elementDragAndDroppingOn);
		bool AllowBoxSelection(VisualElement elementBoxSelectingOn);
		bool AllowContextMenu(VisualElement elementRightClickedOn);
	}
	protected ICustomAllowControls CustomAllowControls;

	IShortcutListener _shortcutListener;

	bool IsNotInitialized => Tooltip == null;

	// ==================================================================================

	/// <summary>
	///    The local mouse coordinates when pan started.
	/// </summary>
	/// <remarks>
	///    Used for calculating proper panning movement delta.
	/// </remarks>
	Vector2 _panStartPointerPos;

	/// <summary>
	///    Whether panning is currently active.
	/// </summary>
	bool _isPanning;

	/// <summary>
	///    Whether spacebar is held down or not acts as our modifier key
	///    (instead of the usual ctrl, alt, shift, etc.)
	/// </summary>
	/// <remarks>
	///    This is hardcoded for now to only check spacebar, but we can
	///    look into making this user-changeable in the future.
	/// </remarks>
	bool _spacebarHeld;

	/// <summary>
	///    Used for zoom-in (ctrl + spacebar then left click is zoom-in).
	/// </summary>
	/// <remarks>
	///    This is hardcoded for now, but we can look into
	///    making this user-changeable in the future.
	/// </remarks>
	bool _ctrl;

	/// <summary>
	///    Used for zoom-out (alt + spacebar then left click is zoom-out).
	/// </summary>
	/// <remarks>
	///    This is hardcoded for now, but we can look into
	///    making this user-changeable in the future.
	/// </remarks>
	bool _alt;

	/// <summary>
	///    Used when dragging to indicate that user doesn't want to parent the dragged element.
	/// </summary>
	/// <remarks>
	///    For single-click (select) this works as an "add to selection".
	/// </remarks>
	bool _shift;

	bool _pointerDownOnEmptyBackground;

	/// <summary>
	///    Position of panning before a new pan operation is performed.
	///    This is used to pan position back to its former value in case user cancelled the pan operation.
	/// </summary>
	Vector3 _moveTargetInitialPos;

	/// <summary>
	///    The element that gets panned and zoomed by this manipulator.
	/// </summary>
	VisualElement _moveTarget;

	// -------------------------------------------------
	// Drag-and-drop

	bool _draggedElementHasBeenMoved;
	bool _isDragging;
	bool _isDraggingClonedElement;

	int _draggingPointerId = -1;

	/// <summary>
	///    VisualElement that the user is dragging.
	/// </summary>
	VisualElement _draggedElement;

	/// <summary>
	///    Specially designated container where <see cref="_draggedElement"/> will be in.
	///    This should be above everything else, so the dragged element is visible above them.
	/// </summary>
	VisualElement _draggedElementContainer;

	/// <summary>
	///    Where in the dragged element it got clicked on when the dragging started.
	/// </summary>
	/// <remarks>
	///    (0, 0) means the mouse was exactly at the element's pivot point (usually its top-left corner).
	/// </remarks>
	Vector2 _draggedElementStartLocalPos;

	/// <summary>
	///    Last known local-position of the mouse given by <see cref="OnPointerMove"/> while user is dragging.
	///    This is used by events that do not have access to the mouse position, like key press events.
	/// </summary>
	Vector3 _draggingPointerLastKnownLocalPos;

	// -------------------------------------------------

	/// <summary>
	///    The element where we listen for key-down and key-up events.
	/// </summary>
	/// <remarks>
	///    <para>
	///       This manipulator's panning is activated only when this element has keyboard focus.
	///       So for example, if the keyboard focus is elsewhere, say, on a TextField,
	///       then pressing spacebar just adds spaces to that TextField, and this
	///       manipulator won't even receive the key-down/key-up events.
	///    </para>
	///    <para>
	///       Note: This element's <see cref="Focusable.focusable"/> needs to be set to true
	///       in order to receive the key-down and key-up events.
	///    </para>
	///    <para>
	///       Since we use spacebar as if it is a modifier key,
	///       we have to listen in on key-down and key-up events
	///       (it's the only way to detect if spacebar is pressed).
	///    </para>
	///    <para>
	///       We can't rely on <see cref="ManipulatorActivationFilter.modifiers"/>
	///       since those things only detect ctrl, alt, shift, and win key.
	///    </para>
	/// </remarks>
	VisualElement _keyEventTarget;

	/// <summary>
	///    The element that causes the mouse cursor to change.
	/// </summary>
	/// <remarks>
	///    <para>
	///       We apply a style sheet to this element that changes the cursor.
	///       So this element needs to be positioned in the same area where
	///       the <see cref="Manipulator.target"/> is.
	///    </para>
	///    <para>
	///       The act of changing the mouse cursor is used to indicate to the user
	///       that this manipulator is active.
	///    </para>
	/// </remarks>
	VisualElement _mouseCursorDisplay;

	/// <summary>
	///    Last known position of the mouse given by <see cref="OnPointerMove"/>.
	///    This is used by events that do not have access to the mouse position, like key press events.
	/// </summary>
	protected Vector2 LastKnownMousePos { get; private set; }

	VisualElement _boxSelection;
	Vector2 _boxSelectionStartPos;

	bool _inBoxSelection;

	// -------------------------------------------------

	ICtrlHoverable _ctrlHoverableElement;
	VisualElement _hoveredCtrlLinkElement;

	// ==================================================================================
	// Event Callbacks

	readonly EventCallback<KeyDownEvent> _onKeyDown;
	readonly EventCallback<KeyUpEvent> _onKeyUp;
	readonly EventCallback<PointerDownEvent> _onPointerDown;
	readonly EventCallback<PointerMoveEvent> _onPointerMove;
	readonly EventCallback<PointerUpEvent> _onPointerUp;
	readonly EventCallback<WheelEvent> _onWheel;

	readonly EventCallback<ValidateCommandEvent> _onValidateCommand;
	readonly EventCallback<ExecuteCommandEvent> _onExecuteCommand;

	// ==================================================================================

	protected EditorMouseManipulator()
	{
		activators.Add(new ManipulatorActivationFilter
		{
			button = MouseButton.LeftMouse
		});
		_isPanning = false;

		_onKeyDown = OnKeyDown;
		_onKeyUp = OnKeyUp;
		_onPointerDown = OnPointerDown;
		_onPointerMove = OnPointerMove;
		_onPointerUp = OnPointerUp;
		_onWheel = OnWheel;

		_onValidateCommand = OnValidateCommand;
		_onExecuteCommand = OnExecuteCommand;
	}

	public void SetBoxSelectionElement(VisualElement boxSelection)
	{
		_boxSelection = boxSelection;

		// hide at first
		_boxSelection.style.display = DisplayStyle.None;
	}

	/// <inheritdoc cref="_moveTarget"/>
	public void SetMoveTarget(VisualElement newMoveTarget)
	{
		if (_isPanning)
		{
			return;
		}

		_moveTarget = newMoveTarget;
	}

	public void SetDraggedElementContainer(VisualElement newContainer)
	{
		_draggedElementContainer = newContainer;
	}

	/// <inheritdoc cref="_keyEventTarget"/>
	public void SetKeyEventTarget(VisualElement newKeyEventTarget)
	{
		_keyEventTarget = newKeyEventTarget;
	}

	/// <inheritdoc cref="_mouseCursorDisplay"/>
	public void SetMouseCursorDisplay(VisualElement newMouseCursorDisplay)
	{
		_mouseCursorDisplay = newMouseCursorDisplay;
	}

	/// <inheritdoc cref="Tooltip"/>
	public void SetTooltip(ITooltip tooltip)
	{
		Tooltip = tooltip;
	}

	// ==================================================================================================

	public bool IsDoingForceMove => _shift;

	public bool IsDragging => _isDragging;

	public bool AllowPanning { get; set; } = true;
	public bool AllowZooming { get; set; } = true;
	public bool AllowMouseWheelZooming { get; set; } = true;
	public bool AllowDragAndDrop { get; set; } = true;
	public bool AllowBoxSelection { get; set; } = true;
	public bool AllowContextMenu { get; set; } = true;

	public void SetCustomAllowControls(ICustomAllowControls newCustomAllowControls)
	{
		CustomAllowControls = newCustomAllowControls;
	}

	public void SetSaveShortcutListener(IShortcutListener newListener)
	{
		_shortcutListener = newListener;
	}

	public void SetZoom(float zoomLevel, bool sendNotify = true)
	{
		_moveTarget.style.scale = Vector3.one * zoomLevel;
	}

	protected void AddToPointerMoveEvent(CallbackEventHandler c)
	{
		c.RegisterCallback(_onPointerMove);
	}

	protected void AddToPointerUpEvent(CallbackEventHandler c)
	{
		c.RegisterCallback(_onPointerUp);
	}

	protected override void RegisterCallbacksOnTarget()
	{
		_keyEventTarget.RegisterCallback(_onKeyDown);
		_keyEventTarget.RegisterCallback(_onKeyUp);
		_keyEventTarget.RegisterCallback(_onValidateCommand);
		_keyEventTarget.RegisterCallback(_onExecuteCommand);
		target.RegisterCallback(_onPointerDown);
		target.RegisterCallback(_onPointerMove);
		target.RegisterCallback(_onPointerUp);
		target.RegisterCallback(_onWheel);
	}

	protected override void UnregisterCallbacksFromTarget()
	{
		_keyEventTarget.UnregisterCallback(_onKeyDown);
		_keyEventTarget.UnregisterCallback(_onKeyUp);
		_keyEventTarget.UnregisterCallback(_onValidateCommand);
		_keyEventTarget.UnregisterCallback(_onExecuteCommand);
		target.UnregisterCallback(_onPointerDown);
		target.UnregisterCallback(_onPointerMove);
		target.UnregisterCallback(_onPointerUp);
		target.UnregisterCallback(_onWheel);
	}

	public void OnLostFocus()
	{
		// Assuming spacebar is still held, we cannot reliably know when/if user
		// will release the spacebar when we no longer have keyboard focus
		// (user might have alt + tabbed) so might as well just assume it's been released.
		_spacebarHeld = false;

		if (_isPanning)
		{
			_isPanning = false;
			_moveTarget.SetPosition(_moveTargetInitialPos);
		}

		// Also reset the mouse cursor.
		_mouseCursorDisplay.RemoveFromClassList(UITkUtil.MouseCursorZoomInStyleClass);
		_mouseCursorDisplay.RemoveFromClassList(UITkUtil.MouseCursorZoomOutStyleClass);
		_mouseCursorDisplay.RemoveFromClassList(UITkUtil.MouseCursorPanStyleClass);
		_mouseCursorDisplay.RemoveFromClassList(UITkUtil.MouseCursorPanDragStyleClass);
		_mouseCursorDisplay.style.display = DisplayStyle.None;

		// If drag-and-drop was happening, cancel it.
		if (_draggingPointerId != -1)
		{
			CancelDrag();
		}

		if (_inBoxSelection)
		{
			CancelBoxSelection();
		}
	}

	void OnKeyDown(KeyDownEvent e)
	{
		// Note: key-down event is called for every frame the key is held down,
		// but this only applies to the last key pressed
		// (if multiple keys are held down, all of them will have a key-down event,
		// but only the latest one pressed has repeated key-down events).

		if (IsNotInitialized)
		{
			return;
		}

		bool customAllowPanning = CustomAllowControls == null ||
		                          CustomAllowControls.AllowPanning(_moveTarget);

		bool changeDetected = false;
		if (e.keyCode == KeyCode.Space && AllowPanning && customAllowPanning)
		{
			changeDetected |= !_spacebarHeld;
			_spacebarHeld = true;

			if (_isDragging && _spacebarHeld && changeDetected)
			{
				// Spacebar started getting pressed while dragging.
				// That means user wants to start panning while dragging.
				_panStartPointerPos = target.ChangeCoordinatesTo(_moveTarget.contentContainer, _draggingPointerLastKnownLocalPos);
			}
		}
		else if (e.keyCode == KeyCode.Escape)
		{
			if (_isDragging)
			{
				changeDetected = true;

				// User pressed Escape while dragging.
				// Interpret this as a cancel to the drag-and-drop operation.
				// Abort it, even if the pointer is still held down.

				CancelDrag();
			}
			else if (_inBoxSelection)
			{
				changeDetected = true;

				CancelBoxSelection();
			}
			else if (_isPanning)
			{
				_isPanning = false;
				_moveTarget.SetPosition(_moveTargetInitialPos);

				// Also reset the mouse cursor.
				_mouseCursorDisplay.RemoveFromClassList(UITkUtil.MouseCursorZoomInStyleClass);
				_mouseCursorDisplay.RemoveFromClassList(UITkUtil.MouseCursorZoomOutStyleClass);
				_mouseCursorDisplay.RemoveFromClassList(UITkUtil.MouseCursorPanStyleClass);
				_mouseCursorDisplay.RemoveFromClassList(UITkUtil.MouseCursorPanDragStyleClass);
				_mouseCursorDisplay.style.display = DisplayStyle.None;
			}
		}

		// We check this way instead of checking the keyCode because these are the latest values
		// (if user alt + tabbed away while alt was still held, then released the alt key once
		// they are out of the INTLord Editor window, we wouldn't have received a key-up event for alt).
		if (_ctrl != e.ctrlKey)
		{
			changeDetected = true;
			_ctrl = e.ctrlKey;
			if (_ctrl)
			{
				_hoveredCtrlLinkElement?.AddToClassList(BaseStyles.LabelCtrlLinkStyleClass);
			}
			else
			{
				_hoveredCtrlLinkElement?.RemoveFromClassList(BaseStyles.LabelCtrlLinkStyleClass);
			}
			_ctrlHoverableElement?.OnCtrlHover(_ctrl);
		}

		if (_alt != e.altKey)
		{
			changeDetected = true;
			_alt = e.altKey;
		}

		bool changeInShift = false;
		if (_shift != e.shiftKey)
		{
			changeDetected = true;
			changeInShift = true;
			_shift = e.shiftKey;
		}

		if (changeDetected)
		{
			RefreshMouseCursor(LastKnownMousePos);
		}

		if (changeInShift)
		{
			OnForceMoveChanged(LastKnownMousePos);
		}
	}

	void OnKeyUp(KeyUpEvent e)
	{
		if (IsNotInitialized)
		{
			return;
		}

		bool changeInShift = false;
		switch (e.keyCode)
		{
			case KeyCode.Space:
				_spacebarHeld = false;
				if (_isDragging)
				{
					// Spacebar was released while dragging.
					// That means user wants to stop panning while dragging.
					// And we switch back to just dragging.
					_mouseCursorDisplay.RemoveFromClassList(UITkUtil.MouseCursorPanDragStyleClass);
					_mouseCursorDisplay.RemoveFromClassList(UITkUtil.MouseCursorPanStyleClass);
				}

				break;
			case KeyCode.LeftControl:
			case KeyCode.RightControl:
				_ctrl = false;
				_hoveredCtrlLinkElement?.RemoveFromClassList(BaseStyles.LabelCtrlLinkStyleClass);
				_ctrlHoverableElement?.OnCtrlHover(_ctrl);
				break;
			case KeyCode.LeftAlt:
			case KeyCode.RightAlt:
				_alt = false;
				break;
			case KeyCode.LeftShift:
			case KeyCode.RightShift:
				_shift = false;
				changeInShift = true;
				break;
			case KeyCode.S:
				if (e.ctrlKey && e.shiftKey)
				{
					_shortcutListener?.OnSaveAsShortcutPressed();
					e.StopImmediatePropagation();
				}
				else if (e.ctrlKey)
				{
					_shortcutListener?.OnSaveShortcutPressed();
					e.StopImmediatePropagation();
				}
				break;
			case KeyCode.Z:
				if (e.ctrlKey && e.shiftKey)
				{
					_shortcutListener?.OnRedoShortcutPressed();
					e.StopImmediatePropagation();
				}
				else if (e.ctrlKey)
				{
					_shortcutListener?.OnUndoShortcutPressed();
					e.StopImmediatePropagation();
				}
				break;
		}

		RefreshMouseCursor(LastKnownMousePos);
		if (changeInShift)
		{
			OnForceMoveChanged(LastKnownMousePos);
		}
	}

	public void OnHoverEnterCtrlLink(PointerEnterEvent e)
	{
		_ctrl = e.ctrlKey;

		if (e.target is ICtrlHoverable ctrlHoverable)
		{
			ctrlHoverable.OnCtrlHover(_ctrl);

			_ctrlHoverableElement = ctrlHoverable;
			_hoveredCtrlLinkElement = null;
		}
		else if (e.target is VisualElement hoveredElement)
		{
			if (_ctrl)
			{
				hoveredElement.AddToClassList(BaseStyles.LabelCtrlLinkStyleClass);
			}
			else
			{
				hoveredElement.RemoveFromClassList(BaseStyles.LabelCtrlLinkStyleClass);
			}

			_ctrlHoverableElement = null;
			_hoveredCtrlLinkElement = hoveredElement;
		}
	}

	public void OnHoverExitCtrlLink(PointerLeaveEvent e)
	{
		_ctrlHoverableElement?.OnCtrlHover(false);
		_hoveredCtrlLinkElement?.RemoveFromClassList(BaseStyles.LabelCtrlLinkStyleClass);

		_ctrlHoverableElement = null;
		_hoveredCtrlLinkElement = null;
	}

	void OnPointerDown(PointerDownEvent e)
	{
		if (IsNotInitialized)
		{
			return;
		}

		// Note: Unlike key-down event, pointer-down event only happens
		// in the single frame that the mouse button/finger starts to be pressed.

		if (_isPanning)
		{
			// Panning is already ongoing, but we received another pointer-down event.
			// This happens on multitouch touchscreens when another finger has started pressing on the screen.
			e.StopImmediatePropagation();
			return;
		}

		if (!_spacebarHeld && e.button == 0)
		{
			_pointerDownOnEmptyBackground = true;
			bool customAllowBoxSelection = CustomAllowControls == null ||
			                               CustomAllowControls.AllowBoxSelection(e.target as VisualElement);

			if (AllowBoxSelection && customAllowBoxSelection)
			{
				_mouseCursorDisplay.style.display = DisplayStyle.None;
				StartBoxSelection(e, null);
			}
			return;
		}

		_pointerDownOnEmptyBackground = false;

		// ---------------------------------------------------
		// Zoom controls take precedence over pan

		// Check again one last time because these can fail to get key-up events
		// (if user alt + tabbed away while the keys were held).
		if (!e.ctrlKey && _ctrl)
		{
			_ctrl = false;
			RefreshMouseCursor(e.position);
			_hoveredCtrlLinkElement?.RemoveFromClassList(BaseStyles.LabelCtrlLinkStyleClass);
			_ctrlHoverableElement?.OnCtrlHover(_ctrl);
		}

		if (!e.altKey && _alt)
		{
			_alt = false;
			RefreshMouseCursor(e.position);
		}

		if (!e.shiftKey && _shift)
		{
			_shift = false;
			RefreshMouseCursor(e.position);
			OnForceMoveChanged(e.position);
		}

		bool customAllowZooming = CustomAllowControls == null ||
		                          CustomAllowControls.AllowZooming(e.target as VisualElement);

		if (AllowZooming && customAllowZooming)
		{
			if (_ctrl && _spacebarHeld)
			{
				// zoom in
				_moveTarget.SetScaleByZoom(6, target, e.localPosition);
				return;
			}

			if (_alt && _spacebarHeld)
			{
				// zoom out
				_moveTarget.SetScaleByZoom(-6, target, e.localPosition);
				return;
			}
		}

		// ---------------------------------------------------

		bool customAllowPanning = CustomAllowControls == null ||
		                          CustomAllowControls.AllowPanning(e.target as VisualElement);

		// pan: spacebar + left-click, or middle-click
		if (AllowPanning && customAllowPanning && ((_spacebarHeld && e.button == 0) || e.button == 2))
		{
			_panStartPointerPos = target.ChangeCoordinatesTo(_moveTarget.contentContainer, e.localPosition);

			_isPanning = true;
			_moveTargetInitialPos = _moveTarget.GetPositionXY();
			target.CapturePointer(e.pointerId);
			e.StopPropagation();

			RefreshMouseCursor(e.position);
		}

		bool customAllowContextMenu = CustomAllowControls == null ||
		                              CustomAllowControls.AllowContextMenu(e.target as VisualElement);

		if (AllowContextMenu && customAllowContextMenu && e.button == RightMouseButton)
		{
			OnRightClickEmptySpace(e);
		}
	}

	protected void OnStartPotentialDrag(VisualElement draggedElement, PointerDownEvent e, Vector2 localPos)
	{
		if (_isPanning)
		{
			// shouldn't be possible to have this method called while _isPanning is true
			Debug.LogAssertion("OnStartDrag got called while _isPanning is already true (shouldn't happen). Panning captures the pointer so it should have prevented nodes from receiving drag events.");
			return;
		}

		if (_spacebarHeld)
		{
			// dragging shouldn't happen if spacebar was already held
			// upon left mouse down
			return;
		}

		_draggedElement = draggedElement;
		_draggedElementHasBeenMoved = false;
		_isDragging = false;
		_isDraggingClonedElement = false;
		_inBoxSelection = false;

		_draggedElementStartLocalPos = localPos;

		_draggingPointerId = e.pointerId;

		// Note: We won't capture the pointer, to allow other Pointer events to still work.
		// The only downside is that for every element that we want to be a drag-and-drop destination,
		// we have to manually register our OnPointerMove callback to it.
	}

	void CommitToDragging(PointerMoveEvent e)
	{
		var gotClonedDragElement = OnStartedDrag(e);
		if (gotClonedDragElement != null)
		{
			_draggedElement = gotClonedDragElement;
			_isDraggingClonedElement = true;
		}

		// While being dragged, the node should be parented to the designated container
		if (_draggedElement.parent != _draggedElementContainer)
		{
			// A NodeView's style.position gets set to Position.Relative when it's assigned into another NodeView
			// (see NodeView.Add and NodeView.Insert).
			// Since we'll take it out of that other NodeView, we need to manually set it back to Position.Absolute.
			// Position.Absolute makes it so that the NodeView's width/height doesn't have to conform to the Tab Body.
			_draggedElement.style.position = Position.Absolute;

			// Since the draggedElement will change parents, its local position doesn't get adjusted automatically.
			// The moment the parent is changed, the draggedElement's local position value doesn't make sense anymore.
			// We need to convert it and assign it manually.
			var newPosition = _draggedElement.ChangeCoordinatesTo(_draggedElementContainer.contentContainer, _draggedElement.resolvedStyle.translate).Round();

			_draggedElementContainer.Add(_draggedElement);

			_draggedElement.SetPosition(newPosition);
		}

		_draggedElement.BringToFront();

		_isDragging = true;
		RefreshMouseCursor(e.position);
	}

	void OnPointerMove(PointerMoveEvent e)
	{
		if (IsNotInitialized)
		{
			return;
		}

		LastKnownMousePos = e.position;

		if (_inBoxSelection)
		{
			UpdateBoxSelection(e);
			e.StopPropagation();
			return;
		}

		if (_isPanning && !target.HasPointerCapture(e.pointerId))
		{
			// This isn't the pointer that initiated the panning, so we're not interested in this pointer-move event.
			// This happens in multitouch touchscreens where more than one finger may
			// be touching and moving on the screen.
			return;
		}

		if (!_isPanning && e.pointerId == _draggingPointerId)
		{
			if (!e.pressedButtons.GetFlag(0))
			{
				// User no longer holding left mouse button.
				// This happens when user moves mouse outside the window,
				// then releases the left mouse button there.
				// In that way, we won't receive a pointer-up event,
				// even though user has released the mouse button.
				// Treat this as a cancel for the drag-and-drop operation.
				CancelDrag();
				CancelBoxSelection();
				return;
			}

			if (!_draggedElementHasBeenMoved)
			{
				_draggedElementHasBeenMoved = true;
				CommitToDragging(e);
			}

			if (_isDragging && !_spacebarHeld)
			{
				if (e.target is VisualElement targetElement)
				{
					// convert mouse pos to be relative to the Tab Body
					Vector2 localPointer;
					(bool gotCustomDragPosition, Vector2 customDragPosition) = GetCustomDragPosition(e, targetElement, _draggedElementContainer.contentContainer);
					if (gotCustomDragPosition)
					{
						localPointer = customDragPosition;
					}
					else if (target.IsOrAncestorOf(targetElement)) // note: target is the TabBodyContainer of the Pane
					{
						// PointerMoveEvent happened on the TabBodyContainer
						// convert the event's mouse position to our special container for dragged elements
						localPointer = target.ChangeCoordinatesTo(_draggedElementContainer.contentContainer, e.localPosition);
					}
					else
					{
						return;
					}

					_draggedElement.SetPosition(localPointer.x - _draggedElementStartLocalPos.x, localPointer.y - _draggedElementStartLocalPos.y);
				}

				_draggingPointerLastKnownLocalPos = e.localPosition;
			}
		}

		if (_isPanning || _isDragging && _spacebarHeld)
		{
			Vector2 delta = target.ChangeCoordinatesTo(_moveTarget.contentContainer, e.localPosition) - _panStartPointerPos;
			float scale = _moveTarget.resolvedStyle.scale.value.x;
			_moveTarget.AddToPosition(delta * scale);

			e.StopPropagation();
		}
	}

	void OnPointerUp(PointerUpEvent e)
	{
		if (IsNotInitialized)
		{
			return;
		}

		if (_inBoxSelection)
		{
			_pointerDownOnEmptyBackground = false;
			EndBoxSelection(e);
			e.StopPropagation();
			return;
		}

		if (!_isDragging && !_isPanning && !_spacebarHeld && _pointerDownOnEmptyBackground)
		{
			_pointerDownOnEmptyBackground = false;
			OnLeftClickEmptySpace(e);
			RefreshMouseCursor(e.position);
			return;
		}

		if (!_draggedElementHasBeenMoved && e.pointerId == _draggingPointerId)
		{
			_draggingPointerId = -1;
			_draggedElement = null;
			OnAbortedPotentialDrag(e);
			Debug.Assert(!_isDragging);
			e.StopPropagation();
		}
		else if (_isDragging && e.pointerId == _draggingPointerId)
		{
			Vector2 draggedElementEndPos = _draggedElement.resolvedStyle.translate;
			Vector2 draggedElementEndWorldPos = _draggedElementContainer.LocalToWorld(draggedElementEndPos);
			if (_isDraggingClonedElement)
			{
				_draggedElement.RemoveFromHierarchy();
			}

			bool dropWasHandledByCustom = HandleCustomDragAndDropEnd(draggedElementEndWorldPos);
			if (!dropWasHandledByCustom)
			{
				Vector2 draggedElementEndLocalPos = _moveTarget.WorldToLocal(draggedElementEndWorldPos).Round();
				OnEndedDrag(e, draggedElementEndLocalPos);
			}

			Tooltip?.HideTooltip();

			_draggingPointerId = -1;
			_draggedElement = null;
			_isDragging = false;

			RefreshMouseCursor(e.position);
			e.StopPropagation();
		}

		if (_isPanning && target.HasPointerCapture(e.pointerId))
		{
			_isPanning = false;
			RefreshMouseCursor(e.position);

			target.ReleasePointer(e.pointerId);
			e.StopPropagation();
		}
	}

	void OnWheel(WheelEvent e)
	{
		if (IsNotInitialized)
		{
			return;
		}

		if (!AllowMouseWheelZooming)
		{
			return;
		}

		bool allowWheelZoomingCustom = CustomAllowControls == null ||
		                               CustomAllowControls.AllowMouseWheelZooming(e.target as VisualElement);

		if (!allowWheelZoomingCustom)
		{
			return;
		}

		_moveTarget.SetScaleByZoom(-e.delta.y, target, e.localMousePosition);
		e.StopPropagation();
	}

	// ==================================================================================================

	void CancelDrag()
	{
		OnCanceledDrag();
		Tooltip?.HideTooltip();
		if (_isDraggingClonedElement)
		{
			_draggedElement.RemoveFromHierarchy();
		}

		_draggingPointerId = -1;
		_draggedElement = null;
		_isDragging = false;
	}

	// ==================================================================================================

	public void StartBoxSelection(PointerDownEvent e, VisualElement startElement)
	{
		if (_inBoxSelection)
		{
			// already in box selection
			return;
		}

		if (!AllowBoxSelection)
		{
			return;
		}

		bool allowBoxSelectionCustom = CustomAllowControls == null ||
		                               CustomAllowControls.AllowBoxSelection(e.target as VisualElement);

		if (!allowBoxSelectionCustom)
		{
			return;
		}

		_boxSelectionStartPos = _boxSelection.parent.WorldToLocal(e.position);
		_boxSelection.style.left = _boxSelectionStartPos.x;
		_boxSelection.style.top = _boxSelectionStartPos.y;
		_boxSelection.style.width = 0;
		_boxSelection.style.height = 0;
		_boxSelection.style.display = DisplayStyle.Flex;
		_inBoxSelection = true;
		e.StopPropagation();
		OnBoxSelectionStart(e, startElement);
	}

	void UpdateBoxSelection(PointerMoveEvent e)
	{
		RefreshMouseCursor(e.position);

		Vector2 pointerLocalPos = _boxSelection.parent.WorldToLocal(e.position);
		float boxX = _boxSelection.style.left.value.value;
		float boxY = _boxSelection.style.top.value.value;

		Rect finalRect = new Rect();
		if (pointerLocalPos.x >= _boxSelectionStartPos.x)
		{
			_boxSelection.style.width = pointerLocalPos.x - boxX;
			finalRect.x = _boxSelectionStartPos.x;
			finalRect.xMax = pointerLocalPos.x;
		}
		else
		{
			_boxSelection.style.left = pointerLocalPos.x;
			_boxSelection.style.width = _boxSelectionStartPos.x - pointerLocalPos.x;
			finalRect.x = pointerLocalPos.x;
			finalRect.xMax = _boxSelectionStartPos.x;
		}

		if (pointerLocalPos.y >= _boxSelectionStartPos.y)
		{
			_boxSelection.style.height = pointerLocalPos.y - boxY;
			finalRect.y = _boxSelectionStartPos.y;
			finalRect.yMax = pointerLocalPos.y;
		}
		else
		{
			_boxSelection.style.top = pointerLocalPos.y;
			_boxSelection.style.height = _boxSelectionStartPos.y - pointerLocalPos.y;
			finalRect.y = pointerLocalPos.y;
			finalRect.yMax = _boxSelectionStartPos.y;
		}

		finalRect.position = _boxSelection.parent.LocalToWorld(finalRect.position);
		OnBoxSelectionUpdate(e, finalRect);
	}

	void EndBoxSelection(PointerUpEvent e)
	{
		Tooltip?.HideTooltipIfContextIs(_mouseCursorDisplay);
		_boxSelection.style.display = DisplayStyle.None;
		_inBoxSelection = false;
		OnBoxSelectionEnd(e);
	}

	void CancelBoxSelection()
	{
		Tooltip?.HideTooltipIfContextIs(_mouseCursorDisplay);
		_boxSelection.style.display = DisplayStyle.None;
		_inBoxSelection = false;
		OnBoxSelectionCanceled();
	}

	// ==================================================================================================
	// Methods that can be overriden by derived classes

	protected virtual void OnLeftClickEmptySpace(PointerUpEvent e)
	{
	}

	/// <summary>
	///    This method is called the moment that the user moves the mouse
	///    while having left mouse button held down on a node.
	/// </summary>
	protected virtual VisualElement OnStartedDrag(PointerMoveEvent e) => null;

	protected virtual (bool, Vector2) GetCustomDragPosition(
		PointerMoveEvent e, VisualElement elementHovered, VisualElement draggedElementContainer)
	{
		return (false, Vector2.zero);
	}

	protected virtual bool HandleCustomDragAndDropEnd(Vector2 draggedElementEndWorldPos) => false;

	/// <summary>
	///    Called when user starts pressing shift, or releases shift.
	///    Shift is used to indicate a "force move" command where the dragged node will not be parented,
	///    even if there is a valid destination node under the mouse cursor.
	/// </summary>
	protected virtual void OnForceMoveChanged(Vector2 mousePos)
	{
	}

	/// <summary>
	///    This method is called when the user releases left mouse button on a node without moving the mouse.
	/// </summary>
	protected virtual void OnAbortedPotentialDrag(PointerUpEvent e)
	{
	}

	/// <summary>
	///    Called when user releases the left mouse button on a node after having moved it.
	/// </summary>
	protected virtual void OnEndedDrag(PointerUpEvent e, Vector2 draggedElementEndPos)
	{
	}

	/// <summary>
	///    Called when user presses ESC or Alt + Tabs out while in the middle of a drag-and-drop operation.
	/// </summary>
	protected virtual void OnCanceledDrag()
	{
	}

	protected virtual void OnBoxSelectionStart(PointerDownEvent e, VisualElement startElement)
	{
	}

	/// <summary>
	///    Called when user moves the mouse while in box selection.
	/// </summary>
	/// <param name="e"></param>
	/// <param name="selectionRect">Values are in world-space.</param>
	protected virtual void OnBoxSelectionUpdate(PointerMoveEvent e, Rect selectionRect)
	{
	}

	protected virtual void OnBoxSelectionEnd(PointerUpEvent e)
	{
	}

	protected virtual void OnBoxSelectionCanceled()
	{
	}

	protected virtual void OnRightClickEmptySpace(PointerDownEvent e)
	{
	}

	protected virtual void OnValidateCommand(ValidateCommandEvent e)
	{
	}

	protected virtual void OnExecuteCommand(ExecuteCommandEvent e)
	{
	}

	// ==================================================================================================

	void RefreshMouseCursor(Vector2 mousePos)
	{
		if (_isDragging)
		{
			if (_shift)
			{
				Tooltip?.ShowTooltipAtMouse(_mouseCursorDisplay, "Move without parenting", BaseIcons.ForceMove, mousePos);
			}
			else
			{
				Tooltip?.HideTooltip();
			}
		}
		else if (_inBoxSelection)
		{
			if (_shift || _ctrl)
			{
				Tooltip?.ShowTooltipAtMouse(_mouseCursorDisplay, "Add to selection", BaseIcons.AddToSelection, mousePos);
			}
			else if (_alt)
			{
				Tooltip?.ShowTooltipAtMouse(_mouseCursorDisplay, "Remove from selection", BaseIcons.RemoveFromSelection, mousePos);
			}
			else
			{
				Tooltip?.HideTooltipIfContextIs(_mouseCursorDisplay);
			}
		}

		if (_isPanning)
		{
			_mouseCursorDisplay.RemoveFromClassList(UITkUtil.MouseCursorDragStyleClass);
			_mouseCursorDisplay.RemoveFromClassList(UITkUtil.MouseCursorZoomInStyleClass);
			_mouseCursorDisplay.RemoveFromClassList(UITkUtil.MouseCursorZoomOutStyleClass);
			_mouseCursorDisplay.RemoveFromClassList(UITkUtil.MouseCursorPanStyleClass);
			_mouseCursorDisplay.AddToClassList(UITkUtil.MouseCursorPanDragStyleClass);
			_mouseCursorDisplay.style.display = DisplayStyle.Flex;
		}
		else if (_isDragging && _spacebarHeld)
		{
			// temporarily panning while dragging
			_mouseCursorDisplay.RemoveFromClassList(UITkUtil.MouseCursorDragStyleClass);
			_mouseCursorDisplay.RemoveFromClassList(UITkUtil.MouseCursorZoomInStyleClass);
			_mouseCursorDisplay.RemoveFromClassList(UITkUtil.MouseCursorZoomOutStyleClass);
			_mouseCursorDisplay.RemoveFromClassList(UITkUtil.MouseCursorPanStyleClass);
			_mouseCursorDisplay.AddToClassList(UITkUtil.MouseCursorPanDragStyleClass);
			_mouseCursorDisplay.style.display = DisplayStyle.Flex;
		}
		else if (_isDragging)
		{
			_mouseCursorDisplay.RemoveFromClassList(UITkUtil.MouseCursorZoomInStyleClass);
			_mouseCursorDisplay.RemoveFromClassList(UITkUtil.MouseCursorZoomOutStyleClass);
			_mouseCursorDisplay.RemoveFromClassList(UITkUtil.MouseCursorPanStyleClass);
			_mouseCursorDisplay.RemoveFromClassList(UITkUtil.MouseCursorPanDragStyleClass);
			_mouseCursorDisplay.AddToClassList(UITkUtil.MouseCursorDragStyleClass);
			_mouseCursorDisplay.style.display = DisplayStyle.None;
		}
		else if (_ctrl && _spacebarHeld)
		{
			// zoom in
			_mouseCursorDisplay.RemoveFromClassList(UITkUtil.MouseCursorDragStyleClass);
			_mouseCursorDisplay.RemoveFromClassList(UITkUtil.MouseCursorPanStyleClass);
			_mouseCursorDisplay.RemoveFromClassList(UITkUtil.MouseCursorZoomOutStyleClass);
			_mouseCursorDisplay.AddToClassList(UITkUtil.MouseCursorZoomInStyleClass);
			_mouseCursorDisplay.style.display = DisplayStyle.Flex;
		}
		else if (_alt && _spacebarHeld)
		{
			// zoom out
			_mouseCursorDisplay.RemoveFromClassList(UITkUtil.MouseCursorDragStyleClass);
			_mouseCursorDisplay.RemoveFromClassList(UITkUtil.MouseCursorPanStyleClass);
			_mouseCursorDisplay.RemoveFromClassList(UITkUtil.MouseCursorZoomInStyleClass);
			_mouseCursorDisplay.AddToClassList(UITkUtil.MouseCursorZoomOutStyleClass);
			_mouseCursorDisplay.style.display = DisplayStyle.Flex;
		}
		else if (_spacebarHeld)
		{
			// pan
			_mouseCursorDisplay.RemoveFromClassList(UITkUtil.MouseCursorDragStyleClass);
			_mouseCursorDisplay.RemoveFromClassList(UITkUtil.MouseCursorZoomInStyleClass);
			_mouseCursorDisplay.RemoveFromClassList(UITkUtil.MouseCursorZoomOutStyleClass);
			_mouseCursorDisplay.RemoveFromClassList(UITkUtil.MouseCursorPanDragStyleClass);
			_mouseCursorDisplay.AddToClassList(UITkUtil.MouseCursorPanStyleClass);
			_mouseCursorDisplay.style.display = DisplayStyle.Flex;
		}
		else
		{
			// No special state detected.
			// Note: no need to remove any of the class styles, because we'll be hiding it.
			_mouseCursorDisplay.style.display = DisplayStyle.None;
		}
	}
}

}
