const WebSocket = require('ws');

// TextDecoderを使って受信データをUTF-8文字列に変換
const textDecoder = new TextDecoder('utf-8');

const server = new WebSocket.Server({port: 8080});
server.on('connection', (socket) => {
    console.log("Client connected");
    
    // メッセージの受信
    socket.on('message', (msg) => {
        // バイナリデータをUTF-8文字列に変換
        const decodedMessage = textDecoder.decode(msg);
        console.log(decodedMessage);

        // パラメータ付きのURLを作成
        const baseUrl = "https://example.com/path";
        const params = new URLSearchParams({ param1: "value1", param2: "value2" });
        const fullUrl = `${baseUrl}?${params.toString()}`;

        socket.send(fullUrl);
        console.log(`クライアントに送信したURL: ${fullUrl}`);
   
        // server.clients.forEach((client) => {
        //     if (client.readyState === WebSocket.OPEN) {
        //         client.send(fullUrl);
        //     }
        // });
    });
    
    // クライアントが切断したとき
    socket.on('close', () => {
        console.log('Client disconnected');
    });
    
    // エラーが発生したとき
    socket.on('error', (err) => {
        console.error('Error : ${err}');
    });
});

server.on('error', (err) => {
    console.error('Server Error : ${err}');
});
    

console.log('Server started on port 8080');
