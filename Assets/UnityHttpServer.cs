// 必要な名前空間
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

class Program : MonoBehaviour
{
    [SerializeField]
    private InputField inputField;
    
    // サーバーのURI
    private Uri serverUri = new Uri("ws://localhost:8080");

    // WebSocketクライアント
    private ClientWebSocket client;
    
    private async void Awake()
    {
        inputField.onEndEdit.AddListener(async (text) =>
        {
            // サーバーにメッセージを送信
            byte[] bytesToSend = Encoding.UTF8.GetBytes(text);
            await client.SendAsync(new ArraySegment<byte>(bytesToSend), WebSocketMessageType.Text, true, CancellationToken.None);
            Debug.Log($"送信: {text}");
        });
        
        // WebSocketクライアントの作成
        client = new ClientWebSocket();
        
        // サーバーに接続
        await client.ConnectAsync(serverUri, CancellationToken.None);
        Debug.Log("サーバーに接続しました。");
        
        
        // 非同期で受信を開始
        _ = Task.Run(ReceiveMessagesAsync); 
    }

    private async Task ReceiveMessagesAsync()
    {
        if(client.State != WebSocketState.Open)
        {
            return;
        }
        
        // サーバーからのメッセージを受信
        byte[] buffer = new byte[1024];
        var result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        string receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
        Debug.Log($"受信: {receivedMessage}");
        
        Uri uri = new Uri(receivedMessage);
        var parameters = uri.GetQueryParameter();
        Debug.Log("URL パラメータ" + parameters.Select(kv => $"{kv.Key}: {kv.Value}").Aggregate((a, b) => $"{a}, {b}"));
    }

    // OnDestroyメソッド
    private async void OnDestroy()
    {
        // 接続を閉じる
        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "終了", CancellationToken.None);
        Debug.Log("接続を閉じました。");
        
        // WebSocketクライアントを破棄
        client.Dispose();
    }
}


public static class UriExtensions
{
    public static Dictionary<string, string> GetQueryParameter(this Uri uri)
    {
        var query = uri.Query.TrimStart('?');
        var queryParams = query.Split('&');
        var ret = new Dictionary<string, string>();
        
        foreach (var param in queryParams)
        {
            var keyValue = param.Split('=');
            if (keyValue.Length == 2)
            {
                ret.Add(keyValue[0], keyValue[1]);
            }
        }

        return ret;
    }
}