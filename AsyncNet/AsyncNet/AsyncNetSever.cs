using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace PENet {
    public class AsyncNetSever {
        private Socket skt = null;
        public int backlog = 10;
        List<AsyncSession> sessionLst = null;
        public void StartSever(string ip, int port) {
            sessionLst = new List<AsyncSession>();
            try {
                skt = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                skt.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
                skt.Listen(backlog);
                AsyncTool.ColorLog(AsyncLogColor.Green, "Sever Start...");
                skt.BeginAccept(new AsyncCallback(ClientConnectCB), null);
            }
            catch (Exception e) {
                AsyncTool.Error(e.Message);
            }

        }

        private void ClientConnectCB(IAsyncResult ar) {
                AsyncSession session = new AsyncSession();
            try {
                Socket clientSkt = skt.EndAccept(ar);
                if (clientSkt.Connected) {
                    lock (sessionLst) {
                        sessionLst.Add(session);
                    }
                    session.InitSession(clientSkt, () => {
                        if (sessionLst.Contains(session)) {
                            lock (sessionLst) {
                                if (sessionLst.Remove(session)) {
                                    AsyncTool.ColorLog(AsyncLogColor.Yellow, "Clear ServerSession Success");
                                }
                                else {
                                    AsyncTool.ColorLog(AsyncLogColor.Yellow, "Clear ServerSession Fail");

                                }
                            }
                        }
                    });
                }
                //TODO
                //开始接受下一个新客户端的连接
                skt.BeginAccept(new AsyncCallback(ClientConnectCB), null);
            }
            catch (Exception e) {
                AsyncTool.Error("ClientConnectCB:{0}:", e.Message);
            }


        }
        public List<AsyncSession> GetSessionLst() {
            return sessionLst;
        }

        public void CloseSever() {

            for (int i = 0; i < sessionLst.Count; i++) {
                sessionLst[i].CloseSession();
            }
            sessionLst = null;
            if (skt != null) {
                skt.Close();
                skt = null;
            }
        }
    }
}
