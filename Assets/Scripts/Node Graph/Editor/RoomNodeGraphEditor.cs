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
        private static RoomNodeGraphSO m_currentRoomNodeGraph;
        private RoomNodeTypeListSO m_roomNodeTypeList;

        // Node layout values
        private const float _nodeWidth = 160f;
        private const float _nodeHeight = 75;
        private const int _nodePadding = 25;
        private const int _nodeBorder = 12;


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
            //Define node layout style
            m_roomNodeStyle = new GUIStyle();
            m_roomNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
            m_roomNodeStyle.normal.textColor = Color.white;
            m_roomNodeStyle.padding = new RectOffset(_nodePadding, _nodePadding, _nodePadding, _nodePadding);
            m_roomNodeStyle.border = new RectOffset(_nodeBorder, _nodeBorder, _nodeBorder, _nodeBorder);

            // Load room node types
            m_roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
        }

        ///<summary>
        ///    Draw Editor GUI
        ///</summary>
        private void OnGUI()
        {
            //if a scriptabke object of type RoomNodeGraphSO has been selected then process
            if(m_currentRoomNodeGraph != null)
            {
                // Process Events
                ProcessEvents(Event.current);

                // Draw Room Nodes
                DrawRoomNodes();
            }

            if(GUI.changed)
            {
                Repaint();
            }

        }

        private void ProcessEvents(Event currentEvent)
        {
            ProcessRoomNodeGraphEvents(currentEvent);
        }
        
        private void ProcessRoomNodeGraphEvents(Event currentEvent)
        {
            switch(currentEvent.type)
            {
                case EventType.MouseDown:
                    ProcessMouseDownEvent(currentEvent);
                    break;

                default: break;
            }
        }

        private void ProcessMouseDownEvent(Event currentEvent)
        {
            if(currentEvent.button == 1)
            {
                ShowContextMenu(currentEvent.mousePosition);
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
        }

        ///<summary>
        /// Draw room nodes in the graph window
        /// </summary>
        
        private void DrawRoomNodes()
        {
            foreach(RoomNodeSO roomNode in m_currentRoomNodeGraph.roomNodeList)
            {
                roomNode.Draw(m_roomNodeStyle);
            }

            GUI.changed = true;
        }

    }

}