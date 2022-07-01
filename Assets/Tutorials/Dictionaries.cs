using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dictionaries : MonoBehaviour
{
    // Dictionaries are a key value pair structure
    Dictionary<int, float> myFloats = null;
    List<float> floatList = null;

    private void Start()
    {
        myFloats = new Dictionary<int, float>();
        myFloats.Add(1, 56f);

        // Will log 56
        Debug.Log(myFloats[1]);

        // Check if dictionary contains key
        if(true == myFloats.ContainsKey(1))
        {
            // Do stuff with the value at key 1
            myFloats[1] = 40f;
        }

        float val = 0f;
        if(false == myFloats.TryGetValue(2, out val))
        {
            // val is still 0 because there there is not key "2"

            // let's add it !
            myFloats.Add(2, 56498f);
        }
        else
        {
            // Key was found, val was set to value 
        }


        // Let's loop ! 
        foreach(KeyValuePair<int, float> pair in myFloats)
        {
            Debug.Log(pair.Key + " , " + pair.Value);
        }


        // Performance
        floatList = new List<float>(50);
        for (int i = 0; i < 50; i++)
        {
            floatList.Add(i);
        }

        // Quite slow : o(n), n being the size of the list
        if(floatList.Contains(40))
        {

        }

        // Super fast : o(1), the size of the dictionary doesn't matter
        if(myFloats.ContainsKey(1))
        {

        }

        // So why make a list ???
        // Dictionaries take more space in RAM
        // Dictionaries are not ordered
    }

}
