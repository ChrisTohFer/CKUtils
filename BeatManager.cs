using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BeatManager : MonoBehaviour
{
    //
    AudioSource m_audioSource;

    //
    [SerializeField] MusicWrapper m_music;

    //
    UnityEvent m_beatEvent, m_offBeatEvent;
    public UnityEvent beatEvent
    {
        get { return m_beatEvent; }
    }
    public UnityEvent offBeatEvent
    {
        get { return m_offBeatEvent; }
    }

    //
    private void Awake()
    {
        m_audioSource = GetComponent<AudioSource>();
        m_beatEvent = new UnityEvent();
        m_offBeatEvent = new UnityEvent();

        if(m_music != null)
        {
            PlayMusic(m_music);
        }
    }

    //
    int lastBeat = 0;
    private void FixedUpdate()
    {
        if(m_music != null)
        {
            var section = GetCurrentSection();
            if(section != null)
            {
                float beatProgressPerFrame = Time.fixedDeltaTime * section.bpm / 60f;
                float currentBeat = GetCurrentBeat(section);

                if (Mathf.FloorToInt(currentBeat) != lastBeat)
                {
                    m_beatEvent.Invoke();
                    lastBeat = Mathf.FloorToInt(currentBeat);
                }
                else if (Mathf.Round(currentBeat) != Mathf.Round(currentBeat + beatProgressPerFrame))
                {
                    m_offBeatEvent.Invoke();
                }
            }
        }
    }

    //
    public void PlayMusic(MusicWrapper music)
    {
        m_music = music;
        m_audioSource.Stop();
        m_audioSource.clip = music.musicTrack;
        m_audioSource.Play();
    }

    /// <summary>
    /// Returns a float which represents the current beat. Will be a whole number when exactly on the beat, or will end in .5 when exactly on an off-beat.
    /// </summary>
    /// <returns></returns>
    float GetCurrentBeat(MusicWrapper.BPMSection section)
    {
        float beatDuration = 60f / section.bpm;
        return (m_audioSource.time - section.startTime) / beatDuration;
    }
    float GetCurrentBeat()
    {
        return GetCurrentBeat(GetCurrentSection());
    }

    MusicWrapper.BPMSection GetCurrentSection()
    {
        float musicTime = m_audioSource.time;
        MusicWrapper.BPMSection[] sections = m_music.sections;

        MusicWrapper.BPMSection section = null;

        for (int i = 0; i < sections.Length; ++i)
        {
            if (musicTime > sections[i].startTime && musicTime < sections[i].endTime)
            {
                //Correct section
                section = sections[i];
                break;
            }
        }

        return section;
    }
}
