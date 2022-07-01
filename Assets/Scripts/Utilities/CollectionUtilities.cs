using System.Collections.Generic;
using System.Linq;

public static class CollectionUtilities
{
	public static T GetNextElementInCircularCollection<T>(T element, IEnumerable<T> collection)
	{
		return GetNextElementInCircularList(element, collection.ToList());
	}

	public static T GetNextElementInCircularList<T>(T element, List<T> collection)
	{
		return GetNextElementInCircularList(collection.IndexOf(element), collection, collection.Count);
	}

	public static T GetNextElementInCircularList<T>(int currentIndex, List<T> collection, int collectionCount)
	{
		var newIndex = currentIndex + 1;

		if (newIndex >= collectionCount)
		{
			newIndex = 0;
		}

		return collection[newIndex];
	}

	public static T GetPreviousElementInCircularCollection<T>(T element, IEnumerable<T> collection)
	{
		return GetPreviousElementInCircularList(element, collection.ToList());
	}

	public static T GetPreviousElementInCircularList<T>(T element, List<T> collection)
	{
		return GetPreviousElementInCircularList(collection.IndexOf(element), collection, collection.Count);
	}

	public static T GetPreviousElementInCircularList<T>(int currentIndex, List<T> collection, int collectionCount)
	{
		var newIndex = currentIndex - 1;

		if (newIndex < 0)
		{
			newIndex = collectionCount - 1;
		}

		return collection[newIndex];
	}

}
