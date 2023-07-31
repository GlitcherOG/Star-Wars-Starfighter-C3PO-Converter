using Microsoft.WindowsAPICodePack.Dialogs;
using System.Diagnostics;
using System.Formats.Tar;

namespace C3PO_Converter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "C3PO File (*.c3po)|*.c3po|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = false
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                C3PO_Handler c3PO_Handler = new C3PO_Handler();
                c3PO_Handler.Load(openFileDialog.FileName);

                SaveFileDialog openFileDialog1 = new SaveFileDialog
                {
                    Filter = "JSON File (*.json)|*.json|All files (*.*)|*.*",
                    FilterIndex = 1,
                    RestoreDirectory = false
                };
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    c3PO_Handler.CreateJson(openFileDialog1.FileName, true);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog openFileDialog1 = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Title = "Select C3PO Folder",
            };
            if (openFileDialog1.ShowDialog() == CommonFileDialogResult.Ok)
            {
                CommonOpenFileDialog openFileDialog2 = new CommonOpenFileDialog
                {
                    IsFolderPicker = true,
                    Title = "Select C3PO Folder",
                };
                if (openFileDialog2.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    string[] NewFiles = Directory.GetFiles(openFileDialog1.FileName, "*.c3po", SearchOption.TopDirectoryOnly);

                    for (int i = 0; i < NewFiles.Length; i++)
                    {
                        C3PO_Handler c3PO_Handler = new C3PO_Handler();
                        c3PO_Handler.Load(NewFiles[i]);

                        c3PO_Handler.CreateJson(openFileDialog2.FileName + "\\" + Path.GetFileNameWithoutExtension(NewFiles[i]) + ".json", true);
                    }

                    MessageBox.Show("Folder Decompiled");
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "JSON File (*.json)|*.json|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = false
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                SaveFileDialog openFileDialog1 = new SaveFileDialog
                {
                    Filter = "C3PO File (*.c3po)|*.c3po|All files (*.*)|*.*",
                    FilterIndex = 1,
                    RestoreDirectory = false
                };
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    C3PO_Handler c3PO_Handler = new C3PO_Handler();
                    c3PO_Handler = C3PO_Handler.LoadJSON(openFileDialog.FileName);
                    c3PO_Handler.Save(openFileDialog1.FileName);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog openFileDialog1 = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Title = "Select Json Folder",
            };
            if (openFileDialog1.ShowDialog() == CommonFileDialogResult.Ok)
            {
                CommonOpenFileDialog openFileDialog2 = new CommonOpenFileDialog
                {
                    IsFolderPicker = true,
                    Title = "Select C3PO Folder",
                };
                if (openFileDialog2.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    string[] NewFiles = Directory.GetFiles(openFileDialog1.FileName, "*.json", SearchOption.TopDirectoryOnly);

                    for (int i = 0; i < NewFiles.Length; i++)
                    {
                        C3PO_Handler c3PO_Handler = new C3PO_Handler();
                        c3PO_Handler = C3PO_Handler.LoadJSON(NewFiles[i]);
                        c3PO_Handler.Save(openFileDialog2.FileName + "\\" + Path.GetFileNameWithoutExtension(NewFiles[i]) + ".c3po");
                    }

                    MessageBox.Show("Folder Compiled");
                }
            }
        }
    }
}