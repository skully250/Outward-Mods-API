using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace OModAPI
{
    public static class ReflectionTools
    {
        private static Dictionary<string, object>  reflectedInfo = new Dictionary<string, object>();

        public static MethodInfo GetMethod(Type instance, string methodName, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic)
        {
            return GetBase<MethodInfo>(instance, methodName, OPTIONS.Method, bindingFlags);
        }

        public static FieldInfo GetField(Type instance, string fieldName, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic)
        {
            return GetBase<FieldInfo>(instance, fieldName, OPTIONS.Field, bindingFlags);
        }

        private static T GetBase<T>(Type instance, string pName, OPTIONS opt, BindingFlags bindingFlags)
        {
            string fullName = instance.ToString() + "#" + pName;

            try
            {
                if (reflectedInfo.ContainsKey(fullName))
                    if (reflectedInfo[fullName] is T)
                        return (T)reflectedInfo[fullName];
                    else
                        throw new Exception(String.Format("{0} ({1}) was expected to be a {2}, but wasn't", fullName, reflectedInfo[fullName].ToString(), typeof(T)));

                object toAdd;

                if (opt == OPTIONS.Field)
                    toAdd = instance.GetField(pName, bindingFlags);
                else if (opt == OPTIONS.Method)
                    toAdd = instance.GetMethod(pName, bindingFlags);
                else
                    throw new ArgumentException(String.Format("Option {0} is not valid", opt));

                reflectedInfo.Add(fullName, toAdd);
                return (T)toAdd;
            }
            // name can come from an untrusted source, so handle if it doesn't actually exist.
            catch (NullReferenceException e)
            {
                Debug.Log(String.Format("{0} {1} was not found in type {2}", typeof(T), pName, instance));
                Debug.Log(e.StackTrace);
                return default(T);
            }
        }

        enum OPTIONS
        {
            Field,
            Method
        }
    }
}
