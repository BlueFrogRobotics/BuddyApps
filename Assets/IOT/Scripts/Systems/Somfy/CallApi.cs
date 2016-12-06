using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Web;

public class CallApi : MonoBehaviour
{

    private static string SERVER_URL = "https://ha102-1.overkiz.com/enduser-mobile-web/enduserAPI/";
    private static string ALLUMER_LUMIERE_EXECID = "6b8047c9-99a8-483d-91f1-cb80043ab600";
    private static string ETEINDRE_LUMIERE_EXECID = "eb2477fc-476a-431b-b381-4017e1abdf77";
    private static string BONNE_NUIT_EXECID = "73deddd2-23ea-4b94-bf46-ef857f850da2";
    private static string BONJOUR_EXECID = "c9d22cdb-8887-4215-8bec-97e7b8aa6a57";
    private static string OUVRIR_VOLETS_EXECID = "1ff2301f-ef7a-4433-bfd2-b3f331c40878";
    private static string FERMER_VOLETS_EXECID = "5a2f57a5-00a9-46d1-95e2-dd4347dd86e9";
    public static string BONNE_NUIT = SERVER_URL + "exec/" + BONNE_NUIT_EXECID;
    public static string BONJOUR = SERVER_URL + "exec/" + BONJOUR_EXECID;
    public static string OUVRIR_VOLETS = SERVER_URL + "exec/" + OUVRIR_VOLETS_EXECID;
    public static string FERMER_VOLETS = SERVER_URL + "exec/" + FERMER_VOLETS_EXECID;
    public static string OUVRIR_LUMIERE = SERVER_URL + "exec/" + ALLUMER_LUMIERE_EXECID;
    public static string FERMER_LUMIERE = SERVER_URL + "exec/" + ETEINDRE_LUMIERE_EXECID;
    private static string LOGIN_URL = SERVER_URL + "login";
    private static string USER_ID = "innofair2";
    private static string USER_PASSWORD = "2016fair2";
    private static string BONJOUR_GAME_OBJECT = "GameObjectBonjour";
    private static string BONNE_NUIT_GAME_OBJECT = "GameObjectBonneNuit";
    private static string OUVRIR_VOLETS_GAME_OBJECT = "GameObjectOuvrirVolets";
    private static string FERMER_VOLETS_GAME_OBJECT = "GameObjectFermerVolets";

    private string sessionIDMemo = null;

    // Use this for initialization
    void Start()
    {
        login();
    }

    void login()
    {
        string url = LOGIN_URL;
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        dictionary.Add("userId", USER_ID);
        dictionary.Add("userPassword", USER_PASSWORD);
        System.Action a = () => print(results + " " + dictionary);

        POST(url, dictionary, a);
    }

    public WWW GET(string url, System.Action onComplete, string sessionId)
    {
        WWWForm form = new WWWForm();
        form.headers.Add("JSESSIONID", sessionId);

        WWW www = new WWW(url, form);
        StartCoroutine(WaitForRequest(www, onComplete));
        return www;
    }

    public WWW POST(string url, Dictionary<string, string> post, System.Action onComplete)
    {
        WWWForm form = new WWWForm();
        foreach (KeyValuePair<string, string> post_arg in post)
        {
            form.AddField(post_arg.Key, post_arg.Value);
        }
        WWW www = new WWW(url, form);
        StartCoroutine(WaitForRequest(www, onComplete));
        return www;
    }

    public WWW POST(string url, Dictionary<string, string> post, System.Action onComplete, string sessionId)
    {
        WWWForm form = new WWWForm();

        Dictionary<string, string> headers = form.headers;
        headers.Add("Cookie", sessionId);
        form.AddField("Cookie", sessionId);

        foreach (KeyValuePair<string, string> post_arg in post)
        {
            form.AddField(post_arg.Key, post_arg.Value);
        }
        print("NEW CALL WITH SESSIONID: " + getSessionId(form.headers));
        print("URL: " + url);
        WWW www = new WWW(url, form.data, headers);
        StartCoroutine(WaitForRequest(www, onComplete));
        return www;
    }

    void OnMouseDown()
    {
        string sessionId = getSessionId(headers);
        if (sessionId == null)
        {
            login();
            sessionId = getSessionId(headers);
        }

        if (sessionId != null)
        {
            string url = null;
            Dictionary<string, string> dictionary = new Dictionary<string, string>();

            if (this.gameObject.name.Equals(BONJOUR_GAME_OBJECT))
            {
                url = BONJOUR;
            }
            else if (this.gameObject.name.Equals(BONNE_NUIT_GAME_OBJECT))
            {
                url = BONNE_NUIT;
            }
            else if (this.gameObject.name.Equals(OUVRIR_VOLETS_GAME_OBJECT))
            {
                url = OUVRIR_VOLETS;
            }
            else if (this.gameObject.name.Equals(FERMER_VOLETS_GAME_OBJECT))
            {
                url = FERMER_VOLETS;
            }

            System.Action a = () => print("actionGroup");
            POST(url, dictionary, a, sessionId);
        }
        else
        {
            print("Login error");
        }

    }

    public void postAction(string Action)
    {
        string sessionId = getSessionId(headers);
        if (sessionId == null)
        {
            login();
            sessionId = getSessionId(headers);
        }

        if (sessionIDMemo != null)
        {
            string url = null;
            url = Action;

            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            System.Action a = () => print("actionGroup");
            POST(url, dictionary, a, sessionIDMemo);
        }
        else
        {
            print("Login error");
        }
    }

    private IEnumerator WaitForRequest(WWW www, System.Action onComplete)
    {
        yield return www;
        // check for errors
        if (www.error == null)
        {
            results = www.text;
            headers = www.responseHeaders;
            onComplete();
        }
        else
        {
            Debug.Log(www.error);
        }
    }

    private string results;
    public string Results
    {
        get
        {
            return results;
        }
    }

    private Dictionary<string, string> headers;
    public Dictionary<string, string> Headers
    {
        get
        {
            return headers;
        }
    }

    private string getSessionId(Dictionary<string, string> headers)
    {
        string res = null;
        string[] data = null;
        if (headers != null)
        {
            foreach (KeyValuePair<string, string> post_arg in headers)
            {
                print(post_arg.Key + " " + post_arg.Value);
                if (post_arg.Key.Equals("SET-COOKIE"))
                {
                    print("SET-COOKIE");
                    data = post_arg.Value.Split(";"[0]);
                    if (data.Length > 0)
                    {
                        res = data[0];
                        sessionIDMemo = res;
                        print("On a trouve un sessionId");
                        print("sessionId: " + res);
                    }
                }
            }
        }
        else
        {
            print("headers is null");
        }
        return res;
    }
}
