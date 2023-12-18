using System;
using System.Collections;
using System.IO;

namespace Meng.Utility
{

    /***************************************************************************************
    //调用示例：
    //复制内容到剪贴板 程序代码
    using System;
    using System.IO;
    using Meng.Utility;

    namespace ConsoleApplication1
    {
        class Program
        {
            static void Main(string[] args)
            {
                DirectoryInfo dirInfo = new DirectoryInfo(@"F:\abc");
                FileInfo[] lstFile = dirInfo.GetFiles();
                Array.Sort(lstFile, new FileSort(FileOrder.LastWriteTime)); //按修改日期升序排列
                foreach (FileInfo file in lstFile)
                    Console.WriteLine(file.Name);

                Console.Read();

            }
        }    
    }
    ********************************************************************************************/
    /// <summary>
    /// 文件排序类
    /// </summary>
    public class FileSort : IComparer
    {
        private FileOrder _fileorder;
        private FileAsc _fileasc;

        /// <summary>
        /// 构造函数
        /// </summary>
        public FileSort() : this(FileOrder.Name, FileAsc.Asc) { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="fileorder"></param>
        public FileSort(FileOrder fileorder) : this(fileorder, FileAsc.Asc) { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="fileorder"></param>
        /// <param name="fileasc"></param>
        public FileSort(FileOrder fileorder, FileAsc fileasc)
        {
            _fileorder = fileorder;
            _fileasc = fileasc;
        }

        /// <summary>
        /// 比较函数
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(object x, object y)
        {
            FileInfo file1 = x as FileInfo;
            FileInfo file2 = y as FileInfo;
            FileInfo file3;

            if (file1 == null || file2 == null)
                throw new ArgumentException("参数不是FileInfo类实例.");

            if (_fileasc == FileAsc.Desc)
            {
                file3 = file1;
                file1 = file2;
                file2 = file3;
            }

            switch (_fileorder)
            {
                case FileOrder.Name:
                    return file1.Name.CompareTo(file2.Name);
                case FileOrder.Length:
                    return file1.Length.CompareTo(file2.Length);
                case FileOrder.Extension:
                    return file1.Extension.CompareTo(file2.Extension);
                case FileOrder.CreationTime:
                    return file1.CreationTime.CompareTo(file2.CreationTime);
                case FileOrder.LastAccessTime:
                    return file1.LastAccessTime.CompareTo(file2.LastAccessTime);
                case FileOrder.LastWriteTime:
                    return file1.LastWriteTime.CompareTo(file2.LastWriteTime);
                default:
                    return 0;
            }
        }
    }

    /// <summary>
    /// 排序依据
    /// </summary>
    public enum FileOrder
    {
        /// <summary>
        /// 文件名
        /// </summary>
        Name,
        /// <summary>
        /// 大小
        /// </summary>
        Length,
        /// <summary>
        /// 类型
        /// </summary>
        Extension,
        /// <summary>
        /// 创建时间
        /// </summary>
        CreationTime,
        /// <summary>
        /// 访问时间
        /// </summary>
        LastAccessTime,
        /// <summary>
        /// 修改时间
        /// </summary>
        LastWriteTime
    }

    /// <summary>
    /// 升序降序
    /// </summary>
    public enum FileAsc
    {
        /// <summary>
        /// 升序
        /// </summary>
        Asc,
        /// <summary>
        /// 降序
        /// </summary>
        Desc
    }
}
