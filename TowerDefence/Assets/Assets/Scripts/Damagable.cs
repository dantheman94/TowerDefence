using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//******************************
//
//  Created by: Daniel Marton
//
//******************************

public class Damagable : MonoBehaviour {

    //***************************************************************
    // INSPECTOR

    [Space]
    [Header("-----------------------------------")]
    [Header("DEFAULT")]
    public MeshFilter _MeshFilter;
    public Mesh _OkayMesh;
    
    [Space]
    [Header("-----------------------------------")]
    [Header("MINOR DAMAGE")]
    public float _DamagedMinorThreshold = 70f;
    public Mesh _DamagedMinorMesh;
    public List<ParticleSystem> _DamagedMinorParticleSystems;
    public List<Transform> _DamagedMinorParticleTransforms;

    [Space]
    [Header("-----------------------------------")]
    [Header("MAJOR DAMAGE")]
    public float _DamagedMajorThreshold = 30f;
    public Mesh _DamagedMajorMesh;
    public List<ParticleSystem> _DamagedMajorParticleSystems;
    public List<Transform> _DamagedMajorParticleTransforms;

    [Space]
    [Header("-----------------------------------")]
    [Header("DESTROYED")]
    public Mesh _DestroyedMesh;
    public List<ParticleSystem> _DestroyedParticleSystems;
    public List<Transform> _DestroyedParticleTransforms;

    //***************************************************************
    // VARIABLES

    public enum EDamageState { Okay, DamagedMinor, DamagedMajor, Destroyed }

    private float _Health = 100;
    private bool _IsAlive = true;
    private EDamageState _DamageState = EDamageState.Okay;

    //***************************************************************
    // FUNCTIONS

    private void Start() {
        
    }

    private void Update() {

        // Update alive state
        _IsAlive = _Health > 0f;

        // Update damaged state
        if      (_Health > _DamagedMinorThreshold)                                      { _DamageState = EDamageState.Okay; }
        else if (_Health <= _DamagedMinorThreshold && _Health > _DamagedMajorThreshold) { _DamageState = EDamageState.DamagedMinor; }
        else if (_Health <= _DamagedMajorThreshold && _Health > 0f)                     { _DamageState = EDamageState.DamagedMajor; }
        else /* (_Health <= 0f) */                                                      { _DamageState = EDamageState.Destroyed; }

        // Update damage state visuals
        switch (_DamageState) {

            // Okay
            case EDamageState.Okay: {

                    // Update mesh
                    if (_OkayMesh) { _MeshFilter.mesh = _OkayMesh; }
                    break;
                }
            
            // Minor damage
            case EDamageState.DamagedMinor: {

                    // Update mesh
                    if (_DamagedMinorMesh) { _MeshFilter.mesh = _DamagedMinorMesh; }
                    
                    // Play particle effects
                    if (_DamagedMinorParticleSystems.Count > 0 && _DamagedMinorParticleTransforms.Count > 0) {

                        int i = 0;
                        foreach(var particleSystem in _DamagedMinorParticleSystems) {

                            Instantiate(particleSystem, _DamagedMinorParticleTransforms[i]).Play();
                            ++i;
                        }
                    }
                    break;
                }

            // Major damage
            case EDamageState.DamagedMajor: {

                    // Update mesh
                    if (_DestroyedMesh) { _MeshFilter.mesh = _DestroyedMesh; }

                    // Play particle effects
                    if (_DamagedMajorParticleSystems.Count > 0 && _DamagedMajorParticleTransforms.Count > 0) {

                        int i = 0;
                        foreach (var particleSystem in _DamagedMajorParticleSystems) {

                            Instantiate(particleSystem, _DamagedMajorParticleTransforms[i]).Play();
                            ++i;
                        }
                    }
                    break;
                }

            // Destroyed
            case EDamageState.Destroyed: {

                    // Update mesh
                    if (_DamagedMajorMesh) { _MeshFilter.mesh = _DamagedMajorMesh; }

                    // Play particle effects
                    if (_DestroyedParticleSystems.Count > 0 && _DestroyedParticleTransforms.Count > 0) {

                        int i = 0;
                        foreach (var particleSystem in _DestroyedParticleSystems) {

                            Instantiate(particleSystem, _DestroyedParticleTransforms[i]).Play();
                            ++i;
                        }
                    }
                    break;
                }

            default: break;
        }
    }

    public void Damage(float damage) { _Health -= damage; }

}