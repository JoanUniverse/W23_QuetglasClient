using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    private const string _httpServerAddress = "https://localhost:44366/";
    public string HttpServerAddress
    {
        get
        {
            return _httpServerAddress;
        } 
    }

    private string _token;
    public string Token
    {
        get { return _token; }
        set { _token = value; }
    }

    private string _id;
    public string Id
    {
        get { return _id; }
        set { _id = value; }
    }

    private string _name;
    public string Name
    {
        get { return _name; }
        set { _name = value; }
    }

    private DateTime _birthday;
    public DateTime BirthDay
    {
        get { return _birthday; }
        set { _birthday = value; }
    }

    private string _email;
    public string Email
    {
        get { return _email; }
        set { _email = value; }
    }

    private void Awake()
    {
        var gameManagers = FindObjectsOfType<Player>();
        if (gameManagers.Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

}
