using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace PENet {
   public class AsyncNetClient {
       public AsyncSession session;
        private Socket skt = null;
        public void StartClient(string ip,int port) {
            try {
                skt = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
                EndPoint pt = new IPEndPoint(IPAddress.Parse(ip),port);
                skt.BeginConnect(pt,new AsyncCallback(SeverConnectCB),null);
                AsyncTool.ColorLog(AsyncLogColor.Green, "Client Start...");
            }
            catch (Exception e) {

                AsyncTool.Error(e.Message);
            }
        }
        private void SeverConnectCB(IAsyncResult ar) {
            session = new AsyncSession();
            try {
                skt.EndConnect(ar);
                if (skt.Connected) {
                    session.InitSession(skt,null);
                }
            }
            catch (Exception e) {

                AsyncTool.Error(e.Message);
            }
        
        }

        public void CloseClient() { 
        if (session != null) {
                session.CloseSession();
                session = null;
            }
            if (skt != null) {
                skt = null;
            }
        }
        
    }
}
