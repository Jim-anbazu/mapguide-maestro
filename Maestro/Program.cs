﻿#region Disclaimer / License
// Copyright (C) 2010, Jackie Ng
// http://trac.osgeo.org/mapguide/wiki/maestro, jumpinjackie@gmail.com
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 
#endregion
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ICSharpCode.Core;
using System.IO;
using Maestro.Base;
using System.Reflection;
using ICSharpCode.Core.Services;
using Maestro.Base.UI;
using OSGeo.MapGuide.MaestroAPI;
using ICSharpCode.Core.WinForms;
using OSGeo.MapGuide.MaestroAPI.Resource.Validation;
using Maestro.Login;
using Maestro.Shared.UI;

namespace Maestro
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(true);
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);

            string lang = PreferredSiteList.InitCulture();

            var btw = BroadcastTextWriter.Instance;

            // Logging service by default uses System.Diagnostics.Debug. Re-route this
            // to our writer
            var wfmsg = WinFormsMessageService.Instance;
            ServiceManager.LoggingService = new TextWriterLoggingService(btw);
            ServiceManager.MessageService = wfmsg;

            // The LoggingService is a small wrapper around log4net.
            // Our application contains a .config file telling log4net to write
            // to System.Diagnostics.Trace.
            LoggingService.Info("Application start");

            if (Platform.IsRunningOnMono)
            {
                LoggingService.Info(Properties.Resources.Warn_Mono);
            }

            //Init our default set of resource validators
            ResourceValidatorLoader.LoadStockValidators();

            AppDomain.CurrentDomain.AssemblyLoad += new AssemblyLoadEventHandler(CurrentDomain_AssemblyLoad);

            // Get a reference to the entry assembly (Startup.exe)
            Assembly exe = typeof(Program).Assembly;

            // Set the root path of our application. ICSharpCode.Core looks for some other
            // paths relative to the application root:
            // "data/resources" for language resources, "data/options" for default options
            FileUtility.ApplicationRootPath = Path.GetDirectoryName(exe.Location);

            LoggingService.Info("Starting core services...");

            // CoreStartup is a helper class making starting the Core easier.
            // The parameter is used as the application name, e.g. for the default title of
            // MessageService.ShowMessage() calls.
            CoreStartup coreStartup = new CoreStartup("MapGuide Maestro");
            // It is also used as default storage location for the application settings:
            // "%Application Data%\%Application Name%", but you can override that by setting c.ConfigDirectory
            
            // Specify the name of the application settings file (.xml is automatically appended)
            coreStartup.PropertiesName = "AppProperties";

            // Initializes the Core services (ResourceService, PropertyService, etc.)
            coreStartup.StartCoreServices();

            LoggingService.Info("Looking for AddIns...");
            // Searches for ".addin" files in the application directory.
            coreStartup.AddAddInsFromDirectory(Path.Combine(FileUtility.ApplicationRootPath, "AddIns"));

            // Searches for a "AddIns.xml" in the user profile that specifies the names of the
            // add-ins that were deactivated by the user, and adds "external" AddIns.
            coreStartup.ConfigureExternalAddIns(Path.Combine(PropertyService.ConfigDirectory, "AddIns.xml"));

            // Searches for add-ins installed by the user into his profile directory. This also
            // performs the job of installing, uninstalling or upgrading add-ins if the user
            // requested it the last time this application was running.
            coreStartup.ConfigureUserAddIns(Path.Combine(PropertyService.ConfigDirectory, "AddInInstallTemp"),
                                            Path.Combine(PropertyService.ConfigDirectory, "AddIns"));

            ResourceService.Language = lang;
            LoggingService.Info("Loading AddInTree...");
            // Now finally initialize the application. This parses the ".addin" files and
            // creates the AddIn tree. It also automatically runs the commands in
            // "/Workspace/Autostart"
            coreStartup.RunInitialization();

            LoggingService.Info("Initializing Workbench...");
            // Workbench is our class from the base project, this method creates an instance
            // of the main form.
            Workbench.InitializeWorkbench();

            try
            {
                LoggingService.Info("Running application...");
                // Workbench.Instance is the instance of the main form, run the message loop.
                Application.Run(Workbench.Instance);
            }
            finally
            {
                try
                {
                    // Save changed properties
                    PropertyService.Save();
                }
                catch (Exception ex)
                {
                    ErrorDialog.Show("Error storing properties", ex.ToString());
                }
            }
            LoggingService.Info("Application shutdown");
        }

        static void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            LoggingService.InfoFormatted("Loaded assembly: {0}", args.LoadedAssembly.GetName().Name);
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            ErrorDialog.Show(e.Exception);
        }
    }
}
