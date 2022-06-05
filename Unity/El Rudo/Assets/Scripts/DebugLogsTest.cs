using UnityEngine;

namespace DebugStuff
{
    public class DebugLogsTest : MonoBehaviour
    {
        public static string myLog = "";
        private string output;
        private string stack;
        GUIStyle style;

        void OnEnable()
        {
            Application.logMessageReceived += Log;
            style = new GUIStyle();
            style.fontSize = 25;
        }

        void OnDisable()
        {
            Application.logMessageReceived -= Log;
        }

        public void Log(string logString, string stackTrace, LogType type)
        {
            output = logString;
            stack = stackTrace;
            myLog = output + "\n" + myLog;
            if (myLog.Length > 5000)
            {
                myLog = myLog.Substring(0, 4000);
            }
        }

        void OnGUI()
        {
            //if (!Application.isEditor) //Do not display in editor ( or you can use the UNITY_EDITOR macro to also disable the rest)
            {
                myLog = GUI.TextArea(new Rect(10, 10, Screen.width/4,  Screen.height - 10), myLog, style);
            }
        }
    }
}
