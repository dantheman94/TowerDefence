using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Angus Secomb
//
//  Last edited by: Angus Secomb
//  Last edited on: 24/07/2018
//
//******************************


public class ResourceNode : WorldObject {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header("RESOURCE NODE PROPERTIES")]
    [Space]
    public float CaptureTime = 30.0f;
    [Tooltip("How long it takes for the node to uncapture.")]
    public float CaptureDecreaseTime = 45.0f;
    [Tooltip("How many resources are gained each tick.")]
    public int GenerationPerTick = 3;
    [Tooltip("How long each resource tick takes.")]
    public float GenerationTickRate = 0.3f;
    [Tooltip("How long it takes to capture the node if enemy has captured.")]
    public float EnemyGenerationTime = 45.0f;
    [Tooltip("How long it takes to earn a capture point")]
    public float CaptureTickRate = 0.6f;
    [Tooltip("The rate at which it decreases")]
    public float CaptureDecreaseRate = 0.7f;
    
    [Tooltip("What type of resource this node generates.")]
    public Generator.eResourceType resourceType;

    public GameObject BottomPad;
    public Color BaseColor;


    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private bool _IsCapturing = false;
    private bool _IsCaptured = false;
    private float _TickTimer = 0.0f;
    private float _ResourceMultiplier = 1.0f;

    private float _CaptureProgressMax = 100;
    private float _CaptureProgress = 0;
    private float _CaptureTickTimer = 0.0f;
    private float _CaptureDecreaseTimer = 0.0f;
    private NodeCaptureCounter _NodeWidget = null;
    private WorldObject _ObjectReference = null;
    private GameManager.Team CapturingTeam;
    private GameManager.Team CapturedTeam;


    private bool _WidgetOn = false;
    
    public float GetCaptureMax() { return _CaptureProgressMax; }
    public float GetCaptureProg() { return _CaptureProgress; }


    // Update is called once per frame
    protected override void Update() {
       
        if(!_Player)
        {
            _Player = GameManager.Instance.Players[0];
        }
        if(_CaptureProgress < 0)
        {
            _CaptureProgress = 0;
        }
        if(_Player && !_WidgetOn)
        {
            GameObject progressWidget = ObjectPooling.Spawn(GameManager.Instance.CaptureProgressPanel.gameObject);
            _NodeWidget = progressWidget.GetComponent<NodeCaptureCounter>();
            _NodeWidget.setObjectAttached(this);
            _NodeWidget.setCameraAttached(_Player.PlayerCamera);
            _NodeWidget.setResourceNode(this);
            _NodeWidget.Offsetting.y += 15;
            progressWidget.gameObject.SetActive(true);
            progressWidget.transform.SetParent(GameManager.Instance.WorldSpaceCanvas.transform, false);
            _WidgetOn = true;
        }
        if (_IsCaptured)
        {
            if (CapturedTeam == GameManager.Team.Attacking)
            {
                BottomPad.GetComponent<Renderer>().material.color = WaveManager.Instance.AttackingTeamColour;
            }
            else
            {
                BottomPad.GetComponent<Renderer>().material.color = _Player.TeamColor;
            }
        }
        else
        {
            BottomPad.GetComponent<Renderer>().material.color = BaseColor;
        }

        CaptureProcess();
        GenerateResources();
        _ResourceMultiplier = (_CaptureProgress / _CaptureProgressMax);
    }
     
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Unit"))
        {
        
            StartCapture();

            if (_ObjectReference != null)
            {

                if (_ObjectReference.gameObject != other.gameObject)
                {
                    _ObjectReference = other.GetComponent<WorldObject>();
                    if (_ObjectReference != null)
                    {
                        CapturingTeam = _ObjectReference.Team;
                    }
                }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Unit"))
        {
            if (_ObjectReference != null)
            {

                if (_ObjectReference.gameObject != other.gameObject)
                {
                    _ObjectReference = other.GetComponent<WorldObject>();
                    if (_ObjectReference != null)
                    {
                        CapturingTeam = _ObjectReference.Team;
                    }
                }
                if (!_IsCapturing)
                {
                    StartCapture();
                }

                if (_ObjectReference != null)
                {
                    CapturingTeam = _ObjectReference.Team;
                }
            }
        }
       
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Unit"))
        {
            CancelCapture();
        }
 
    }

    /// <summary>
    /// Starts captures process.
    /// </summary>
    private void StartCapture()
    {
        _IsCapturing = true;
    }

    /// <summary>
    /// Cancels capture process.
    /// </summary>
    private void CancelCapture()
    {
        _IsCapturing = false;
    }

    /// <summary>
    /// Checks if the conditions to capture have been met.
    /// </summary>
    private void CaptureProcess()
    {
     
        if(_IsCapturing)
        {
            if(_CaptureTickTimer < CaptureTickRate)
            {
                _CaptureTickTimer += Time.deltaTime;
            }
            else
            {
                _CaptureTickTimer = 0;
                if(_CaptureProgress < _CaptureProgressMax)
                {
                    _CaptureProgress++;
                }
            }

            if(_CaptureProgress >= _CaptureProgressMax)
            {
                _IsCaptured = true;
                if(_ObjectReference != null)
                {
                    CapturedTeam = _ObjectReference.Team;
                }
            }

        }

        if(_CaptureProgress > _CaptureProgressMax)
        {
            _CaptureProgress = _CaptureProgressMax;
        }

        if(!_IsCapturing)
        {
            if(_CaptureDecreaseTimer < CaptureDecreaseRate)
            {
                _CaptureDecreaseTimer += Time.deltaTime;
            }
            else
            {
                _CaptureDecreaseTimer = 0;
                if(_CaptureProgress <= _CaptureProgressMax && 
                   _CaptureProgress > 0)
                {
                    _CaptureProgress--;
                }
            }

            if(_CaptureProgress <= 0)
            {
                _IsCaptured = false;
            }       
        }
    }

    /// <summary>
    /// Generates resources if node is captured.
    /// </summary>
    private void GenerateResources()
    {
        if(_IsCaptured)
        {
            if (_Player)
            {
                if (_TickTimer < GenerationTickRate)
                {
                    _TickTimer += Time.deltaTime;
                }
                else
                {
                    _TickTimer = 0.0f;
                    switch (resourceType)
                    {
                        case Generator.eResourceType.Power:
                            _Player.PowerCount += (int)(GenerationPerTick * _ResourceMultiplier);
                            break;
                        case Generator.eResourceType.Supplies:
                            _Player.SuppliesCount += (int)(GenerationPerTick * _ResourceMultiplier);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}
