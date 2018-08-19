using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 1/8/2018
//
//******************************

public class UpgradeBuildingCounter : MonoBehaviour {

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
    private UpgradeTree _Upgrade = null;
    private Text _TextComponent;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when this object is created.
    /// </summary>
    private void Start() {

        // Get component references
        _TextComponent = GetComponentInChildren<Text>();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame.
    /// </summary>
    private void Update() {

        if (_Upgrade != null && _CameraAttached != null) {

            // Only show widget if the upgrade is currently being built
            if (_Upgrade.IsUpgrading()) {

                // Update text to show how much time is remaining in the build
                int time = (int)_Upgrade.UpgradeTimeRemaining();
                string healthString = time.ToString();
                _TextComponent.text = healthString;

                // Set world space position
                Vector3 pos = _Upgrade.GetBuildingAttached().transform.position + Offsetting;
                pos.y = pos.y + _Upgrade.GetBuildingAttached().GetObjectHeight();
                transform.position = pos;

                // Constantly face the widget towards the camera
                transform.LookAt(2 * transform.position - _CameraAttached.transform.position);
            }

            // Destroy prefab instance as we no longer need it anymore
            else { ObjectPooling.Despawn(this.gameObject); }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// 
    /// </summary>
    /// <param name="upgrade"></param>
    public void SetUpgradeAttached(UpgradeTree upgrade) { _Upgrade = upgrade; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cam"></param>
    public void SetCameraAttached(Camera cam) { _CameraAttached = cam; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
