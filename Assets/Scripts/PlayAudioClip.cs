using UnityEngine;

public class PlayAudioClip : MonoBehaviour
{
    public AudioClip audioClip;
    public float offset = 0.0f;
    public bool playOnStart = false;
    public float volume = 1f;

    private void Start()
    {
        if (playOnStart)
        {
            Play();
        }
    }
    public void Play()
    {
        Invoke("PlayWithOffset", offset);
    }

    void PlayWithOffset()
    {
        BNG.VRUtils.Instance.PlaySpatialClipAt(audioClip, transform.position, volume, 1f);
    }
}
    