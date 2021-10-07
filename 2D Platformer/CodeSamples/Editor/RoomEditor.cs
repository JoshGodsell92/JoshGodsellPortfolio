#if (UNITY_EDITOR)
//////////////////////////////////////////////////////////////////
// File Name: Roomeditor.cs                                     //
// Author: Josh Godsell                                         //
// Date Created: 3/2/19                                         //
// Date Last Edited: 3/2/19                                     //
// Brief: Editor window class for room planning                 //
//////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RoomEditor : EditorWindow {

    private Rect m_RoomItemSize = new Rect(0,0,1.6f,1.0f);

    [MenuItem ("Window/Room Planner")]

    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(RoomEditor));
    }

    private void OnGUI()
    {

        GUILayout.Space(15);
        GUILayout.BeginHorizontal();

        if(GUI.Button(new Rect(10,10,100,25), "Add Room"))
        {
           
        }

        string t_RoomName = "";

        GUILayout.Space(125);
        GUILayout.Label("Room Name:");

        GUILayout.TextField(t_RoomName);

        GUILayout.EndHorizontal();

        GUI.Box(new Rect(5, 50, EditorGUIUtility.currentViewWidth - 10, 300),"Level Layout");


        DragDropGUI();
    }

    private void DragDropGUI()
    {
        // Cache References:
        Event currentEvent = Event.current;




    }

    public class DragableRoom
    {
       // private string m_RoomName;
       // private Vector2 m_Dimesions = new Vector2(32, 20);
       // private Vector2 m_Position;

        public DragableRoom(string a_name, Vector2 a_pos)
        {
           // m_RoomName = a_name;
           // m_Position = a_pos;
        }


    }


}
#endif