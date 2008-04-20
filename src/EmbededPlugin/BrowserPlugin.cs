using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using System.IO;

namespace YouWebIt.SilverlightSpyPlugin
{
    public class BrowserPlugin : IPlugin
    {
        #region IPlugin Members

        public void InitializePlugin(IServiceContainerHelper serviceContainerHelper)
        {
            IMenuService service = serviceContainerHelper.GetService<IMenuService>();
            IYouWebIt youWebIt = serviceContainerHelper.GetService<IYouWebIt>();
            if (service != null)
            {
                #region IE

                RegistryKey ieRegistryKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\IEXPLORE.EXE");
                if (ieRegistryKey == null || !File.Exists((string)ieRegistryKey.GetValue(null)))
                {
                    Debug.WriteLine("Internet Explorer not found.");
                    return;
                }
                ToolStripMenuItem ietoolStripMenuItem = new ToolStripMenuItem();
                ietoolStripMenuItem.Name = "IEToolStripMenuItem";
                ietoolStripMenuItem.Image = global::YouWebIt.Properties.Resources.ie7;
                ietoolStripMenuItem.Size = new System.Drawing.Size(184, 22);
                ietoolStripMenuItem.Text = "IE";
                ietoolStripMenuItem.Click += delegate
                {
                    MethodInvoker v = delegate
                    {
                        Uri homeUri = youWebIt.GetWebServerHomePageUri();
                        if (homeUri == null)
                        {
                            MessageBox.Show(string.Format("Cant find any aspx, html or htm in {0}.", youWebIt.ServerPhysicalPath), "ServerLiht", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        Process.Start((string)ieRegistryKey.GetValue(null), homeUri.ToString());
                    };
                    v.BeginInvoke(null, null);
                };
                service.AddToolStripMenuItem(ietoolStripMenuItem);

                #endregion IE

                #region FireFox

                RegistryKey fireFoxRegistryKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\firefox.exe");
                if (fireFoxRegistryKey == null || !File.Exists((string)fireFoxRegistryKey.GetValue(null)))
                {
                    Debug.WriteLine("FireFow not found.");
                    return;
                }
                ToolStripMenuItem fireFoxToolStripMenuItem = new ToolStripMenuItem();
                fireFoxToolStripMenuItem.Name = "FireFoxToolStripMenuItem";
                fireFoxToolStripMenuItem.Image = global::YouWebIt.Properties.Resources.firefox4;
                fireFoxToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
                fireFoxToolStripMenuItem.Text = "FireFox";
                fireFoxToolStripMenuItem.Click += delegate
                {
                    MethodInvoker v = delegate
                    {
                        Uri homeUri = youWebIt.GetWebServerHomePageUri();
                        if (homeUri == null)
                        {
                            MessageBox.Show(string.Format("Cant find any aspx, html or htm in {0}.", youWebIt.ServerPhysicalPath), "ServerLiht", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        Process.Start((string)fireFoxRegistryKey.GetValue(null), homeUri.ToString());
                    };
                    v.BeginInvoke(null, null);
                };
                service.AddToolStripMenuItem(fireFoxToolStripMenuItem);

                #endregion FireFox
            }

        }

        #endregion
    }
}
