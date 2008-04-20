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
using System.Threading;
using System.Windows.Forms;
using YouWebIt;

namespace YouWebIt
{
    public delegate void VoidMethodDelegate();

    public class YouWebItConsoletHost
    {
        private static readonly AutoResetEvent s_YouSendItLauncherAutoResetEvent = new AutoResetEvent(false);
        private static YouWebIt s_YouWebItInstance;
        private static readonly IServiceContainerHelper s_serviceContainerHelper = new ServiceContainerHelper();

        public YouWebItConsoletHost()
        {
            #region Setup Unhandled Exception Handler

            Application.ThreadException += ShowErrorBox;
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            AppDomain.CurrentDomain.UnhandledException += ShowErrorBox;

            #endregion
        }

        private static void ShowErrorBox(object sender, ThreadExceptionEventArgs e)
        {
            ShowErrorBox(e.Exception, null);
        }

        private static void ShowErrorBox(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            ShowErrorBox(ex, "Unhandled exception", e.IsTerminating);
        }

        private static void ShowErrorBox(Exception exception, string message)
        {
            ShowErrorBox(exception, message, false);
        }

        private static void ShowErrorBox(Exception exception, string message, bool mustTerminate)
        {
            try
            {
                using (ExceptionBox box = new ExceptionBox(exception, message, mustTerminate))
                {
                    try
                    {
                        box.ShowDialog(NotifyIconForm.Instance);
                    }
                    catch (InvalidOperationException)
                    {
                        box.ShowDialog();
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show(exception.ToString());
            }
        }

        internal static YouWebIt YouWebItInstance
        {
            get
            {
                if (s_YouWebItInstance == null)
                {
                    throw new Exception("You should call YouWebIt.Main(string[] args) before.");
                }
                return s_YouWebItInstance;
            }
        }

        [STAThread, LoaderOptimization(LoaderOptimization.MultiDomainHost)]
        public static void Main(string[] args)
        {
            MainCore();
        }


        private static void MainCore()
        {
            //if YouWebIt is allready launch, just exit.
            if (ProcessHelper.IsRegister(GetUniqueName()))
            {
                MessageBox.Show("YouWebIt is allready running in this directory.", "YouWebIt", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
                
            try
            {
                ProcessHelper.Register(GetUniqueName());
                LaunchYouWebIt();
            }
            finally
            {
                ProcessHelper.UnRegister(GetUniqueName());
            }
        }

        private static string GetUniqueName()
        {
            return System.Text.RegularExpressions.Regex.Replace(Environment.CommandLine,@"\W*",string.Empty).Replace(".vshost",string.Empty);
        }

        private static void LaunchYouWebIt()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            s_YouWebItInstance = new YouWebIt();
            s_YouWebItInstance.StartWebServer();

            VoidMethodDelegate asynchLauncher = delegate
                                       {
                                           NotifyIconForm.Instance.NotifyIcon.ShowBalloonTip(1000, "YouWebIt", s_YouWebItInstance.WebServerUri.ToString(), ToolTipIcon.Info);
                                           string path = s_YouWebItInstance.ServerPhysicalPath;
                                           if(path.Length >= 50)
                                           {
                                               path = path.Substring(s_YouWebItInstance.ServerPhysicalPath.Length - 50, 50);
                                           }
                                           NotifyIconForm.Instance.NotifyIcon.Text = "YouWebIt:" +
                                               path;
                                           s_serviceContainerHelper.AddService<IMenuService>(NotifyIconForm.Instance);
                                           s_serviceContainerHelper.AddService<IYouWebIt>(s_YouWebItInstance);
                                           
                                           PluginLoader pluginLoader = new PluginLoader(AppDomain.CurrentDomain.BaseDirectory);
                                           pluginLoader.LoadPluginAssemblies(s_serviceContainerHelper);  
                        
                                           YouWebItInstance.LaunchDefaultWebBrowser();
                                           Application.Run(NotifyIconForm.Instance);
                                       };

            asynchLauncher.BeginInvoke(delegate(IAsyncResult result)
                              {
                                  VoidMethodDelegate asynchLauncherCopy = (VoidMethodDelegate)result.AsyncState;
                                  asynchLauncherCopy.EndInvoke(result);
                                  s_YouSendItLauncherAutoResetEvent.Set();
                              }, asynchLauncher);

            s_YouSendItLauncherAutoResetEvent.WaitOne();
            s_YouSendItLauncherAutoResetEvent.Close();

            s_YouWebItInstance.StopWebServer();
        }
    }
}