using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace BKEUnpake.V40
{
    public class BKARCFile
    {
        private byte[] mFileData;
        private FileHeader mFileHeader;
        private TableKeyGroup mTableKeyGroup;
        private FileInfo mArcFile;
        private Dictionary<FileListTable, byte[]> mArchiveData;

        public Dictionary<FileListTable, byte[]> ArchiveData => this.mArchiveData;

        public BKARCFile(string path)
        {
            try
            {
                this.mArcFile = new FileInfo(path);
                this.mFileData=File.ReadAllBytes(this.mArcFile.FullName);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 分析文件
        /// </summary>
        /// <returns>文件表压缩数据</returns>
        public byte[] AnalysisFile()
        {
            try
            {
                
                this.mFileHeader = StructureConvert.GetStructure<FileHeader>(this.mFileData, 0);

                if (this.mFileHeader.IsVaild == false)
                {
                    return null;
                }
                
             
                uint keyGroupTableFOA = DecryptHelper.DecryptTableKeyGroupFileOffset(mFileHeader);

                
                this.mTableKeyGroup = StructureConvert.GetStructure<TableKeyGroup>(this.mFileData, (int)keyGroupTableFOA);

                uint DecompressedLength;        
                
                uint listTableLength = DecryptHelper.DecryptTableSize(this.mFileHeader, this.mTableKeyGroup, out DecompressedLength);   

                
                uint listTableFOA = keyGroupTableFOA + (uint)Marshal.SizeOf(typeof(TableKeyGroup));

                
                byte[] listTableData = new byte[listTableLength];
                Array.Copy(this.mFileData, listTableFOA, listTableData, 0, listTableLength);
                
                DecryptHelper.DecryptCompressedTable(listTableData, listTableData.Length, this.mTableKeyGroup.TableKey);

                return listTableData;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 解密文件数据
        /// </summary>
        /// <param name="uncompressedListData">解压后文件表</param>
        /// <returns></returns>
        public void DecryptArchive(byte[] uncompressedListData)
        {
            this.mArchiveData = new Dictionary<FileListTable, byte[]>();

           
            List<FileListTable> fileListTables = BKARCList.ListTableAnalysis(uncompressedListData);

            fileListTables.ForEach(fileListTable =>
            {
                byte[] buffer;
                switch (fileListTable.FileType)
                {
                    case FileType.NormalArchive:        

                        
                        buffer = new byte[fileListTable.FileSize];
                        Array.Copy(this.mFileData, fileListTable.FileOffset, buffer, 0, fileListTable.FileSize);

                      
                        uint xorKey, xorLength;
                        DecryptHelper.DecryptFileKey(fileListTable.Key, out xorKey, out xorLength);

                      
                        DecryptHelper.DecryptFile(buffer, xorKey, xorLength);

                        this.mArchiveData.Add(fileListTable, buffer);

                        break;
                    case FileType.CompressedArchive:     

                        
                        buffer = new byte[fileListTable.FileSize];
                        Array.Copy(this.mFileData, fileListTable.FileOffset, buffer, 0, fileListTable.FileSize);

                      
                        buffer = FileFix.CompressedResourcesFix(buffer);

                        this.mArchiveData.Add(fileListTable, buffer);

                        break;
                }

            });

        }
        /// <summary>
        /// 导出资源
        /// </summary>
        public void OutputArchiveData()
        {

  
            string directory = string.Concat(this.mArcFile.DirectoryName,"/Extract/",Path.GetFileNameWithoutExtension(this.mArcFile.FullName));

            if (Directory.Exists(directory) == false)
            {
                Directory.CreateDirectory(directory);
            }
                

            foreach(KeyValuePair<FileListTable,byte[]> singleArcData in this.ArchiveData)
            {
                string filePath = string.Concat(directory, "/", singleArcData.Key.FileName);

                try
                {
                    FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                    fs.Write(singleArcData.Value, 0, singleArcData.Value.Length);
                    fs.Flush();
                    fs.Close();
                    fs.Dispose();
                    fs = null;
                }
                catch 
                {
                }
            }
        }
    }
}
