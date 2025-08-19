using DLD.Utility;
using UnityEngine;
using UnityEngine.UIElements;

namespace DLD.UIToolkit
{
	public class PanZoomManipulator : PointerManipulator
	{
		/// <summary>
		/// The local mouse coordinates when pan started.
		/// </summary>
		/// <remarks>
		/// Used for calculating proper panning movement delta.
		/// </remarks>
		Vector2 _panStartPointerPos;

		/// <summary>
		/// Whether panning is currently active.
		/// </summary>
		bool _isPanning;

		/// <summary>
		/// Whether spacebar is held down or not acts as our modifier key
		/// (instead of the usual ctrl, alt, shift, etc.)
		/// </summary>
		/// <remarks>
		/// This is hardcoded for now to only check spacebar, but we can
		/// look into making this user-changeable in the future.
		/// </remarks>
		bool _spacebarHeld;

		/// <summary>
		/// Used for zoom-in (ctrl + spacebar then left click is zoom-in).
		/// </summary>
		/// <remarks>
		/// This is hardcoded for now, but we can look into
		/// making this user-changeable in the future.
		/// </remarks>
		bool _ctrl;

		/// <summary>
		/// Used for zoom-out (alt + spacebar then left click is zoom-out).
		/// </summary>
		/// <remarks>
		/// This is hardcoded for now, but we can look into
		/// making this user-changeable in the future.
		/// </remarks>
		bool _alt;

		bool _draggedElementHasBeenMoved;
		bool _isDragging;
		bool _isDraggingClonedElement;

		int _draggingPointerId = -1;

		/// <summary>
		/// Where in the dragged element it got clicked on when the dragging started.
		/// </summary>
		/// <remarks>
		/// (0, 0) means the mouse was exactly at the element's pivot point.
		/// </remarks>
		Vector2 _draggedElementStartLocalPos;

		Vector3 _pointerLastKnownLocalPos;

		/// <summary>
		/// The element that gets panned and zoomed by this manipulator.
		/// </summary>
		VisualElement _moveTarget;

		/// <summary>
		/// The element where we listen for key-down and key-up events.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This manipulator's panning is activated only when this element has keyboard focus.
		/// So for example, if the keyboard focus is elsewhere, say, on a TextField,
		/// then pressing spacebar just adds spaces to that TextField, and this
		/// manipulator won't even receive the key-down/key-up events.
		/// </para>
		/// <para>
		/// Note: This element's <see cref="Focusable.focusable"/> needs to be set to true
		/// in order to receive the key-down and key-up events.
		/// </para>
		/// <para>
		/// Since we use spacebar as if it is a modifier key,
		/// we have to listen in on key-down and key-up events
		/// (it's the only way to detect if spacebar is pressed).
		/// </para>
		/// <para>
		/// We can't rely on <see cref="ManipulatorActivationFilter.modifiers"/>
		/// since those things only detect ctrl, alt, shift, and win key.
		/// </para>
		/// </remarks>
		VisualElement _keyEventTarget;

		VisualElement _draggedElement;

		/// <summary>
		/// The element that causes the mouse cursor to change.
		/// </summary>
		/// <remarks>
		/// <para>
		/// We apply a style sheet to this element that changes the cursor.
		/// So this element needs to be positioned in the same area where
		/// the <see cref="Manipulator.target"/> is.
		/// </para>
		/// <para>
		/// The act of changing the mouse cursor is used to indicate to the user
		/// that this manipulator is active.
		/// </para>
		/// </remarks>
		VisualElement _mouseCursorDisplay;

		Vector2 _lastKnownMousePos;

		protected ITooltip _tooltip;

		readonly EventCallback<FocusOutEvent> _onFocusOut;
		readonly EventCallback<KeyDownEvent> _onKeyDown;
		readonly EventCallback<KeyUpEvent> _onKeyUp;
		readonly EventCallback<PointerDownEvent> _onPointerDown;
		readonly EventCallback<PointerMoveEvent> _onPointerMove;
		readonly EventCallback<PointerUpEvent> _onPointerUp;
		readonly EventCallback<WheelEvent> _onWheel;

		// ==================================================================================================

		public PanZoomManipulator()
		{
			activators.Add(new ManipulatorActivationFilter
			{
				button = MouseButton.LeftMouse
			});
			_isPanning = false;

			_onFocusOut = OnFocusOut;
			_onKeyDown = OnKeyDown;
			_onKeyUp = OnKeyUp;
			_onPointerDown = OnPointerDown;
			_onPointerMove = OnPointerMove;
			_onPointerUp = OnPointerUp;
			_onWheel = OnWheel;
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

		public void SetTooltip(ITooltip tooltip)
		{
			_tooltip = tooltip;
		}

		// ==================================================================================================

		protected override void RegisterCallbacksOnTarget()
		{
			_keyEventTarget.RegisterCallback(_onFocusOut);
			_keyEventTarget.RegisterCallback(_onKeyDown);
			_keyEventTarget.RegisterCallback(_onKeyUp);
			target.RegisterCallback(_onPointerDown);
			target.RegisterCallback(_onPointerMove);
			target.RegisterCallback(_onPointerUp);
			target.RegisterCallback(_onWheel);
		}

		protected override void UnregisterCallbacksFromTarget()
		{
			_keyEventTarget.UnregisterCallback(_onFocusOut);
			_keyEventTarget.UnregisterCallback(_onKeyDown);
			_keyEventTarget.UnregisterCallback(_onKeyUp);
			target.UnregisterCallback(_onPointerDown);
			target.UnregisterCallback(_onPointerMove);
			target.UnregisterCallback(_onPointerUp);
			target.UnregisterCallback(_onWheel);
		}

		void OnFocusOut(FocusOutEvent e)
		{
			// Assuming spacebar is still held, we cannot reliably know when/if user
			// will release the spacebar when we no longer have keyboard focus
			// (user might have alt + tabbed) so might as well just assume it's been released.
			_spacebarHeld = false;
			_isPanning = false;

			// Also reset the mouse cursor.
			_mouseCursorDisplay.RemoveFromClassList(UITkUtil.MOUSE_CURSOR_ZOOM_IN_STYLE_CLASS);
			_mouseCursorDisplay.RemoveFromClassList(UITkUtil.MOUSE_CURSOR_ZOOM_OUT_STYLE_CLASS);
			_mouseCursorDisplay.RemoveFromClassList(UITkUtil.MOUSE_CURSOR_PAN_STYLE_CLASS);
			_mouseCursorDisplay.RemoveFromClassList(UITkUtil.MOUSE_CURSOR_PAN_DRAG_STYLE_CLASS);
			_mouseCursorDisplay.style.display = DisplayStyle.None;

			// If drag-and-drop was happening, cancel it.
			if (_draggingPointerId != -1)
			{
				CancelDrag();
			}
		}

		void OnKeyDown(KeyDownEvent e)
		{
			// Note: key-down event is called for every frame the key is held down,
			// but this only applies to the last key pressed
			// (if multiple keys are held down, all of them will have a key-down event,
			// but only the latest one pressed has repeated key-down events).

			bool changeDetected = false;
			if (e.keyCode == KeyCode.Space)
			{
				changeDetected |= !_spacebarHeld;
				_spacebarHeld = true;

				if (_isDragging && _spacebarHeld && changeDetected)
				{
					// Spacebar started getting pressed while dragging.
					// That means user wants to start panning while dragging.
					_panStartPointerPos = target.ChangeCoordinatesTo(_moveTarget.contentContainer, _pointerLastKnownLocalPos);
				}
			}
			else if (e.keyCode == KeyCode.Escape && _isDragging)
			{
				changeDetected = true;
				// User pressed Escape while dragging.
				// Interpret this as a cancel to the drag-and-drop operation.
				// Abort it, even if the pointer is still held down.

				CancelDrag();
			}

			// We check this way instead of checking the keyCode because these are the latest values
			// (if user alt + tabbed away while alt was still held, then released the alt key once
			// they are out of the INTLord Editor window, we wouldn't have received a key-up event for alt).
			if (_ctrl != e.ctrlKey)
			{
				changeDetected = true;
				_ctrl = e.ctrlKey;
			}

			if (_alt != e.altKey)
			{
				changeDetected = true;
				_alt = e.altKey;
			}

			if (changeDetected)
			{
				RefreshMouseCursor(_lastKnownMousePos);
			}
		}

		void OnKeyUp(KeyUpEvent e)
		{
			switch (e.keyCode)
			{
				case KeyCode.Space:
					_spacebarHeld = false;
					if (_isDragging)
					{
						// Spacebar was released while dragging.
						// That means user wants to stop panning while dragging.
						// And we switch back to just dragging.
						_mouseCursorDisplay.RemoveFromClassList(UITkUtil.MOUSE_CURSOR_PAN_DRAG_STYLE_CLASS);
						_mouseCursorDisplay.RemoveFromClassList(UITkUtil.MOUSE_CURSOR_PAN_STYLE_CLASS);
					}
					break;
				case KeyCode.LeftControl:
				case KeyCode.RightControl:
					_ctrl = false;
					break;
				case KeyCode.LeftAlt:
				case KeyCode.RightAlt:
					_alt = false;
					break;
			}

			RefreshMouseCursor(_lastKnownMousePos);
		}

		void OnPointerDown(PointerDownEvent e)
		{
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
				Debug.Log($"NodeDragAndDrop OnPointerDown {e.position} Clicked on empty background");
				_mouseCursorDisplay.style.display = DisplayStyle.None;

				return;
			}

			// ---------------------------------------------------
			// Zoom controls take precedence over pan

			// Check again one last time because these can fail to get key-up events
			// (if user alt + tabbed away while the keys were held).
			if (!e.ctrlKey && _ctrl)
			{
				_ctrl = false;
				RefreshMouseCursor(e.position);
			}
			if (!e.altKey && _alt)
			{
				_alt = false;
				RefreshMouseCursor(e.position);
			}

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

			// ---------------------------------------------------

			if ((_spacebarHeld && e.button == 0) || e.button == 2) // pan: spacebar + left-click, or middle-click
			{
				_panStartPointerPos = target.ChangeCoordinatesTo(_moveTarget.contentContainer, e.localPosition);

				_isPanning = true;
				target.CapturePointer(e.pointerId);
				e.StopPropagation();

				RefreshMouseCursor(e.position);
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

			_draggedElement = draggedElement;
			_draggedElementHasBeenMoved = false;
			_isDragging = false;
			_isDraggingClonedElement = false;

			_draggedElementStartLocalPos = localPos;

			_draggingPointerId = e.pointerId;
		}

		void CommitToDragging(PointerMoveEvent e)
		{
			var gotClonedDragElement = OnStartedDrag();
			if (gotClonedDragElement != null)
			{
				_draggedElement = gotClonedDragElement;
				_isDraggingClonedElement = true;
			}

			// While being dragged, the node should be directly on the Tab Body
			if (_draggedElement.parent != _moveTarget)
			{
				// A NodeView's style.position gets set to Position.Relative when it's assigned into another NodeView
				// (see NodeView.Add and NodeView.Insert).
				// Since we'll take it out of that other NodeView, we need to manually set it back to Position.Absolute.
				// Position.Absolute makes it so that the NodeView's width/height doesn't have to conform to the Tab Body.
				_draggedElement.style.position = Position.Absolute;

				// Since the draggedElement will change parents, its local position doesn't get adjusted automatically.
				// The moment the parent is changed, the draggedElement's local position value doesn't make sense anymore.
				// We need to convert it and assign it manually.
				var newPosition = _draggedElement.ChangeCoordinatesTo(_moveTarget.contentContainer, _draggedElement.resolvedStyle.translate);

				// Note: _moveTarget is the Tab Body
				_moveTarget.Add(_draggedElement);

				_draggedElement.SetPosition(newPosition);
			}

			_draggedElement.BringToFront();

			_isDragging = true;
			RefreshMouseCursor(e.position);
		}

		void OnPointerMove(PointerMoveEvent e)
		{
			_lastKnownMousePos = e.position;

			if (_isPanning && !target.HasPointerCapture(e.pointerId))
			{
				// This isn't the pointer that initiated the panning, so we're not interested in this pointer-move event.
				// This happens in multitouch touchscreens where more than one finger may
				// be touching and moving on the screen.
				return;
			}

			if (e.pointerId == _draggingPointerId)
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
					return;
				}

				if (!_draggedElementHasBeenMoved)
				{
					_draggedElementHasBeenMoved = true;
					CommitToDragging(e);
				}
				if (_isDragging && !_spacebarHeld)
				{
					// convert mouse pos to be relative to the Tab Body
					var localPointer = target.ChangeCoordinatesTo(_moveTarget.contentContainer, e.localPosition);
					_draggedElement.SetPosition(localPointer.x - _draggedElementStartLocalPos.x, localPointer.y - _draggedElementStartLocalPos.y);

					_pointerLastKnownLocalPos = e.localPosition;
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
			if (!CanStopManipulation(e))
			{
				// Left mouse button wasn't the one released.
				return;
			}

			if (!_draggedElementHasBeenMoved && e.pointerId == _draggingPointerId)
			{
				_draggingPointerId = -1;
				_draggedElement = null;
				OnAbortedPotentialDrag();
				Debug.Assert(!_isDragging);
			}
			else if (_isDragging && e.pointerId == _draggingPointerId)
			{
				OnEndedDrag(e, _draggedElement.resolvedStyle.translate);
				_tooltip?.HideTooltip();
				if (_isDraggingClonedElement)
				{
					_draggedElement.RemoveFromHierarchy();
				}

				_draggingPointerId = -1;
				_draggedElement = null;
				_isDragging = false;

				RefreshMouseCursor(e.position);
			}

			if (_isPanning && target.HasPointerCapture(e.pointerId))
			{
				_mouseCursorDisplay.RemoveFromClassList(UITkUtil.MOUSE_CURSOR_PAN_DRAG_STYLE_CLASS);
				if (_spacebarHeld)
				{
					_mouseCursorDisplay.AddToClassList(UITkUtil.MOUSE_CURSOR_PAN_STYLE_CLASS);
				}
				else
				{
					_mouseCursorDisplay.RemoveFromClassList(UITkUtil.MOUSE_CURSOR_PAN_STYLE_CLASS);
				}

				_isPanning = false;
				target.ReleasePointer(e.pointerId);
				e.StopPropagation();
			}
		}

		void OnWheel(WheelEvent e)
		{
			_moveTarget.SetScaleByZoom(-e.delta.y, target, e.localMousePosition);
			e.StopPropagation();
		}

		/// <summary>
		/// This method is called the moment that the user moves the mouse while having left mouse button held down on a node.
		/// </summary>
		protected virtual VisualElement OnStartedDrag()
		{
			return null;
		}

		/// <summary>
		/// This method is called when the user releases left mouse button on a node without moving the mouse.
		/// </summary>
		protected virtual void OnAbortedPotentialDrag()
		{
		}

		/// <summary>
		/// Called when user releases the left mouse button on a node after having moved it.
		/// </summary>
		protected virtual void OnEndedDrag(PointerUpEvent e, Vector2 draggedElementEndPos)
		{
		}

		/// <summary>
		/// Called when user presses ESC or Alt + Tabs out while in the middle of a drag-and-drop operation.
		/// </summary>
		protected virtual void OnCanceledDrag()
		{
		}

		// ==================================================================================================

		void CancelDrag()
		{
			OnCanceledDrag();
			_tooltip?.HideTooltip();
			if (_isDraggingClonedElement)
			{
				_draggedElement.RemoveFromHierarchy();
			}

			_draggingPointerId = -1;
			_draggedElement = null;
			_isDragging = false;
		}

		void RefreshMouseCursor(Vector2 mousePos)
		{
			if (_isPanning)
			{
				_mouseCursorDisplay.RemoveFromClassList(UITkUtil.MOUSE_CURSOR_DRAG_STYLE_CLASS);
				_mouseCursorDisplay.RemoveFromClassList(UITkUtil.MOUSE_CURSOR_ZOOM_IN_STYLE_CLASS);
				_mouseCursorDisplay.RemoveFromClassList(UITkUtil.MOUSE_CURSOR_ZOOM_OUT_STYLE_CLASS);
				_mouseCursorDisplay.RemoveFromClassList(UITkUtil.MOUSE_CURSOR_PAN_STYLE_CLASS);
				_mouseCursorDisplay.AddToClassList(UITkUtil.MOUSE_CURSOR_PAN_DRAG_STYLE_CLASS);
				_mouseCursorDisplay.style.display = DisplayStyle.Flex;
			}
			else if (_isDragging && _spacebarHeld)
			{
				// temporarily panning while dragging
				_mouseCursorDisplay.RemoveFromClassList(UITkUtil.MOUSE_CURSOR_DRAG_STYLE_CLASS);
				_mouseCursorDisplay.RemoveFromClassList(UITkUtil.MOUSE_CURSOR_ZOOM_IN_STYLE_CLASS);
				_mouseCursorDisplay.RemoveFromClassList(UITkUtil.MOUSE_CURSOR_ZOOM_OUT_STYLE_CLASS);
				_mouseCursorDisplay.RemoveFromClassList(UITkUtil.MOUSE_CURSOR_PAN_STYLE_CLASS);
				_mouseCursorDisplay.AddToClassList(UITkUtil.MOUSE_CURSOR_PAN_DRAG_STYLE_CLASS);
				_mouseCursorDisplay.style.display = DisplayStyle.Flex;
			}
			else if (_isDragging)
			{
				_mouseCursorDisplay.RemoveFromClassList(UITkUtil.MOUSE_CURSOR_ZOOM_IN_STYLE_CLASS);
				_mouseCursorDisplay.RemoveFromClassList(UITkUtil.MOUSE_CURSOR_ZOOM_OUT_STYLE_CLASS);
				_mouseCursorDisplay.RemoveFromClassList(UITkUtil.MOUSE_CURSOR_PAN_STYLE_CLASS);
				_mouseCursorDisplay.RemoveFromClassList(UITkUtil.MOUSE_CURSOR_PAN_DRAG_STYLE_CLASS);
				_mouseCursorDisplay.AddToClassList(UITkUtil.MOUSE_CURSOR_DRAG_STYLE_CLASS);
				_mouseCursorDisplay.style.display = DisplayStyle.None;
			}
			else if (_ctrl && _spacebarHeld)
			{
				// zoom in
				_mouseCursorDisplay.RemoveFromClassList(UITkUtil.MOUSE_CURSOR_DRAG_STYLE_CLASS);
				_mouseCursorDisplay.RemoveFromClassList(UITkUtil.MOUSE_CURSOR_PAN_STYLE_CLASS);
				_mouseCursorDisplay.RemoveFromClassList(UITkUtil.MOUSE_CURSOR_ZOOM_OUT_STYLE_CLASS);
				_mouseCursorDisplay.AddToClassList(UITkUtil.MOUSE_CURSOR_ZOOM_IN_STYLE_CLASS);
				_mouseCursorDisplay.style.display = DisplayStyle.Flex;
			}
			else if (_alt && _spacebarHeld)
			{
				// zoom out
				_mouseCursorDisplay.RemoveFromClassList(UITkUtil.MOUSE_CURSOR_DRAG_STYLE_CLASS);
				_mouseCursorDisplay.RemoveFromClassList(UITkUtil.MOUSE_CURSOR_PAN_STYLE_CLASS);
				_mouseCursorDisplay.RemoveFromClassList(UITkUtil.MOUSE_CURSOR_ZOOM_IN_STYLE_CLASS);
				_mouseCursorDisplay.AddToClassList(UITkUtil.MOUSE_CURSOR_ZOOM_OUT_STYLE_CLASS);
				_mouseCursorDisplay.style.display = DisplayStyle.Flex;
			}
			else if (_spacebarHeld)
			{
				// pan
				_mouseCursorDisplay.RemoveFromClassList(UITkUtil.MOUSE_CURSOR_DRAG_STYLE_CLASS);
				_mouseCursorDisplay.RemoveFromClassList(UITkUtil.MOUSE_CURSOR_ZOOM_IN_STYLE_CLASS);
				_mouseCursorDisplay.RemoveFromClassList(UITkUtil.MOUSE_CURSOR_ZOOM_OUT_STYLE_CLASS);
				_mouseCursorDisplay.AddToClassList(UITkUtil.MOUSE_CURSOR_PAN_STYLE_CLASS);
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
