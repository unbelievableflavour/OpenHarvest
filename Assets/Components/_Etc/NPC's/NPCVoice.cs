using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[DisallowMultipleComponent]
public class NPCVoice : MonoBehaviour
{
    [Header("Voice bank")]
    [Tooltip("One-shot clips randomly-but-consistently chosen per word.")]
    public AudioClip[] voiceClips;

    [Tooltip("Audio source to play the voice through. If empty, one is added to this GameObject at runtime.")]
    public AudioSource audioSource;

    [Header("Voice variation")]
    [Range(0.1f, 3f)] public float minPitch = 0.9f;
    [Range(0.1f, 3f)] public float maxPitch = 1.15f;
    [Range(0f, 1f)] public float volume = 1f;

    [Tooltip("How much of each clip to play before cutting to the next word. 0-1 of the clip length. Keeps speech snappy.")]
    [Range(0.05f, 1f)] public float clipPlayFraction = 0.55f;

    [Header("Word pacing")]
    [Tooltip("Silence between words in seconds (min).")]
    public float wordGapMin = 0.02f;
    [Tooltip("Silence between words in seconds (max).")]
    public float wordGapMax = 0.06f;

    [Tooltip("Additional pause after sentence-ending punctuation (. ! ?).")]
    public float sentenceEndPause = 0.18f;
    [Tooltip("Additional pause after soft punctuation (, ; :).")]
    public float softPunctuationPause = 0.08f;

    [Tooltip("Minimum sounds played per word.")]
    [Min(1)] public int minSyllablesPerWord = 1;
    [Tooltip("Maximum sounds played per word. Longer words statistically get more sounds.")]
    [Min(1)] public int maxSyllablesPerWord = 2;

    [Header("Mouth")]
    [Tooltip("Shown while NOT talking (e.g. closed mouth). Optional.")]
    public GameObject mouthClosed;
    [Tooltip("Enabled while a sound is playing for a word (e.g. open mouth).")]
    public GameObject mouthOpen;

    public bool IsSpeaking { get; private set; }

    private Coroutine speakRoutine;

    private void Awake()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                audioSource.spatialBlend = 1f;
                audioSource.rolloffMode = AudioRolloffMode.Linear;
                audioSource.minDistance = 1f;
                audioSource.maxDistance = 15f;
            }
        }

        // Make sure the source we ended up with is actually usable. A
        // disabled AudioSource component would otherwise spam
        // "Can not play a disabled audio source" once per syllable.
        if (audioSource != null && !audioSource.enabled)
        {
            audioSource.enabled = true;
        }

        SetMouthOpen(false);
    }

    private void OnDisable()
    {
        StopSpeaking();
    }

    /// <summary>
    /// Start speaking the given text. Cancels any currently-playing speech.
    /// </summary>
    public void Speak(string text)
    {
        StopSpeaking();

        if (string.IsNullOrWhiteSpace(text) || voiceClips == null || voiceClips.Length == 0)
        {
            return;
        }

        if (!isActiveAndEnabled)
        {
            return;
        }

        speakRoutine = StartCoroutine(SpeakRoutine(text));
    }

    public void StopSpeaking()
    {
        if (speakRoutine != null)
        {
            StopCoroutine(speakRoutine);
            speakRoutine = null;
        }

        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        IsSpeaking = false;
        SetMouthOpen(false);
    }

    private IEnumerator SpeakRoutine(string text)
    {
        IsSpeaking = true;
        bool warnedDisabled = false;

        try
        {
            foreach (string rawWord in SplitIntoWords(text))
            {
                string trailingPunct;
                string cleaned = StripAndReturnTrailingPunctuation(rawWord, out trailingPunct);

                if (cleaned.Length > 0)
                {
                    int hash = StableHash(cleaned);
                    int syllables = ChooseSyllableCount(cleaned, hash);

                    for (int s = 0; s < syllables; s++)
                    {
                        int clipIndex = Mathf.Abs(HashMix(hash, s)) % voiceClips.Length;
                        AudioClip clip = voiceClips[clipIndex];
                        if (clip == null)
                        {
                            continue;
                        }

                        if (audioSource == null || !audioSource.isActiveAndEnabled)
                        {
                            if (!warnedDisabled)
                            {
                                Debug.LogWarning(
                                    $"[NPCVoice] AudioSource on '{name}' is unavailable (missing, disabled, or on an inactive GameObject). Skipping speech.",
                                    this
                                );
                                warnedDisabled = true;
                            }
                            yield break;
                        }

                        float t = HashTo01(HashMix(hash, s + 7919));
                        float pitch = Mathf.Lerp(minPitch, maxPitch, t);

                        audioSource.pitch = pitch;
                        audioSource.volume = volume;
                        audioSource.clip = clip;
                        audioSource.Play();
                        SetMouthOpen(true);

                        float pitchAdjustedLength = clip.length / Mathf.Max(0.01f, pitch);
                        float playFor = pitchAdjustedLength * Mathf.Clamp01(clipPlayFraction);
                        yield return new WaitForSeconds(playFor);

                        if (audioSource != null && audioSource.isPlaying)
                        {
                            audioSource.Stop();
                        }
                        SetMouthOpen(false);
                    }
                }

                float gap = Random.Range(wordGapMin, wordGapMax);
                if (ContainsSentenceEnd(trailingPunct)) gap += sentenceEndPause;
                else if (ContainsSoftPunctuation(trailingPunct)) gap += softPunctuationPause;

                if (gap > 0f)
                {
                    yield return new WaitForSeconds(gap);
                }
            }
        }
        finally
        {
            IsSpeaking = false;
            SetMouthOpen(false);
            speakRoutine = null;
        }
    }

    private void SetMouthOpen(bool open)
    {
        if (mouthOpen != null && mouthOpen.activeSelf != open)
        {
            mouthOpen.SetActive(open);
        }
        if (mouthClosed != null && mouthClosed.activeSelf != !open)
        {
            mouthClosed.SetActive(!open);
        }
    }

    private int ChooseSyllableCount(string word, int hash)
    {
        int min = Mathf.Max(1, minSyllablesPerWord);
        int max = Mathf.Max(min, maxSyllablesPerWord);
        if (min == max) return min;

        // Longer words lean toward max; still deterministic per-word.
        float lengthBias = Mathf.Clamp01((word.Length - 2) / 6f);
        float r = HashTo01(HashMix(hash, 31));
        float blended = Mathf.Lerp(r, 1f, lengthBias * 0.5f);
        return min + Mathf.FloorToInt(blended * (max - min + 1 - 0.0001f));
    }

    // djb2-style stable string hash so the same word always produces the same
    // result across sessions (unlike string.GetHashCode in .NET Core).
    private static int StableHash(string s)
    {
        unchecked
        {
            int h = 5381;
            for (int i = 0; i < s.Length; i++)
            {
                h = ((h << 5) + h) ^ char.ToLowerInvariant(s[i]);
            }
            return h;
        }
    }

    private static int HashMix(int h, int salt)
    {
        unchecked
        {
            int x = h ^ (salt * unchecked((int)2654435761));
            x ^= x >> 16;
            x *= unchecked((int)0x7feb352d);
            x ^= x >> 15;
            x *= unchecked((int)0x846ca68b);
            x ^= x >> 16;
            return x;
        }
    }

    private static float HashTo01(int h)
    {
        return (Mathf.Abs(h) % 10000) / 10000f;
    }

    private static IEnumerable<string> SplitIntoWords(string text)
    {
        int start = -1;
        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];
            if (char.IsWhiteSpace(c))
            {
                if (start >= 0)
                {
                    yield return text.Substring(start, i - start);
                    start = -1;
                }
            }
            else if (start < 0)
            {
                start = i;
            }
        }

        if (start >= 0)
        {
            yield return text.Substring(start, text.Length - start);
        }
    }

    private static string StripAndReturnTrailingPunctuation(string word, out string trailing)
    {
        int end = word.Length;
        var sbTrail = new StringBuilder();
        while (end > 0 && !char.IsLetterOrDigit(word[end - 1]))
        {
            sbTrail.Insert(0, word[end - 1]);
            end--;
        }
        int start = 0;
        while (start < end && !char.IsLetterOrDigit(word[start]))
        {
            start++;
        }

        trailing = sbTrail.ToString();
        return start < end ? word.Substring(start, end - start) : string.Empty;
    }

    private static bool ContainsSentenceEnd(string s)
    {
        if (string.IsNullOrEmpty(s)) return false;
        for (int i = 0; i < s.Length; i++)
        {
            if (s[i] == '.' || s[i] == '!' || s[i] == '?') return true;
        }
        return false;
    }

    private static bool ContainsSoftPunctuation(string s)
    {
        if (string.IsNullOrEmpty(s)) return false;
        for (int i = 0; i < s.Length; i++)
        {
            if (s[i] == ',' || s[i] == ';' || s[i] == ':') return true;
        }
        return false;
    }
}
