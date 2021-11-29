using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BKEUnpake.V21
{
    /// <summary>
    /// 数据解密
    /// </summary>
    public class DecryptHelper
    {
        /// <summary>
        /// 常量key1
        /// </summary>
        public static uint constKey1 = 0x811C9DC5;
        /// <summary>
        /// 常量key2
        /// </summary>
        public static uint constKey2 = 0x01000193;
        /// <summary>
        /// 常量key3
        /// </summary>
        public static uint constKey3 = 0x5D588B65;
        /// <summary>
        /// 常量key4
        /// </summary>
        private static byte[] constKey4 = new byte[32]
        {
            0xCD,0x1E,0xCE,0x21,    0x18,0xD6,0x72,0x6C,    0x78,0x55,0x4C,0x90,    0x88,0x59,0x92,0xF1,
            0xD5,0x50,0xE3,0x9C,    0xC0,0x14,0xEE,0xD0,    0xD4,0x59,0x42,0xDE,    0xDD,0x22,0x56,0x39
        };
        /// <summary>
        /// 列表解密
        /// </summary>
        /// <param name="databuffer">数据字节数组</param>
        /// <param name="datalength">数据长度</param>
        /// <param name="listkey">解密listkey</param>
        /// <returns>文件key</returns>
        public static uint DecryptList(byte[] databuffer,uint datalength,uint listkey)
        {
            uint index = 0;                        
            uint nowlength = datalength;           
            uint temp=listkey;                 
            while(index < datalength)
            {
                byte singledata = databuffer[index];   
                uint uint_data = Convert.ToUInt32(singledata);     
                
                temp = temp ^ uint_data;
                temp = temp + constKey3;
                temp = temp + (uint)((int)uint_data * (int)nowlength);

                byte xorkey = (byte)(temp & 0x000000FF);         
                databuffer[index] = (byte)(singledata ^ xorkey);   
                index++;                    
                nowlength--;            
            }
            return temp;       
        }
        /// <summary>
        /// 解密文件listkey索引
        /// </summary>
        /// <param name="rangekey">索引范围listkey</param>
        /// <param name="indexrange">索引范围</param>
        /// <param name="unknowkey"></param>
        /// <returns></returns>
        public static uint DecryptIndex(uint rangekey,uint indexrange,uint unknowkey)
        {
            uint key1 = unknowkey & 0x000000FF;                      
            uint key2 = (unknowkey & 0x0000FF00)>>8;
            uint key3 = (unknowkey & 0x00FF0000) >> 16;
            uint key4 = (unknowkey & 0xFF000000) >> 24;
            
            uint result = 0;
            result = (uint)((int)(key1 ^ constKey1) * (int)constKey2);
            result = (uint)((int)(key2 ^ result) * (int)constKey2);
            result = (uint)((int)(key3 ^ result) * (int)constKey2);
            result = (uint)((int)(key4 ^ result) * (int)constKey2);
            result = result & rangekey;

            if (indexrange > result)
            {   
                return result;
            }
            result = result - 1 - (rangekey >> 1);
            return result;
        }
        /// <summary>
        /// 解密文件
        /// </summary>
        /// <param name="filebuffer">文件数据流</param>
        /// <param name="filekey">解密listkey</param>
        /// <param name="filesize">文件大小</param>
        /// <param name="fileoffset">文件在封包内偏移</param>
        /// <param name="memoryoffset">文件读取偏移</param>
        /// <returns>True为文件进行解密操作 False为未进行解密操作</returns>
        public static byte[] DecryptFile(byte[] filebuffer,int filekey,uint filesize,uint fileoffset,uint memoryoffset=0)
        {
            uint decryptfilesize;                              
            uint thisdecryptlength=filesize;           
            uint actuallength;                               
            decryptfilesize = ((uint)(filekey % 0x00000023) + 2 * fileoffset + 3 * filesize) % 0x000001E8 + 0x000000EC;  
            if (memoryoffset < decryptfilesize)
            {
                actuallength = decryptfilesize - memoryoffset;       
                if (actuallength < filesize)
                {   
                    thisdecryptlength = actuallength;
                }
                if (thisdecryptlength > 0)
                {   
                     for(uint index = 0; index < thisdecryptlength; index++)
                     {
                        
                        uint uint_xorkey = (7 * fileoffset + (uint)(filekey % 0x0000001B) + 5 * filesize) % 0x000000F1 + 0x0000000B;

                        byte b_xorkey = (byte)(uint_xorkey & 0x000000FF);           
                        filebuffer[index] = (byte)(filebuffer[index] ^ b_xorkey);           
                     }
                }
                return filebuffer;
            }
            return null;
        }
    }
}
