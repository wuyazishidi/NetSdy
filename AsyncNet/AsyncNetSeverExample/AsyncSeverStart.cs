using AsyncProtocol;
using PENet;
using System;
using System.Collections.Generic;

namespace AsyncNetSeverExample {
    //IP 192.168.1.41
    class AsyncSeverStart {
        static void Main(string[] args) {
            AsyncNetSever sever = new AsyncNetSever();
            sever.StartSever("192.168.1.41", 17777);
            AsyncTool.ColorLog(AsyncLogColor.Yellow, "Input 'quit' to stop sever");
            while (true) {
                string ipt = Console.ReadLine();
                if (ipt == "quit") {
                    sever.CloseSever();
                    break;
                }
                else {
                    AsyncMsg msg = new AsyncMsg {
                        hellomsg = ipt
                    };
                    byte[] data = AsyncTool.PackLenInfo(AsyncTool.Serialize(msg));

                    List<AsyncSession> sessionLst = sever.GetSessionLst();
                    for (int i = 0; i < sessionLst.Count; i++) {

                        sessionLst[i].SendMsg(data);
                    }
                }
               
               
            }
            Console.ReadKey();
        }
    }
}
