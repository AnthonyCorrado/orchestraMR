using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity;

public class MixerManager : MonoBehaviour {

    public string activeSong;

    SongManager songManager;
    Track track;
    GameObject mixer;

    public List<SongManager.Song> allSongs;
    public List<Track> allTracks;
    public List<Track> activeSongTracks;
    public List<Track> allSoloed;

    public List<Track> toBeMuted;
    public List<Track> toBeSoloed;

    public Vector3 instrumentScale = new Vector3(1, 1, 1);

    Transform muteButton;
    Transform soloButton;
    //IsActiveEffect activeMuteEffect;
    //IsActiveEffect activeSoloEffect;
    //IsActiveEffect instrumentEffect;

    Transform currentSongDir;
    Transform currentTrackDir;
    Transform instrument;

    UnityEngine.Object trackPrefab;
    Vector3 cameraPos;

    string trackName;
    string action;

    bool effectsActive;

    // Use this for initialization
    void Start() {
        songManager = GetComponent<SongManager>();
        allTracks = new List<Track>();
        activeSongTracks = new List<Track>();
        allSoloed = new List<Track>();
        toBeMuted = new List<Track>();
        toBeSoloed = new List<Track>();

        mixer = GameObject.Find("Mixer");

        allSongs = songManager.getAllSongs();
        initializeSongs(allSongs);
        effectsActive = true;
    }

    // Update is called once per frame
    void Update() {

    }

    void initializeSongs(List<SongManager.Song> songs)
    {
        for (int i = 0; i < songs.Count; i++)
        {
            instantiateTracks(songs[i]);
        }
    }

    void instantiateTracks(SongManager.Song song)
    {
        GameObject songFolder = new GameObject(song.name);
        songFolder.transform.parent = mixer.transform;

        UnityEngine.Object instrumentPanel = Resources.Load("Prefabs/InstrumentPanel");

        for (int i = 0; i < song.tracks.Count; i++)
        {
            string prefabName = song.tracks[i].name;

            // plots prefab tracks equally spaced around the user
            int angle = i * (360 / song.tracks.Count);
            Vector3 plotPos = Circle(cameraPos, 1.5f, angle);
            Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, cameraPos - plotPos);

            // creates prefab based on instrument/track type or family
            string prefabType = song.tracks[i].type;
            trackPrefab = Resources.Load("Prefabs/" + prefabType);

            // recalibrates rotation to ensure first prefab is facing center
            rotation.x = 0.0f;

            // instantiates a track prefab
            var track = Instantiate(trackPrefab, plotPos, rotation);
            track.name = prefabName;

            // sets this track prefab to child of Mixer/:songName
            GameObject currentTrack = GameObject.Find(prefabName);
            currentTrack.transform.parent = songFolder.transform;
            currentTrack.transform.localScale = instrumentScale;
            AudioClip currentClip;

            // dynamically sets current tracks audio clip
            currentClip = Resources.Load<AudioClip>("Audio/" + song.tracks[i].clipName);
            initTrackAudio(prefabName, currentClip, song.tracks[i]);

            //foreach (Transform child in currentTrack.transform)
            //{
            //    if (child.GetComponent<Collider>())
            //    {
            //        objectWidth = child.GetComponent<Collider>().bounds.size.x;
            //    }
            //}

            // adds instrumentPanel to newly instantiated object
            float adjustedPosX;

            //GameObject instroPanel = Instantiate(instrumentPanel, plotPos, rotation) as GameObject;
            //Vector3 adjustedPos;

            //instroPanel.name = "InstrumentPanel";
            //instroPanel.transform.parent = currentTrack.transform;

            // returns corrected position.x of instroPanel based on instrument
            //adjustedPosX = adjustPosition(objectWidth);

            // Drums are natually rotatated 180 - instroPanel needs x and z axis corrected manually 
            //if (trackPrefab.name == "drums" || trackPrefab.name == "brass")
            //{
            //    adjustedPos = new Vector3(-1 * adjustedPosX, 0.15f, -0.2f);
            //    instroPanel.transform.localRotation = Quaternion.Euler(0, 180, 0);
            //}
            //else
            //{
            //    adjustedPos = new Vector3(adjustedPosX, 0.15f, 0.2f);
            //}
            //instroPanel.transform.localPosition = adjustedPos;

            // adds current track to allTracks for mixing board
            allTracks.Add(new Track(song.tracks[i].name, song.tracks[i].type, song.tracks[i].volume, song.tracks[i].isMuted, song.tracks[i].isSoloed, song.tracks[i].clipName, song.tracks[i].audioSource, song.tracks[i].songName));

            if (song.tracks[i].isSoloed && song.isActive)
            {
                allSoloed.Add(song.tracks[i]);
            }
        }
        // sets GameObject active/inactive state based on SongManager  
        songFolder.SetActive(song.isActive);

        if (song.isActive)
        {
            Debug.Log("Manager ready");
            activeSong = song.name;
        }

        if (song.isActive)
        {
            StartCoroutine(getAudibleStatus());
        }
        setActiveSong();
        mixer.GetComponent<MeshToggle>().GrabAllRenderers();
        mixer.GetComponent<EffectToggle>().GrabAllEffectsObjects();
    }

    private void initTrackAudio(string name, AudioClip audioClip, Track track)
    {
        AudioSource audioSource = GameObject.Find(name).transform.GetChild(0).GetComponent<AudioSource>();
        track.audioSource = audioSource;

        // initializes audioClip of instantiated prefab
        track.audioSource.clip = audioClip;
        track.audioSource.Play();

    }

    private void ToggleSpecialEffects()
    {
        foreach (Track cTrack in allTracks)
        {
            GameObject go = GameObject.Find(cTrack.name);
            if (go.transform.childCount > 1)
            {
                Transform tf = go.transform.GetChild(1);
                ParticleSystem effect = tf.GetComponent<ParticleSystem>();

                if (effectsActive)
                {
                    effect.Stop();
                }
                else
                {
                    effect.Play();
                }
            }
        }
        effectsActive = !effectsActive;
    }

    IEnumerator getAudibleStatus()
    {
        toBeMuted.Clear();
        toBeSoloed.Clear();

        foreach (Track track in activeSongTracks)
        {
            if (track.isSoloed)
            {
                toBeSoloed.Add(track);
            }
            else
            {
                toBeMuted.Add(track);
            }
        }

        yield return null;
        updateMixingBoard();
    }

    // sets current song as the active list for mute, sole, and UI iterations
    void setActiveSong()
    {
        foreach (SongManager.Song song in allSongs)
        {
            if (song.name == activeSong)
            {
                activeSongTracks = song.tracks;
            }
        }

        // MixerController relies on startup song to be loaded first
        mixer.AddComponent<MixerController>();
    }

    public void ResetInstrumentPositions()
    {
        Transform camPos = Camera.main.transform;
        float posCounter = -1.5f;

        posCounter += 0.5f;
    
        for (int i = 0; i < allTracks.Count; i++)
        {
            GameObject go;
            go = GameObject.Find(allTracks[i].name);

            int angle = i * (180 / allTracks.Count);
            angle = angle + (int)camPos.eulerAngles.y - 90;
            Vector3 plotPos = Circle(camPos.position, 1.25f, angle);

            StartCoroutine(SmoothTransform(go, plotPos, 4));
            //go.transform.position = plotPos;
            go.transform.LookAt(go.transform.position + Camera.main.transform.rotation * Vector3.back,
                          Camera.main.transform.rotation * Vector3.up);

            //Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, cameraPos - plotPos);

            posCounter += 0.5f;
        }
    }

    IEnumerator SmoothTransform(GameObject go, Vector3 direction, float speed)
    {
        float elapsedTime = 0;
        
        while (elapsedTime < speed)
        {
            go.transform.position = Vector3.Lerp(go.transform.position, direction, (elapsedTime / speed));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    // iterates over assembled lists to determine proper track state for active song
    void updateMixingBoard()
    {
        if (allSoloed.Count == 0)
        {
            foreach (Track track in activeSongTracks)
            {
                if (track.isMuted)
                {
                    track.audioSource.mute = true;
                }
                else
                {
                    track.audioSource.mute = false;
                }
                updateTrackUI(track);
            }
        }
        else
        {
            foreach (Track track in toBeMuted)
            {
                track.audioSource.mute = true;
                updateTrackUI(track);
            }
            foreach (Track track in toBeSoloed)
            {
                track.audioSource.mute = false;
                updateTrackUI(track);
            }
        }
    }

    // sets visual cues for instruments, Solo button, Mute button based on active song track state
    void updateTrackUI(Track track)
    {
        currentSongDir = mixer.transform.Find(activeSong);
        currentTrackDir = currentSongDir.transform.Find(track.name);

        instrument = currentTrackDir.transform.GetChild(0);

        //muteButton = currentTrackDir.transform.Find("InstrumentPanel/InterfacePanel/MuteSoloPanel/MuteButton/MuteButtonOutline");
        //soloButton = currentTrackDir.transform.Find("InstrumentPanel/InterfacePanel/MuteSoloPanel/SoloButton/SoloButtonOutline");

        //activeMuteEffect = muteButton.GetComponent<IsActiveEffect>();
        //activeSoloEffect = soloButton.GetComponent<IsActiveEffect>();
       // instrumentEffect = instrument.GetComponent<IsActiveEffect>();

        if (track.audioSource.mute)
        {
            //activeMuteEffect.AddActivePanelState();
            //instrumentEffect.RemoveActivePanelState();
        }
        else if (!track.audioSource.mute)
        {
            //activeMuteEffect.RemoveActivePanelState();
            //instrumentEffect.AddActivePanelState();
        }

        if (track.isSoloed)
        {
           // activeSoloEffect.AddActivePanelState();
        }
        else if (!track.isSoloed)
        {
            //activeSoloEffect.RemoveActivePanelState();
        }
    }

    // helper function to determine where to spawn instantiated instruments
    Vector3 Circle(Vector3 center, float radius, int ang)
    {
        float angle = ang;
        Vector3 pos;
        pos.x = center.x + radius * Mathf.Sin(angle * Mathf.Deg2Rad);
        pos.y = center.y;
        pos.z = center.z + radius * Mathf.Cos(angle * Mathf.Deg2Rad);
        return pos;
    }

    // normalized gameObject positions on X axis
    float adjustPosition(float width)
    {
        float positionX;
        if (width < 0.1f)
        {
            positionX = 0.12f;
        }
        else if (width > 0.5f)
        {
            positionX = 0.45f;
        }
        else
        {
            positionX = 0.175f;
        }
        return positionX;
    }
}
