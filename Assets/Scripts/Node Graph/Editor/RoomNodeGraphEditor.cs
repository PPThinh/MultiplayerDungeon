using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

using Multiplayer.NodeGraph;
using Multiplayer.GameManager;
using UnityEditor.MPE;

namespace MultiplayerGame.NodeGraph.Editor
{
    public class RoomNodeGraphEditor : EditorWindow
    {
        private GUIStyle m_roomNodeStyle;
        private GUIStyle m_roomNodeSelectedStyle;

        private static RoomNodeGraphSO m_currentRoomNodeGraph;
        private static RoomNodeSO m_currentRoomNode = null;
        private RoomNodeTypeListSO m_roomNodeTypeList;

        // Node layout values
        private const float _nodeWidth = 160f;
        private const float _nodeHeight = 75;
        private const int _nodePadding = 25;
        private const int _nodeBorder = 12;

        // Connecting line values
        private const float _connectingLineWidth = 3f;
        private const float _connectingLineArrowSize = 6f;

        [MenuItem("Room Node Graph Editor", menuItem = "Window/Dungeon Editor/Room Node Graph Editor")]
        private static void OpenWindow() => GetWindow<RoomNodeGraphEditor>("Room Node Graph Editor");

        /// <summary>
        /// Open the room node graph editor window if aroom node graph scriptable object asset is double clicked in the inspector
        /// </summary>
        [OnOpenAsset(0)]
        public static bool OnDoubleClickAsset(int instanceID, int line)
        {
            RoomNodeGraphSO roomNodeGraph = EditorUtility.InstanceIDToObject(instanceID) as RoomNodeGraphSO;
            
            if(roomNodeGraph != null)
            {
                OpenWindow();
                m_currentRoomNodeGraph = roomNodeGraph;

                return true;
            }
            return false;
        }


        private void OnEnable()
        {
            // Subscribe to the inspector selection changed event
            Selection.selectionChanged += InspectorSelectionChanged;

            // Define node layout style
            m_roomNodeStyle = new GUIStyle();
            m_roomNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
            m_roomNodeStyle.normal.textColor = Color.white;
            m_roomNodeStyle.padding = new RectOffset(_nodePadding, _nodePadding, _nodePadding, _nodePadding);
            m_roomNodeStyle.border = new RectOffset(_nodeBorder, _nodeBorder, _nodeBorder, _nodeBorder);

            // Define node selected node style
            m_roomNodeSelectedStyle = new GUIStyle();
            m_roomNodeSelectedStyle.normal.background = EditorGUIUtility.Load("node1 on") as Texture2D;
            m_roomNodeSelectedStyle.normal.textColor= Color.white;
            m_roomNodeSelectedStyle.padding = new RectOffset(_nodePadding, _nodePadding, _nodePadding, _nodePadding);
            m_roomNodeSelectedStyle.border = new RectOffset(_nodeBorder, _nodeBorder, _nodeBorder, _nodeBorder);

            // Load room node types
            m_roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
        }

        private void OnDisable()
        {
            // Unsubscribe from the inspector selection changed event
            Selection.selectionChanged -= InspectorSelectionChanged;
        }

        ///<summary>
        ///    Draw Editor GUI
        ///</summary>
        private void OnGUI()
        {
            //if a scriptabke object of type RoomNodeGraphSO has been selected then process
            if(m_currentRoomNodeGraph != null)
            {
                // Draw line if being dragged
                DrawDraggedLine();


                // Process Events
                ProcessEvents(Event.current);

                // Draw connections between room node
                DrawRoomConnection();

                // Draw Room Nodes
                DrawRoomNodes();
            }

            if(GUI.changed)
            {
                Repaint();
            }

        }

        private void DrawDraggedLine()
        {
            if(m_currentRoomNodeGraph.linePosition != Vector2.zero)
            {
                // draw line from node to line position
                Handles.DrawBezier(m_currentRoomNodeGraph.roomNodeToDrawLineFrom.rect.center,
                                    m_currentRoomNodeGraph.linePosition, m_currentRoomNodeGraph.roomNodeToDrawLineFrom.rect.center,
                                    m_currentRoomNodeGraph.linePosition, Color.white, null, _connectingLineWidth);
            }
        }

        private void ProcessEvents(Event currentEvent)
        {
            // get node the mouse is over if it's null or not currently being dragged
            if(m_currentRoomNode == null || m_currentRoomNode.isLeftClickDragging == false)
            {
                m_currentRoomNode = IsMouseOverRoomNode(currentEvent);
            }
            // if mouse isn't over a room node or currenly dragging a line from the node then process graph events
            if(m_currentRoomNode == null || m_currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
            {
                ProcessRoomNodeGraphEvents(currentEvent);
            }
            // process node events
            else
            {
                m_currentRoomNode.ProcessEvents(currentEvent);
            }
        }

        /// <summary>
        /// Check to see to room node is over a room node
        /// </summary>
        private RoomNodeSO IsMouseOverRoomNode(Event currentEvent)
        {
            for(int i = m_currentRoomNodeGraph.roomNodeList.Count -1; i >= 0; i--)
            {
                if (m_currentRoomNodeGraph.roomNodeList[i].rect.Contains(currentEvent.mousePosition))
                {
                    return m_currentRoomNodeGraph.roomNodeList[i];
                }
            }

            return null;
        }
        
        private void ProcessRoomNodeGraphEvents(Event currentEvent)
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

                default: break;
            }
        }

        // case 1: mouse down event
        private void ProcessMouseDownEvent(Event currentEvent)
        {
            if(currentEvent.button == 1)
            {
                ShowContextMenu(currentEvent.mousePosition);
            }

            else if(currentEvent.button == 0)
            {
                ClearLineDrag();
                ClearAllSelectedRoomNodes();
            }
        }

        private void ShowContextMenu(Vector2 mousePosition)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Create Room Node"), false, CreateRoomNode, mousePosition);
            menu.ShowAsContext();
        }

        private void CreateRoomNode(object mousePositionObj)
        {
            if(m_currentRoomNodeGraph.roomNodeList.Count == 0)
            {
                // if current nodoe graph empty then add entrace room node first
                CreateRoomNode(new Vector2(200f, 200f), m_roomNodeTypeList.list.Find(x => x.isEntrance));
            }

            CreateRoomNode(mousePositionObj, m_roomNodeTypeList.list.Find(x => x.isNone));
        }

        private void CreateRoomNode(object mousePositionObj, RoomNodeTypeSO roomNodeType)
        {
            Vector2 mousePosition = (Vector2) mousePositionObj;

            // create room node script object asset
            RoomNodeSO roomNode = ScriptableObject.CreateInstance<RoomNodeSO>();

            // add room node to current room node graph room node list
            m_currentRoomNodeGraph.roomNodeList.Add(roomNode);

            // set room node values
            roomNode.Initialize(new Rect(mousePosition, new Vector2(_nodeWidth, _nodeHeight)), m_currentRoomNodeGraph, roomNodeType);

            // add room node to room nod graph scriptable object asset database
            AssetDatabase.AddObjectToAsset(roomNode, m_currentRoomNodeGraph);
            AssetDatabase.SaveAssets();

            // refresh graph node dictionary
            m_currentRoomNodeGraph.OnValidate();
        }

        /// <summary>
        /// Clear selection from all room nodes
        /// </summary>
        private void ClearAllSelectedRoomNodes()
        {
            foreach(RoomNodeSO roomNode in m_currentRoomNodeGraph.roomNodeList)
            {
                if(roomNode.isSelected)
                {
                    roomNode.isSelected = false;
                    GUI.changed = true;
                }
            }
        }

        // case 2: mouse up event
        private void ProcessMouseUpEvent(Event currentEvent)
        {
            if(currentEvent.button == 1 && m_currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
            {
                // check if over a room node
                RoomNodeSO roomNode = IsMouseOverRoomNode(currentEvent);

                if(roomNode != null)
                {
                    // if so set it as a child of the parent room node if it can be added
                    if (m_currentRoomNodeGraph.roomNodeToDrawLineFrom.AddChildRoomNodeIDToRoomNode(roomNode.id))
                    {
                        // set parent ID in child room node
                        roomNode.AddParentRoomNodeIDToRoomNode(m_currentRoomNodeGraph.roomNodeToDrawLineFrom.id);
                    }
                }

                ClearLineDrag();
            }
        }


        // case 3: mouse drag event
        private void ProcessMouseDragEvent(Event currentEvent)
        {
            // right click frag event - drag line
            if(currentEvent.button == 1)
            {
                ProcessRightMouseDragEvent(currentEvent);
            }
        }

        private void ProcessRightMouseDragEvent(Event currentEvent)
        {
            if(m_currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
            {
                DragConnectingLine(currentEvent.delta);
                GUI.changed = true;
            }
        }

        private void DragConnectingLine(Vector2 delta)
        {
            m_currentRoomNodeGraph.linePosition += delta;
        }
        
        // clear line
        private void ClearLineDrag()
        {
            m_currentRoomNodeGraph.roomNodeToDrawLineFrom = null;
            m_currentRoomNodeGraph.linePosition = Vector2.zero;
            GUI.changed = true;
        }

        /// <summary>
        /// Draw connection between room nodes
        /// </summary>
        private void DrawRoomConnection()
        {
            // loop through all room nodes
            foreach(RoomNodeSO roomNode in m_currentRoomNodeGraph.roomNodeList)
            {
                if(roomNode.childRoomNodeIDList.Count > 0)
                {
                    // loop through child room nodes
                    foreach(string childRoomNodeID in roomNode.childRoomNodeIDList)
                    {
                        if (m_currentRoomNodeGraph.roomNodeDictionary.ContainsKey(childRoomNodeID))
                        {
                            // get child room node from dictionary
                            DrawConnectionLine(roomNode, m_currentRoomNodeGraph.roomNodeDictionary[childRoomNodeID]);

                            GUI.changed = true;
                        }
                    }
                }
            }
        }

        ///<summary>
        /// Connection line between the parent room node and child room node
        /// </summary>
        private void DrawConnectionLine(RoomNodeSO parentRoomNode, RoomNodeSO childRoomNode)
        {
            Vector2 startPosition = parentRoomNode.rect.center;
            Vector2 endPosition = childRoomNode.rect.center;

            // calculate midway point
            Vector2 midPosition = (endPosition + startPosition)/2;
            // vector from start to end position of line
            Vector2 direction = endPosition - startPosition;

            // calculate normalized perpendicular positions from the mid point
            Vector2 arrowTailPoint1 = midPosition - new Vector2(-direction.y, direction.x).normalized * _connectingLineArrowSize;
            Vector2 arrowTailPoint2 = midPosition + new Vector2(-direction.y, direction.x).normalized * _connectingLineArrowSize;

            // calculate mid point offset position for arrow head
            Vector2 arrowHeadPoint = midPosition + direction.normalized * _connectingLineArrowSize;

            // draw arrow
            Handles.DrawBezier(arrowHeadPoint, arrowTailPoint1, arrowHeadPoint, arrowTailPoint1, Color.white, null, _connectingLineWidth);
            Handles.DrawBezier(arrowHeadPoint, arrowTailPoint2, arrowHeadPoint, arrowTailPoint2, Color.white, null, _connectingLineWidth);


            // draw line
            Handles.DrawBezier(startPosition, endPosition, startPosition, endPosition, Color.white, null, _connectingLineWidth);

            GUI.changed = true;
        }


        ///<summary>
        /// Draw room nodes in the graph window
        /// </summary>

        private void DrawRoomNodes()
        {
            foreach(RoomNodeSO roomNode in m_currentRoomNodeGraph.roomNodeList)
            {
                if (roomNode.isSelected)
                    roomNode.Draw(m_roomNodeSelectedStyle);
                else
                    roomNode.Draw(m_roomNodeStyle);
            }

            GUI.changed = true;
        }

        private void InspectorSelectionChanged()
        {
            RoomNodeGraphSO roomNodeGraph = Selection.activeObject as RoomNodeGraphSO;
            if(roomNodeGraph != null)
            {
                m_currentRoomNodeGraph = roomNodeGraph;
                GUI.changed = true;
            }
        }

    }

}