using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.IO;
using System.Text;

/// <summary>
///FileOperate 的摘要说明
/// </summary>
public class FileOperate
{
    public FileOperate()
    { 
        //
        //TODO: 在此处添加构造函数逻辑
        //
    }

    /// <summary>
    /// 文件下载
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="path"></param>
    public void DownFile(HttpResponse Response, string fileName, string path)
    { 

        //以字符流的形式下载文件
        FileStream fs = new FileStream(path, FileMode.Open);
        byte[] bytes = new byte[(int)fs.Length];
        fs.Read(bytes, 0, bytes.Length);
        fs.Close();
        Response.ContentType = "application/octet-stream";
        //通知浏览器下载文件而不是打开
        Response.AddHeader("Content-Disposition", "attachment;  filename=" + HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8));
        Response.BinaryWrite(bytes);
        Response.Flush();
        Response.End();
    }

    public void DownFile(string displayName, string f_Download, string TextEncoding)
    {
        try
        {
            string DownPath = f_Download;
            String FullFileName = DownPath;
            FileInfo DownloadFile = new FileInfo(FullFileName);
            string FileExtend = System.IO.Path.GetExtension(FullFileName);

            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ClearHeaders();
            HttpContext.Current.Response.Buffer = false;
            HttpContext.Current.Response.ContentType = GetMimeType(FileExtend);

            if (TextEncoding != null)
            {
                HttpContext.Current.Response.ContentType = TextEncoding;
            }

            displayName = ToHexString(displayName);

            HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + displayName);
            HttpContext.Current.Response.AppendHeader("Content-Length", DownloadFile.Length.ToString());
            HttpContext.Current.Response.WriteFile(DownloadFile.FullName);
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();
        }
        catch (Exception ex)
        {
            throw ex;
        }
      
    }

    #region 编码

    /// <summary>
    /// 对字符串中的非 ASCII 字符进行编码
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string ToHexString(string s)
    {
        char[] chars = s.ToCharArray();
        StringBuilder builder = new StringBuilder();
        for (int index = 0; index < chars.Length; index++)
        {
            bool needToEncode = NeedToEncode(chars[index]);
            if (needToEncode)
            {
                string encodedString = ToHexString(chars[index]);
                builder.Append(encodedString);
            }
            else
            {
                builder.Append(chars[index]);
            }
        }

        return builder.ToString();
    }

    /// <summary>
    /// 判断字符是否需要使用特殊的 ToHexString 的编码方式
    /// </summary>
    /// <param name="chr"></param>
    /// <returns></returns>
    private static bool NeedToEncode(char chr)
    {
        string reservedChars = "$-_.+!*'(),@=&";

        if (chr > 127)
            return true;
        if (char.IsLetterOrDigit(chr) || reservedChars.IndexOf(chr) >= 0)
            return false;

        return true;
    }

    /// <summary>
    /// 为非 ASCII 字符编码
    /// </summary>
    /// <param name="chr"></param>
    /// <returns></returns>
    private static string ToHexString(char chr)
    {
        UTF8Encoding utf8 = new UTF8Encoding();
        byte[] encodedBytes = utf8.GetBytes(chr.ToString());
        StringBuilder builder = new StringBuilder();
        for (int index = 0; index < encodedBytes.Length; index++)
        {
            builder.AppendFormat("%{0}", Convert.ToString(encodedBytes[index], 16));
        }

        return builder.ToString();
    }



    /// <summary>
    /// 根据文件后缀来获取MIME类型字符串
    /// </summary>
    /// <param name="extension">文件后缀</param>
    /// <returns></returns>
    static string GetMimeType(string extension)
    {
        string mime = string.Empty;
        extension = extension.ToLower();
        switch (extension)
        {
            case ".avi": mime = "video/x-msvideo"; break;
            case ".bin":
            case ".exe":
            case ".msi":
            case ".dll":
            case ".class": mime = "application/octet-stream"; break;
            case ".csv": mime = "text/comma-separated-values"; break;
            case ".html":
            case ".htm":
            case ".shtml": mime = "text/html"; break;
            case ".css": mime = "text/css"; break;
            case ".js": mime = "text/javascript"; break;
            case ".doc":
            case ".dot":
            case ".docx": mime = "application/msword"; break;
            case ".xla":
            case ".xls":
            case ".xlsx": mime = "application/msexcel"; break;
            case ".ppt":
            case ".pptx": mime = "application/mspowerpoint"; break;
            case ".gz": mime = "application/gzip"; break;
            case ".gif": mime = "image/gif"; break;
            case ".bmp": mime = "image/bmp"; break;
            case ".jpeg":
            case ".jpg":
            case ".jpe":
            case ".png": mime = "image/jpeg"; break;
            case ".mpeg":
            case ".mpg":
            case ".mpe":
            case ".wmv": mime = "video/mpeg"; break;
            case ".mp3":
            case ".wma": mime = "audio/mpeg"; break;
            case ".pdf": mime = "application/pdf"; break;
            case ".rar": mime = "application/octet-stream"; break;
            case ".txt": mime = "text/plain"; break;
            case ".7z":
            case ".z": mime = "application/x-compress"; break;
            case ".zip": mime = "application/x-zip-compressed"; break;
            default:
                mime = "application/octet-stream";
                break;
        }
        return mime;
    }
    #endregion
  
    /// <summary>
    /// 上传附件(附件物理名称为随机数)
    /// </summary>
    /// <param name="fileUpload">上传控件</param>
    /// <param name="rootDir">服务器上文件夹的路径</param>
    public string UpFile(FileUpload fileUpload, string rootDir)
    {
        //判断文件夹是否存在
        if (!Directory.Exists(rootDir))
        {
            Directory.CreateDirectory(rootDir);
        }

        Random random = new Random();

        //获取客户端文件的路径，并保存到服务器上
        FileInfo file = new FileInfo(fileUpload.PostedFile.FileName);
        //文件保存到服务器上的名称
        string fileUrlName = DateTime.Now.ToString("yyyyMMddhhmmss") + random.Next(999) + file.Extension;
        //file.CopyTo(rootDir + "/" + fileUrlName);//保存到服务器上的文件 
        fileUpload.SaveAs(rootDir + "/" + fileUrlName);
        return fileUrlName;
    }

    /// <summary>
    /// 删除文件
    /// </summary>
    public void DelFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }

    /// <summary>
    /// 将文件夹的文件拷贝到另一个目录下
    /// </summary>
    public void CopyFileToOtherFolder(string folderUrl, string otherFolderUrl)
    {
        if (Directory.Exists(folderUrl))
        {
            //判断目标文件夹是否存在
            if (!Directory.Exists(otherFolderUrl))
            {
                Directory.CreateDirectory(otherFolderUrl);
            }

            //获取所有文件
            DirectoryInfo fileFolder = new DirectoryInfo(folderUrl);
            FileInfo[] fileList= fileFolder.GetFiles();
            for (int i = 0; i < fileList.Length; i++)
            {
                fileList[i].CopyTo(otherFolderUrl+"/" + fileList[i].Name);
                fileList[i].Delete();//复制完成后删除
            }
        }
    }

    /// <summary>
    /// 清空文件夹下的文件
    /// </summary>
    /// <param name="folder"></param>
    public void DelFolderFile(string folderUrl)
    {
        if (Directory.Exists(folderUrl))
        {
            DirectoryInfo fileFolder = new DirectoryInfo(folderUrl);
            FileInfo[] fileList = fileFolder.GetFiles();
            for (int i = 0; i < fileList.Length; i++)
            {
                fileList[i].Delete();
            }
        }
    }


    /// <summary>
    /// 判断文件类型
    /// </summary>
    /// <param name="type">文件的后缀名</param>
    /// <returns>是否符合   True：格式正确   False：格式错误</returns>
    public bool CheckType(string type)
    {
        #region 判断文件格式
        string[] NotAllowExtension = { ".exe", ".dll", "aspx", "html", "xml", "config" };
        bool result = false;
        for (int i = 0; i < NotAllowExtension.Length; i++)
        {
            if (!type.ToLower().Equals(NotAllowExtension[i]))
                result = true;
            else
                break;
        }
        return result;
        #endregion
    }
}
