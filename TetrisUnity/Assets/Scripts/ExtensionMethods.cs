using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{

    public static void Randomize<T>(this List<T> list)
    {
        List<T> copy = new List<T>(list);

        int current = 0;
        while(copy.Count > 0)
        {
            int randomIndex = Random.Range(0, copy.Count);
            list[current] = copy[randomIndex];
            copy.RemoveAt(randomIndex);
            current++;
        }
    }
    
}
