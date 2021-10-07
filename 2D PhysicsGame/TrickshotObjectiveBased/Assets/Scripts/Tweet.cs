using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tweet : MonoBehaviour
{

    //Twitter Adresses and message to be sent - JG
    private string TwitterAddress = "https://twitter.com/intent/tweet";
    public string TwitterMessage = "";
    private string TwitterLink = "https://twitter.com/TrickShotUoG";


    public void PostTweet()
    {
        // opens browser and tweets - JG
        Application.OpenURL(TwitterAddress + "?text=" + WWW.EscapeURL(TwitterMessage));
    }
}
