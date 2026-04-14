using BNG;
using UnityEngine;

public class PetSound : MonoBehaviour
{
    public float pitch = 0f;
    public int minTimeBetweenSound = 3;
    public int maxTimeBetweenSound = 5;
    public AudioClip[] soundsToPlay;

    private float randomNumberOfSeconds = 0f;
    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer < randomNumberOfSeconds)
        {
            return;
        }

        randomNumberOfSeconds = Random.Range(minTimeBetweenSound, maxTimeBetweenSound);
        timer = 0;
        Play();
    }


    public void Play()
    {
        if (soundsToPlay.Length != 0)
        {
            VRUtils.Instance.PlaySpatialClipAt(soundsToPlay[Random.Range(0, soundsToPlay.Length)], transform.position, 1f, 1f, pitch);
        }
    }

    public AudioSource PlaySpatialClipAt(AudioClip clip, Vector3 pos, float volume, float spatialBlend = 1f, float pitch = 0)
    {
        if (clip == null)
        {
            return null;
        }

        GameObject go = new GameObject("SpatialAudio - Temp");
        go.transform.position = pos;

        AudioSource source = go.AddComponent<AudioSource>();
        source.clip = clip;

        // Currently only Oculus Integration supports spatial audio
#if OCULUS_INTEGRATION
            source.spatialize = true;
#endif
        source.pitch = getPitch(pitch);
        source.spatialBlend = spatialBlend;
        source.volume = volume;
        source.Play();

        Destroy(go, clip.length);

        return source;
    }

    float getPitch(float pitch)
    {
        return Time.timeScale + pitch;
    }
}
