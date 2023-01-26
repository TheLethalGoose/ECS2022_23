using Microsoft.Xna.Framework.Audio;

namespace ECS2022_23.Core.Manager;

public static class SoundManager
{
    private static SoundEffectInstance SoundEffectInstance;

    public static void Play(SoundEffect sound)
    {
        sound.Play();
    }

    public static void StopMusic()
    {
        SoundEffectInstance?.Dispose();
    }

    public static void PlayMusic(SoundEffect Sound)
    {
        if (Sound == null) return;

        SoundEffectInstance = Sound.CreateInstance();
        SoundEffectInstance.Volume = 0.1f;
        SoundEffectInstance.IsLooped = true;
        SoundEffectInstance.Play();
    }

    public static void Initialize()
    {
        SoundEffect.MasterVolume = 0.5f;
    }
}