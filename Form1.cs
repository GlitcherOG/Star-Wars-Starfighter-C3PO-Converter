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
                SaveFileDialog openFileDialog1 = new SaveFileDialog
                {
                    Filter = "JSON File (*.json)|*.json|All files (*.*)|*.*",
                    FilterIndex = 1,
                    RestoreDirectory = false
                    
                };
                openFileDialog1.FileName = Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    int Version = C3POVersionDetector.ReadVersionID(openFileDialog.FileName);
                    if (Version == 5)
                    {
                        C3POHandlerVersion5 c3PO_Handler = new C3POHandlerVersion5();
                        c3PO_Handler.Load(openFileDialog.FileName);
                        c3PO_Handler.CreateJson(openFileDialog1.FileName, true);
                    }
                    else if (Version == 4)
                    {
                        C3POHandlerVersion4 c3PO_Handler = new C3POHandlerVersion4();
                        c3PO_Handler.Load(openFileDialog.FileName);
                        c3PO_Handler.CreateJson(openFileDialog1.FileName, true);
                    }
                    else if (Version == 3)
                    {
                        C3POHandlerVersion3 c3PO_Handler = new C3POHandlerVersion3();
                        c3PO_Handler.Load(openFileDialog.FileName);
                        c3PO_Handler.CreateJson(openFileDialog1.FileName, true);
                    }
                    else
                    {
                        MessageBox.Show("Error Unknown Type Detected " + Version);
                    }
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
                    Title = "Select Extraction Folder",
                };
                if (openFileDialog2.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    string[] NewFiles = Directory.GetFiles(openFileDialog1.FileName, "*.c3po", SearchOption.TopDirectoryOnly);

                    for (int i = 0; i < NewFiles.Length; i++)
                    {
                        int Version = C3POVersionDetector.ReadVersionID(NewFiles[i]);
                        if (Version == 5)
                        {
                            C3POHandlerVersion5 c3PO_Handler = new C3POHandlerVersion5();
                            c3PO_Handler.Load(NewFiles[i]);
                            c3PO_Handler.CreateJson(openFileDialog2.FileName + "\\" + Path.GetFileNameWithoutExtension(NewFiles[i]) + ".json", true);
                        }
                        else if (Version == 4)
                        {
                            C3POHandlerVersion4 c3POHandlerVersion4 = new C3POHandlerVersion4();
                            c3POHandlerVersion4.Load(NewFiles[i]);
                            c3POHandlerVersion4.CreateJson(openFileDialog2.FileName + "\\" + Path.GetFileNameWithoutExtension(NewFiles[i]) + ".json", true);
                        }
                        else if (Version == 3)
                        {
                            C3POHandlerVersion3 c3POHandlerVersion4 = new C3POHandlerVersion3();
                            c3POHandlerVersion4.Load(NewFiles[i]);
                            c3POHandlerVersion4.CreateJson(openFileDialog2.FileName + "\\" + Path.GetFileNameWithoutExtension(NewFiles[i]) + ".json", true);
                        }
                        else
                        {
                            MessageBox.Show("Error Unknown Type Detected " + Version + "\n" + NewFiles[i]);
                            break;
                        }
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
                openFileDialog1.FileName = Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    int Version = C3POVersionDetector.ReadJsonVersionID(openFileDialog.FileName);
                    if (Version == 5)
                    {
                        C3POHandlerVersion5 c3PO_Handler = new C3POHandlerVersion5();
                        c3PO_Handler = C3POHandlerVersion5.LoadJSON(openFileDialog.FileName);
                        c3PO_Handler.Save(openFileDialog1.FileName);
                    }
                    else if (Version == 4)
                    {
                        C3POHandlerVersion4 c3PO_Handler = new C3POHandlerVersion4();
                        c3PO_Handler = C3POHandlerVersion4.LoadJSON(openFileDialog.FileName);
                        c3PO_Handler.Save(openFileDialog1.FileName);
                    }
                    else if (Version == 3)
                    {
                        C3POHandlerVersion3 c3PO_Handler = new C3POHandlerVersion3();
                        c3PO_Handler = C3POHandlerVersion3.LoadJSON(openFileDialog.FileName);
                        c3PO_Handler.Save(openFileDialog1.FileName);
                    }
                    else
                    {
                        MessageBox.Show("Error Unknown Type Detected " + Version);
                    }
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
                        int Version = C3POVersionDetector.ReadJsonVersionID(NewFiles[i]);
                        if (Version == 5)
                        {
                            C3POHandlerVersion5 c3PO_Handler = new C3POHandlerVersion5();
                            c3PO_Handler = C3POHandlerVersion5.LoadJSON(NewFiles[i]);
                            c3PO_Handler.Save(openFileDialog2.FileName + "\\" + Path.GetFileNameWithoutExtension(NewFiles[i]) + ".c3po");
                        }
                        else if (Version == 4)
                        {
                            C3POHandlerVersion4 c3PO_Handler = new C3POHandlerVersion4();
                            c3PO_Handler = C3POHandlerVersion4.LoadJSON(NewFiles[i]);
                            c3PO_Handler.Save(openFileDialog2.FileName + "\\" + Path.GetFileNameWithoutExtension(NewFiles[i]) + ".c3po");
                        }
                        else if (Version == 3)
                        {
                            C3POHandlerVersion3 c3PO_Handler = new C3POHandlerVersion3();
                            c3PO_Handler = C3POHandlerVersion3.LoadJSON(NewFiles[i]);
                            c3PO_Handler.Save(openFileDialog2.FileName + "\\" + Path.GetFileNameWithoutExtension(NewFiles[i]) + ".c3po");
                        }
                        else
                        {
                            MessageBox.Show("Error Unknown Type Detected " + Version);
                        }
                    }

                    MessageBox.Show("Folder Compiled");
                }
            }
        }
    }
}