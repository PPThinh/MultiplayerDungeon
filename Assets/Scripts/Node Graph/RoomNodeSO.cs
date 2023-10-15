using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using Multiplayer.NodeGraph;
using Multiplayer.GameManager;

namespace MultiplayerGame.NodeGraph
{
    public class RoomNodeSO : ScriptableObject
    {
        [HideInInspector] public string id;
        [HideInInspector] public List<string> parentRoomNodeIDList = new List<string>();
        [HideInInspector] public List<string> childRoomNodeIDList = new List<string>();
        [HideInInspector] public RoomNodeGraphSO roomNodeGraph;
        public RoomNodeTypeSO roomNodeType;
        [HideInInspector] public RoomNodeTypeListSO roomNodeTypeList;

        #region Editor Code

#if UNITY_EDITOR
        [HideInInspector] public Rect rect;

        ///<summary>
        /// Initialize node
        /// </summary>
        public void Initialize(Rect rect, RoomNodeGraphSO nodeGraph, RoomNodeTypeSO roomNodeType)
        {
            this.rect = rect;
            this.id = Guid.NewGuid().ToString();
            this.name = "RoomNode";
            this.roomNodeGraph = nodeGraph;
            this.roomNodeType = roomNodeType;


            // Load room node type list
            roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
        }

        ///<summary>
        /// Draw node with the nodestyle
        /// </summary>
        
        public void Draw(GUIStyle nodeStyle)
        {
            // Draw node box using begin area
            GUILayout.BeginArea(rect, nodeStyle);

            // Start region to detect popup selection changes
            EditorGUI.BeginChangeCheck();

            // Display a popup using the RoomNodeType name values that can be selected from (default to the currently set roomNodeStyle)
            int selected = roomNodeTypeList.list.FindIndex(x => x == roomNodeType);

            int selection = EditorGUILayout.Popup("", selected, GetRoomNodeTypesToDisplay());

            roomNodeType = roomNodeTypeList.list[selection];

            if(EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(this);
            }
            GUILayout.EndArea();

        }

        ///<summary>
        /// Populate a string array with this room node types to display that can be selected
        /// </summary>
        
        public string[] GetRoomNodeTypesToDisplay()
        {
            string[] roomArray = new string[roomNodeTypeList.list.Count];

            for(int i = 0; i < roomNodeTypeList.list.Count; i++)
            {
                if (roomNodeTypeList.list[i].displayInNodeGraphEditor)
                    roomArray[i] = roomNodeTypeList.list[i].roomNodeTypeName;
            }

            return roomArray;
        }
#endif

        #endregion Editor Code

    }

}
