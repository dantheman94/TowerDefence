using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 07/11/2018
//
//******************************

public class UnitDialogue : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" SOUNDS")]
    [Space]
    public List<string> OnSelectSounds;
    public List<string> OnSeekSounds;
    public List<string> OnAttackSounds;
    public List<string> OnDeathSounds;
    public List<string> OnSpawnSounds;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private Unit _UnitAttached = null;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when the object is created.
    /// </summary>
    private void Start() {

        // Get component reference
        _UnitAttached = GetComponent<Unit>();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  A coroutine that waits a few seconds before despawning the sound gameobject reference
    /// </summary>
    /// <returns></returns>
    private IEnumerator DestroySound(AudioSource sound, float delayTime) {

        // Delay
        float normalizedTime = 0f;
        while (normalizedTime <= 1f) {

            normalizedTime += Time.deltaTime / delayTime;
            yield return null;
        }

        // Despawn
        ObjectPooling.Despawn(sound.gameObject);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Plays a random sound from the pool list
    /// </summary>
    public void PlaySelectSound() {

        // If theres no current dialogue playing
        if (!SoundManager.Instance._DialogueIsPlaying) {

            // Play random sound
            int i = Random.Range(0, OnSelectSounds.Count);
            AudioSource sound = SoundManager.Instance.PlaySound(OnSelectSounds[i], 1f, 1f);
            SoundManager.Instance.SetCurrentDialogue(sound);

            // Despawn sound
            StartCoroutine(DestroySound(sound, sound.clip.length));
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Plays a random sound from the pool list
    /// </summary>
    public void PlaySeekSound() {

        // If theres no current dialogue playing
        if (!SoundManager.Instance._DialogueIsPlaying) {

            // Play random sound
            int i = Random.Range(0, OnSeekSounds.Count);
            AudioSource sound = SoundManager.Instance.PlaySound(OnSeekSounds[i], 1f, 1f);
            SoundManager.Instance.SetCurrentDialogue(sound);

            // Despawn sound
            StartCoroutine(DestroySound(sound, sound.clip.length));
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Plays a random sound from the pool list
    /// </summary>
    public void PlayAttackSound() {

        // If theres no current dialogue playing
        if (!SoundManager.Instance._DialogueIsPlaying) {

            // Play random sound
            int i = Random.Range(0, OnAttackSounds.Count);
            AudioSource sound = SoundManager.Instance.PlaySound(OnAttackSounds[i], 1f, 1f);
            SoundManager.Instance.SetCurrentDialogue(sound);

            // Despawn sound
            StartCoroutine(DestroySound(sound, sound.clip.length));
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Plays a random sound from the pool list
    /// </summary>
    public void PlayDeathSound() {

        // If theres no current dialogue playing
        if (!SoundManager.Instance._DialogueIsPlaying) {

            // Play random sound
            int i = Random.Range(0, OnDeathSounds.Count);
            AudioSource sound = SoundManager.Instance.PlaySound(OnDeathSounds[i], 1f, 1f);
            SoundManager.Instance.SetCurrentDialogue(sound);

            // Despawn sound
            StartCoroutine(DestroySound(sound, sound.clip.length));
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Plays a random sound from the pool list
    /// </summary>
    public void PlaySpawnSound() {

        // If theres no current dialogue playing
        if (!SoundManager.Instance._DialogueIsPlaying) {

            // Play random sound
            int i = Random.Range(0, OnDeathSounds.Count);
            AudioSource sound = SoundManager.Instance.PlaySound(OnSpawnSounds[i], 1f, 1f);
            SoundManager.Instance.SetCurrentDialogue(sound);

            // Despawn sound
            StartCoroutine(DestroySound(sound, sound.clip.length));
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
