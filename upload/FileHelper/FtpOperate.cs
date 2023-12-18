using Meng.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

public class FtpOperate
{
    private string ftpServerIp;
    private string ftpUserID;
    private string ftpPassword;
    public FtpOperate(string ftpServerIp,string ftpUserID, string ftpPassword)
    {
        this.ftpServerIp = ftpServerIp;
        this.ftpUserID = ftpUserID;
        this.ftpPassword = ftpPassword;
    }
    public FtpOperate() { }
    #region 判断ftp服务器上该目录是否存在  
    /// <summary>  
    /// 判断ftp服务器上该目录是否存在  
    /// </summary>  
    /// <param name="ftpPath">FTP路径目录</param>  
    /// <param name="dirName">目录上的文件夹名称</param>  
    /// <returns></returns>  
    public bool CheckDirectoryExist(string dirName)
    {
        bool flag = true;
        try
        {
            //实例化FTP连接  
            FtpWebRequest ftp = (FtpWebRequest)FtpWebRequest.Create(ftpServerIp + dirName);
            ftp.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
            ftp.Method = WebRequestMethods.Ftp.ListDirectory;
            FtpWebResponse response = (FtpWebResponse)ftp.GetResponse();
            response.Close();
        }
        catch (Exception)
        {
            flag = false;
        }
        return flag;
    }
    #endregion

    #region 上传文件
 /// <summary>
 /// ftp上传
 /// </summary>
 /// <param name="ftpfilepath">服务器文件路径</param>
 /// <param name="inputfilepath">本地文件路径</param>
    public bool ftpfile(string ftpfilepath, string inputfilepath)
    {
        bool states = false;
        try
        {
            string ftpfullpath = "ftp://" + ftpServerIp + ftpfilepath;
            FtpWebRequest ftp = (FtpWebRequest)FtpWebRequest.Create(ftpfullpath);
            ftp.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
            ftp.KeepAlive = true;
            ftp.UseBinary = true;
            ftp.UsePassive = true;

            ftp.Method = WebRequestMethods.Ftp.UploadFile;
            FileStream fs = File.OpenRead(inputfilepath);
            byte[] buffer = new byte[fs.Length];
            fs.Read(buffer, 0, buffer.Length);
            fs.Close();
            Stream ftpstream = ftp.GetRequestStream();
            ftpstream.Write(buffer, 0, buffer.Length);
            ftpstream.Close();
            states = true;
        }
        catch
        {
            states = false;
        }
        return states;
    }

    #endregion


    #region 获取服务文件内容
    /// <summary>  
    ///从ftp服务器上下载文件的功能  
    /// </summary>  
    /// <param name="ftpUrl">ftp地址</param>  
    /// <param name="filePath">本地文件路径</param>  
    /// <param name="fileName">服务器文件夹名称</param>  
    public void DownloadFtpFile(string filePath, string fileName)
    {
        FtpWebRequest reqFTP = null;
        FtpWebResponse response = null;
        try
        {
            string url = "ftp://" + ftpServerIp + "/";
            string ftpfullpath = url + fileName;
            string localpath = filePath + fileName;
            FileStream outputStream = new FileStream(localpath, FileMode.Create);
            reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpfullpath));
            reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
            reqFTP.UseBinary = true;
            reqFTP.UsePassive = true;
            reqFTP.KeepAlive = true;
            reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
            response = (FtpWebResponse)reqFTP.GetResponse();
            Stream ftpStream = response.GetResponseStream();
            long cl = response.ContentLength;
            int bufferSize = 2048;
            int readCount;
            byte[] buffer = new byte[bufferSize];
            readCount = ftpStream.Read(buffer, 0, bufferSize);
            while (readCount > 0)
            {
                outputStream.Write(buffer, 0, readCount);
                readCount = ftpStream.Read(buffer, 0, bufferSize);
            }
            ftpStream.Close();
            outputStream.Close();
            response.Close();


        }
        catch (Exception ex)
        {
            throw ex;
        }
    }



    /// <summary>
    /// 从ftp服务器上获取文件并将内容全部转换成string返回
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="dir"></param>
    /// <returns></returns>
    public string GetFileStr(string fileName, string dir)
    {
        FtpWebRequest reqFTP;
        try
        {
            string ftpfullpath = "ftp://" + ftpServerIp + "/";
            reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpfullpath + dir + "/" + fileName));
            reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
            reqFTP.UseBinary = true;
            reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
            reqFTP.UsePassive = false; //选择主动还是被动模式 , 这句要加上的。
            reqFTP.KeepAlive = false;//一定要设置此属性，否则一次性下载多个文件的时候，会出现异常。
            FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
            Stream ftpStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(ftpStream);
            string fileStr = reader.ReadToEnd();

            reader.Close();
            ftpStream.Close();
            response.Close();
            return fileStr;
        }
        catch (Exception ex)
        {
            new Exception("获取ftp文件并读取内容失败：" + ex.Message);
            return null;
        }
    }

    #endregion

    #region 删除服务器上的文件
    /// <summary>
    /// 删除文件
    /// </summary>
    /// <param name="fileName">服务器下的相对路径 包括文件名</param>
    public void DeleteFileName(string fileName)
    {
        try
        {
            string ftpfullpath = "ftp://" + ftpServerIp + "/";
            //FileInfo fileInf = new FileInfo(ftpfullpath + "" + fileName);
            string uri = ftpfullpath + fileName;
            FtpWebRequest reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
            // 指定数据传输类型
            reqFTP.UseBinary = true;
            // ftp用户名和密码
            reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
            // 默认为true，连接不会被关闭
            // 在一个命令之后被执行
            reqFTP.KeepAlive = false;
            // 指定执行什么命令
            reqFTP.Method = WebRequestMethods.Ftp.DeleteFile;
            FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
            response.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine("删除文件出错：" + ex.Message);
        }
    }

    #endregion
    private static FtpWebRequest GetRequest(string URI, string username, string password)
    {
        //根据服务器信息FtpWebRequest创建类的对象
        FtpWebRequest result = (FtpWebRequest)FtpWebRequest.Create(URI);
        //提供身份验证信息
        result.Credentials = new System.Net.NetworkCredential(username, password);
        //设置请求完成之后是否保持到FTP服务器的控制连接，默认值为true
        result.KeepAlive = false;
        return result;
    }
    #region 获取远程文件夹内容列表
    /// <summary>
    /// 获取文件名列表
    /// </summary>
    /// <param name="filename">文件夹名称</param>
    /// <returns></returns>
    public string[] GetFtpFileName(string filename)
    {
         FtpWebRequest ftpRequest = null;
         FtpWebResponse ftpResponse = null;
         Stream ftpStream = null;
         string[] downloadFiles;
        StringBuilder result = new StringBuilder();
        try
        {
            string ftpfullpath = "ftp://" + ftpServerIp + "/";
            string uri = ftpfullpath + filename;
            ftpRequest = (FtpWebRequest)FtpWebRequest.Create(uri);
            ftpRequest.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
            ftpRequest.UseBinary = true;
            ftpRequest.UsePassive = true;
            ftpRequest.KeepAlive = true;
            ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;
            ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
            ftpStream = ftpResponse.GetResponseStream();
            StreamReader ftpReader = new StreamReader(ftpStream);
            string line = ftpReader.ReadLine();
            while (line != null)
            {
                result.Append(line);
                result.Append("\n");
                line = ftpReader.ReadLine();
            }
            result.Remove(result.ToString().LastIndexOf('\n'), 1);

            ftpReader.Close();
            ftpStream.Close();
            ftpResponse.Close();
            ftpRequest = null;
            return result.ToString().Split('\n');

        }
        catch (Exception ex)
        {
            downloadFiles = null;
            return downloadFiles;
        }
    }
    #endregion

    #region 写文件并保存
    /// <summary>
    /// 写文件并保存
    /// </summary>
    /// <param name="path">文件所保存路径</param>
    /// <param name="fContent">文件内容</param>
    public bool WritelocalInfo(string strTemp, string str)
    {
        bool states = false;
        StreamWriter sw = new StreamWriter(strTemp);
        try
        {
            sw.Write(str);
            states = true;
        }
        catch (Exception e)
        {
            sw.Write(e.ToString());

        }
        finally
        {
            sw.Close();
            sw.Dispose();

        }
        return states;

    }
    #endregion
}

