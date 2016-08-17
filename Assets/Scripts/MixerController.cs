using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MixerController : MonoBehaviour {

    MixerManager mixerManager;
    string activeSong;

    Transform currentSongDir;
    Transform currentTrackDir;
    Transform instrument;

    List<GameObject> mixingBoard;

    // Use this for initialization
    void Start () {
        mixerManager = GameObject.Find("Manager").GetComponent<MixerManager>();
        mixingBoard = new List<GameObject>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void MuteTrack(string name)
    {
        activeSong = mixerManager.activeSong;
        currentSongDir = transform.Find(activeSong);
        currentTrackDir = currentSongDir.transform.Find(name);
        instrument = currentTrackDir.transform.GetChild(0);

        //muteUI(muteButton, instrument, isMuted);

        // targets the object containing the audio source
        foreach (Transform child in currentTrackDir)
        {
            if (child.GetComponent<AudioSource>())
            {
                AudioSource clip = child.GetComponent<AudioSource>();
                clip.mute = !clip.mute;
                //muteUI(instrument, clip.mute);
                //child.GetComponent<AudioSource>().mute = !;
            }
        }
    }

    //void muteUI(Transform instrument, bool isMuted)
    //{
    //    IsActiveEffect instrumentEffect;

    //    instrumentEffect = instrument.GetComponent<IsActiveEffect>();
    //    if (isMuted)
    //    {
    //        instrumentEffect.RemoveActivePanelState();
    //    }
    //    else
    //    {
    //        instrumentEffect.AddActivePanelState();
    //    }
    //    updateMixerStatus(instrument, "mute");
    //}

    void updateMixerStatus(Transform instrument, string action)
    {

        GameObject[] taggedObjects;
        taggedObjects = GameObject.FindGameObjectsWithTag("Track");

        foreach (GameObject obj in taggedObjects)
        {
            mixingBoard.Add(obj);
        }

        for (int z = 0; z < mixingBoard.Count; z++)
        {
            Debug.Log("mixName" + mixingBoard[z].name);
            AudioSource thisTrack = mixingBoard[z].GetComponent<AudioSource>();
            if (action == "mute")
            {
                if (mixingBoard[z].name == instrument.name)
                {
                    thisTrack.mute = !thisTrack.mute;
                }
            }
            
        }
    }
}
