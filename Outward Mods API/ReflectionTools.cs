using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace OModAPI
{
    public static class ReflectionTools
    {
        private static Dictionary<string, object> reflectedInfo = new Dictionary<string, object>();

        public static MethodInfo GetMethod(object instance, string methodName)
        {
            return GetBase<MethodInfo>(instance.GetType(), methodName, OPTIONS.Method);
        }

        public static FieldInfo GetField(object instance, string fieldName)
        {
            return GetBase<FieldInfo>(instance.GetType(), fieldName, OPTIONS.Field);
        }

        private static T GetBase<T>(Type instance, string pName, OPTIONS opt)
        {
            string fullName = instance.ToString() + "#" + pName;

            try
            {
                if (reflectedInfo.ContainsKey(fullName))
                    if (reflectedInfo[fullName] is T)
                        return (T)reflectedInfo[fullName];
                    else
                        throw new Exception(String.Format("{0} ({1}) was expected to be a MethodInfo, but wasn't", fullName, reflectedInfo[fullName].ToString()));

                object toAdd;

                if (opt == OPTIONS.Field)
                    toAdd = instance.GetField(pName, BindingFlags.Instance | BindingFlags.NonPublic);
                else if (opt == OPTIONS.Method)
                    toAdd = instance.GetMethod(pName, BindingFlags.Instance | BindingFlags.NonPublic);
                else
                    throw new ArgumentException(String.Format("Option {0} is not valid", opt));

                reflectedInfo.Add(fullName, toAdd);
                return (T)toAdd;
            }
            // name can come from an untrusted source, so handle if it doesn't actually exist.
            catch (NullReferenceException e)
            {
                Debug.Log(String.Format("Method {0} was not found in type {1}", pName, instance));
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