using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using Multiplayer.NodeGraph;
using Multiplayer.GameManager;
using Unity.VisualScripting;

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
        [HideInInspector] public bool isLeftClickDragging = false;
        [HideInInspector] public bool isSelected = false;
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

        ///<summary>
        /// Process events for the node
        /// </summary>
        public void ProcessEvents(Event currentEvent)
        {
            switch(currentEvent.type)
            {
                case EventType.MouseDown:
                    ProcessMouseDownEvent(currentEvent);
                    break;
                case EventType.MouseUp:
                    ProcessMouseUpEvent(currentEvent);
                    break;

                case EventType.MouseDrag:
                    ProcessMouseDragEvent(currentEvent);
                    break;

                default:
                    break;

            }
        }

        private void ProcessMouseDownEvent(Event currentEvent)
        {
            // left click
            if(currentEvent.button == 0)
            {
                ProcessLeftClickDownEvent();
            }

            else if(currentEvent.button == 1)
            {
                ProcessRightClickDownEvent(currentEvent);
            }
        }

        private void ProcessLeftClickDownEvent()
        {
            // select in Assets/ScriptableObjectAssets/Dungeon/RoomNodeGraphs/RoomNodeGraph
            Selection.activeObject = this;

            // Toggle node selection
            if (isSelected)
                isSelected = false;
            else
                isSelected = true;
        }

        private void ProcessRightClickDownEvent(Event currentEvent)
        {
            roomNodeGraph.SetNodeToDrawConnectionLineFrom(this, currentEvent.mousePosition);
        }

        private void ProcessMouseUpEvent(Event currentEvent)
        {
            if (currentEvent.button == 0)
            {
                ProcessLeftClickUpEvent();
            }
        }

        private void ProcessLeftClickUpEvent()
        {
            if(isLeftClickDragging)
            {
                isLeftClickDragging = false;
            }
        }

        private void ProcessMouseDragEvent(Event currentEvent)
        {
            if(currentEvent.button == 0)
            {
                ProcessLeftMouseDragEvent(currentEvent);
            }
        }

        private void ProcessLeftMouseDragEvent(Event currentEvent)
        {
            isLeftClickDragging = true;
            DragNode(currentEvent.delta);
            GUI.changed = true;
        }

        private void DragNode(Vector2 delta)
        {
            rect.position += delta;
            EditorUtility.SetDirty(this);
        }

        ///<summary>
        /// Add childID to the node
        /// </summary>
        public bool AddChildRoomNodeIDToRoomNode(string childID)
        {
            childRoomNodeIDList.Add(childID);
            return true;
        }

        public bool AddParentRoomNodeIDToRoomNode(string parentID)
        {
            parentRoomNodeIDList.Add(parentID);
            return true;
        }

#endif

        #endregion Editor Code

    }

}
