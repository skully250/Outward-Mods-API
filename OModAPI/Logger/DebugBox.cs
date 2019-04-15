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
        private string m_BoxName;
        internal List<DebugType> textLines;

        private Vector2 m_virtualSize;
        private Vector2 m_currentSize;
        private Matrix4x4 m_scaledMatrix;
        private GUIStyle m_guiStyle;

        private int maxLines;
        private int linesCharacterCount;

        private float m_currentScroll;
        private float m_scrollStart;
        private int m_offset;

        //used to see if text needs to be update
        private float m_prevOffset;

        //variables for GUI window
        private Rect m_windowRect;
        private int m_GUIID;

        private bool m_showGUI;
        private bool m_writeToDisk;

        private StreamWriter m_writer;
        private int m_msgCount = 0;
        private string m_currentOutputText;
        private bool m_updateCurrentText;

        public DebugBox(string _boxName, Rect _rect, int _maxStoredLines, int _GUIID, bool _writeToDisk)
        {
            //setup basic variables required
            m_BoxName = _boxName;
            m_showGUI = false;
            m_writeToDisk = _writeToDisk;

            m_GUIID = _GUIID;
            maxLines = _maxStoredLines;

            textLines = new List<DebugType>();

            m_offset = 0;
            m_currentScroll = maxLines;
            m_scrollStart = m_currentScroll;

            //used to scale GUI at a later point
            m_windowRect = _rect;
            m_virtualSize = new Vector2(1920, 1080);
            m_currentSize = new Vector2(Screen.width, Screen.height);
            m_scaledMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(Screen.width / m_virtualSize.x, Screen.height / m_virtualSize.y, 1));

            //setup GUIStyle
            m_guiStyle = new GUIStyle();
            m_guiStyle.richText = true;
            m_guiStyle.wordWrap = true;
            m_guiStyle.fontSize = 16;

            //used to setup output string
            m_currentOutputText = "";
            m_updateCurrentText = true;

            if (_writeToDisk)
            {

                //Setup debug text folder
                OLogger.SetupDirectory();

                //Setup debug text file
                m_writer = new StreamWriter("mods/Debug/" + m_BoxName + ".txt", false);

            }
        }

        public void OnApplicationQuit()
        {
            //close writer when quitting
            if (m_writer != null)
            {
                m_writer.Close();
            }
            textLines.Clear();
        }

        internal void AddText(string _msg, string _msgColor)
        {

            textLines.Insert(0, new DebugType(m_msgCount.ToString() + ": " + _msg, _msgColor));
            m_updateCurrentText = true;

            //debug info to file
            if (m_writeToDisk)
            {
                if (m_writer == null)
                {
                    OLogger.SetupDirectory();
                    m_writer = new StreamWriter("mods/Debug/" + m_BoxName + ".txt", true);
                }
                m_writer.WriteLine(m_msgCount.ToString() + " :Message: " + _msg);
            }

            m_msgCount++;

            if (textLines.Count > maxLines)
            {
                textLines.RemoveAt(maxLines);
            }

        }

        //update output text
        internal void UpdateGUIText()
        {
            //setup text and loop through all available text
            string output = "";

            long time = System.DateTime.Now.Ticks;
            TimeSpan timeSpan = new TimeSpan();

            //calculate size of characters and total character size
            GUIContent tempContent = new GUIContent("C");
            Vector2 charSize = m_guiStyle.CalcSize(tempContent);
            Vector2 sizeLeft = new Vector2(m_windowRect.width - 25, m_windowRect.height - 40);

            //calculate how many lines are supposed to be copied
            int linesToCopy = Mathf.CeilToInt(sizeLeft.y / (charSize.y + 1));
            float heightOfLine = 0;

            for (int a = m_offset; a < m_offset + linesToCopy; a++)
            {

                //force ignore if time taken is > 10ms
                timeSpan = new TimeSpan(System.DateTime.Now.Ticks - time);
                if (timeSpan.Milliseconds > OLogger.maxSetupTextTime)
                {
                    linesCharacterCount = output.Length;
                    m_currentOutputText = output;
                    return;
                }

                if (a < textLines.Count)
                {
                    string currentString = "<color=#" + textLines[a].messageColor + ">" + textLines[a].message + "</color>";

                    //calculate current height of line
                    tempContent = new GUIContent(currentString);
                    heightOfLine = m_guiStyle.CalcHeight(tempContent, sizeLeft.x);

                    //add color + message + newline to text
                    output += currentString + Environment.NewLine;

                    //check height is still available
                    if (heightOfLine > sizeLeft.y)
                    {
                        //stop trying to get lines
                        break;
                    }

                    //remove current height of line from sizeLeft
                    sizeLeft.y -= heightOfLine;

                }
                else
                {
                    //break if past texLines count
                    break;
                }

            }

            linesCharacterCount = output.Length;
            m_currentOutputText = output;
            m_updateCurrentText = false;
        }

        //Set GUI on/off
        internal void SetGUIEnabled(bool _enabled)
        {
            m_showGUI = _enabled;
        }

        //Set writeToDisk on/off
        internal void SetWriteToDisk(bool _writeToDisk)
        {
            m_writeToDisk = _writeToDisk;
        }

        //Clear all text
        internal void ClearText()
        {
            textLines.Clear();
            m_currentScroll = m_scrollStart;
        }

        internal void Update()
        {

            //check if text needs to be updated and then update on a frame basis
            if (m_showGUI)
            {
                if (m_currentSize.x != Screen.width || m_currentSize.y != Screen.height)
                {
                    m_scaledMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(Screen.width / m_virtualSize.x, Screen.height / m_virtualSize.y, 1));
                    m_currentSize = new Vector2(Screen.width, Screen.height);
                }

                if (m_updateCurrentText)
                {
                    UpdateGUIText();
                }
            }

        }

        //Draw GUI
        internal void OnGUI()
        {

            //scale UI to current resolution
            GUI.matrix = m_scaledMatrix;

            if (m_showGUI)
            {
                //create debugWindow
                m_windowRect = GUI.Window(m_GUIID, m_windowRect, WindowFunction, m_BoxName);
            }

        }

        internal void WindowFunction(int windowID)
        {

            //allow window to be dragged
            GUI.DragWindow(new Rect(0, 0, m_windowRect.width, 20));

            //check if text needs to be update
            m_prevOffset = m_offset;

            //begin scroll area
            GUILayout.BeginArea(new Rect(m_windowRect.width - 25, 30, 20, m_windowRect.height - 40));

            //display offset scroll to screen
            m_currentScroll = GUILayout.VerticalScrollbar(m_currentScroll, 10, maxLines + 10, 24, GUILayout.Height(m_windowRect.height - 40));
            m_offset = (int)(maxLines - m_currentScroll);
            GUILayout.EndArea();

            //check if text needs to be update
            if (m_prevOffset != m_offset)
            {
                m_updateCurrentText = true;
            }

            //begin text area
            GUILayout.BeginArea(new Rect(10, 30, m_windowRect.width - 50, m_windowRect.height - 40));

            GUILayout.TextArea(m_currentOutputText, linesCharacterCount, m_guiStyle, GUILayout.Height(m_windowRect.height - 40));
            GUILayout.EndArea();

        }

    }


}