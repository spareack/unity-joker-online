using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System;
using System.Drawing;

public class networkManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private game GameScript;

    void Start()
    {
        PhotonPeer.RegisterType(typeof(Texture2D), 242, SerializeImage, DeserializeImage);
    }

    void Update()
    {
        if (PhotonNetwork.NetworkClientState == ClientState.Disconnected) PhotonNetwork.Reconnect();

    }

    public void leaveGame() => PhotonNetwork.LeaveRoom();

    public override void OnLeftRoom() => SceneManager.LoadScene(0);

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.PlayerList.Length == 4)
        {
            GameScript.startLoadGame();
        }
    }

    public override void OnPlayerLeftRoom(Player newPlayer)
    {
        if (PhotonNetwork.PlayerList.Length == 3)
        {
            leaveGame();
        }
    }

    public static object DeserializeIntArray(byte[] data)
    {
        int [] result = new int[data.Length / sizeof(int)];

        for (int i = 0; i < result.Length; i += 1)
        {
            result[0] = BitConverter.ToInt32(data, i * sizeof(int));
        }
        return result;
    }

    public static byte[] SerializeIntArray(object obj)
    {
        int[] array = (int[]) obj;
        byte[] result = new byte[array.Length * sizeof(int)];

        for (int i = 0; i < array.Length; i += 1)
        {
            BitConverter.GetBytes(array[i]).CopyTo(array, i * sizeof(int));
        }
        return result;
    }

    public static object DeserializeImage(byte[] data)
    {
        if (data.Length == 0) return Texture2D.whiteTexture;
        Debug.Log(data.Length);
        Texture2D texCopy = new Texture2D(100, 100, TextureFormat.RGBA32, false);
        texCopy.LoadRawTextureData(data);
        texCopy.Apply();
        return texCopy;
    }

    public static byte[] SerializeImage(object obj)
    {
        Texture2D texture = (Texture2D)obj;
        Debug.Log((texture.width, texture.height, texture.format));
        Debug.Log("!^tNG!*fgb!*&bjf!&%!ng1*gm!*H");
        return texture.GetRawTextureData();
    }

    public void OnOperationResponse(OperationResponse operationResponse)
    {
        // Debug.Log(operationResponse.DebugMessage);
        // switch (operationResponse.OperationCode)
        // {
        //     case OperationCode.LeaveLobby:
        //         Debug.Log(operationResponse.DebugMessage);
        //         break;
        // }

    }
}
