﻿using System;
//using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using DevExpress.XtraEditors;
using System.Collections;

namespace UpYun_Model
{
    public class FileInformationForListView
    {

        public FileInformationForListView()
        { }


        public void getFileInformationForListView(ListView listview,ImageList imagelist,string path)
        {
            listview.SmallImageList = imagelist;
            string[] dirs, files;
            try
            {
                dirs = Directory.GetDirectories(path);//获取指定目录中子目录的名称
                files = Directory.GetFiles(path);//获取目录中文件的名称
            }
            catch{ return;}
            listview.Items.Clear();
            imagelist.Images.Clear();
            listview.Items.Add("上级目录");
            int index = 0;
            for (int i = 0; i < dirs.Length; i++)//遍历子文件夹
            {
                string[] info = new string[3];//定义一个数组
                DirectoryInfo dir = new DirectoryInfo(dirs[i]);//根据文件夹的路径实例化DirectoryInfo类
                if (!(dir.Name == "RECYCLER" || dir.Name == "RECYCLED" || dir.Name == "Recycled" || dir.Name == "System Volume Information") && (dir.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
                {
                    info[0] = dir.Name;
                    info[1] = "      ";
                    info[2] = dir.LastWriteTime.ToString();
                    ListViewItem item = new ListViewItem(info, index);//实例化ListViewItem类
                    listview.Items.Add(item);//添加当前文件夹的基本信息
                    imagelist.Images.Add(dir.Name, ToolsLibrary.GetIcon.GetDirectoryIcon(dir.FullName));
                    index++;
                }
            }
            for (int i = 0; i < files.Length; i++)//遍历文件
            {
                string[] info = new string[3];//定义一个数组
                FileInfo fi = new FileInfo(files[i]);//根据文件的路径实例化FileInfo类
                string Filetype = "unknown";
                if(fi.Name.Contains("."))
                    Filetype = fi.Name.Substring(fi.Name.LastIndexOf(".")).ToLower();//获取文件的类型              
                if (!(Filetype == "sys" || Filetype == "ini" || Filetype == "bin" || Filetype == "log" || Filetype == "com" || Filetype == "bat" || Filetype == "db") && (fi.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
                {
                    info[0] = fi.Name;
                    info[1] = ToolsLibrary.Tools.getCommonSize(fi.Length);
                    info[2] = fi.LastWriteTime.ToString();
                    ListViewItem item = new ListViewItem(info, index);//实例化ListViewItem类
                    listview.Items.Add(item);//添加当前文件的基本信息
                    imagelist.Images.Add(fi.Name, ToolsLibrary.GetIcon.GetFileIcon(Filetype, false));
                    index++;
                }
            }
        }

        public void getFileInformationForListViewMyPc(ListView listview, ImageList imagelist, string path)
        {
            listview.Items.Clear();
            imagelist.Images.Clear();
            listview.SmallImageList = imagelist;
            try
            {
                DriveInfo[] sysdir = System.IO.Directory.GetLogicalDrives().Select(q => new System.IO.DriveInfo(q)).ToArray();
                foreach (var sd in sysdir)
                {
                    string drivepath = sd.Name.Substring(0, sd.Name.LastIndexOf(@"\"));
                    imagelist.Images.Add(sd.Name, ToolsLibrary.GetIcon.GetDirectoryIcon(drivepath));//添加图标
                    ListViewItem item = new ListViewItem(sd.VolumeLabel + " (" + sd.Name + ")");
                    item.ImageKey = sd.Name;
                    item.SubItems.Add(ToolsLibrary.Tools.getCommonSize(sd.TotalFreeSpace));
                    item.SubItems.Add("本地磁盘");
                    listview.Items.Add(item);
                    //LocalPath = LocalPath + @"\";
                }
            }
            catch { }
        }

        public void delFileByListView(ListView listview,string path)
        {
            if (XtraMessageBox.Show("确定删除选中文件(文件夹)？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk) == DialogResult.OK)
            {
                for (int i = 0; i < listview.SelectedItems.Count; i++)
                {
                    if (listview.SelectedItems[i].SubItems[1].Text == "      ")
                    {
                        DirectoryInfo di = new DirectoryInfo(path + listview.SelectedItems[i].Text);
                        di.Delete(true);
                    }
                    else
                    {
                        System.IO.File.Delete(path + listview.SelectedItems[i].Text);
                    }
                }
            }
        }

        public void newFolderForLocal(string foldername,string path)
        {
            string fullfoldername = path + foldername;
            System.IO.Directory.CreateDirectory(fullfoldername);
        }

        public void getFileInformationForListViewWeb(ListView listview, ImageList imagelist, string path, UserInformation userInformation)
        {
            ArrayList str;          
            try
            {
                str = userInformation.upYun.readDir(path);
            }
            catch { return; }
            listview.Items.Clear();
            imagelist.Images.Clear();

            if (path != @"\")
                listview.Items.Add("上级目录");
            ListViewItem lvi = new ListViewItem();
            int index = 1;
            string filetypename = "unknown";
            imagelist.Images.Add("folder", ToolsLibrary.GetIcon.GetDirectoryIcon(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles) + @"\"));
            foreach (var item in str)
            {
                UpYunLibrary.FolderItem a = (UpYunLibrary.FolderItem)item;
                lvi = new ListViewItem(a.filename);
                if (a.filetype == "F")
                    lvi.ImageIndex = 0;
                else if (a.filetype == "N")
                {
                    lvi.ImageIndex = index;
                    if (a.filename.Contains("."))
                        filetypename = a.filename.Substring(a.filename.LastIndexOf(".")).ToLower();//获取文件的类型
                    imagelist.Images.Add(a.filename, ToolsLibrary.GetIcon.GetFileIcon(filetypename, false));
                    index++;
                }
                lvi.SubItems.Add(ToolsLibrary.Tools.getCommonSize(a.size));
                lvi.SubItems.Add(ToolsLibrary.Tools.getCommonTime(Convert.ToDouble(a.number)).ToString());
                listview.Items.Add(lvi);               
            }
        }

    }
}