using UnityEngine;
using System;

public class Bose : MonoBehaviour
{
    private AndroidJavaObject pluginClass;
    private int stepVolume = 25;
    private string[] playlist = new string[6];
    //public string addrBose;

    public void Awake()
    {
        AndroidJavaObject lCurrentActivity;
        using (AndroidJavaClass lMainClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            lCurrentActivity = lMainClass.GetStatic<AndroidJavaObject>("currentActivity");
        }

        using (AndroidJavaClass lSTTClass = new AndroidJavaClass("com.bfr.bose.Bose"))
        {
            pluginClass = lSTTClass.CallStatic<AndroidJavaObject>("instance");
            pluginClass.Call("setContext", lCurrentActivity);
            pluginClass.Call("init");
        }
    }

    public void setPlaylist1(string p)
    {
        this.playlist[0] = p;
    }

    public void setPlaylist2(string p)
    {
        this.playlist[1] = p;
    }

    public void setPlaylist3(string p)
    {
        this.playlist[2] = p;
    }

    public void setPlaylist4(string p)
    {
        this.playlist[3] = p;
    }

    public void setPlaylist5(string p)
    {
        this.playlist[4] = p;
    }

    public void setPlaylist6(string p)
    {
        this.playlist[5] = p;
    }

    //trouver un des mots clefs dans une playlist enregistrée
    private bool inPlaylist(int i, string[] arg)
    {
        foreach (string key in arg)
        {
            if (this.playlist[i].ToLower().Contains(key))
                return (true);
        }
        return false;
    }

    //chercher la playlist demandée et la jouer si elle existe
    public bool searchPlaylist(string p)
    {
        string[] arg = p.Split(' ');
        int i = 0;

        while (i < 6)
        {
            if (this.inPlaylist(i, arg))
            {
                this.button(i + 1);
                return true;
            }
            ++i;
        }
        return false;
    }

    //changer l'ip de l'appareil Bose
    public void setAddrBose(string addr)
    {
        pluginClass.Call("setAddrBose", addr);
    }

    //appuyer sur un bouton
    private void pressKey(string key)
    {
        pluginClass.Call("pressKey", key);
    }

    //changer le volume
    public void setVolume(int volume)
    {
        pluginClass.Call("setVolume", volume);
    }

    //bouton play
    public void play()
    {
        this.pressKey("PLAY");
    }

    //bouton pause
    public void pause()
    {
        this.pressKey("PAUSE");
    }

    //bouton stop
    public void stop()
    {
        this.pressKey("STOP");
    }

    //bouton précédent
    public void prevTrack()
    {
        this.pressKey("PREV_TRACK");
    }

    //bouton suivant
    public void nextTrack()
    {
        this.pressKey("NEXT_TRACK");
    }

    //bouton mute
    public void mute()
    {
        this.pressKey("MUTE");
    }

    //active le mode aléatoire
    public void shuffleOn()
    {
        this.pressKey("SHUFFLE_ON");
    }

    //désactive le mode aléatoire
    public void shuffleOff()
    {
        this.pressKey("SHUFFLE_OFF");
    }

    //ne pas répéter
    public void repeatOff()
    {
        this.pressKey("REPEAT_OFF");
    }

    //répéter la chanson en cours
    public void repeatOne()
    {
        this.pressKey("REPEAT_ONE");
    }

    //répéter la playlist
    public void repeatAll()
    {
        this.pressKey("REPEAT_ALL");
    }

    //bouton power
    public void power()
    {
        this.pressKey("POWER");
    }

    //augmenter le volume
    public void volumeUp()
    {
        pluginClass.Call("changeVolume", stepVolume);
    }

    //diminuer le volume
    public void volumeDown()
    {
        pluginClass.Call("changeVolume", -stepVolume);
    }

    //appuyer sur un des 6 presets
    public void button(int b)
    {
        if (b > 0 && b < 7)
            this.pressKey("PRESET_" + b);
    }
}
