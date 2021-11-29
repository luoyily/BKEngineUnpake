using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace BKEUnpake
{
    public class StructureConvert
    {
        /// <summary>
        /// 将指定字节数据以Ascii编码模式转化为字符串
        /// </summary>
        /// <param name="data">字节流数据</param>
        /// <param name="offset">数据偏移</param>
        /// <param name="length">字符串长度</param>
        /// <returns></returns>
        public static string GetUTF8String(byte[] data, uint offset,out uint length)
        {
            List<byte> byteAsciiString = new List<byte>();
            uint dataindex = offset;                       
            while (data[dataindex] != 0)
            {
             
                byteAsciiString.Add(data[dataindex]);     
                dataindex++;
            }
           
            length = dataindex - offset;
            return Encoding.UTF8.GetString(byteAsciiString.ToArray());
        }

        /// <summary>
        /// 将字节流数据转化为结构体类型
        /// </summary>
        /// <typeparam name="T">结构体类型</typeparam>
        /// <param name="data">字节流数据</param>
        /// <param name="offset">起始偏移</param>
        /// <returns>指定类型结构体数据</returns>
        public static T GetStructure<T>(byte[] data, int offset = 0)
        {
            int size = Marshal.SizeOf(typeof(T));                       
            IntPtr unmanagedMemory = Marshal.AllocHGlobal(size);      
            if (size > data.Length)
            {
                size = Math.Min(data.Length, size);        
            }
            Marshal.Copy(data, offset, unmanagedMemory, size);         
            var structure = (T)Marshal.PtrToStructure(unmanagedMemory, typeof(T));      
            Marshal.FreeHGlobal(unmanagedMemory);                 
            return structure;
        }
    }
}
