﻿using System.Windows.Forms;

namespace Com.Skewky.Cam
{
    public partial class MergeFilesForm : Form
    {
        public MergeFilesForm()
        {
            InitializeComponent();
        }

        public void PlayTestFile()
        {
            var path = @"d:\Users\bin\Desktop\新建文件夹 (2)\2016年01月03日\21时\52分00秒.mp4";
            vlcCtrl.PlayFile(path);
        }

        private void MergeFilesForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            vlcCtrl.Release();
        }
    }
}