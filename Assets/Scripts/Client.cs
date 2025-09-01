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
                ReceiveCheckPasswordResult(message: ref messageBytes);
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
    public void RequestStudioData(int password)
    {
        List<byte> messages = new List<byte>();
        messages.AddRange(BitConverter.GetBytes(ConstantValues.CMD_REQUEST_GET_STUDIO_DATA));
        messages.AddRange(BitConverter.GetBytes(password));

        client.Send(messages.ToArray());
        Debug.Log("Request Studio Data");
    }
    public void SendEditorData()
    {
        client.Send(BitConverter.GetBytes(ConstantValues.CMD_REQUEST_ADD_EDITOR_DATA));
        Debug.Log("SendEditorData");
    }
    private void ReceiveCheckPasswordResult(ref byte[] message)
    {
        byte[] resultBytes = new byte[1];
        Array.Copy(message, 4, resultBytes, 0, 1);
        bool result = BitConverter.ToBoolean(resultBytes);

        Debug.Log("ReceivePassword::" + result);
        if (result)
        {
            RequestStudioData(StaticValues.password);
        }
    }
    private void ReceiveStudioData(ref byte[] message)
    {
        byte[] headerLengthBytes = new byte[4];
        Array.Copy(message, 4, headerLengthBytes, 0, 4);
        int headerLength = BitConverter.ToInt32(headerLengthBytes);

        byte[] headerBytes = new byte[headerLength];
        Array.Copy(message, 8, headerBytes, 0, headerLength);
        string headerStr = Encoding.UTF8.GetString(headerBytes);

        StudioDataRaw.Header header = JsonUtility.FromJson<StudioDataRaw.Header>(headerStr);

        byte[] textureBytes = new byte[header.TextureLength];
        Array.Copy(message, 8 + headerLength, textureBytes, 0, header.TextureLength);

        StaticValues.studioDataRaw = new StudioDataRaw(
            id: header.Id, 
            password: header.Password, 
            registerDateTime: header.RegisterDateTime, 
            textureRaw: textureBytes
        );

        Debug.Log("ReceiveStudioData" + StaticValues.studioDataRaw.ToString());

        GameObject.Find("Ctrl").GetComponent<Ctrl_Select>().Init();
    }
    private void ReceiveResult(ref byte[] message)
    {
        Debug.Log("ReceiveResult");
    }
}
