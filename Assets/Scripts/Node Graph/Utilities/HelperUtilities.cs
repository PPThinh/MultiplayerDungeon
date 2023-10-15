using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Multiplayer
{
    public static class HelperUtilities
    {
        ///<summary>
        /// Empty string debug check
        /// </summary>
        public static bool ValidateCheckEmptyString(Object thisObj, string fileName, string strToCheck)
        {
            if( strToCheck == "")
            {
                Debug.Log(fileName + " is empty and must contain a value in object " + thisObj.name.ToString());
                return true;
            }
            return false;
        }

        ///<summary>
        /// List empty or cotains null value check - return true if there is an error
        /// </summary>
        
        public static bool ValidateCheckEnumerableValues(Object thisObj, string fileName, IEnumerable enumerableObjToCheck)
        {
            bool error = false;
            int count = 0;

            foreach( var item in enumerableObjToCheck )
            {
                if(item == null)
                {
                    Debug.Log(fileName + " has null values in object " + thisObj.name.ToString());
                    error = true;
                }
                else
                {
                    count++;
                }
            }

            if(count == 0)
            {
                Debug.Log(fileName + " has no values in object " + thisObj.name.ToString());
                error = true;
            }

            return error;
        }
    }
}
