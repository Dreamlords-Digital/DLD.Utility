// COPYRIGHT (C) DREAMLORDS DIGITAL INC. - ALL RIGHTS RESERVED.

namespace DLD.Utility
{

/// <summary>
///    Similar to <see cref="System.Collections.Generic.IReadOnlyList{T}"/>,
///    except this one doesn't have enumerators.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IReadOnlyIndexedList<out T>
{
	int Count { get; }
	T this[int index] { get; }
}

}
