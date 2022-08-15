using AsyncProtocol;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace PENet {
    public enum AsyncSessionState {
        None,
        Connected,
        DisConnected
    }
    public class AsyncSession {
        private Socket skt;
        private Action closeCB;
        private AsyncSessionState sessionState = AsyncSessionState.None;
        public bool IsConnected { 
        get {
                return sessionState == AsyncSessionState.Connected;
            }
        }
        public void InitSession(Socket skt,Action closeCB) {
            bool result = false;
            try {
                this.skt = skt;
                this.closeCB = closeCB;
                AsyncPackage pack = new AsyncPackage();
                skt.BeginReceive(pack.headBuff, 0, AsyncPackage.headLen, SocketFlags.None, new AsyncCallback(RcvHeadData), pack);
                result = true;
                sessionState = AsyncSessionState.Connected;
            }
            catch (Exception e) {
                AsyncTool.Error(e.Message);
            }
            finally {
                OnConnected(result);
            }
        }
        private void RcvHeadData(IAsyncResult ar) {
            try {
                if (skt == null || skt.Connected == false) {
                    AsyncTool.Warn("Socket is null or not connected");
                    return;
                }
                AsyncPackage pack = (AsyncPackage)ar.AsyncState;
                int len = skt.EndReceive(ar);
                if (len==0) {
                    AsyncTool.ColorLog(AsyncLogColor.Yellow,"远程连接正常下线");
                    CloseSession();
                    return;
                }
                else {
                    pack.headIndex += len;
                    if (pack.headIndex < AsyncPackage.headLen) {
                        skt.BeginReceive(pack.headBuff,pack.headIndex,AsyncPackage.headLen-pack.headIndex,SocketFlags.None,new AsyncCallback(RcvHeadData),pack);
                    }
                    else {
                        pack.InitBodyBuff();
                        skt.BeginReceive(pack.bodyBuff,0,pack.bodyLen,SocketFlags.None,new AsyncCallback(RcvBodyData),pack);
                    }
                }
            }
            catch (Exception e) {

                AsyncTool.Warn("RcvHeadWarn:{0}",e.ToString());
                CloseSession();
            }
        }
        void RcvBodyData(IAsyncResult ar) {
            try {
                if (skt == null || skt.Connected == false) {
                    AsyncTool.Warn("Socket is null or not connected");
                    return;
                }
                int len = skt.EndReceive(ar);
                AsyncPackage pack = (AsyncPackage)ar.AsyncState;
                if (len == 0) {
                    AsyncTool.ColorLog(AsyncLogColor.Yellow, "远程连接正常下线");
                    CloseSession();
                    return;
                }
                pack.bodyIndex += len;
                if (pack.bodyIndex < pack.bodyLen) {
                    skt.BeginReceive(pack.bodyBuff,pack.bodyIndex,pack.bodyLen-pack.bodyIndex,SocketFlags.None,new AsyncCallback(RcvBodyData),pack);
                }
                else {
                    //TODO反序列化，处理网络消息的业务逻辑
                    AsyncMsg msg = AsyncTool.DeSerialize(pack.bodyBuff);
                    OnReceiveMsg(msg);
                    pack.ResetData();
                    skt.BeginReceive(pack.headBuff,0,AsyncPackage.headLen,SocketFlags.None,new AsyncCallback(RcvHeadData),pack);
                }
            }
            catch (Exception e) {
                AsyncTool.Warn("RcvBodyWarn:{0}", e.ToString());
                CloseSession();
            }
        }
        public bool SendMsg(AsyncMsg msg) {
            byte[] data = AsyncTool.PackLenInfo(AsyncTool.Serialize(msg));
            return SendMsg(data);
        }
        public bool SendMsg(byte[] data) {
            bool result = false;
            if (sessionState != AsyncSessionState.Connected) {
                AsyncTool.Warn("Connection is Disconnected can not send net msg");
            }
            else {
                NetworkStream ns = null;
                try {
                    ns = new NetworkStream(skt);
                    if (ns.CanWrite) {
                    ns.BeginWrite(data,0,data.Length,new AsyncCallback(SendCB),ns);
                    }
                    result = true;
                }
                catch (Exception e) {
                    AsyncTool.Error("SndMsgNSError{0}", e.ToString());
                }
            }
            return result;
        }
        private void SendCB(IAsyncResult ar) {
            NetworkStream ns = (NetworkStream)ar.AsyncState;
            try {
                ns.EndWrite(ar);
                ns.Flush();
                ns.Close();

            }
            catch (Exception e) {
                AsyncTool.Error("SndMsgNSError{0}", e.ToString());
            }
        }
        void OnConnected(bool result) {
            AsyncTool.Log("Client OnLine:" + result);
        }

        void OnDisConnected() {
            AsyncTool.Log("OffLine");
        }
        void OnReceiveMsg(AsyncMsg msg) {
            AsyncTool.Log("RcvMsg:"+msg.hellomsg);
        }
        public void CloseSession() {
            sessionState = AsyncSessionState.DisConnected;
            OnDisConnected();
            closeCB?.Invoke();
            try {
                if (skt != null) {
                    skt.Shutdown(SocketShutdown.Both);
                    skt.Close();
                    skt = null;
                }
            }
            catch (Exception e) {
                AsyncTool.Error("ShutDown Socket Error:{0}",e.Message);
            }
        }
    }
}
