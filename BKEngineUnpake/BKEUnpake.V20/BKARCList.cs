using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BKEUnpake.V21;

namespace BKEUnpake.V20
{
    /// <summary>
    /// 文件资源表
    /// </summary>
    public class BKARCList
    {
        /// <summary>
        /// 解析文件中列表信息结构
        /// </summary>
        /// <param name="filelistinfodata">文件中列表数据</param>
        /// <param name="listInfo"></param>
        public static void FileListInfoAnalysis(byte[] filelistinfodata,out FilePackageListInfo listInfo)
        {
            listInfo.ListDataSize = BitConverter.ToUInt32(filelistinfodata, 0);
            listInfo.ListCount = BitConverter.ToUInt32(filelistinfodata, 4);
            listInfo.ListDecryptKey = BitConverter.ToUInt32(filelistinfodata, 8);
        }

        /// <summary>
        /// 解密列表数据
        /// </summary>
        /// <param name="listdata">文件中列表原数据</param>
        /// <param name="listdecryptkey">表解密key</param>
        /// <param name="filekey">文件解密key</param>
        /// <returns>解密后的文件列表压缩数据</returns>
        public static byte[] DecryptList(byte[] listdata,uint listdecryptkey,out uint filekey)
        {
            
            filekey = DecryptHelper.DecryptList(listdata, (uint)listdata.Length, listdecryptkey);
            return BZip2Helper.DecompressData(listdata);           
        }

        /// <summary>
        /// 文件表解析
        /// </summary>
        /// <param name="metalistdata">文件表原数据</param>
        /// <param name="listcount">列表项项数</param>
        /// <param name="compressedreslist">压缩资源表数组</param>
        /// <param name="normalreslist">普通资源表数组</param>
        public static void ListAnalysis(byte[] metalistdata, uint listcount, out List<BZip2CompressedResources> compressedreslist, out List<NormalResources> normalreslist)
        {
            uint count = 0;               
            BZip2CompressedResources compressedRes;
            NormalResources normalRes;
            List<BZip2CompressedResources> mCompressedReslist = new List<BZip2CompressedResources>();
            List<NormalResources> mNormalReslist = new List<NormalResources>();

           
            uint listPointer = 0;

            while (count < listcount)
            {
               
                uint utf8strLength;
                
                string fileName = StructureConvert.GetUTF8String(metalistdata, listPointer,out utf8strLength);

                
                listPointer += utf8strLength+1;

                uint type = BitConverter.ToUInt32(metalistdata, (int)(listPointer + 0x8));
                if (type == 0)

                {  
                    normalRes.FileName = fileName;
                    normalRes.FileOffset = BitConverter.ToUInt32(metalistdata, (int)(listPointer));
                    normalRes.FileSize = BitConverter.ToUInt32(metalistdata, (int)(listPointer + 0x4));
                    normalRes.ResourcesType = 0;
                    mNormalReslist.Add(normalRes);       
                    listPointer += 0x0C;
                }
                else if (type == 1)

                {   
                    compressedRes.FileName= fileName;
                    compressedRes.FileOffset = BitConverter.ToUInt32(metalistdata, (int)(listPointer));
                    compressedRes.UncompressedSize = BitConverter.ToUInt32(metalistdata, (int)(listPointer + 0x4));
                    compressedRes.ResourcesType = 1;
                    compressedRes.FileSize = BitConverter.ToUInt32(metalistdata, (int)(listPointer + 0xC));
                    mCompressedReslist.Add(compressedRes);  
                    listPointer += 0x10;
                }
                count++;            
            }
            compressedreslist = mCompressedReslist;     
            normalreslist = mNormalReslist;
        }
    }
}
