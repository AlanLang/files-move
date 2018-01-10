using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

namespace FilesMove
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        string zipedFolder = AppDomain.CurrentDomain.BaseDirectory + "Datas\\" + DateTime.Now.ToString("yyyyMMddHHmmss");
        string backFolder = AppDomain.CurrentDomain.BaseDirectory + "Bak";
        string configName = "config.json";
        public MainWindow()
        {
            InitializeComponent();
            Msg("请选择安装包");
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void ImpFileBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "压缩文件(*.zip)|*.zip";
            dialog.Multiselect = false;
            if (dialog.ShowDialog() == true)
            {
                FileStream fs = new FileStream(dialog.FileName, FileMode.Open, FileAccess.Read);
                bool IsUpLoad = UnLoadZip(fs);
                if (IsUpLoad)
                {
                    Msg("压缩包解压成功");
                    try
                    {
                        MoveFiles();
                    }
                    catch (Exception ex)
                    {
                        Msg("文件处理异常：" + ex.Message);
                    }
                }
            }
        }

        private void closeBtn_Click(object sender, RoutedEventArgs e)
        {
            Msg("程序退出");
            this.Close();
        }

        private bool UnLoadZip(FileStream fs)
        {
            string fileName = "";
            ZipInputStream zipStream = null;
            ZipEntry ent = null;
            try
            {
                if (!Directory.Exists(zipedFolder))
                    Directory.CreateDirectory(zipedFolder);
                zipStream = new ZipInputStream(fs);
                while ((ent = zipStream.GetNextEntry()) != null)
                {
                    if (!string.IsNullOrEmpty(ent.Name))
                    {
                        fileName = System.IO.Path.Combine(zipedFolder, ent.Name);
                        fileName = fileName.Replace('/', '\\');
                    }
                    if (fileName != string.Empty)
                    {
                        using (FileStream streamWriter = File.Create(fileName))
                        {
                            int size = 4096;
                            byte[] data = new byte[4 * 1024];
                            while (true)
                            {
                                size = zipStream.Read(data, 0, data.Length);
                                if (size > 0)
                                {
                                    streamWriter.Write(data, 0, size);
                                }
                                else break;
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Msg("解压文件异常：" + ex.Message);
                return false;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
                if (zipStream != null)
                {
                    zipStream.Close();
                    zipStream.Dispose();
                }
                if (ent != null)
                {
                    ent = null;
                }
                GC.Collect();
                GC.Collect(1);
            }
        }

        private void MoveFiles()
        {
            string ConfigFilePath = System.IO.Path.Combine(zipedFolder, configName);
            if (!File.Exists(ConfigFilePath))
            {
                Msg("没有找到配置文件，操作终止。");
                Msg("***********************");
                DeleteFolder(zipedFolder);
                return;
            }
            string json = File.ReadAllText(ConfigFilePath);
            json = json.Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", "");
            FileConfi fcon = JsonConvert.DeserializeObject<FileConfi>(json);//result为上面的Json数据 
            Msg("开始解析配置文件，版本：" + fcon.version + "，作者：" + fcon.person);
            string[] files = Directory.GetFiles(zipedFolder);
            Msg("找个需要更新的文件共" + (files.Length - 1) + "个");
            int index = 1;
            for (int i = 0; i < files.Length; i++)
            {
                string[] arr = files[i].Split('\\');
                string thefilename = arr[arr.Length - 1];
                if (thefilename == configName)
                {
                }
                else
                {
                    Msg("开始更新第" + index + "个文件：" + thefilename);
                    index++;
                    List<files> filemsg = fcon.files.Where<files>(t => thefilename.Contains(t.name)).ToList();
                    if (filemsg.Count == 0)
                    {
                        Msg("无法在配置中找到文件：" + thefilename + ",该文件被跳过");
                    }
                    else
                    {
                        string newpath = System.IO.Path.Combine(fcon.basepath, filemsg[0].path);
                        if (!System.IO.Directory.Exists(newpath))
                        {
                            System.IO.Directory.CreateDirectory(newpath);
                        }
                        string newname = thefilename;
                        string odlFilePath = System.IO.Path.Combine(zipedFolder, newname);
                        string newFilePath = System.IO.Path.Combine(newpath, newname);
                        string backFilePath = System.IO.Path.Combine(backFolder, newname);
                        if (File.Exists(newFilePath))
                        {
                            if (!System.IO.Directory.Exists(backFolder))
                            {
                                System.IO.Directory.CreateDirectory(backFolder);
                            }
                            System.IO.File.Copy(newFilePath, backFilePath, true);
                            Msg("存放备份文件：" + backFilePath);
                        }
                        System.IO.File.Copy(odlFilePath, newFilePath, true);
                    }
                }
            }
            Msg("文件更新完毕");
            Msg("*************");
            DeleteFolder(zipedFolder);
        }

        private void DeleteFolder(string directoryPath)
        {
            foreach (string d in Directory.GetFileSystemEntries(directoryPath))
            {
                if (File.Exists(d))
                {
                    FileInfo fi = new FileInfo(d);
                    if (fi.Attributes.ToString().IndexOf("ReadOnly") != -1)
                        fi.Attributes = FileAttributes.Normal;
                    File.Delete(d);     //删除文件   
                }
                else
                    DeleteFolder(d);    //删除文件夹
            }
            Directory.Delete(directoryPath);    //删除空文件夹
        }

        private void Msg(string msg)
        {
            messagelog.AppendText(msg + "\n");
            msg += "  [" + DateTime.Now.ToString("yyyyMMddHHmmss") + "]";
            string logPath = AppDomain.CurrentDomain.BaseDirectory + "Log/";
            if (!System.IO.Directory.Exists(logPath))
                System.IO.Directory.CreateDirectory(logPath);
            string logFile = logPath + "Log-" + DateTime.Now.ToString("yyyy-MM-dd") + ".log";
            System.IO.File.AppendAllLines(logFile, new string[] { msg });
        }

    }
}
