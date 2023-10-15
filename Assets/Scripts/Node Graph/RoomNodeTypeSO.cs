using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Multiplayer.NodeGraph
{
    [CreateAssetMenu(fileName = "RoomNodeType_", menuName = "Scriptable Objects/Dungeon/Room Node Type")]
    public class RoomNodeTypeSO : ScriptableObject
    {
        public string roomNodeTypeName;

        #region Header
        [Header("Only flag the RoomNodeTypes that should be visibl in the editor")]
        #endregion Header
        public bool displayInNodeGraphEditor = true;
        #region Header
        [Header("One Type Should Be A(n) Corridor/NS/EW/\n      Entrance/Boss Room/None")]
        #endregion Header
        public bool isCorridor;
        public bool isCorridorNS;
        public bool isCorridorEW;
        public bool isEntrance;
        public bool isBossRoom;
        public bool isNone;

        #region Validation
#if UNITY_EDITOR
        private void OnValidate()
        {
            HelperUtilities.ValidateCheckEmptyString(this, nameof(roomNodeTypeName), roomNodeTypeName);
        }
#endif
        #endregion
    }
}
