using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Client : MonoSingleton<Client>
{
    public Telepathy.Client client = new Telepathy.Client(1920 * 1080 + 1024);


    private void Awake()
    {
        // update even if window isn't focused, otherwise we don't receive.
        Application.runInBackground = true;

        // use Debug.Log functions for Telepathy so we can see it in the console
        Telepathy.Log.Info = Debug.Log;
        Telepathy.Log.Warning = Debug.LogWarning;
        Telepathy.Log.Error = Debug.LogError;

        // hook up events
        client.OnConnected = () => Debug.Log("Client Connected");
        client.OnData = (message) => ReceiveMessage(message);
        client.OnDisconnected = () => Debug.Log("Client Disconnected");
    }
    private void Update()
    {
        if (client.Connected)
        {
            // tick to process messages
            // (even if not connected so we still process disconnect messages)
            client.Tick(10);
        }
        else
        {
            client.Connect("127.0.0.1", 45604);
        }
    }

    private void OnApplicationQuit()
    {
        // the client/server threads won't receive the OnQuit info if we are
        // running them in the Editor. they would only quit when we press Play
        // again later. this is fine, but let's shut them down here for consistency
        client.Disconnect();
    }
    public void ReceiveMessage(ArraySegment<byte> message)
    {
        // clear previous message
        byte[] messageBytes = new byte[message.Count];
        for (int i = 0; i < messageBytes.Length; i++)
        {
            messageBytes[i] = message.Array[i];
        }

        byte[] commandBytes = new byte[4];
        Array.Copy(messageBytes, 0, commandBytes, 0, 4);
        int command = BitConverter.ToInt32(commandBytes);

        switch (command)
        {
            case ConstantValues.CMD_RESPONSE_CHECK_PASSWORD_RESULT:
                ReceivePassword(message: ref messageBytes);
                break;
            case ConstantValues.CMD_RESPONSE_GET_STUDIO_DATA:
                ReceiveStudioData(message: ref messageBytes);
                break;
            case ConstantValues.CMD_RESPONSE_ADD_EDITOR_DATA:
                ReceiveResult(message: ref messageBytes);
                break;
            default:
                break;
        }
    }

    public void RequestCheckPassword(int password)
    {
        List<byte> messages = new List<byte>();
        messages.AddRange(BitConverter.GetBytes(ConstantValues.CMD_REQUEST_CHECK_PASSWORD));
        messages.AddRange(BitConverter.GetBytes(password));

        client.Send(messages.ToArray());
        Debug.Log("RequestCheckPassword::" + password);
    }
    public void RequestStudioData()
    {
        client.Send(BitConverter.GetBytes(ConstantValues.CMD_REQUEST_GET_STUDIO_DATA));
        Debug.Log("RequestStudioData");
    }
    public void SendEditorData()
    {
        client.Send(BitConverter.GetBytes(ConstantValues.CMD_REQUEST_ADD_EDITOR_DATA));
        Debug.Log("SendEditorData");
    }
    private void ReceivePassword(ref byte[] message)
    {
        byte[] bytes = new byte[1];
        Array.Copy(message, 4, bytes, 0, 1);
        bool result = BitConverter.ToBoolean(bytes);


        Debug.Log("ReceivePassword::" + result);
    }
    private void ReceiveStudioData(ref byte[] message)
    {
        Debug.Log("ReceiveStudioData");
    }
    private void ReceiveResult(ref byte[] message)
    {
        Debug.Log("ReceiveResult");
    }
}
