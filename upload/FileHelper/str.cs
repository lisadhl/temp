using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

/// <summary>
/// str 的摘要说明
/// </summary>
public class str
{
	public str()
	{
		//
		// TODO: 在此处添加构造函数逻辑
		//
	}
    /// <summary>
    /// 截取字符串函数
    /// </summary>
    /// <param name="str">所要截取的字符串</param>
    /// <param name="num">截取字符串的长度</param>
    /// <returns></returns>
    static public string Len(string str, int num)
    {
        return (str.Length > num) ? str.Substring(0, num) + "&#8230;" : str;
    }
    /// <summary>
    /// 截取字符串函数
    /// </summary>
    /// <param name="str">所要截取的字符串</param>
    /// <param name="num">截取字符串的长度</param>
    /// <param name="apostrophe">是否显示省略号</param>
    /// <returns></returns>
    static public string Len(string str, int num, bool apostrophe)
    {
        return (str.Length > num) ? str.Substring(0, num) + (apostrophe ? "&#8230;" : "") : str;
    }
    /// <summary>
    /// 截取字符串函数
    /// </summary>
    /// <param name="str">所要截取的字符串</param>
    /// <param name="num">截取字符串的长度</param>
    /// <returns></returns>
    static public string Len(object str, int num)
    {
        return (str.ToString().Length > num) ? str.ToString().Substring(0, num) + "&#8230;" : str.ToString();
    }
    /// <summary>
    /// 截取字符串函数
    /// </summary>
    /// <param name="str">所要截取的字符串</param>
    /// <param name="num">截取字符串的长度</param>
    /// <param name="apostrophe">是否显示省略号</param>
    /// <returns></returns>
    static public string Len(object str, int num, bool apostrophe)
    {
        return (str.ToString().Length > num) ? str.ToString().Substring(0, num) + (apostrophe ? "&#8230;" : "") : str.ToString();
    }
    /// <summary>
    /// 绑定图片字段
    /// </summary>
    /// <param name="str">所要截取的字符串</param>
    /// <returns></returns>
    static public string Img(string str)
    {
        return (str.Length > 1) ? str.Substring(1) : string.Empty;
    }
    /// <summary>
    /// 绑定图片字段
    /// </summary>
    /// <param name="str">图片</param>
    /// <returns></returns>
    static public string Img(object str)
    {
        return (str.ToString().Length > 1) ? str.ToString().Substring(1) : string.Empty;
    }

    /// <summary>
    /// 清除字符中的HTML代码
    /// </summary>
    /// <param name="html"></param>
    /// <returns></returns>
    public static string ClearHtml(object html)
    {
        if (html == null)
        {
            return string.Empty;
        }
        string content = Convert.ToString(html);

        content = Regex.Replace(content, @"<scrip[^<>]*?>.*?</\s*script\s*>", " ", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Singleline);
        content = Regex.Replace(content, @"<[^>]+?>", " ");
        content = content.Replace("&nbsp;", " ");
        content = content.Replace("<br/>;", " ");

        return content;
    }
    /// <summary>
    /// 截取等宽中英文字符串
    /// </summary>
    /// <param name="str">要截取的字符串</param>
    /// <param name="length">要截取的中文字符长度</param>
    /// <returns>截取后的字符串</returns>
    public static string CutStr(string str, int length)
    {
        if (str == null) return string.Empty;

        int len = length * 2;
        //aequilateLength为中英文等宽长度,cutLength为要截取的字符串长度
        int aequilateLength = 0, cutLength = 0;
        Encoding encoding = Encoding.GetEncoding("gb2312");

        string cutStr = str.ToString();
        int strLength = cutStr.Length;
        byte[] bytes;
        for (int i = 0; i < strLength; i++)
        {
            bytes = encoding.GetBytes(cutStr.Substring(i, 1));
            if (bytes.Length == 2)//不是英文
                aequilateLength += 2;
            else
                aequilateLength++;

            if (aequilateLength <= len) cutLength += 1;

            if (aequilateLength > len)
                return cutStr.Substring(0, cutLength);
        }
        return cutStr;
    }
}