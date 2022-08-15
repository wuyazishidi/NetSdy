using AsyncProtocol;
using PENet;
using System;
namespace AsyncNetClientExample {
    class AsyncClientStart {
        static void Main(string[] args) {
            AsyncNetClient client = new AsyncNetClient();
            client.StartClient("192.168.1.41",17777);
            AsyncTool.ColorLog(AsyncLogColor.Yellow,"Input 'quit' to stop client");
            while (true) {
                string ipt = Console.ReadLine();
                if (ipt == "quit") {
                    client.CloseClient();
                    break;
                }
                AsyncMsg msg = new AsyncMsg {
                    hellomsg = ipt
                };
                    client.session.SendMsg(msg);
                #region
                /*
                byte[] data = AsyncTool.PackLenInfo(AsyncTool.Serialize(msg));
                switch (ipt) {
                    case "1":
                        byte[] head_1 = new byte[] { data[0], data[1] };
                        client.session.SendMsg(head_1);
                        break;
                    case "2":
                        byte[] head_2 = new byte[] { data[2], data[3], data[4] };
                        client.session.SendMsg(head_2);
                        break;
                    case "3":
                        byte[] body = new byte[200];
                        for (int i = 0; i < body.Length; i++) {
                            body[i] = 77;
                        }
                        for (int i = 5; i < data.Length; i++) {
                            body[i-5] = data[i];
                        }
                        client.session.SendMsg(body);
                        break;
                    default:
                        break;
                }
                //client.session.SendMsg(msg);*/
                #endregion
            }
        }
    }
}
