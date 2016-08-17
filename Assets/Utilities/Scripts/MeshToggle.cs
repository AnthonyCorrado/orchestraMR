using UnityEngine;
using System.Collections;

public class MeshToggle : MonoBehaviour
{

    GameObject SongDir;
    Renderer[] rs;

    bool meshGrabbed = false;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // called when MixerManager has finished loading tracks and creating prefabs
    public void GrabAllRenderers()
    {
        SongDir = GameObject.Find("Evolution");
        rs = SongDir.GetComponentsInChildren<Renderer>();
        meshGrabbed = true;
    }

    public void InstrumentDisplayToggle()
    {
        if (meshGrabbed)
        {

            foreach (Renderer rend in rs)
            {
                rend.enabled = !rend.enabled;
            }
        }
    }
}