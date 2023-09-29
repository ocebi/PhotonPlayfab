using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
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
}