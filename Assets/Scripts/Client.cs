using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
        client.OnConnected = () => OnConnected();
        client.OnData = (message) => ReceiveMessage(message);
        client.OnDisconnected = () => Debug.Log("Client Disconnected");
    }
    private void Update()
    {
        if (client.Connected)
        {
            // tick to process messages
            // (even if not connected so we still process disconnect messages)
            client.Tick(1000);
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
    private void OnConnected()
    {
        Debug.Log("Client Connected");

        using (MemoryStream ms = new MemoryStream())
        using (BinaryWriter bw = new BinaryWriter(ms))
        {
            bw.Write(ConstantValues.CMD_REQUEST_CONNECT_EDITOR);

            client.Send(ms.ToArray());
        }
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
                ReceiveGetStudioData(message: ref messageBytes);
                break;
            case ConstantValues.CMD_RESPONSE_ADD_EDITOR_DATA:
                ReceiveAddEditorDataResult(message: ref messageBytes);
                break;
            default:
                break;
        }
    }

    public void RequestCheckPassword(int password)
    {
        using (MemoryStream ms = new MemoryStream())
        using (BinaryWriter bw = new BinaryWriter(ms))
        {
            bw.Write(ConstantValues.CMD_REQUEST_CHECK_PASSWORD);
            bw.Write(password);

            client.Send(ms.ToArray());
        }

        Debug.Log($"Request Check Password::{password}");
    }
    public void RequestGetStudioData(int password)
    {
        using (MemoryStream ms = new MemoryStream())
        using (BinaryWriter bw = new BinaryWriter(ms))
        {
            bw.Write(ConstantValues.CMD_REQUEST_GET_STUDIO_DATA);
            bw.Write(password);

            client.Send(ms.ToArray());
        }

        Debug.Log($"Request Get Studio Data::{password}");
    }
    public void RequestAddEditorData(EditorDataRaw editorDataRaw)
    {
        using (MemoryStream ms = new MemoryStream())
        using (BinaryWriter bw = new BinaryWriter(ms))
        {
            bw.Write(ConstantValues.CMD_REQUEST_ADD_EDITOR_DATA);
            bw.Write(editorDataRaw.ToBytes());

            client.Send(ms.ToArray());
        }

        Debug.Log($"Request Add Editor Data::{editorDataRaw.ToString()}");
    }
    private void ReceiveCheckPasswordResult(ref byte[] message)
    {
        byte[] resultBytes = new byte[1];
        Buffer.BlockCopy(message, 4, resultBytes, 0, 1);
        bool result = BitConverter.ToBoolean(resultBytes);

        Debug.Log($"Receive Check Password Result::{result}");

        if (result)
        {
            RequestGetStudioData(StaticValues.password);
        }
        else
        {
            GameObject.Find("Ctrl").GetComponent<Ctrl_Title>().FailPassword();
        }
    }
    private void ReceiveGetStudioData(ref byte[] message)
    {
        byte[] headerLengthBytes = new byte[4];
        Buffer.BlockCopy(message, 4, headerLengthBytes, 0, 4);
        int headerLength = BitConverter.ToInt32(headerLengthBytes);

        byte[] headerBytes = new byte[headerLength];
        Buffer.BlockCopy(message, 8, headerBytes, 0, headerLength);
        string headerStr = Encoding.UTF8.GetString(headerBytes);

        StudioDataRaw.Header header = JsonUtility.FromJson<StudioDataRaw.Header>(headerStr);

        byte[] textureBytes = new byte[header.TextureLength];
        Buffer.BlockCopy(message, 8 + headerLength, textureBytes, 0, header.TextureLength);

        StudioDataRaw raw = new StudioDataRaw(
            id: header.Id, 
            password: header.Password, 
            registerDateTime: header.RegisterDateTime, 
            textureRaw: textureBytes
        );
        StaticValues.studioDataRaw = raw;

        Debug.Log($"Receive Get Studio Data::{raw.ToString()}");

        if (raw.TextureRaw.Length > 0)
        {
            GameObject.Find("Ctrl").GetComponent<Ctrl_Title>().NextScene();
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("99_Error");
        }
    }
    private void ReceiveAddEditorDataResult(ref byte[] message)
    {
        byte[] resultBytes = new byte[1];
        Buffer.BlockCopy(message, 4, resultBytes, 0, 1);
        bool result = BitConverter.ToBoolean(resultBytes);

        Debug.Log($"Receive Add Editor Data Result::{result}");

        if (result)
        {
            GameObject.Find("Ctrl").GetComponent<Ctrl_Edit>().Finish();
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("99_Error");
        }
    }
}
