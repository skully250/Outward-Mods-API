using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace OModAPI
{

    //static class to call debug functions
    public static class OLogger
    {
        //store debug box variables
        private static Dictionary<string, DebugBox> debugPanels = new Dictionary<string, DebugBox>();
        private static int currentGUIID = 0;
        private static GameObject m_obj;

        //store Unity Debug ignore data
        public static IgnoreData ignoreList = new IgnoreData();

        //function to hook into so OLogger can debug as Unity
        public static void DebugMethodHook(string logString, string stackTrace, LogType type)
        {
            //check if current log string is in ignore list
            foreach (string _toCheck in ignoreList.ignoreInDefaultCompiler)
            {
                if (logString.Contains(_toCheck))
                {
                    return;
                }
            }

            //switch type of log into correct colors
            switch (type)
            {
                case LogType.Assert:
                    Log(logString, "00ff0ff", "Default Unity Compiler");
                    break;
                case LogType.Error:
                    Error(logString, "Default Unity Compiler");
                    break;
                case LogType.Exception:
                    Log(logString, "9999ffff", "Default Unity Compiler");
                    break;
                case LogType.Log:
                    Log(logString, null, "Default Unity Compiler");
                    break;
                case LogType.Warning:
                    Warning(logString, "Default Unity Compiler");
                    break;
                default:
                    break;
            }

            //log the stack trace to panel
            Log(stackTrace, null, "Default Unity Stack Trace");

        }

        //setup base folder
        internal static void SetupDirectory()
        {
            if (!Directory.Exists("mods/Debug"))
            {
                Directory.CreateDirectory("mods/Debug");
            }
        }

        //setup base game object
        internal static void SetupObject(GameObject _obj = null)
        {
            if (m_obj == null)
            {
                if (_obj == null)
                {
                    m_obj = new GameObject("DebugLogger");
                    GameObject.DontDestroyOnLoad(m_obj);
                }
                else
                {
                    m_obj = _obj;
                }
            }
        }

        //test if panel exists and return it
        private static bool PanelExistTest(DebugBox _panel, string _funcCalledFrom, out DebugBox _retDebugBox)
        {
            //setup output variable
            _retDebugBox = null;

            //check panel isn't null
            if (_panel != null)
            {
                //return true along with panel
                _retDebugBox = _panel;
                return true;
            }

            //debug into specific panel that this panel does not exist then return false
            Log(_panel + " : does not exist during call: " + _funcCalledFrom + ", try creating this panel before you toggle it", null, "Debug Logger Panel");
            return false;
        }

        //set the panel UI on/off
        public static void SetUIPanelEnabled(string _panel, bool _enabled)
        {
            DebugBox temp;
            if (PanelExistTest(FindUIPanel(_panel, true), "Toggle UI", out temp))
            {
                temp.SetGUIEnabled(_enabled);
            }
        }

        //clear text of a panel UI
        public static void ClearUIPanel(string _panel)
        {
            DebugBox temp;
            if (PanelExistTest(FindUIPanel(_panel, true), "Clear UI", out temp))
            {
                temp.ClearText();
            }
        }

        //set the panel's writeToDisk on/off
        public static void SetPanelWriteToDisk(string _panel, bool _writeToDisk)
        {
            DebugBox temp;
            if (PanelExistTest(FindUIPanel(_panel, true), "Toggle UI Write to disk", out temp))
            {
                temp.SetWriteToDisk(_writeToDisk);
            }
        }

        //try to return existing DebugBox
        internal static DebugBox FindUIPanel(string _panel = "Default", bool justSearching = false)
        {
            DebugBox ret;

            //check if debugPanel exists
            if (debugPanels.TryGetValue(_panel, out ret))
            {
                return ret;
            }

            if (justSearching)
            {
                return null;
            }

            //return newly created debug
            return CreateUIPanel(new Rect(400, 400, 400, 400), _panel);

        }

        //expose function to create panel
        public static void CreateLog(Rect _rect, string _panel = "Default", bool _writeToDisk = true, bool _enabledOnCreation = true)
        {
            CreateUIPanel(_rect, _panel, _writeToDisk, _enabledOnCreation);
        }

        //create debug box
        internal static DebugBox CreateUIPanel(Rect _rect, string _panel = "Default", bool _writeToDisk = true, bool _enabledOnCreation = false)
        {
            DebugBox ret;

            //try to see if panel exists
            if (debugPanels.TryGetValue(_panel, out ret))
            {
                return ret;
            }

            //check if master object is needed
            SetupObject();

            //create an object and add DebugBox component to it
            GameObject DebugObj = new GameObject("DebugObj");
            GameObject.DontDestroyOnLoad(DebugObj);
            DebugObj.transform.parent = m_obj.transform;
            ret = DebugObj.AddComponent<DebugBox>(new DebugBox(_panel, _rect, 100, currentGUIID++, _writeToDisk));
            ret.SetGUIEnabled(_enabledOnCreation);


            //add panel then return
            debugPanels.Add(_panel, ret);
            return ret;
        }

        public static void DestroyUIPanel(string _panel)
        {
            //grab DebugBox
            DebugBox temp;
            if (PanelExistTest(FindUIPanel(_panel, true), "Destroy UI", out temp))
            {
                temp.OnApplicationQuit();
                debugPanels.Remove(_panel);
            }
        }

        //Log text in given color or default to white
        public static void Log(object _obj, string _color = "ffffffff", string _panel = "Default")
        {
            //Test if appropriate color has been given
            if (_color == null || _color.Length != 8)
            {
                //Set color to white
                _color = "ffffffff";
            }

            //check if panel is good
            if (_panel == null)
            {
                //set panel to default
                _panel = "Default";
            }

            //find _chatPanel with _panel name then add text to it
            DebugBox _chatPanel = FindUIPanel(_panel);
            _chatPanel.AddText(_obj.ToString(), _color);
        }

        //log text in yellow
        public static void Warning(object _obj, string _panel = "Default")
        {
            Log(_obj, "ffff00ff", _panel);
        }

        //log text in red
        public static void Error(object _obj, string _panel = "Default")
        {
            Log(_obj, "ff0000ff", _panel);
        }
    }

}