<%@ webhandler Language="C#" class="Upload" %>


using System;
using System.Collections;
using System.Web;
using System.IO;
using System.Globalization;
using LitJson;
/// <summary>
/// 件图片上传后台一般程序
/// </summary>
public class Upload : IHttpHandler
{
	private HttpContext _context;

	public void ProcessRequest(HttpContext context)
	{
        _context = context;
        
        string aspxUrl = _context.Request.Path.Substring(0, context.Request.Path.LastIndexOf("/") + 1);
        string p = _context.Request["p"];
        //创建临时保存路径
        string tempSavePath = "~/Files/Attachment_Temp/";
        if (!Directory.Exists(_context.Server.MapPath(tempSavePath)))
        {
            Directory.CreateDirectory(_context.Server.MapPath(tempSavePath));
        }
        //获取文件
		HttpPostedFile imgFile = _context.Request.Files["imgFile"];
		if (imgFile == null)
		{
			showError("请选择文件！");
		}
        //检测图片大小
        int maxSize = 1024 * 1024; //最大文件大小，1MB
		if (imgFile.InputStream == null || imgFile.InputStream.Length > maxSize)
		{
			showError("上传图片大小不能超过1MB！");
		}
        //检测图片类型
        string fileName = imgFile.FileName;//获取文件完全限定名称
        string fileExt = Path.GetExtension(fileName).ToLower(); //获取文件后缀名，如：.jpg
        if (p == "0")
        {
            string[] extArray = { "doc", "docx" };//定义允许上传的文件扩展名
            if (string.IsNullOrEmpty(fileExt) || Array.IndexOf(extArray, fileExt.Substring(1).ToLower()) == -1)
            {
                showError("只允许上传word文档！");
            }
        }
        else
        {
            string[] extArray = { "gif", "jpg", "jpeg", "png", "bmp" };//定义允许上传的文件扩展名
            if (string.IsNullOrEmpty(fileExt) || Array.IndexOf(extArray, fileExt.Substring(1).ToLower()) == -1)
            {
                showError("只允许上传gif，jpg，jpeg，png，bmp格式图片！");
            }
        }
        //生成随机文件名
        string tempFileName = System.Guid.NewGuid().ToString() + fileExt;
        string filePath = tempSavePath + tempFileName;
        //保存文件到服务器
		imgFile.SaveAs(_context.Server.MapPath( filePath));

        //返回保存文件路径结果集
        string fileUrl = tempSavePath + tempFileName;
        Hashtable result = new Hashtable(); //返回结果Hash表
		result["error"] = 0;
		result["url"] = fileUrl;
        Write(JsonMapper.ToJson(result));
	}

    /// <summary>
    /// 返回错误信息Json
    /// </summary>
	private void showError(string message)
	{
        Hashtable result = new Hashtable();//返回结果Hash表
		result["error"] = 1;
		result["message"] = message;
		Write(JsonMapper.ToJson(result));
	}
    /// <summary>
    /// 输出结果字字符串
    /// </summary>
    /// <param name="mes"></param>
    private void Write(string mes)
    {
        _context.Response.AddHeader("Content-Type", "text/html; charset=UTF-8");
        _context.Response.Write(mes);
        _context.Response.End();
    }

	public bool IsReusable
	{
		get
		{
			return true;
		}
	}
}
