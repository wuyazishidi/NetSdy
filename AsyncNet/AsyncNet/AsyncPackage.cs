﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PENet {
    public class AsyncPackage {
        public const int headLen = 4;
        public byte[] headBuff = null;
        public int headIndex = 0;

        public int bodyLen = 0;
        public byte[] bodyBuff = null;
        public int bodyIndex = 0;

        public AsyncPackage() {
            headBuff = new byte[4];
        }

        public void InitBodyBuff() {
            bodyLen = BitConverter.ToInt32(headBuff,0);
            bodyBuff = new byte[bodyLen];
        }
        public void ResetData() {
            headIndex = 0;
            bodyLen = 0;
            bodyBuff = null;
            bodyIndex = 0;
        }
    }
}
