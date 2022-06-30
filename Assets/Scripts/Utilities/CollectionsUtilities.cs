using System.Collections.Generic;
using System.Linq;

public static class CollectionsUtilities
{
	public static T GetNextElementInCircularCollection<T>(T element, IEnumerable<T> collection)
	{
		var index = collection.ToList().IndexOf(element) + 1;

		if (index >= collection.Count())
		{
			index = 0;
		}

		return collection.ElementAt(index);
	}

	public static T GetPreviousElementInCircularCollection<T>(T element, IEnumerable<T> collection)
	{
		var index = collection.ToList().IndexOf(element) - 1;

		if (index < 0)
		{
			index = 0;
		}

		return collection.ElementAt(index);
	}

}
