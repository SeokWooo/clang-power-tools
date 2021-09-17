using ClangPowerTools;
using ClangPowerTools.Commands;
using ClangPowerTools.Helpers;
using ClangPowerTools.Services;
using ClangPowerTools.SilentFile;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Text;
using Task = System.Threading.Tasks.Task;


namespace ClangPowerToolsShared.Commands
{
  public class UndoTidyFixCommand : ClangCommand
  {
    #region Properties

    /// <summary>
    /// Gets the instance of the command.
    /// </summary>
    public static UndoTidyFixCommand Instance
    {
      get;
      private set;
    }

    #endregion


    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="CompileCommand"/> class.
    /// Adds our command handlers for menu (commands must exist in the command table file)
    /// </summary>
    /// <param name="package">Owner package, not null.</param>
    protected UndoTidyFixCommand(OleMenuCommandService aCommandService, CommandController aCommandController,
      AsyncPackage aPackage, Guid aGuid, int aId)
        : base(aPackage, aGuid, aId)
    {
      if (null != aCommandService)
      {
        var menuCommandID = new CommandID(CommandSet, Id);
        var menuCommand = new OleMenuCommand(aCommandController.Execute, menuCommandID);
        menuCommand.BeforeQueryStatus += aCommandController.OnBeforeClangCommand;
        menuCommand.Enabled = true;
        aCommandService.AddCommand(menuCommand);
      }
    }

    #endregion


    #region Public Methods

    /// <summary>
    /// Initializes the singleton instance of the command.
    /// </summary>
    /// <param name="package">Owner package, not null.</param>
    public static async Task InitializeAsync(CommandController aCommandController,
      AsyncPackage aPackage, Guid aGuid, int aId)
    {
      // Switch to the main thread - the call to AddCommand in CompileCommand's constructor requires
      // the UI thread.
      await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(aPackage.DisposalToken);

      OleMenuCommandService commandService = await aPackage.GetServiceAsync((typeof(IMenuCommandService))) as OleMenuCommandService;
      Instance = new UndoTidyFixCommand(commandService, aCommandController, aPackage, aGuid, aId);
    }


    public async Task RunClangUndoTidyFixAsync(int aCommandId, CommandUILocation commandUILocation, bool jsonCompilationDbActive = false)
    {
      await PrepareCommmandAsync(commandUILocation, jsonCompilationDbActive);


      await Task.Run(() =>
      {
        lock (mutex)
        {
          try
          {
            using var silentFileController = new SilentFileChangerController();
            using var fileChangerWatcher = new FileChangerWatcher();

            //var tidySettings = SettingsProvider.TidySettingsModel;
            //fileChangerWatcher.OnChanged += FileOpener.Open;

            var dte2 = VsServiceProvider.GetService(typeof(DTE2)) as DTE2;
            string solutionFolderPath = SolutionInfo.IsOpenFolderModeActive() ?
              dte2.Solution.FullName : dte2.Solution.FullName
                                        .Substring(0, dte2.Solution.FullName.LastIndexOf('\\'));
            fileChangerWatcher.Run(solutionFolderPath);

            FilePathCollector fileCollector = new FilePathCollector();
            var filesPath = fileCollector.Collect(mItemsCollector.Items).ToList();

            silentFileController.SilentFiles(filesPath);
            silentFileController.SilentFiles(dte2.Documents);

            if (Directory.Exists(TidyConstants.TidyTempPath))
            {
              foreach (string path in filesPath)
              {
                FileInfo file = new(path);
                string text = File.ReadAllText(Path.Combine(TidyConstants.TidyTempPath, "_" + file.Name));
                File.WriteAllText(file.FullName, text);
              }
              Directory.Delete(TidyConstants.TidyTempPath, true);
            }
          }
          catch (Exception exception)
          {
            VsShellUtilities.ShowMessageBox(AsyncPackage, exception.Message, "Error",
              OLEMSGICON.OLEMSGICON_CRITICAL, OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
          }
        }
      });
    }
    #endregion
  }

}

