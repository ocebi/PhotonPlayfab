using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    /// <summary>
    /// Calls the delegate if it is not null
    /// </summary>
    /// <param name="i_Action">The action to be called</param>
    public static void InvokeSafe(this Action i_Action)
    {
        if (i_Action != null)
        {
            i_Action.Invoke();
        }
    }
    
    /// <summary>
    /// Calls the delegate if it is not null
    /// </summary>
    /// <param name="action">The action to be called</param>
    /// <param name="i_Object">The object passed</param>
    public static void InvokeSafe<T>(this Action<T> action, T value)
    {
        if (action != null)
        {
            action.Invoke(value);
        }
    }
    
    //Breadth-first search
    public static Transform FindDeepChild(this Transform aParent, string aName)
    {
        if (aParent != null)
        {
            var result = aParent.Find(aName);
            if (result != null)
                return result;

            foreach (Transform child in aParent)
            {
                result = child.FindDeepChild(aName);
                if (result != null)
                    return result;
            }
        }

        return null;
    }

    public static T FindDeepChild<T>(this Transform aParent, string aName)
    {
        T result = default(T);

        var transform = aParent.FindDeepChild(aName);

        if (transform != null)
        {
            result = (typeof(T) == typeof(GameObject)) ? (T)Convert.ChangeType(transform.gameObject, typeof(T)) : transform.GetComponent<T>();
        }

        if (result == null)
        {
            Debug.LogError($"FindDeepChild didn't find: '{aName}' on GameObject: '{aParent.name}'");
        }

        return result;
    }
    
    public static string AddColorToString(string message, Color color) => $"<color=#{ColorUtility.ToHtmlStringRGB(color)}> {message}</color>";
}