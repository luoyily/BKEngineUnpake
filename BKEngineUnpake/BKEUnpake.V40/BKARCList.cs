using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace BKEUnpake.V40
{
    public class BKARCList
    {
        /// <summary>
        /// 分析文件表
        /// </summary>
        /// <param name="listTableData">文件表数据</param>
        /// <returns></returns>
        public static List<FileListTable> ListTableAnalysis(byte[] listTableData)
        {
            List<FileListTable> fileTable = new List<FileListTable>();

            uint listDataPointer =(uint)Marshal.SizeOf(typeof(ListHeader));

          
            while (listDataPointer < (listTableData.Length-0x11))
            {
                
                FileType fileType = (FileType)BitConverter.ToUInt32(listTableData, (int)listDataPointer);
                listDataPointer += 4;
                uint fileSize = BitConverter.ToUInt32(listTableData, (int)listDataPointer);
                listDataPointer += 4;
                uint fileOffset = BitConverter.ToUInt32(listTableData, (int)listDataPointer);
                listDataPointer += 4;
                uint fileKeyOrDecompressLength = BitConverter.ToUInt32(listTableData, (int)listDataPointer);
                listDataPointer += 4;

                uint fileNameStrLength;
                string fileName = StructureConvert.GetUTF8String(listTableData, listDataPointer, out fileNameStrLength);

                listDataPointer += fileNameStrLength + 1;       

                listDataPointer = AilgnmentManagar.GetAlignment(listDataPointer, 8);     

                switch (fileType)
                {
                    case FileType.NormalArchive:       
                        FileListTable fileNor = new FileListTable()
                        {
                            FileType = fileType,
                            FileSize=fileSize,
                            FileOffset=fileOffset,
                            Key= fileKeyOrDecompressLength,
                            FileName=fileName
                        };
                        fileTable.Add(fileNor);
                        break;
                    case FileType.Signuature:      
                        break;
                    case FileType.CompressedArchive:     
                        FileListTable fileCom = new FileListTable()
                        {
                            FileType = fileType,
                            FileSize = fileSize,
                            FileOffset = fileOffset,
                            DecompressLength = fileKeyOrDecompressLength,
                            FileName = fileName
                        };
                        fileTable.Add(fileCom);
                        break;
                }

            }
            return fileTable;
        }
    }
}
