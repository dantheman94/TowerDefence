version 1.0.1

Support platforms:

PC/Consoles/VR/Mobiles
All effects tested on Oculus Rift CV1 with single and dual mode rendering. 

------------------------------------------------------------------------------------------------------------------------------------------

NOTE:
For correct work on PC in your project scene you need:

1) Download unity free posteffects 
https://assetstore.unity.com/packages/essentials/post-processing-stack-83912
2) Add "PostProcessingBehaviour.cs" on main Camera.
3) Set the "PostEffects" profile. (path "Assets\KriptoFX\Realistic Effects Pack v1\PostEffects.asset")
4) You should turn on "HDR" on main camera for correct posteffects. 
If you have forward rendering path (by default in Unity), you need disable antialiasing "edit->project settings->quality->antialiasing"
or turn of "MSAA" on main camera, because HDR does not works with msaa. If you want to use HDR and MSAA then use "post effect msaa". 

For correct work on MOBILES in your project scene you need:
1) Add script "RFX1_DistortionAndBloom.cs" on main camera. It's allow you to see correct distortion, soft particles and physical bloom 
The mobile bloom posteffect work if mobiles supported HDR textures or supported openGLES 3.0

------------------------------------------------------------------------------------------------------------------------------------------

FPS pack works on mobile / PC / consoles /VR (single and dual pass) with vertexlit / forward / deferred renderer and dx9, dx11, openGL. Unity4 and Unity5 supported.
All effects optimized for mobile and pc. So you can use this effects even on old mobiles. For mobile uses optimized shaders.
 
For scale you need add the script "FPSParticleSystemScaler" on prefab effect and change "scale" property. 
Also, on Unity 5.3+ you can change particle system by tranform scale. 

For creating effect in runtime, just use follow code:

var instanceEffect = Instantiate(Effect, Position, new Quaternion()) as GameObject;

You can use "objects pool" for effects optimizing. Just reactivate effect after time. 

instanceEffect.SetActive(false);
instanceEffect.SetActive(true)

If you have some questions, you can write me to email "kripto289@gmail.com" 