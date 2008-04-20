//Copyright (c) 2007 Maxence Dislaire

//Permission is hereby granted, free of charge, to any person
//obtaining a copy of this software and associated documentation
//files (the "Software"), to deal in the Software without
//restriction, including without limitation the rights to use,
//copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the
//Software is furnished to do so, subject to the following
//conditions:

//The above copyright notice and this permission notice shall be
//included in all copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
//OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
//HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
//WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
//FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
//OTHER DEALINGS IN THE SOFTWARE.
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using YouWebIt;

namespace YouWebIt
{
    internal partial class NotifyIconForm : Form , IMenuService
    {
        private static NotifyIconForm s_NotifyIconForm;

        private NotifyIconForm()
        {
            InitializeComponent();
            Closing += delegate
                          {
                              notifyIcon1.Visible = false;
                              notifyIcon1.Dispose();
                          };
        }

        internal static NotifyIconForm Instance
        {
            get
            {
                if (s_NotifyIconForm == null)
                {
                    s_NotifyIconForm = new NotifyIconForm();
                }
                return s_NotifyIconForm;
            }
        }

        public NotifyIcon NotifyIcon
        {
            get { return notifyIcon1; }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void openRootDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            YouWebItConsoletHost.YouWebItInstance.OpenRootDirectory();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            YouWebItConsoletHost.YouWebItInstance.LaunchDefaultWebBrowser();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            new InfoForm().ShowDialog();
        }

        #region IMenuService Members

        public void AddToolStripMenuItem(ToolStripMenuItem t)
        {
            this.contextMenuStripNotiFyIcon.Items.Insert(0,t);
        }

        #endregion

        private void saveConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string configFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
            string configFileContent = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<configuration>
  <appSettings>
    <!--Relative path where your website files are located -->
    <!--Example : ./WebSite or ../src/WebApp -->
    <add key=""ServerPhysicalPath"" value=""{0}""/>
    
    <!--Your HomeUrl. YouWebIt search for : default.aspx, Default.aspx, Default.html, default.html, Index.html, index.html. -->
    <add key=""HomeUrl"" value=""{1}""/>

    <!--Webserver Port. Use this if yo want to allways use the same port number.-->
    <add key=""ServerPort"" value=""{2}""/>
</appSettings>
</configuration>";
            string format = string.Format(configFileContent, 
                                          PathHelper.ToRelativePath(new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory),new DirectoryInfo(YouWebItConsoletHost.YouWebItInstance.ServerPhysicalPath)),
                                          YouWebIt.GetHomePageFileName(YouWebItConsoletHost.YouWebItInstance.ServerPhysicalPath),
                                          YouWebItConsoletHost.YouWebItInstance.ServerPort);
            File.WriteAllText(configFileName,format,Encoding.UTF8);

            NotifyIcon.ShowBalloonTip(5000, "YouWebIt", "Config File Saved in " + YouWebItConsoletHost.YouWebItInstance.ServerPhysicalPath, ToolTipIcon.Info);
            NotifyIcon.BalloonTipClicked += new EventHandler(NotifyIcon_BalloonTipClicked);
        }

        void NotifyIcon_BalloonTipClicked(object sender, EventArgs e)
        {
            NotifyIcon.BalloonTipClicked -= new EventHandler(NotifyIcon_BalloonTipClicked);
            VoidMethodDelegate asynchLauncher = delegate
            {
                string configFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
                if (string.IsNullOrEmpty(configFileName))
                {
                    return;
                }
                Process.Start(configFileName);
            };
            asynchLauncher.BeginInvoke(null, null);
        }
    }

    public interface IMenuService
    {
        void AddToolStripMenuItem(ToolStripMenuItem t);
    }
}