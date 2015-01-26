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

            label1.Text = "Current Path";
            comboBox2.SelectedItem = "File";
        }

        private void dirsTreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
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
                        TreeNode node = new TreeNode(di.Name, 0, 1);
                        try
                        {
                            node.Tag = dir;  //keep the directory's full path in the tag for use later
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
            String temp = dirsTreeView.SelectedNode.Tag.ToString();
            String newString = temp.Substring(0, (temp.LastIndexOf('\\') + 1));
            DirectoryInfo directoryInfo = new DirectoryInfo(newString);
            if (dirsTreeView.SelectedNode != null)
            {
                DirectoryInfo[] directories = directoryInfo.GetDirectories();
                foreach (FileInfo file in directoryInfo.GetFiles()) // for files outside folders
                {
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
                        catch (Exception)
                        { }
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
            if (e.Node.Tag.ToString() != null)
            {
            diree = e.Node.Tag.ToString();
            label1.Text = diree; // showing the current path
            }
            DirectoryInfo directoryInfo = new DirectoryInfo(diree.Substring(0, diree.LastIndexOf('\\') + 1));
            try
            {
                DirectoryInfo[] directories = directoryInfo.GetDirectories();

            }
            catch (IOException)
            {
                
            }
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
                                    String targetDir = dirsTreeView.SelectedNode.Tag.ToString();
                                    File.Copy(Path.Combine(file.Replace(Path.GetFileName(file), ""), Path.GetFileName(file)), Path.Combine(targetDir, Path.GetFileName(file)), true);
                                }
                                catch (UnauthorizedAccessException)
                                { }
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
            List<string> filepaths = new List<string>();
            List<string> Sfiles = new List<string>();
            List<string> Allfiles = new List<string>();
            List<string> AllFolders = new List<string>();
            bool found = false;
            try
            {
                if (comboBox2.SelectedItem.ToString() == "File")
                {
                    foreach (string file in Directory.EnumerateFiles(diree, textBox_Search.Text, SearchOption.AllDirectories))
                    {
                        MessageBox.Show(file);
                        found = true;
                    }

                    if (!found)
                    {
                        MessageBox.Show(textBox_Search.Text + " not found.");
                    }
                }

                if (comboBox2.SelectedItem.ToString() == "Folder")
                {
                    foreach (string folder in Directory.EnumerateDirectories(diree, textBox_Search.Text, SearchOption.AllDirectories))
                    {
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
            { }

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
            String temp = dirsTreeView.SelectedNode.Tag.ToString();
            String newString = temp.Substring(0, (temp.LastIndexOf('\\') + 1));
            DirectoryInfo directoryInfo = new DirectoryInfo(newString);
            if (dirsTreeView.SelectedNode != null)
            {
                DirectoryInfo[] directories = directoryInfo.GetDirectories();
                foreach (FileInfo file in directoryInfo.GetFiles()) // for files outside folders
                {
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
                        catch (Exception)
                        {
                        }
                    }


                }

            }

            MessageBox.Show("Paste NOW!!!!"); // can add check to see if file/folder is removed from clipboard
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
            var newNode = dirsTreeView.SelectedNode.Nodes.Add("New Folder");
            newNode.Tag = newNode.FullPath;
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            string pathString = path + @"\New Folder";
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