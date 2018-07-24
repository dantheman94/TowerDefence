using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//******************************
//
//  Created by: Angus Secomb
//
//  Last edited by: Angus Secomb
//  Last edited on: 24/07/2018
//
//******************************

public class NodeCaptureCounter : MonoBehaviour
{  
     //******************************************************************************************************************************
     //
     //      INSPECTOR
     //
     //******************************************************************************************************************************

     [Space]
     [Header("-----------------------------------")]
     [Header(" OFFSETS")]
     [Space]
     public Vector3 Offsetting = Vector3.zero;

     //******************************************************************************************************************************
     //
     //      VARIABLES
     //
     //******************************************************************************************************************************

     private Camera _CameraAttached = null;
     private WorldObject _WorldObject = null;
     private Text _TextComponent;
     private ResourceNode _ResourceNode = null;
    

     //******************************************************************************************************************************
     //
     //      FUNCTIONS
     //
     //******************************************************************************************************************************

     ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

     /// <summary>
     //  Called when this object is created.
     /// </summary>
     private void Start()
     {

         // Get component references
         _TextComponent = GetComponentInChildren<Text>();
        
     }

     ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

     /// <summary>
     //  Called each frame.
     /// </summary>
     private void Update()
     {

        if (_WorldObject != null && _CameraAttached != null)
        {

            if (_ResourceNode != null)
            {
                _TextComponent.text = _ResourceNode.GetCaptureProg().ToString() + "/" + _ResourceNode.GetCaptureMax().ToString();
            }

            // Set world space position
            Vector3 pos = _WorldObject.transform.position + Offsetting;
            pos.y = pos.y + _WorldObject.GetObjectHeight();
            transform.position = pos;

            // Constantly face the widget towards the camera
            transform.LookAt(2 * transform.position - _CameraAttached.transform.position);
        }
     }

     ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

     /// <summary>
     /// 
     /// </summary>
     /// <param name="obj"></param>
     public void setObjectAttached(WorldObject obj) { _WorldObject = obj; }

     ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

     /// <summary>
     /// 
     /// </summary>
     /// <param name="cam"></param>
     public void setCameraAttached(Camera cam) { _CameraAttached = cam; }

     ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
     
    public void setResourceNode(ResourceNode obj) { _ResourceNode = obj; }
   }