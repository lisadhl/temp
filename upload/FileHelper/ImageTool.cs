using System;
using System.IO;
using System.Web;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;

namespace Meng.Utility
{
    /// <summary>
    /// 图片处理工具类
    /// </summary>
    public class ImageTool
    {
        /// <summary>
        /// 是否为指定的图片格式
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static bool IsPicture(HttpPostedFile file)
        {
            return IsPicture(file.FileName);
        }
        /// <summary>
        /// 是否为指定的图片格式
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static bool IsPicture(string fileName)
        {
            string[] exts = "jpg,jpeg,png,gif".Split(new char[] { ',' });//只设置常用的图片格式
            List<string> extList = new List<string>();
            extList.AddRange(exts);
            string ext = Path.GetExtension(fileName).Substring(1).ToLower();
            return extList.Contains(ext);
        }
        /// <summary>
        /// 验证图片宽、高
        /// </summary>
        /// <param name="_FileUpload">上传控件</param>
        /// <param name="_width">宽</param>
        /// <param name="_height">高</param>
        /// <returns></returns>
        public static bool CheckPicWidthHeight(HttpPostedFile file, int _width, int _height)
        {
            bool isSafe = false;

            //图片尺寸检查
            Stream picstream = file.InputStream;
            Image img = Image.FromStream(picstream);
            if (img.Width > 0 && img.Height > 0)
            {
                if (img.Width > _width && img.Height > _height)
                    isSafe = false;//如果大于指定宽高
                else
                    isSafe = true;
            }
            else
                isSafe = false;//非法的图片文件";

            picstream.Close();
            picstream.Flush();
            picstream.Dispose();
            img.Dispose();

            return isSafe;
        }
        /// <summary>
        /// 验证图片格式、大小、宽、高
        /// </summary>
        /// <param name="file">上传控件</param>
        /// <param name="byteSize">大小(byte)</param>
        /// <param name="_width">宽</param>
        /// <param name="_height">高</param>
        /// <returns></returns>
        public static bool CheckPicSizeWidthHeight(HttpPostedFile file, int byteSize, int _width, int _height)
        {
            //格式判断,大小判断
            return IsPicture(file) && FileTool.CheckPicSize(file, byteSize) && CheckPicWidthHeight(file, _width, _height);
        }

        /// <summary>
        /// 获取文件的MD5值
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string GetMD5(HttpPostedFile file)
        {
            return MD5.GetMD5(file.InputStream);
        }
        /// <summary>
        /// 获取图像的文件格式
        /// </summary>
        /// <param name="strExtension">后缀名，如(jpg或.jpg)</param>
        /// <returns></returns>
        public static ImageFormat GetImageFormat(string strExtension)
        {
            ImageFormat imgFormat = ImageFormat.Jpeg;//默认jpg
            switch (strExtension.ToLower())
            {
                case "gif":
                case ".gif":
                    imgFormat=ImageFormat.Gif;
                    break;
                case "jpg":
                case ".jpg":
                case "jpeg":
                case ".jpeg":
                    imgFormat=ImageFormat.Jpeg;
                    break;
                case "png":
                case ".png":
                    imgFormat=ImageFormat.Png;
                    break;
                case "icon":
                case ".icon":
                    imgFormat=ImageFormat.Icon;
                    break;
                case "bmp":
                case ".bmp":
                    imgFormat=ImageFormat.Bmp;
                    break;
            }
            return imgFormat;
        }

        /// <summary>
        /// 上传图片
        /// 
        /// 前台应用方法
        /// string newpicname = "images/" + HtmlTool.GetRamCode() + ".jpg";//要保存图片的地址
        /// ImageTool.imgUploader(this.FileUpload1.PostedFile, newpicname);
        /// </summary>
        /// <param name="file">原图片</param>
        /// <param name="newPicName">要保存图片的地址</param>
        public static bool imgUploader(HttpPostedFile file, string newPicName)
        {
            Image uimg = Image.FromFile(file.FileName);
            try
            {
                uimg.Save(newPicName);
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                uimg.Dispose();
            }
        }
        /// <summary>
        /// 使用html控件上传图片类型文件
        /// </summary>
        /// <param name="ucFile">前台html上传控件的ID</param>
        /// <param name="strNewPath">保存目录</param>
        /// <param name="strFileSize">文件的大小</param>
        /// <returns>Dictionary（新的文件名，新的文件路径）</returns>
        public static Dictionary<string, string> Upload_HtmlFile(System.Web.UI.HtmlControls.HtmlInputFile ucFile, string strNewPath, Int32 strFileSize)
        {

            if (ucFile.PostedFile.ContentLength == 0)
                return null;

            string strNewName = DateTime.Now.ToString("yyyyMMddHHmmssffff");
            if (ucFile.PostedFile.ContentLength / 1024 > strFileSize)
                return null;

            if (IsPicture(ucFile.PostedFile))
            {
                ucFile.PostedFile.SaveAs(System.Web.HttpContext.Current.Server.MapPath(strNewPath) + strNewName);

                Dictionary<string, string> fileNewnameAndPath = new Dictionary<string, string>();
                fileNewnameAndPath.Add(strNewName, strNewPath + strNewName);
                return fileNewnameAndPath;
            }
            return null;
        }

        /// <summary>
        /// 下载远程图片保存到本地
        /// downRemoteImg("/Files", "http://images.chinaz.com/newindex_images/icp.gif")
        /// </summary>
        /// <param name="savedir">本地保存路径(虚拟目录)</param>
        /// <param name="imgpath">远程图片文件</param>
        /// <returns></returns>
        public static string downRemoteImg(string savedir, string imgpath)
        {
            if (string.IsNullOrEmpty(imgpath))
                return string.Empty;
            else
            {
                string imgName = string.Empty;
                string imgExt = string.Empty;
                string saveFilePath = string.Empty;

                imgName = imgpath.Substring(imgpath.LastIndexOf("/"));
                imgExt = imgpath.Substring(imgpath.LastIndexOf("."));

                if (GetData.GetRoot().IsNotNullOrEmpty())
                    savedir = savedir.Replace(GetData.GetRoot(), "");//将原有的根目录清空
                savedir = GetData.GetRoot() + savedir;//根目录加指定目录,如：/ylcms/Files

                //保存路径是否存在
                FolderTool.CreateFolder2(savedir);

                try
                {
                    saveFilePath = System.Web.HttpContext.Current.Server.MapPath(savedir);

                    Stream stream = FileTool.GetWebFileStream(imgpath);
                    Image img = Image.FromStream(stream);
                    img.Save(saveFilePath + imgName, GetImageFormat(imgExt));

                    stream.Flush();
                    stream.Close();
                    stream.Dispose();
                    img.Dispose();

                    return savedir + imgName;
                }
                catch
                {
                    return imgpath;
                }
            }
        }
        /// <summary>
        /// 生成缩略图
        /// 
        /// 前台应用方法
        /// string oldpicurl = this.FileUpload1.PostedFile.FileName.ToString();//原图片地址
        /// string newpicname = "images/" + HtmlTool.GetRamCode() + ".jpg";//生成缩略图后的新地址及名称
        /// ImageTool.imgMakeThumbnail(oldpicurl,newpicname, 100, 100, "HW");
        /// </summary>
        /// <param name="originalImagePath">源图路径（物理路径）</param>
        /// <param name="thumbnailPath">缩略图路径及缩略图名称（物理路径）</param>
        /// <param name="width">缩略图宽度</param>
        /// <param name="height">缩略图高度</param>
        /// <param name="mode">生成缩略图的方式</param>    
        public static void imgMakeThumbnail(string originalImagePath, string thumbnailPath, int width, int height, string mode)
        {
            Image originalImage = Image.FromFile(originalImagePath);

            int towidth = width;
            int toheight = height;

            int x = 0;
            int y = 0;
            int ow = originalImage.Width;
            int oh = originalImage.Height;

            switch (mode)
            {
                case "HW"://指定高宽缩放（可能变形）                
                    break;
                case "W"://指定宽，高按比例                    
                    toheight = originalImage.Height * width / originalImage.Width;
                    break;
                case "H"://指定高，宽按比例
                    towidth = originalImage.Width * height / originalImage.Height;
                    break;
                case "Cut"://指定高宽裁减（不变形）                
                    if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                    {
                        oh = originalImage.Height;
                        ow = originalImage.Height * towidth / toheight;
                        y = 0;
                        x = (originalImage.Width - ow) / 2;
                    }
                    else
                    {
                        ow = originalImage.Width;
                        oh = originalImage.Width * height / towidth;
                        x = 0;
                        y = (originalImage.Height - oh) / 2;
                    }
                    break;
                default:
                    break;
            }

            //新建一个bmp图片
            Image bitmap = new System.Drawing.Bitmap(towidth, toheight);

            //新建一个画板
            Graphics g = System.Drawing.Graphics.FromImage(bitmap);

            //设置高质量插值法
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

            //设置高质量,低速度呈现平滑程度
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            //清空画布并以透明背景色填充
            g.Clear(Color.Transparent);

            //在指定位置并且按指定大小绘制原图片的指定部分
            g.DrawImage(originalImage, new Rectangle(0, 0, towidth, toheight),
                            new Rectangle(x, y, ow, oh),
                            GraphicsUnit.Pixel);

            try
            {
                //以jpg格式保存缩略图
                bitmap.Save(thumbnailPath, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            catch (System.Exception e)
            {
                throw e;
            }
            finally
            {
                originalImage.Dispose();
                bitmap.Dispose();
                g.Dispose();
            }
        }
    }
}
