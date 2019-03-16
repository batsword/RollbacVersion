using NDKTV.Utils.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DeletePasteFiles
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        string _foldName = "backup";
        string _exeName = "DeletePasteFiles.exe";
        int _nums = 0; int _allNums = 0;
        string _callProcess = "NDKTV.exe";

        private void b1_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show(Environment.CurrentDirectory);
            //MessageBox.Show(GetAllFilesNum(Environment.CurrentDirectory+"\\Back").ToString());
            //MessageBox.Show(System.Windows.Forms.Application.ExecutablePath);

            HuiGun();
            
        }

        private void Delete(string srcPath)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(srcPath);
                FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录
                foreach (FileSystemInfo i in fileinfo)
                {
                    if (i is DirectoryInfo)            //判断是否文件夹
                    {
                        if (i.Name != _foldName)
                        {
                            DirectoryInfo subdir = new DirectoryInfo(i.FullName);

                            int foldFilesNum = GetFilesCount(subdir);
                            _nums += foldFilesNum-1; //删除文件时特殊 需要统计子目录下文件数好显示到进度条

                            subdir.Delete(true);          //删除子目录和文件
                            ShowValue();
                        }
                    }
                    else
                    {
                        if( (i.Name == _exeName)||(i.Name == "DeletePasteFiles.vshost.exe") || (i.Name == "DeletePasteFiles.pdb")||
                            (i.Name == _callProcess) ||(i.Name == "NDKTV.pdb"))
                        {

                        }
                        else
                        {
                            File.Delete(i.FullName);      //删除指定文件
                            ShowValue();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //throw;
            }
        }

        private void ShowValue()
        {
            _nums++;
            gunBar.Value = _nums;
            valueText.Text = (_nums / (float)_allNums * 100.0).ToString("#0.00") + "%";
            DispatcherHelper.DoEvents();
        }

        
        public  void CopyDirectoryEx(string srcPath, string destPath)
        {
            try
            {
                Dispatcher.Invoke(new Action(()=> {
                    DirectoryInfo dir = new DirectoryInfo(srcPath);
                    FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //获取目录下（不包含子目录）的文件和子目录
                    foreach (FileSystemInfo i in fileinfo)
                    {
                        if (i is DirectoryInfo)     //判断是否文件夹
                        {
                            if (!Directory.Exists(destPath + "\\" + i.Name))
                            {
                                Directory.CreateDirectory(destPath + "\\" + i.Name);   //目标目录下不存在此文件夹即创建子文件夹
                                
                            }
                            CopyDirectoryEx(i.FullName, destPath + "\\" + i.Name);    //递归调用复制子文件夹
                            //ShowValue();
                        }
                        else
                        {
                            if(i.Name == _exeName)
                            {
                                continue;
                            }

                            File.Copy(i.FullName, destPath + "\\" + i.Name, true);      //不是文件夹即复制文件，true表示可以覆盖同名文件
                            ShowValue();
                        }
                    }
                }));               
            }
            catch (Exception e)
            {
                throw;
            }
        }


        public static void CopyDirectory(string srcPath, string destPath)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(srcPath);
                FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //获取目录下（不包含子目录）的文件和子目录
                foreach (FileSystemInfo i in fileinfo)
                {
                    if (i is DirectoryInfo)     //判断是否文件夹
                    {
                        if (!Directory.Exists(destPath + "\\" + i.Name))
                        {
                            Directory.CreateDirectory(destPath + "\\" + i.Name);   //目标目录下不存在此文件夹即创建子文件夹
                        }
                        CopyDirectory(i.FullName, destPath + "\\" + i.Name);    //递归调用复制子文件夹
                    }
                    else
                    {
                        File.Copy(i.FullName, destPath + "\\" + i.Name, true);      //不是文件夹即复制文件，true表示可以覆盖同名文件
                    }
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }


        
        public static int GetFilesCount(System.IO.DirectoryInfo dirInfo)
        {
            int totalFile = 0;
            totalFile += dirInfo.GetFiles().Length;
            foreach (System.IO.DirectoryInfo subdir in dirInfo.GetDirectories())
            {
                totalFile += GetFilesCount(subdir);
            }
            return totalFile;
        }

        public int GetAllFilesNum(string dirPath)
        {
            System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(dirPath);
            return GetFilesCount(dirInfo);
        }

        private void HuiGun()
        {

            string srcPath = Environment.CurrentDirectory + @"\" + _foldName;

            //ktv文件目录还有一层
            DirectoryInfo dir = new DirectoryInfo(srcPath);
            DirectoryInfo[] dirinfo = dir.GetDirectories();  //返回目录中所有文件和子目录

            if (dirinfo.Count() == 1)
            {
                foreach (DirectoryInfo i in dirinfo)
                {
                    srcPath += @"\" + i.Name;
                }
            }

            string destPath = Environment.CurrentDirectory;

            Delete(destPath);

            //this.Title = "已删除" + _nums.ToString();
            //MessageBox.Show("暂停");
            //new Thread(() => {
            CopyDirectoryEx(srcPath, destPath);
            //this.Title = "已拷贝" + _nums.ToString();
            //}).Start();
        }
        Thread processThread =null;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            processThread = new Thread(() => {
                Dispatcher.Invoke(new Action(() => {
                    for (int i = 0; i < 30; i++)
                    {
                        Thread.Sleep(100);
                        DispatcherHelper.DoEvents();
                    }

                    //GetAllFilesNum(Environment.CurrentDirectory + @"\" + _foldName) +
                    _allNums = GetAllFilesNum(Environment.CurrentDirectory);
                    
                    gunBar.Maximum = _allNums;
                    gunBar.Value = 0;

                    HuiGun();

                    gunBar.Value = _allNums;
                    valueText.Text = "100%";
                    DispatcherHelper.DoEvents();
                    Thread.Sleep(200);


                    CallKTV();
                }));
            });
            processThread.Start();
            
        }

        private void CallKTV()
        {
            try
            {
                Process.Start(Environment.CurrentDirectory + @"\" + _callProcess);
                this.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("启动Ktv出现程序异常,请检查ktv文件名是否:"+ _callProcess);
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if ((processThread != null))
            {
                processThread.Abort();
                
            }
            Application.Current.Shutdown();
            Environment.Exit(0);// 可以立即中断程序执行并退出
        }
    }


}
