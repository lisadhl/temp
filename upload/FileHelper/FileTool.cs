using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;

namespace Meng.Utility
{
    /// <summary>
    /// 工具箱--文件管理工具：上传、删除等
    /// </summary>
    public class FileTool
    {
        /// <summary>
        /// 验证文件大小(byte)
        /// </summary>
        /// <param name="file">上传控件</param>
        /// <param name="byteSize">大小(字节)</param>
        /// <returns></returns>
        public static bool CheckPicSize(HttpPostedFile file, int byteSize)
        {
            return file.ContentLength <= byteSize;
        }
        /// <summary>
        /// 验证文件大小(MB)
        /// </summary>
        /// <param name="file">上传控件</param>
        /// <param name="mbSize">大小(MB)</param>
        /// <returns></returns>
        public static bool CheckPicSize(HttpPostedFile file, decimal mbSize)
        {
            return (file.ContentLength/1024)<=mbSize;
        }
        /// <summary>
        /// 获取网络文件的文件流
        /// </summary>
        /// <param name="url">http或其它网络文件</param>
        /// <returns></returns>
        public static Stream GetWebFileStream(string url)
        {
            WebRequest wreq = WebRequest.Create(url);
            wreq.Timeout = 10000;

            HttpWebResponse wresp = (HttpWebResponse)wreq.GetResponse();
            return wresp.GetResponseStream();
        }
        /// <summary>
        /// 取得文件后缀
        /// </summary>
        /// <param name="filename">文件名称</param>
        /// <returns></returns>
        public static string GetFileExtends(string filename)
        {
            if (filename.LastIndexOf('.') > 0)
            {
                return filename.Substring(filename.LastIndexOf('.')+1);
            }
            return string.Empty;
        }

        /// <summary>
        /// 单文件上传，不能同名
        /// </summary>
        /// <param name="fload">上传控件</param>
        /// <param name="savepath">保存路径</param>
        /// <returns>返回为空表示存在，否则返回文件名</returns>
        public static string Upload_File_Single(System.Web.UI.WebControls.FileUpload fload, string savepath)
        {
            string filepath = fload.PostedFile.FileName;//客户端文件地址
            string filename = filepath.Substring(filepath.LastIndexOf("\\") + 1);//上传文件的文件名
            string filetype = filepath.Substring(filepath.LastIndexOf("."));//上传文件的扩展名
            int filelength = fload.PostedFile.ContentLength;//文件大小(字节)

            //如果文件存在，直接返回
            if (ExistsFile(savepath + filename))
                return string.Empty;

            fload.PostedFile.SaveAs(System.Web.HttpContext.Current.Server.MapPath(savepath) + filename);
            return filename;
        }

        /// <summary>
        /// 上传多个文件，如果文件名存在，自动重命名
        /// </summary>
        /// <param name="_savePath">保存路径</param>
        /// <returns></returns>
        public static List<string> uploadfile(string _savePath)
        {
            List<string> filelist = new List<string>();

            //以下是上传
            HttpFileCollection files = HttpContext.Current.Request.Files;
            System.Text.StringBuilder strMsg = new System.Text.StringBuilder();
            for (int iFile = 0; iFile < files.Count; iFile++)
            {
                HttpPostedFile postedFile = files[iFile];
                if (postedFile.ContentLength > 0 && postedFile.FileName.IsNotNullOrEmpty())
                {
                    string filepath = postedFile.FileName;//客户端文件地址
                    string fileName = filepath.Substring(filepath.LastIndexOf("\\") + 1);//上传文件的文件名

                    //判断文件存在的次数++
                    int file_append = 0;
                    while (System.IO.File.Exists(_savePath + "\\" + fileName))
                    {
                        file_append++;
                        fileName = System.IO.Path.GetFileNameWithoutExtension(postedFile.FileName)
                            + file_append.ToString() + fileName.Substring(fileName.LastIndexOf("."));
                    }
                    //上传
                    postedFile.SaveAs(_savePath + "\\" + fileName.ClearReplace());

                    filelist.Add(fileName.ClearReplace());
                }
            }
            return filelist;
        }

        /// <summary>
        /// 验证文件是否存在
        /// </summary>
        /// <param name="strFilePath">虚拟路径</param>
        /// <returns></returns>
        public static bool ExistsFile(string strFilePath)
        {
            return File.Exists(System.Web.HttpContext.Current.Server.MapPath(strFilePath));
        }
        /// <summary>
        /// 删除文件
        /// 前台使用：UploaderTool.DeleteFile("images/233333.jpg");
        /// </summary>
        /// <param name="strFilePath"></param>
        public static void DeleteFile(string strFilePath)
        {
            if (ExistsFile(strFilePath))
            {
                if (File.GetAttributes(HttpContext.Current.Server.MapPath(strFilePath)).ToString().IndexOf("ReadOnly") != -1)
                {
                    File.SetAttributes(HttpContext.Current.Server.MapPath(strFilePath), FileAttributes.Normal);
                }
                System.IO.File.Delete(System.Web.HttpContext.Current.Server.MapPath(strFilePath));
            }
        }
        /// <summary>
        /// 根据文件类型大类名称取子类,以 .a|.b|.c的形式返回,如果为空或*将返回*
        /// </summary>
        /// <param name="_TypeClass">大类</param>
        /// <returns></returns>
        public static string GetFileType(string _TypeClass)
        {
            string _getType = "";
            switch (_TypeClass)
            {
                case "image":
                    _getType = ".jpg|.gif|.jpeg|.png|.bmp";
                    break;
                case "jpg":
                case "jpeg":
                    _getType = ".jpg|.jpeg";
                    break;
                case "gif":
                    _getType = ".gif";
                    break;
                case "bmp":
                    _getType = ".bmp";
                    break;
                case "png":
                    _getType = ".png";
                    break;
                case "flash":
                    _getType = ".swf|.flv";
                    break;
                case "flv":
                    _getType = ".flv";
                    break;
                case "swf":
                    _getType = ".swf";
                    break;
                case "media":
                    _getType = ".mid|.mov|.mp3|.mp4|.mpc|.mpeg|.mpg|.rm|.rmi|.rmvb|.swf|.flv|.wav|.wma|.wmv";
                    break;
                case "file":
                    _getType = ".doc|.docx|.xls|.ppt|.pdf|.txt";
                    break;
                case "page":
                    _getType = ".htm|.html|.asp|.aspx|.php|.ascx|.css|.page|.js";
                    break;
                case "rar":
                    _getType = ".rar|.zip";
                    break;
                case "*":
                    _getType = ".7z|.aiff|.asf|.avi|.bmp|.csv|.doc|.fla|.flv|.gif|.gz|.gzip|.jpeg|.jpg|.mid|.mov|.mp3|.mp4|.mpc|.mpeg|.mpg|.ods|.odt|.pdf|.png|.ppt|.pxd|.qt|.ram|.rar|.rm|.rmi|.rmvb|.rtf|.sdc|.sitd|.swf|.sxc|.sxw|.tar|.tgz|.tif|.tiff|.txt|.vsd|.wav|.wma|.wmv|.xls|.xml|.zip|.htm|.html|.asp|.aspx|.php|.ascx|.css|.page|.js";
                    break;
                default:
                    _getType = ".jpg|.gif|.jpeg|.png|.bmp";
                    break;
            }
            return _getType;
        }
        /// <summary>
        /// 将属性转换为中文显示
        /// </summary>
        /// <param name="_Attributes"></param>
        /// <returns></returns>
        public static string GetFileAttributes(string _Attributes)
        {
            string _getAttributes = "";
            string[] sArrey = _Attributes.Split(',');
            for (int i = 0; i < sArrey.Length; i++)
            {
                _getAttributes += GetFileAttributess(sArrey[i].ToString().ClearReplace())+",";
            }
            if (_getAttributes.IsNotNullOrEmpty())
                _getAttributes = _getAttributes.Substring(0, _getAttributes.Length - 1);

            return _getAttributes;
        }
        /// <summary>
        /// 将属性转换为中文显示
        /// </summary>
        /// <param name="_Attributes"></param>
        /// <returns></returns>
        public static string GetFileAttributess(string _Attributes)
        {
            string _getAttributes = "不明确";
            switch (_Attributes)
            {
                case "ReadOnly":
                    _getAttributes = "只读文件";
                    break;
                case "System":
                    _getAttributes = "系统文件";
                    break;
                case "Hidden":
                    _getAttributes = "隐藏文件";
                    break;
                case "Archive":
                    _getAttributes = "归档文件";
                    break;
                case "Temporary":
                    _getAttributes = "临时文件";
                    break;
            }
            return _getAttributes;
        }
    }
}
