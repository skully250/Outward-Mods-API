using System;
using System.IO;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace OModAPI
{

    internal class DebugBox : MonoBehaviour
    {
        //store basic variables of a debugBox
        private string BoxName;
        internal List<DebugType> textLines;

        private Vector2 virtualSize;
        private int maxLines;
        private float currentScroll;
        private float scrollStart;
        private int offset;

        //variables for GUI window
        private Rect windowRect;
        private int GUIID;

        private bool showGUI;
        private bool writeToDisk;

        private StreamWriter writer;

        public DebugBox(string _boxName, Rect _rect, int _maxStoredLines, int _GUIID, bool _writeToDisk)
        {
            //setup basic variables required
            BoxName = _boxName;
            showGUI = false;
            writeToDisk = _writeToDisk;

            GUIID = _GUIID;
            maxLines = _maxStoredLines;
            textLines = new List<DebugType>();

            offset = 0;
            currentScroll = maxLines;
            scrollStart = currentScroll;

            //used to scale GUI at a later point
            windowRect = _rect;
            virtualSize = new Vector2(1920, 1080);

            if (_writeToDisk)
            {

                //Setup debug text folder
                OLogger.SetupDirectory();

                //Setup debug text file
                writer = new StreamWriter("mods/Debug/" + BoxName + ".txt", false);

            }
        }

        public void OnApplicationQuit()
        {
            if (writer != null)
            {
                writer.Close();
            }
            textLines.Clear();
        }

        private int msgCount = 0;

        internal void AddText(string _msg, string _msgColor)
        {

            textLines.Insert(0, new DebugType(msgCount.ToString() + ": " + _msg , _msgColor));

            //debug info to file
            if (writeToDisk)
            {
                if (writer == null)
                {
                    OLogger.SetupDirectory();
                    writer = new StreamWriter("mods/Debug/" + BoxName + ".txt", true);
                }
                writer.WriteLine(msgCount.ToString() + " :Message: " + _msg);
            }

            msgCount++;

            if (textLines.Count > maxLines)
            {
                textLines.RemoveAt(maxLines);
            }

        }

        //Set GUI on/off
        internal void SetGUIEnabled(bool _enabled)
        {
            showGUI = _enabled;
        }

        //Set writeToDisk on/off
        internal void SetWriteToDisk(bool _writeToDisk)
        {
            writeToDisk = _writeToDisk;
        }

        //Clear all text
        internal void ClearText()
        {
            textLines.Clear();
            currentScroll = scrollStart;
        }

        //Draw GUI
        internal void OnGUI()
        {

            //scale UI to current resolution
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(Screen.width / virtualSize.x, Screen.height / virtualSize.y, 1));

            if (showGUI)
            {
                //create debugWindow
                windowRect = GUI.Window(GUIID, windowRect, WindowFunction, BoxName);
            }

        }

        internal void WindowFunction(int windowID)
        {

            //allow window to be dragged
            GUI.DragWindow(new Rect(0, 0, windowRect.width, 20));

            //begin scroll area
            GUILayout.BeginArea(new Rect(windowRect.width - 25, 30, 20, windowRect.height - 40));

            //display offset scroll to screen
            currentScroll = GUILayout.VerticalScrollbar(currentScroll, 10, maxLines + 10, 24, GUILayout.Height(windowRect.height - 40));
            offset = (int)(maxLines - currentScroll);
            GUILayout.EndArea();

            //begin text area
            GUILayout.BeginArea(new Rect(10, 30, windowRect.width - 50, windowRect.height - 40));

            //setup gui style for colors/rich txt
            GUIStyle style = new GUIStyle();
            style.richText = true;

            //setup text and loop through all available text
            string output = "";

            for (int a = offset; a < textLines.Count; a++)
            {
                //check if output is past a certain length to stop the
                //text getting cut off mid way
                if (output.Length + textLines[a].message.Length > 3000)
                {
                    break;
                }
                else
                {
                    //add color + message + newline to text
                    output += "<color=#" + textLines[a].messageColor + ">" + textLines[a].message + "</color>";
                    output += Environment.NewLine;
                }

            }

            //print text and then end the area
            style.wordWrap = true;
            GUILayout.TextArea(output, 6000, style, GUILayout.Height(windowRect.height - 40));
            GUILayout.EndArea();

        }

    }


}