// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

using UnityEngine;
using UnityEngine.UIElements;

namespace DLD.UIToolkit
{

public interface IResizeManipulatorListener
{
	void OnResized(VisualElement resizedElement, float newWidth);
}

public class ResizeManipulator : PointerManipulator
{
	Vector2 _start;
	bool _active;
	int _pointerId;
	readonly VisualElement _moveTarget;
	readonly Length _minWidth;
	readonly Length _maxWidth;
	readonly bool _reverseDirection;

	readonly EventCallback<PointerDownEvent> _onPointerDown;
	readonly EventCallback<PointerMoveEvent> _onPointerMove;
	readonly EventCallback<PointerUpEvent> _onPointerUp;

	IResizeManipulatorListener _listener;

	public ResizeManipulator(VisualElement newMoveTarget, Length minWidth, Length maxWidth, bool reverseDirection = false, IResizeManipulatorListener newListener = null)
	{
		_moveTarget = newMoveTarget;
		_minWidth = minWidth;
		_maxWidth = maxWidth;
		_reverseDirection = reverseDirection;

		_pointerId = -1;
		activators.Add(new ManipulatorActivationFilter
		{
			button = MouseButton.LeftMouse
		});
		_active = false;

		_onPointerDown = OnPointerDown;
		_onPointerMove = OnPointerMove;
		_onPointerUp = OnPointerUp;

		_listener = newListener;
	}

	public void SetListener(IResizeManipulatorListener newListener)
	{
		_listener = newListener;
	}

	protected override void RegisterCallbacksOnTarget()
	{
		target.RegisterCallback(_onPointerDown);
		target.RegisterCallback(_onPointerMove);
		target.RegisterCallback(_onPointerUp);
	}

	protected override void UnregisterCallbacksFromTarget()
	{
		target.UnregisterCallback(_onPointerDown);
		target.UnregisterCallback(_onPointerMove);
		target.UnregisterCallback(_onPointerUp);
	}

	protected void OnPointerDown(PointerDownEvent e)
	{
		if (_active)
		{
			e.StopImmediatePropagation();
			return;
		}

		if (CanStartManipulation(e))
		{
			_start = e.localPosition;
			_pointerId = e.pointerId;

			_active = true;
			target.CapturePointer(_pointerId);
			e.StopPropagation();

			if (_moveTarget.style.width == StyleKeyword.Null)
			{
				_moveTarget.style.width = _moveTarget.resolvedStyle.width;
			}
		}
	}

	protected void OnPointerMove(PointerMoveEvent e)
	{
		if (!_active || !target.HasPointerCapture(_pointerId))
		{
			return;
		}

		float delta = e.localPosition.x - _start.x;
		var currentWidth = _moveTarget.style.width;
		float newWidth;
		if (_reverseDirection)
		{
			newWidth = currentWidth.value.value - delta;
		}
		else
		{
			newWidth = currentWidth.value.value + delta;
		}

		float minWidth;
		if (_minWidth.unit == LengthUnit.Percent)
		{
			minWidth = _moveTarget.parent.contentRect.width * (_minWidth.value / 100);
		}
		else // assume LengthUnit.Pixels
		{
			minWidth = _minWidth.value;
		}

		bool fitsMinWidth = newWidth >= minWidth;

		float maxWidth;
		if (_maxWidth.unit == LengthUnit.Percent)
		{
			maxWidth = _moveTarget.parent.contentRect.width * (_maxWidth.value / 100);
		}
		else // assume LengthUnit.Pixels
		{
			maxWidth = _maxWidth.value;
		}

		bool fitsMaxWidth = newWidth <= maxWidth;

		if (fitsMinWidth && fitsMaxWidth)
		{
			currentWidth.value = newWidth;
			_moveTarget.style.width = currentWidth;
		}
		else if (!fitsMinWidth)
		{
			currentWidth.value = minWidth;
			_moveTarget.style.width = currentWidth;
		}
		else // !fitsMaxWidth
		{
			currentWidth.value = maxWidth;
			_moveTarget.style.width = currentWidth;
		}

		e.StopPropagation();
	}

	protected void OnPointerUp(PointerUpEvent e)
	{
		if (!_active || !target.HasPointerCapture(_pointerId) || !CanStopManipulation(e))
		{
			return;
		}

		_active = false;
		target.ReleaseMouse();
		e.StopPropagation();

		_listener?.OnResized(_moveTarget, _moveTarget.style.width.value.value);
	}
}

}
