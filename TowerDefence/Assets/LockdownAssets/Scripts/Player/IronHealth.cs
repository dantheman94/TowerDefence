using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//-=-=-=-=-=-=-=-=-=-=-=-=
//Created by: Angus Secomb
//Last modified: 22/10/18
//Editor: Angus
//-=-=-=-=-=-=-=-=-=-=-=-=

public class IronHealth : MonoBehaviour {

     
    //         INSPECTOR
    /////////////////////////////////////////////

    public float HealthTimer = 15.0f;



    //        VARIABLES
    //////////////////////////////////////////////

    private Base _DesiredBase;
    private List<Building> _Buildings;
    private bool _OnDestroy = false;

    //        FUNCTIONS
    //////////////////////////////////////////////

    // Update is called once per frame
    void Update () {
        
        HealthTimer -= Time.deltaTime;
        if(HealthTimer < 0)
        {
            if(!_OnDestroy)
            {
                DestroyObject();
            }

        }
	}

    ////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Gets list of buildings from the base.
    /// </summary>
    /// <param name="buildings"></param>
    public void SetBuildings(List<Building> buildings) { _Buildings = buildings; }

    ////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Gets base reference.
    /// </summary>
    /// <param name="a_base"></param>
    public void SetBase(Base a_base) { _DesiredBase = a_base; }

    ////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Destroys the iron health object.
    /// </summary>
    void DestroyObject()
    {
        _OnDestroy = true;
        _DesiredBase.SetCanBeDamaged(true);
        for(int i =0; i < _Buildings.Count; ++i)
        {
            _Buildings[i].SetCanBeDamaged(true);
        }
      
        Destroy(this.gameObject);
    }

    ////////////////////////////////////////////////////////////////////////
}
