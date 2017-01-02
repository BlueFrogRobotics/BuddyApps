using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using SimpleJSON;

public class Recast : MonoBehaviour
{

    private string response;
    private string str_intents;
    private string str_entities;
    private string str_slug;
    private bool state;
    private string langage = "fr";
    private string token;

    public Recast()
    {
        state = false;
    }

    ~Recast()
    {
    }

    public void send(String msg)
    {
        state = false;
        StartCoroutine(DoSend(msg));
    }

    public void setToken(String tok)
    {
        this.token = tok;
    }

    private IEnumerator DoSend(String msg)
    {
        WWWForm form = new WWWForm();
        form.AddField("text", msg);
        form.AddField("language", this.langage);
        byte[] rawData = form.data;
        Dictionary<String, String> headers = form.headers;
        headers["Authorization"] = "Token " + token;
        WWW w = new WWW("https://api.recast.ai/v2/request", rawData, headers);
        yield return w;
        response = w.text;
        JSONNode json = JSON.Parse(response);
        //string results = json["results"].ToString();
        str_intents = json["results"]["intents"].AsArray.ToString();
        str_entities = json["results"]["entities"].ToString();
        str_slug = json["results"]["intents"][0]["slug"].Value.ToString();
        state = true;
    }

    public void setLangage(String lang)
    {
        this.langage = lang;
    }

    public string getResponse()
    {
        return response;
    }

    public string getIntent()
    {
        return str_intents;
    }

    public string getSlug()
    {
        return str_slug;
    }

    public string getEntities()
    {
        return str_entities;
    }

    public bool hasAnswered()
    {
        bool bol = state;
        if (bol)
            state = false;
        return bol;
    }

}