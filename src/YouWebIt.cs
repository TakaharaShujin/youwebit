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
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.VisualStudio.WebHost;

namespace YouWebIt
{
    internal class YouWebIt : IYouWebIt
    {
        private readonly string m_ServerPhysicalPath;
        private readonly int m_ServerPort;
        private readonly string m_ServerVirtualPath;
        private Uri m_webServerUri;
        private Server webServer;

        public YouWebIt()
        {
            //Resolve Physical Path
            string serverPhysicalPathFromSetting;
            try
            {
                serverPhysicalPathFromSetting = ConfigurationManager.AppSettings["ServerPhysicalPath"];
                if (!string.IsNullOrEmpty(serverPhysicalPathFromSetting))
                {
                    m_ServerPhysicalPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PathHelper.FromRelativePath(new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory), serverPhysicalPathFromSetting));
                }
                else
                {
                    m_ServerPhysicalPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).FullName);
                }
            }
            catch (ConfigurationException)
            {
                MessageBox.Show("YouWebIt cant read your config file do to a bad file format.", "YouWebIt", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Environment.Exit(1);
            }

            //Resolve server port
            if (!Int32.TryParse(ConfigurationManager.AppSettings["ServerPort"], out m_ServerPort))
            {
                m_ServerPort = PortHelper.GetRandomPortAvailable();
            }

            m_ServerVirtualPath = "/";


            //If Visual Studio is not installed, we need to return WebDev.WebHost to the CLR. 
            //As Aps.Net WebServer will create a new AppDomain just after that. We need to extract 
            //WebDev.WebHost to the WebSite bin directory. 
            //thank to Scott Hanselman : http://www.hanselman.com/blog/NUnitUnitTestingOfASPNETPagesBaseClassesControlsAndOtherWidgetryUsingCassiniASPNETWebMatrixVisualStudioWebDeveloper.aspx
            //And Phil Haack : http://haacked.com/archive/2006/12/12/Using_WebServer.WebDev_For_Unit_Tests.aspx
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(
                delegate(object sender, ResolveEventArgs args)
                    {
                        if (!args.Name.Equals("WebDev.WebHost, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", StringComparison.Ordinal))
                        {
                            return null;
                        }

                        Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("YouWebIt.WebDev.WebHost.dll");
                        byte[] buf = new byte[stream.Length];
                        stream.Read(buf, 0, (Int32) stream.Length);

                        //NOTE: WebDev.WebHost is going to load itself AGAIN into another AppDomain,
                        // and will be getting it's Assembliesfrom the BIN, including another copy of itself!
                        // Therefore we need to do this step FIRST because I've removed Cassini from the GAC
                        //Copy our assemblies down into the web server's BIN folder

                        string webSiteBinPath = Path.Combine(m_ServerPhysicalPath, "bin");
                        EmbeddedResourceFileHelper.ExtractFile("WebDev.WebHost.dll", webSiteBinPath);

                        return Assembly.Load((byte[]) buf);
                    });
        }

        public Uri WebServerUri
        {
            get
            {
                if (m_webServerUri == null)
                {
                    throw new Exception("m_webServerUri was null. Start the WebServer before.");
                }
                return m_webServerUri;
            }
        }

        public Uri GetWebServerHomePageUri()
        {
            string homePageFileName = GetHomePageFileName(m_ServerPhysicalPath);
            if (string.IsNullOrEmpty(homePageFileName))
            {
                EmbeddedResourceFileHelper.ExtractFile("YouWebIt.Greeting.html", m_ServerPhysicalPath);
                EmbeddedResourceFileHelper.ExtractFile("YouWebIt.GreetingFiles.Greeting.css", Path.Combine(m_ServerPhysicalPath, "GreetingFiles"));
                EmbeddedResourceFileHelper.ExtractFile("YouWebIt.GreetingFiles.screenshot.png", Path.Combine(m_ServerPhysicalPath, "GreetingFiles"));
                EmbeddedResourceFileHelper.ExtractFile("YouWebIt.GreetingFiles.YouWebItLogo.png", Path.Combine(m_ServerPhysicalPath, "GreetingFiles"));
                return new Uri(m_webServerUri, "YouWebIt.Greeting.html");
            }
            Uri homePageUri = new Uri(m_webServerUri, homePageFileName);
            return homePageUri;
        }

        public string ServerPhysicalPath
        {
            get
            {

                return m_ServerPhysicalPath;
            }
        }

        public int ServerPort
        {
            get { return m_ServerPort; }
        }

        #region IYouWebIt Members

        public void LaunchDefaultWebBrowser()
        {
            VoidMethodDelegate asynchLauncher = delegate
                                       {
                                           Uri homePageUri = GetWebServerHomePageUri();
                                           if (homePageUri == null)
                                           {
                                               return;
                                           }                                           
                                           Process.Start(homePageUri.ToString());
                                       };
            asynchLauncher.BeginInvoke(null, null);
        }

        public void OpenRootDirectory()
        {
            VoidMethodDelegate v = delegate
                                       {
                                           Process.Start(@"explorer.exe", m_ServerPhysicalPath);
                                       };
            v.BeginInvoke(null, null);
        }

        #endregion

        public void StartWebServer()
        {
            webServer = new Server(ServerPort, m_ServerVirtualPath, m_ServerPhysicalPath);
            string webServerUrl = String.Format("http://127.0.0.1:{0}{1}", ServerPort, m_ServerVirtualPath);
            m_webServerUri = new Uri(webServerUrl);

            webServer.Start();
        }

        public void StopWebServer()
        {
            webServer.Stop();
        }

        internal static string GetHomePageFileName(string serverPhysicalPath)
        {
            string homePageFileNameFromAppSettins = ConfigurationManager.AppSettings["HomePageFileName"];
            if (!string.IsNullOrEmpty(homePageFileNameFromAppSettins))
            {
                if (!File.Exists(Path.Combine(serverPhysicalPath, homePageFileNameFromAppSettins)))
                {
                    MessageBox.Show("Specified HomePageFileName in config file does not exist.", "YouWebIt", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Process.GetCurrentProcess().Kill();
                }
                return homePageFileNameFromAppSettins;
            }
            if (File.Exists(Path.Combine(serverPhysicalPath, "default.aspx")))
            {
                return "default.aspx";
            }
            if (File.Exists(Path.Combine(serverPhysicalPath, "Default.aspx")))
            {
                return "Default.aspx";
            }
            if (File.Exists(Path.Combine(serverPhysicalPath, "Default.html")))
            {
                return "Default.html";
            }
            if (File.Exists(Path.Combine(serverPhysicalPath, "default.html")))
            {
                return "default.html";
            }
            if (File.Exists(Path.Combine(serverPhysicalPath, "Index.html")))
            {
                return "Index.html";
            }
            if (File.Exists(Path.Combine(serverPhysicalPath, "index.html")))
            {
                return "index.html";
            }
            string[] aspxFiles = Directory.GetFiles(serverPhysicalPath, "*.aspx");
            if (aspxFiles.Length > 0)
            {
                return Path.GetFileName(aspxFiles[0]);
            }
            string[] htmFiles = Directory.GetFiles(serverPhysicalPath, "*.htm");
            if (htmFiles.Length > 0)
            {
                return Path.GetFileName(htmFiles[0]);
            }
            return null;
        }
    }
}