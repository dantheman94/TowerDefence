using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Angus Secomb
//
//  Last edited by: Angus Secomb
//  Last edited on: 23/07/2018
//
//******************************


public class ResourceNode : WorldObject {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************
    [Header("RESOURCE NODE PROPERTIES")]
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
    public ResourceType resourceType;
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
    
    public enum ResourceType { POWER, SUPPLY};

    // Use this for initialization
    void Start () {

	}

    private void FixedUpdate()
    {
        Debug.Log("Resource multiplier: " + _ResourceMultiplier);
        Debug.Log("Capture progress: " + _CaptureProgress + " / " + _CaptureProgressMax);
    }

    // Update is called once per frame
    void Update () {

        if(!_Player)
        {
            _Player = GameManager.Instance.Players[0];
        }
        CaptureProcess();
        GenerateResources();
        _ResourceMultiplier = (_CaptureProgress / _CaptureProgressMax);
    }
     
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Unit")
        {
            StartCapture();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Unit")
        {
            CancelCapture();
        }
    }

    private void StartCapture()
    {
        _IsCapturing = true;
    }

    private void CancelCapture()
    {
        _IsCapturing = false;
    }

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
                   _CaptureProgress >= 0)
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
                        case ResourceType.POWER:
                            _Player.PowerCount += (int)(GenerationPerTick * _ResourceMultiplier);
                            break;
                        case ResourceType.SUPPLY:
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
