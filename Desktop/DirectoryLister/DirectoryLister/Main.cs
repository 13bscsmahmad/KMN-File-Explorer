using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Security.AccessControl;
using System.Threading;
using System.Threading.Tasks;

namespace DirectoryLister
{
    public partial class MainForm : Form
    {
        string diree = "";
        bool clipboardEmpty = false;
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            //get a list of the drives
            string[] drives = Environment.GetLogicalDrives();

            foreach (string drive in drives)
            {
                DriveInfo di = new DriveInfo(drive);
                int driveImage;
                //comboBox1.Items.Add(drive);
                switch (di.DriveType)    //set the drive's icon
                {
                    case DriveType.CDRom:
                        driveImage = 3;
                        break;
                    case DriveType.Network:
                        driveImage = 6;
                        break;
                    case DriveType.NoRootDirectory:
                        driveImage = 8;
                        break;
                    case DriveType.Unknown:
                        driveImage = 8;
                        break;
                    default:
                        driveImage = 2;
                        break;
                }
                TreeNode node = new TreeNode(drive.Substring(0, 1), driveImage, driveImage);
                node.Tag = drive;

                if (di.IsReady == true)
                    node.Nodes.Add("...");

                dirsTreeView.Nodes.Add(node);
            }

            label1.Text = "Path";
            //comboBox1.SelectedItem = "C:\\";
            comboBox2.SelectedItem = "File";
        }

        private void dirsTreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {

            //comboBox1.Items.Clear();
            //DirectoryInfo directoryInfo = new DirectoryInfo(dirsTreeView.SelectedNode.Tag.ToString());

            if (e.Node.Nodes.Count > 0)
            {
                if (e.Node.Nodes[0].Text == "..." && e.Node.Nodes[0].Tag == null)
                {
                    e.Node.Nodes.Clear();

                    //get the list of sub direcotires
                    string[] dirs = Directory.GetDirectories(e.Node.Tag.ToString());
                    string[] files = Directory.GetFiles(e.Node.Tag.ToString());




                    foreach (string dir in dirs)
                    {
                        DirectoryInfo di = new DirectoryInfo(dir);
                        //comboBox1.Items.Add(dir);
                        TreeNode node = new TreeNode(di.Name, 0, 1);
                        try
                        {
                            node.Tag = dir;  //keep the directory's full path in the tag for use later

                            //if the directory has any sub directories add the place holder
                            if (di.GetDirectories().Count() > 0)
                                node.Nodes.Add(null, "...", 0, 0);
                        }
                        catch (UnauthorizedAccessException)
                        {
                            //if an unauthorized access exception occured display a locked folder
                            node.ImageIndex = 12;
                            node.SelectedImageIndex = 12;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "DirectoryLister", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                        }
                        finally
                        {
                            e.Node.Nodes.Add(node);
                        }
                    }
                    foreach (string file in files)
                    {
                        FileInfo fi = new FileInfo(file);
                        TreeNode node = new TreeNode(fi.Name, 13, 13);
                        try
                        {
                            node.Tag = file;
                        }
                        catch (UnauthorizedAccessException)
                        {
                            //if an unauthorized access exception occured display a locked folder
                            node.ImageIndex = 12;
                            node.SelectedImageIndex = 12;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "DirectoryLister", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                        }
                        finally
                        {
                            e.Node.Nodes.Add(node);
                        }

                    }
                }
            }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void dirsTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void cutToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        public static void AddDirectorySecurity(string FileName, string Account, FileSystemRights Rights, AccessControlType ControlType)
        {
            // Create a new DirectoryInfo object.
            DirectoryInfo dInfo = new DirectoryInfo(FileName);

            // Get a DirectorySecurity object that represents the  
            // current security settings.
            DirectorySecurity dSecurity = dInfo.GetAccessControl();

            // Add the FileSystemAccessRule to the security settings. 
            dSecurity.AddAccessRule(new FileSystemAccessRule(Account,
                                                            Rights,
                                                            ControlType));

            // Set the new access settings.
            dInfo.SetAccessControl(dSecurity);

        }

        private void Copy(object sender, EventArgs e)
        {

            //DirectoryInfo directoryInfo = new DirectoryInfo("C:\\");


            String temp = dirsTreeView.SelectedNode.Tag.ToString();
            String newString = temp.Substring(0, (temp.LastIndexOf('\\') + 1));
            DirectoryInfo directoryInfo = new DirectoryInfo(newString);
            //MessageBox.Show(newString);

            if (dirsTreeView.SelectedNode != null)
            {
                //DirectoryInfo directoryInfo = new DirectoryInfo(directoryInfo1);

                DirectoryInfo[] directories = directoryInfo.GetDirectories();
                foreach (FileInfo file in directoryInfo.GetFiles()) // for files outside folders
                {
                    //   MessageBox.Show(file.FullName);
                    if (file.Exists && file.FullName == diree)
                    {
                        System.Collections.Specialized.StringCollection filepath = new System.Collections.Specialized.StringCollection();
                        filepath.Add(file.FullName);
                        Clipboard.SetFileDropList(filepath);
                    }
                }

                if (directories.Length > 0)
                {
                    foreach (DirectoryInfo directory in directories)  // for folders
                    {
                        try
                        {
                            // AddDirectorySecurity(directory.FullName, @"Global", FileSystemRights.ReadData, AccessControlType.Allow);
                            // MessageBox.Show(directory.FullName);
                            Directory.GetAccessControl(directory.FullName);
                            if (directory.FullName == diree)
                            {
                                System.Collections.Specialized.StringCollection folderpath = new System.Collections.Specialized.StringCollection();
                                folderpath.Add(directory.FullName);
                                Clipboard.SetFileDropList(folderpath);
                            }
                        }
                        catch (UnauthorizedAccessException)
                        {

                        }
                        try
                        {
                            foreach (FileInfo file in directory.GetFiles()) // for files inside folders
                            {
                                if (file.Exists && file.Name == dirsTreeView.SelectedNode.Text)
                                {
                                    System.Collections.Specialized.StringCollection filepath = new System.Collections.Specialized.StringCollection();
                                    filepath.Add(file.FullName);
                                    Clipboard.SetFileDropList(filepath);

                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            // MessageBox.Show(ex.Message);
                        }
                    }


                }

            }
        }

        private void dirsTreeView_Click(object sender, EventArgs e)
        {

        }

        private void dirsTreeView_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void dirsTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            //MessageBox.Show(e.Node.Tag.ToString());
            if (e.Node.Tag.ToString() != null)
            {
            diree = e.Node.Tag.ToString();
            //Path.Text = diree;
            label1.Text = diree; // showing the current path
            }
            //comboBox1.Items.Clear();

            DirectoryInfo directoryInfo = new DirectoryInfo(diree.Substring(0, diree.LastIndexOf('\\') + 1));
            DirectoryInfo[] directories = directoryInfo.GetDirectories();

            //foreach (DirectoryInfo directory in directories)
            //{
            //    comboBox1.Items.Add(directory.Name);
            //}

        }

        private void button2_Click(object sender, EventArgs e)
        {


            if (dirsTreeView.SelectedNode != null)
            {
                bool copy = false;
                String temp = dirsTreeView.SelectedNode.Tag.ToString();
                String newString = temp.Substring(0, (temp.LastIndexOf('\\') + 1));
                DirectoryInfo directoryInfo = new DirectoryInfo(newString);

                DirectoryInfo[] directories = directoryInfo.GetDirectories();
                if (directories.Length > 0)
                {
                    foreach (DirectoryInfo directory in directories)
                    {
                        if (directory.Name == dirsTreeView.SelectedNode.Text && Clipboard.ContainsFileDropList())
                        {
                            foreach (string file in Clipboard.GetFileDropList())
                            {
                                try
                                {
                                    //String targetDir = directoryInfo.FullName + directory.Name;
                                    String targetDir = dirsTreeView.SelectedNode.Tag.ToString();
                                    File.Copy(Path.Combine(file.Replace(Path.GetFileName(file), ""), Path.GetFileName(file)), Path.Combine(targetDir, Path.GetFileName(file)), true);
                                }
                                catch (UnauthorizedAccessException)
                                {
                                    // MessageBox.Show("hahaha");
                                }
                            }
                            copy = true;

                        }
                    }
                }

                if (copy == true)
                {
                    foreach (string file in Clipboard.GetFileDropList())
                    {
                        TreeNode node = dirsTreeView.Nodes[0].Nodes[dirsTreeView.SelectedNode.Index].Nodes.Add(Path.GetFileName(file));
                        node.ImageIndex = node.SelectedImageIndex = 1;
                    }

                    copy = false;

                }
            }

            //diree = "C:\\";
            this.Hide();
            dirsTreeView.Refresh();
            MainForm d1 = new MainForm();
            
            d1.Show();
            d1.Refresh();



        }

        private void dirsTreeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {

        }

        private void dirsTreeView_NodeMouseDoubleClick_1(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (dirsTreeView.SelectedNode.Text.Contains("."))
            {
                String TreeNodeName = dirsTreeView.SelectedNode.ToString().Replace("TreeNode: ", String.Empty);
                try
                {
                    //System.Diagnostics.Process.Start(path + "\\" + TreeNodeName);
                    System.Diagnostics.Process.Start(dirsTreeView.SelectedNode.Tag.ToString());
                }
                catch (System.ComponentModel.Win32Exception)
                {
                    MessageBox.Show("File not found.");
                }
                //MessageBox.Show(path + "\\" + TreeNodeName);
            }
        }

        private void button_Search_Click(object sender, EventArgs e)
        {

            //DirectoryInfo directoryInfo = new DirectoryInfo(comboBox1.SelectedItem.ToString());

            //MessageBox.Show(comboBox1.SelectedItem.ToString());

            //DirectoryInfo[] directories = directoryInfo.GetDirectories();
            //FileInfo[] files = directoryInfo.GetFiles();
            // System.Collections.Specialized.StringCollection filepaths = new System.Collections.Specialized.StringCollection();
            List<string> filepaths = new List<string>();
            List<string> Sfiles = new List<string>();
            List<string> Allfiles = new List<string>();
            List<string> AllFolders = new List<string>();
            bool found = false;

            //bool found = false;
            try
            {
                if (comboBox2.SelectedItem.ToString() == "File")
                {
                    foreach (string file in Directory.EnumerateFiles(diree, textBox_Search.Text, SearchOption.AllDirectories))
                    {
                        // Display file path.
                        //Allfiles.Add(file);
                        MessageBox.Show(file);
                        found = true;
                    }

                    if (!found)
                    {
                        MessageBox.Show(textBox_Search.Text + " not found.");
                    }

                    

                    //var allFiles = Directory.GetFiles(comboBox1.SelectedItem.ToString(), textBox_Search.Text, SearchOption.AllDirectories);
                    //foreach (String file in allFiles)
                    //{
                    //    MessageBox.Show(file);
                    //}
                }

                if (comboBox2.SelectedItem.ToString() == "Folder")
                {

                    //var allFolders = Directory.GetDirectories(comboBox1.SelectedItem.ToString(), textBox_Search.Text, SearchOption.AllDirectories);
                    //foreach (String folder in allFolders)
                    //{
                    //    MessageBox.Show(folder);
                    //}
                    foreach (string folder in Directory.EnumerateDirectories(diree, textBox_Search.Text, SearchOption.AllDirectories))
                    {
                        // Display file path.
                        MessageBox.Show(folder);
                        found = true;
                    }

                    if (!found)
                    {
                        MessageBox.Show(textBox_Search.Text + " not found.");
                    }

                    
                }

            }
            catch (UnauthorizedAccessException ex)
            {
                //MessageBox.Show(ex.Message);
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                found = false;
            }

            foreach (string file in Sfiles)
            {
                MessageBox.Show(file);
                found = false;
            }

            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



            //    foreach (DirectoryInfo directory in directories) 
            //    {
            //        if (directory.Name.Contains(textBox_Search.Text))
            //        {
            //            MessageBox.Show(directory.FullName);
            //            found = true;
            //        }
            //        try
            //        {
            //            foreach (FileInfo file in directory.GetFiles())
            //            {
            //                if (file.Name.Contains(textBox_Search.Text))
            //                {

            //                    found = true;
            //                    filepaths.Add(file.FullName);
            //                    MessageBox.Show(file.FullName);
            //                    break;
            //                }
            //            }

            //        }
            //        catch (Exception ex)
            //        {

            //        }

            //    }

            //    foreach (FileInfo file in files)
            //    {
            //        if (file.Name.Contains(textBox_Search.Text))
            //        {
            //            found = true;
            //            filepaths.Add(file.FullName);
            //            MessageBox.Show(file.FullName);
            //            break;
            //        }


            //    }

            //    if (!found)
            //    {
            //        MessageBox.Show("\"" + textBox_Search.Text + "\"" + " not found.");
            //    }




        }

        private void Delete_Click(object sender, EventArgs e)
        {

            String temp = dirsTreeView.SelectedNode.Tag.ToString();
            String newString = temp.Substring(0, (temp.LastIndexOf('\\') + 1));
            DirectoryInfo directoryInfo = new DirectoryInfo(newString);

            bool deleted = false;

            DirectoryInfo[] directories = directoryInfo.GetDirectories();
            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                if (file.Exists && file.Name == dirsTreeView.SelectedNode.Text)
                {
                    file.Delete();
                    deleted = true;
                }
            }

            if (directories.Length > 0)
            {
                foreach (DirectoryInfo directory in directories)
                {
                    try
                    {

                        foreach (FileInfo file in directory.GetFiles())
                        {
                            if (file.Exists && file.Name == dirsTreeView.SelectedNode.Text)
                            {
                                file.Delete();
                                deleted = true;
                            }
                        }
                    }
                    catch (UnauthorizedAccessException ex) { }

                    if (dirsTreeView.SelectedNode.Text == directory.Name)
                    {
                        foreach (FileInfo file in directory.GetFiles())
                        {
                            if (file.Exists)
                                file.Delete();
                        }
                        directory.Delete();
                        deleted = true;
                    }
                }

            }
            if (deleted)
                dirsTreeView.SelectedNode.Remove();
        }

        private void Cut_Click(object sender, EventArgs e)
        {


            //DirectoryInfo directoryInfo = new DirectoryInfo("C:\\");


            String temp = dirsTreeView.SelectedNode.Tag.ToString();
            String newString = temp.Substring(0, (temp.LastIndexOf('\\') + 1));
            DirectoryInfo directoryInfo = new DirectoryInfo(newString);
            //MessageBox.Show(newString);

            if (dirsTreeView.SelectedNode != null)
            {
                //DirectoryInfo directoryInfo = new DirectoryInfo(directoryInfo1);

                DirectoryInfo[] directories = directoryInfo.GetDirectories();
                foreach (FileInfo file in directoryInfo.GetFiles()) // for files outside folders
                {
                    //   MessageBox.Show(file.FullName);
                    if (file.Exists && file.FullName == diree)
                    {
                        System.Collections.Specialized.StringCollection filepath = new System.Collections.Specialized.StringCollection();
                        filepath.Add(file.FullName);
                        Clipboard.SetFileDropList(filepath);
                    }
                }

                if (directories.Length > 0)
                {
                    foreach (DirectoryInfo directory in directories)  // for folders
                    {
                        try
                        {
                            // AddDirectorySecurity(directory.FullName, @"Global", FileSystemRights.ReadData, AccessControlType.Allow);
                            // MessageBox.Show(directory.FullName);
                            Directory.GetAccessControl(directory.FullName);
                            if (directory.FullName == diree)
                            {
                                System.Collections.Specialized.StringCollection folderpath = new System.Collections.Specialized.StringCollection();
                                folderpath.Add(directory.FullName);
                                Clipboard.SetFileDropList(folderpath);
                            }
                        }
                        catch (UnauthorizedAccessException)
                        {

                        }
                        try
                        {
                            foreach (FileInfo file in directory.GetFiles()) // for files inside folders
                            {
                                if (file.Exists && file.Name == dirsTreeView.SelectedNode.Text)
                                {
                                    System.Collections.Specialized.StringCollection filepath = new System.Collections.Specialized.StringCollection();
                                    filepath.Add(file.FullName);
                                    Clipboard.SetFileDropList(filepath);

                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            // MessageBox.Show(ex.Message);
                        }
                    }


                }

            }

            MessageBox.Show("Paste NOW!!!!"); // can add check to see if file/folder is removed from clipboard

            
            //while (!clipboardEmpty)
            //{
            //    Task taskA = Task.Factory.StartNew(() => checkClipboard());
            //    taskA.Wait();

            //}
            bool deleted = false;

            DirectoryInfo[] directories1 = directoryInfo.GetDirectories();
            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                if (file.Exists && file.Name == dirsTreeView.SelectedNode.Text)
                {
                    file.Delete();
                    deleted = true;
                }
            }

            if (directories1.Length > 0)
            {
                foreach (DirectoryInfo directory in directories1)
                {
                    foreach (FileInfo file in directory.GetFiles())
                    {
                        if (file.Exists && file.Name == dirsTreeView.SelectedNode.Text)
                        {
                            file.Delete();
                            deleted = true;
                        }
                    }

                    if (dirsTreeView.SelectedNode.Text == directory.Name)
                    {
                        foreach (FileInfo file in directory.GetFiles())
                        {
                            if (file.Exists)
                                file.Delete();
                        }
                        directory.Delete();
                        deleted = true;
                    }
                }

            }
            if (deleted)
                dirsTreeView.SelectedNode.Remove();

        }

        private void Create_Click(object sender, EventArgs e)
        {
            String path = dirsTreeView.SelectedNode.Tag.ToString();
            //MessageBox.Show(dirsTreeView.SelectedNode.Tag.ToString());
            var newNode = dirsTreeView.SelectedNode.Nodes.Add("New Folder");
            //newNode.Tag = path + @"\" + newNode.Name;
            newNode.Tag = newNode.FullPath;
            //dirsTreeView.LabelEdit = true;
            //newNode.BeginEdit();
            //dirsTreeView.LabelEdit = false;
            //MessageBox.Show(newNode.Tag.ToString());

            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            string pathString = path + @"\New Folder";
            //string pathString = path + @"\" + newNode.Name;
            System.IO.Directory.CreateDirectory(pathString);

            MessageBox.Show("Folder created succesfully!");
            return;
            
        }

        private bool checkClipboard()
        {
            if (!Clipboard.ContainsFileDropList())
            {
                clipboardEmpty = true;
                return clipboardEmpty;
            }
            else
            {
                clipboardEmpty = false;
                return clipboardEmpty;
            }
        }
    }

}
