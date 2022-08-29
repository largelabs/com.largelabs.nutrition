/*using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DictionaryBasicsExplained : MonoBehaviour
{
    // Dictionaries are a key value pair structure
    // They are not ordered
    Dictionary<string, float> myDictionary = null;

    // A list is an ordered collection
    List<float> myList = null;

    // You cannot serialize a dictionary (unlike a list or an array). The line below will not work
    [SerializeField] Dictionary<string, float> serializedDic = null; // Won't appear in the inspector because not serializable !! 

    private void explain()
    {
        myDictionary = new Dictionary<string, float>();
        myDictionary.Add("fifty six", 56f);

        // Will log 56
        Debug.Log(myDictionary["fifty six"]);

        // Check if dictionary contains key
        if(true == myDictionary.ContainsKey("fifty six"))
        {
            // Do stuff with the value at key 1
            myDictionary["fifty six"] = 56f;
        }

        float val = 0f;
        if(false == myDictionary.TryGetValue("a weird number", out val))
        {
            // val is still 0 because there there is not key "2"

            // let's add it !
            myDictionary.Add("a weird number", 56498f);
        }
        else
        {
            // Key was found, val was set to value 
        }


        // Let's loop ! 
        foreach(KeyValuePair<string, float> pair in myDictionary)
        {
            Debug.Log(pair.Key + " , " + pair.Value);
        }


        // Performance
        myList = new List<float>(50);
        for (int i = 0; i < 50; i++)
        {
            myList.Add(i);
        }

        // Quite slow : o(n), n being the size of the list
        if(myList.Contains(40))
        {

        }

        // Super fast : o(1), the size of the dictionary doesn't matter
        if(myDictionary.ContainsKey("forty"))
        {

        }

        // So why make a list if dictionaries are so performant ???
        // Dictionaries take more space in RAM
        // Dictionaries are not ordered (you cannot sort a dictionary or access a value by index)


        // You can get all keys and all values and convert them to a list thanks to Linq
        List<string> keys = myDictionary.Keys.ToList();
        List<float> values = myDictionary.Values.ToList();

        // However, since you cannot guaranty the order of elements (keys and values) in a dictionary,
        // do not use this as a replacement for lists.
        // Also, calling.ToList() will create garbage in your heap (temporary list allocation). 
    }
}*/
