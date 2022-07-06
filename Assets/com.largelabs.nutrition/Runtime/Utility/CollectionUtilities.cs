using System.Collections.Generic;
using System.Linq;

public static class CollectionUtilities
{
	public static T GetNextElementInCircularCollection<T>(T i_element, IEnumerable<T> i_collection)
	{
		return GetNextElementInCircularList(i_element, i_collection.ToList());
	}

	public static T GetNextElementInCircularList<T>(T i_element, List<T> i_collection)
	{
		return GetNextElementInCircularList(i_collection.IndexOf(i_element), i_collection, i_collection.Count);
	}

	public static T GetNextElementInCircularList<T>(int i_currentIndex, List<T> i_collection, int i_collectionCount)
	{
		var newIndex = i_currentIndex + 1;

		if (newIndex >= i_collectionCount)
		{
			newIndex = 0;
		}

		return i_collection[newIndex];
	}

	public static T GetPreviousElementInCircularCollection<T>(T i_element, IEnumerable<T> i_collection)
	{
		return GetPreviousElementInCircularList(i_element, i_collection.ToList());
	}

	public static T GetPreviousElementInCircularList<T>(T i_element, List<T> i_collection)
	{
		return GetPreviousElementInCircularList(i_collection.IndexOf(i_element), i_collection, i_collection.Count);
	}

	public static T GetPreviousElementInCircularList<T>(int i_currentIndex, List<T> i_collection, int i_collectionCount)
	{
		var newIndex = i_currentIndex - 1;

		if (newIndex < 0)
		{
			newIndex = i_collectionCount - 1;
		}

		return i_collection[newIndex];
	}

}
