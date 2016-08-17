using UnityEngine;
using System.Collections;

public class EffectToggle : MonoBehaviour {

    GameObject SongDir;
    GameObject[] effects;

    bool effectsGrabbed = false;
    bool isEffectActive = true;

    // called when MixerManager has finished loading tracks and creating prefabs
    public void GrabAllEffectsObjects()
    {
        SongDir = GameObject.Find("Evolution");
        effects = GameObject.FindGameObjectsWithTag("Effect");

        effectsGrabbed = true;
        ToggleEffects();
    }

    public void ToggleEffects()
    {
        if (effectsGrabbed)
        {
            isEffectActive = !isEffectActive;
            foreach (GameObject go in effects)
            {
                go.SetActive(isEffectActive);
            }
        }
    }
}
