using UnityEngine;
using SimpleJSON;
using System;
using BuddyOS;

public class BoseRecast : MonoBehaviour
{

    private int NB_CMD = 8;
    private Cmd[] cmds = null;

    [SerializeField]
    private Bose bose;

    private TextToSpeech mTextToSpeech;

    private Mood mFace;

    void Start()
    {
        this.cmds = initCmd();
        mTextToSpeech = BYOS.Instance.TextToSpeech;
        mFace = BYOS.Instance.Mood;
    }

    private struct Cmd
    {
        public string key;
        public Func<JSONNode, bool> fun;
    }

    private Cmd addCmd(string key, Func<JSONNode, bool> fun)
    {
        Cmd cmd;
        cmd.key = key;
        cmd.fun = fun;
        return cmd;
    }

    private Cmd[] initCmd()
    {
        Cmd[] cmd = new Cmd[NB_CMD];

        cmd[0] = addCmd("volume", changeVolume);
        cmd[1] = addCmd("play", play);
        cmd[2] = addCmd("pause", pause);
        cmd[3] = addCmd("stop", stop);
        cmd[4] = addCmd("prev", prevTrack);
        cmd[5] = addCmd("next", nextTrack);
        cmd[6] = addCmd("mute", mute);
        cmd[7] = addCmd("playlist", button);
        return cmd;
    }

    public bool execute(string slug, string arg)
    {
        JSONNode entities = JSON.Parse(arg);
        int i = 0;
        while (i < NB_CMD)
        {
            if (cmds[i].key.Equals(slug))
                return (cmds[i].fun(entities));
            ++i;
        }
        return (false);
    }

    private bool changeVolume(JSONNode entities)
    {
        int volume;
        if (Int32.TryParse(entities["number"][0]["scalar"].Value, out volume))
        {
            mFace.Set(MoodType.LISTENING);
            mTextToSpeech.Say("Je met le volume à " + volume);
            bose.setVolume(volume);
        }
        else if (entities["down"].ToString() != "")
        {
            mFace.Set(MoodType.SAD);
            mTextToSpeech.Say("chut, moins de bruit");
            bose.volumeDown();
        }
        else if (entities["up"].ToString() != "")
        {
            mFace.Set(MoodType.ANGRY);
            mTextToSpeech.Say("faisons plus de bruit");
            bose.volumeUp();
        }
        return true;
    }

    private bool play(JSONNode entities)
    {
        mFace.Set(MoodType.HAPPY);
        mTextToSpeech.Say("go");
        bose.play();
        return true;
    }

    private bool pause(JSONNode entities)
    {
        mFace.Set(MoodType.LISTENING);
        mTextToSpeech.Say("on attend");
        bose.pause();
        return true;
    }

    private bool stop(JSONNode entities)
    {
        mFace.Set(MoodType.LISTENING);
        mTextToSpeech.Say("c'est fini");
        bose.stop();
        return true;
    }

    private bool prevTrack(JSONNode entities)
    {
        mFace.Set(MoodType.THINKING);
        mTextToSpeech.Say("Je l'aimais bien");
        bose.prevTrack();
        bose.prevTrack();
        return true;
    }

    private bool nextTrack(JSONNode entities)
    {
        mFace.Set(MoodType.SURPRISED);
        mTextToSpeech.Say("Une autre chanson");
        bose.nextTrack();
        return true;
    }

    private bool mute(JSONNode entities)
    {
        mFace.Set(MoodType.TIRED);
        mTextToSpeech.Say("Ne faisons plus de bruit");
        bose.mute();
        return true;
    }

    private bool button(JSONNode entities)
    {
        int b;
        if (Int32.TryParse(entities["number"][0]["scalar"].Value, out b))
        {
            mFace.Set(MoodType.HAPPY);
            mTextToSpeech.Say("je mets la sélection " + b);
            bose.button(b);
            return true;
        }
        string play = entities["playlist"][0]["value"].Value;
        string module = entities["module"][0]["value"].Value;
        if (play != null && module != null && module.Equals("playlist"))
        {
            mFace.Set(MoodType.SURPRISED);
            if (bose.searchPlaylist(play))
            {
                mFace.Set(MoodType.HAPPY);
                mTextToSpeech.Say("je lance la sélection " + play);
                return true;
            }
            else
                mTextToSpeech.Say("je ne trouve pas la sélection " + play);
        }
        return false;
    }
}
