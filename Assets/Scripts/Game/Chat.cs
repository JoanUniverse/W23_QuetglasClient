using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Chat : MonoBehaviour
{
    public Text messageText;
    public InputField messageInputField;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(RefreshMessages());
    }

    public void OnSendButtonClicked()
    {
        StartCoroutine(InsertMessage());
    }

    private IEnumerator RefreshMessages()
    {
        while (true)
        {
            StartCoroutine(GetMessages());
            yield return new WaitForSeconds(2);
        }
    }

    private IEnumerator InsertMessage()
    {
        Player player = FindObjectOfType<Player>();
        PlayerSerializable playerSerializable = new PlayerSerializable();
        playerSerializable.Id = player.Id;
        playerSerializable.Name = player.Name;
        playerSerializable.Email = player.Email;
        playerSerializable.BirthDay = player.BirthDay.ToString();

        MessageModel messageModel = new MessageModel();
        messageModel.Message = messageInputField.text;

        using (UnityWebRequest httpClient = new UnityWebRequest(player.HttpServerAddress + "api/Message/InsertMessage", "POST"))
        {
            string messageData = JsonUtility.ToJson(messageModel);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(messageData);
            httpClient.uploadHandler = new UploadHandlerRaw(bodyRaw);
            httpClient.downloadHandler = new DownloadHandlerBuffer();
            httpClient.SetRequestHeader("Content-type", "application/json");
            httpClient.SetRequestHeader("Authorization", "bearer " + player.Token);
            httpClient.certificateHandler = new BypassCertificate();
            yield return httpClient.SendWebRequest();

            if (httpClient.isNetworkError || httpClient.isHttpError)
            {
                throw new Exception("InsertMessage > Message: " + httpClient.error);
            }
            StartCoroutine(GetMessages());
            messageInputField.text = "";
            Debug.Log("Works");
        }
    }

    private IEnumerator GetMessages()
    {
        Player player = FindObjectOfType<Player>();
        PlayerSerializable playerSerializable = new PlayerSerializable();
        playerSerializable.LastLogin = player.LastLogin.ToString();

        UnityWebRequest httpClient = new UnityWebRequest(player.HttpServerAddress + "api/Message/GetMessages", "POST");

        httpClient.SetRequestHeader("Authorization", "bearer " + player.Token);
        httpClient.SetRequestHeader("Content-type", "application/json");
        string messageData = JsonUtility.ToJson(playerSerializable);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(messageData);
        httpClient.uploadHandler = new UploadHandlerRaw(bodyRaw);
        httpClient.downloadHandler = new DownloadHandlerBuffer();
        httpClient.certificateHandler = new BypassCertificate();
        yield return httpClient.SendWebRequest();

        if (httpClient.isNetworkError || httpClient.isHttpError)
        {
            throw new Exception("Chat > GetMessages: " + httpClient.error);
        }
        else
        {
            string jsonResponse = httpClient.downloadHandler.text;
            string response = "{\"messages\":" + jsonResponse + "}";
            ListOfMessages listOfMessages = JsonUtility.FromJson<ListOfMessages>(response);
            messageText.text = "";
            foreach (MessageModel m in listOfMessages.messages)
            {
                string userName = m.PlayerId.Substring(0, 3);
                messageText.text += "\n" + userName + "> " + m.Message;
            }

        }

        httpClient.Dispose();
    }
}
