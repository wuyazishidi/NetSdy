using System;
using System.IO;
using AsyncProtocol;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace PENet {
    public class AsyncTool {
        public static byte[] PackLenInfo(byte[] data) {
            int len = data.Length;
            byte[] pkg = new byte[len+4];
            byte[] head = BitConverter.GetBytes(len);
            head.CopyTo(pkg,0);
            data.CopyTo(pkg,4);
            return pkg;
        }
        public static byte[] Serialize(AsyncMsg msg) {
            byte[] data = null;
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            try {
                bf.Serialize(ms, msg);
                ms.Seek(0, SeekOrigin.Begin);
                data = ms.ToArray();
            }
            catch (SerializationException e) {
                Error("Faild to Serialize.Reason:{0}", e.Message);
            }
            finally {
                ms.Close();
            }
            return data;
        }

        public static AsyncMsg DeSerialize(byte[]bytes) {
            AsyncMsg msg = null; 
            MemoryStream ms = new MemoryStream(bytes);
            BinaryFormatter bf = new BinaryFormatter();
            try {
               msg=(AsyncMsg)bf.Deserialize(ms);
            }
            catch (SerializationException e) {
                Error("Faild to DeSerialize.Reason:{0} bytesLen{1}", e.Message,bytes.Length);
            }
            finally {
                ms.Close();
            }
            return msg;
        }
        #region Log
        public static Action<string> LogFunc;
        public static Action<string> ColorLogFunc;
        public static Action<string> WarnFunc;
        public static Action<string> ErrorFunc;

        public static void Log(string msg,params object[]args) {
            msg = string.Format(msg,args);
            if (LogFunc != null) {
                LogFunc(msg);
            }
            else {
                Console.WriteLine(msg);
            }
        }
        public static void Warn(string msg, params object[] args) {
            msg = string.Format(msg, args);
            if (WarnFunc != null) {
                WarnFunc(msg);
            }
            else {
                Console.WriteLine(msg);
            }
        }
        public static void Error(string msg, params object[] args) {
            msg = string.Format(msg, args);
            if (ErrorFunc != null) {
                ErrorFunc(msg);
            }
            else {
                Console.WriteLine(msg);
            }
        }
        public static void ColorLog(AsyncLogColor color,string msg, params object[] args) {
            msg = string.Format(msg, args);
            if (ColorLogFunc != null) {
                ColorLogFunc(msg);
            }
            else {
                ConsoleLog(msg, color);
            }
        }
        public static void ConsoleLog(string msg,AsyncLogColor color) {
            switch (color) {
                case AsyncLogColor.DarkRed:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine(msg);
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                case AsyncLogColor.Green:
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(msg);
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                case AsyncLogColor.Blue:
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine(msg);
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                case AsyncLogColor.Cyan:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine(msg);
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                case AsyncLogColor.Magenta:
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine(msg);
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                case AsyncLogColor.Yellow:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(msg);
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                case AsyncLogColor.None:
                default:
                    Console.WriteLine(msg);
                    break;
            }
        }
        #endregion
    }

    public enum AsyncLogColor { 
        None,
        DarkRed,
        Green,
        Blue,
        Cyan,
        Magenta,
        Yellow
    }
}
