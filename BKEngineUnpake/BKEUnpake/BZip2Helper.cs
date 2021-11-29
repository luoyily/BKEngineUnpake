using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ICSharpCode.SharpZipLib.BZip2;

namespace BKEUnpake
{
    /// <summary>
    /// 数据解压
    /// </summary>
    public class BZip2Helper
    {
        /// <summary>
        /// 解压数据
        /// </summary>
        /// <param name="data">压缩包数据</param>
        /// <returns>解压后的数据  返回null则为解压失败</returns>
        public static byte[] DecompressData(byte[] data)
        {
            MemoryStream ms = new MemoryStream(data);
            try
            {
                List<byte> metadata = new List<byte>();
               
                BZip2InputStream unbZip2 = new BZip2InputStream(ms);
                int temp;
                while (true)
                {   
                    temp = unbZip2.ReadByte();
                    if (temp == -1)
                    {   
                       
                        break;
                    }
                    metadata.Add((byte)(uint)temp);        
                }

                return metadata.ToArray();                 
            }
            catch
            {
                return null;
            }
            finally
            {
                ms?.Close();                 //释放资源
                ms?.Dispose();
            }
        }
        /// <summary>
        /// 解压一组压缩数据
        /// </summary>
        /// <param name="data">一组压缩数据</param>
        /// <returns>解压后的数据数组 null为解压失败</returns>
        public static List<List<byte>> DecompressData(List<List<byte>> data)
        {
            List<List<byte>> unzipdata = new List<List<byte>>();        //定义解压后数据数组
            foreach(List<byte> databuffer in data)
            {
                //解压数据
                byte[] unzipbuffer = DecompressData(databuffer.ToArray());
                //解压失败
                if (unzipbuffer == null)
                {
                    return null;           
                }
                unzipdata.Add(unzipbuffer.ToList());     //添加到已解压数组
            }
            return unzipdata;
        }

    }
}
