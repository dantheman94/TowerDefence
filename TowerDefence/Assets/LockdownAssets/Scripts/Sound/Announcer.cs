using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 20/11/2018
//
//******************************

public class Announcer : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" SOUNDS")]
    [Space]
    public string A_NewWave = "";
    public string A_WaveComplete = "";
    [Space]
    public string A_SpireAttacked = "";
    public string A_CoreAttacked = "";
    public string A_FriendlyBaseAttacked = "";
    [Space]
    public string A_AllUnits = "";
    public string A_LocalUnits = "";
    [Space]
    public string A_BuildSupplies = "";
    public string A_BuildPower = "";
    public string A_BuildingComplete = "";
    [Space]
    public string A_WinBasesDestroyed = "";
    public string A_LoseBasesDestroyed = "";
    public string A_LoseCoreDestroyed = "";

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

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
    public void PlayNewWaveSound() {

        // Stop any current dialogues that are being played
        if (SoundManager.Instance.GetCurrentDialogue() != null) { SoundManager.Instance.GetCurrentDialogue().Stop(); }

        // Play random sound
        AudioSource sound = SoundManager.Instance.PlaySound(A_NewWave, 1f, 1f);
        SoundManager.Instance.SetCurrentDialogue(sound);

        // Despawn sound
        StartCoroutine(DestroySound(sound, sound.clip.length));
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Plays a random sound from the pool list
    /// </summary>
    public void PlayWaveCompleteSound() {

        // Stop any current dialogues that are being played
        if (SoundManager.Instance.GetCurrentDialogue() != null) { SoundManager.Instance.GetCurrentDialogue().Stop(); }

        // Play random sound
        AudioSource sound = SoundManager.Instance.PlaySound(A_WaveComplete, 1f, 1f);
        SoundManager.Instance.SetCurrentDialogue(sound);

        // Despawn sound
        StartCoroutine(DestroySound(sound, sound.clip.length));
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Plays a random sound from the pool list
    /// </summary>
    public void PlaySpireAttackedSound() {

        // Stop any current dialogues that are being played
        if (SoundManager.Instance.GetCurrentDialogue() != null) { SoundManager.Instance.GetCurrentDialogue().Stop(); }

        // Play random sound
        AudioSource sound = SoundManager.Instance.PlaySound(A_SpireAttacked, 1f, 1f);
        SoundManager.Instance.SetCurrentDialogue(sound);

        // Despawn sound
        StartCoroutine(DestroySound(sound, sound.clip.length));
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Plays a random sound from the pool list
    /// </summary>
    public void PlayCoreAttackedSound() {

        // Stop any current dialogues that are being played
        if (SoundManager.Instance.GetCurrentDialogue() != null) { SoundManager.Instance.GetCurrentDialogue().Stop(); }

        // Play random sound
        AudioSource sound = SoundManager.Instance.PlaySound(A_CoreAttacked, 1f, 1f);
        SoundManager.Instance.SetCurrentDialogue(sound);

        // Despawn sound
        StartCoroutine(DestroySound(sound, sound.clip.length));
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Plays a random sound from the pool list
    /// </summary>
    public void PlayBaseAttackedSound() {

        // Stop any current dialogues that are being played
        if (SoundManager.Instance.GetCurrentDialogue() != null) { SoundManager.Instance.GetCurrentDialogue().Stop(); }

        // Play random sound
        AudioSource sound = SoundManager.Instance.PlaySound(A_FriendlyBaseAttacked, 1f, 1f);
        SoundManager.Instance.SetCurrentDialogue(sound);

        // Despawn sound
        StartCoroutine(DestroySound(sound, sound.clip.length));
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Plays a random sound from the pool list
    /// </summary>
    public void PlayBuildSuppliesSound() {

        // Stop any current dialogues that are being played
        if (SoundManager.Instance.GetCurrentDialogue() != null) { SoundManager.Instance.GetCurrentDialogue().Stop(); }

        // Play random sound
        AudioSource sound = SoundManager.Instance.PlaySound(A_BuildSupplies, 1f, 1f);
        SoundManager.Instance.SetCurrentDialogue(sound);

        // Despawn sound
        StartCoroutine(DestroySound(sound, sound.clip.length));
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Plays a random sound from the pool list
    /// </summary>
    public void PlayBuildPowerSound() {

        // Stop any current dialogues that are being played
        if (SoundManager.Instance.GetCurrentDialogue() != null) { SoundManager.Instance.GetCurrentDialogue().Stop(); }

        // Play random sound
        AudioSource sound = SoundManager.Instance.PlaySound(A_BuildPower, 1f, 1f);
        SoundManager.Instance.SetCurrentDialogue(sound);

        // Despawn sound
        StartCoroutine(DestroySound(sound, sound.clip.length));
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Plays a random sound from the pool list
    /// </summary>
    public void PlayBuildingCompleteSound() {

        // Stop any current dialogues that are being played
        if (SoundManager.Instance.GetCurrentDialogue() != null) { SoundManager.Instance.GetCurrentDialogue().Stop(); }

        // Play random sound
        AudioSource sound = SoundManager.Instance.PlaySound(A_BuildingComplete, 1f, 1f);
        SoundManager.Instance.SetCurrentDialogue(sound);

        // Despawn sound
        StartCoroutine(DestroySound(sound, sound.clip.length));
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Plays a random sound from the pool list
    /// </summary>
    public void PlayAllUnitsSound() {

        // Stop any current dialogues that are being played
        if (SoundManager.Instance.GetCurrentDialogue() != null) { SoundManager.Instance.GetCurrentDialogue().Stop(); }

        // Play random sound
        AudioSource sound = SoundManager.Instance.PlaySound(A_AllUnits, 1f, 1f);
        SoundManager.Instance.SetCurrentDialogue(sound);

        // Despawn sound
        StartCoroutine(DestroySound(sound, sound.clip.length));
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Plays a random sound from the pool list
    /// </summary>
    public void PlayLocalUnitsSound() {

        // Stop any current dialogues that are being played
        if (SoundManager.Instance.GetCurrentDialogue() != null) { SoundManager.Instance.GetCurrentDialogue().Stop(); }

        // Play random sound
        AudioSource sound = SoundManager.Instance.PlaySound(A_LocalUnits, 1f, 1f);
        SoundManager.Instance.SetCurrentDialogue(sound);

        // Despawn sound
        StartCoroutine(DestroySound(sound, sound.clip.length));
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Plays a random sound from the pool list
    /// </summary>
    public void PlayWinBasesDestroyedSound() {

        // Stop any current dialogues that are being played
        if (SoundManager.Instance.GetCurrentDialogue() != null) { SoundManager.Instance.GetCurrentDialogue().Stop(); }

        // Play random sound
        AudioSource sound = SoundManager.Instance.PlaySound(A_WinBasesDestroyed, 1f, 1f);
        SoundManager.Instance.SetCurrentDialogue(sound);

        // Despawn sound
        StartCoroutine(DestroySound(sound, sound.clip.length));
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Plays a random sound from the pool list
    /// </summary>
    public void PlayLoseBasesDestroyedSound() {

        // Stop any current dialogues that are being played
        if (SoundManager.Instance.GetCurrentDialogue() != null) { SoundManager.Instance.GetCurrentDialogue().Stop(); }

        // Play random sound
        AudioSource sound = SoundManager.Instance.PlaySound(A_LoseBasesDestroyed, 1f, 1f);
        SoundManager.Instance.SetCurrentDialogue(sound);

        // Despawn sound
        StartCoroutine(DestroySound(sound, sound.clip.length));
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Plays a random sound from the pool list
    /// </summary>
    public void PlayLoseCoreDestroyedSound() {

        // Stop any current dialogues that are being played
        if (SoundManager.Instance.GetCurrentDialogue() != null) { SoundManager.Instance.GetCurrentDialogue().Stop(); }

        // Play random sound
        AudioSource sound = SoundManager.Instance.PlaySound(A_LoseCoreDestroyed, 1f, 1f);
        SoundManager.Instance.SetCurrentDialogue(sound);

        // Despawn sound
        StartCoroutine(DestroySound(sound, sound.clip.length));
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}