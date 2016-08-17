using UnityEngine;
using System.Collections;

public class MuteAction : MonoBehaviour {

    GameObject mixer;
    MixerController mixerController;

    string instrumentName;
    string buttonName;

    void Start()
    {
    }

    void OnSelect()
    {
        mixer = GameObject.Find("Mixer");
        mixerController = mixer.GetComponent<MixerController>();
        instrumentName = transform.parent.name;
        mixerController.MuteTrack(instrumentName);
    }
}
