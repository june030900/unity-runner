using UnityEngine;

/// <summary>
/// 用代码合成音效（正弦扫频），无需任何音频素材
/// </summary>
public static class SoundFX
{
    const int SampleRate = 44100;

    static AudioSource source;

    public static void Init(AudioSource audioSource)
    {
        source = audioSource;
    }

    public static void PlayJump()
    {
        Play(520f, 900f, 0.11f, 0.22f);
    }

    public static void PlayGameOver()
    {
        Play(420f, 110f, 0.45f, 0.28f);
    }

    /// <summary>频率从 from 扫到 to 的一段正弦音</summary>
    static void Play(float from, float to, float duration, float volume)
    {
        if (source == null) return;

        int sampleCount = Mathf.Max(1, Mathf.RoundToInt(SampleRate * duration));
        var data = new float[sampleCount];

        double phase = 0.0;
        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / sampleCount;
            float freq = Mathf.Lerp(from, to, t);

            // 相位累加，避免直接代入时间导致扫频失真
            phase += 2.0 * Mathf.PI * freq / SampleRate;

            // 首尾淡入淡出，避免爆音
            float envelope = Mathf.Sin(Mathf.PI * t);
            data[i] = (float)System.Math.Sin(phase) * envelope * volume;
        }

        var clip = AudioClip.Create("sfx", sampleCount, 1, SampleRate, false);
        clip.SetData(data, 0);
        source.PlayOneShot(clip);
        Object.Destroy(clip, duration + 0.2f);
    }
}
