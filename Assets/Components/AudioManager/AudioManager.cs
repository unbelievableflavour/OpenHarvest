using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public AudioSource effectsSource;
	public AudioSource musicSource;

	public AudioClip shovel;
	public AudioClip hoe;
	public AudioClip seed;
	public AudioClip buy;
	public AudioClip sell;

	// Random pitch adjustment range.
	public float LowPitchRange = .95f;
	public float HighPitchRange = 1.05f;

	// Singleton instance.
	public static AudioManager Instance = null;

	// Initialize the singleton instance.
	private void Awake()
	{
		// If there is not already an instance of SoundManager, set it to this.
		if (Instance == null)
		{
			Instance = this;
		}
		//If an instance already exists, destroy whatever this object is to enforce the singleton.
		else if (Instance != this)
		{
			Destroy(gameObject);
		}

		//Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
		DontDestroyOnLoad(gameObject);
	}

	// Play a single clip through the sound effects source.
	public void Play(AudioClip clip)
	{
		effectsSource.clip = clip;
		effectsSource.Play();
	}

	public void PlayClip(string clip)
	{
		float randomPitch = Random.Range(LowPitchRange, HighPitchRange);
		effectsSource.pitch = randomPitch;

		if (clip == "shovel")
		{
			effectsSource.clip = shovel;
			effectsSource.Play();
			return;
		}
		if (clip == "hoe") {
			effectsSource.clip = hoe;
			effectsSource.Play();
			return;
		}
			
		if (clip == "seed")
		{
			effectsSource.clip = seed;
			effectsSource.Play();
			return;
		}
		if (clip == "buy")
		{
			effectsSource.clip = buy;
			effectsSource.Play();
			return;
		}

		if (clip == "sell")
		{
			effectsSource.clip = sell;
			effectsSource.Play();
			return;
		}
	}

	// Play a random clip from an array, and randomize the pitch slightly.
	public void RandomSoundEffect(params AudioClip[] clips)
	{
		int randomIndex = Random.Range(0, clips.Length);
		float randomPitch = Random.Range(LowPitchRange, HighPitchRange);

		effectsSource.pitch = randomPitch;
		effectsSource.clip = clips[randomIndex];
		effectsSource.Play();
	}

	public void PlayMusic(AudioClip clip)
	{
		musicSource.clip = clip;
		musicSource.Play();
	}

	public void ChangeMusicVolume(float volume)
	{
		musicSource.volume = volume;
	}

	public float GetMusicVolume()
	{
		return musicSource.volume;
	}

	public void StopMusic()
	{
		musicSource.Stop();
	}


	//AUDIO FADE FUNCTION, maybe add it back later
	//public IEnumerator StartFade(AudioSource audioSource, float duration, float targetVolume)
	//{
	//	float currentTime = 0;
	//	float start = audioSource.volume;
	//
	//	while (currentTime < duration)
	//{
	//	currentTime += Time.deltaTime;
	//		audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
	//			yield return null;
	//		}
	//		yield break;
	//}
}
