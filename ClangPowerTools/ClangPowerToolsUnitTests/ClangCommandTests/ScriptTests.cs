using ClangPowerTools.Helpers;
using ClangPowerTools.Services;
using EnvDTE;
using EnvDTE80;
using Microsoft;
using Microsoft.VisualStudio.Shell;
using System.Collections.Generic;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace ClangPowerTools.Tests.ClangCommandTests
{

  [VsTestSettings(UIThread = true)]
  public class ScriptTests
  {
    #region Members

    private readonly string solutionPath = @"C:\GitRepos\ClangPowerToolsTests\CustomAllocator\CustomAllocator.sln";

    // Expected results for files
    private readonly string compileOnFileExpectedResult = @"PowerShell.exe -ExecutionPolicy Unrestricted -NoProfile -Noninteractive -command '& ''c:\users\enache ionut\appdata\local\microsoft\visualstudio\16.0_ada83f57exp\extensions\caphyon\clang power tools\5.2.1\clang-build.ps1''  -proj ''C:\GitRepos\ClangPowerToolsTests\CustomAllocator\CustomAllocatorTest\CustomAllocatorTest.vcxproj'' -file ''C:\GitRepos\ClangPowerToolsTests\CustomAllocator\CustomAllocatorTest\CustomAllocatorTest.cpp'' -active-config ''Debug|x64'' -clang-flags  (''-Wall'',''-fms-compatibility-version=19.10'',''-Wmicrosoft'',''-Wno-invalid-token-paste'',''-Wno-unknown-pragmas'',''-Wno-unused-value'') -parallel -vs-ver 2019 -vs-sku Professional -dir ''C:\GitRepos\ClangPowerToolsTests\CustomAllocator\CustomAllocator.sln'' '";
    private readonly string tidyOnFileExpectedResult = @"PowerShell.exe -ExecutionPolicy Unrestricted -NoProfile -Noninteractive -command '& ''c:\users\enache ionut\appdata\local\microsoft\visualstudio\16.0_ada83f57exp\extensions\caphyon\clang power tools\5.2.1\clang-build.ps1''  -proj ''C:\GitRepos\ClangPowerToolsTests\CustomAllocator\CustomAllocatorTest\CustomAllocatorTest.vcxproj'' -file ''C:\GitRepos\ClangPowerToolsTests\CustomAllocator\CustomAllocatorTest\CustomAllocatorTest.cpp'' -active-config ''Debug|x64'' -clang-flags  (''-Wall'',''-fms-compatibility-version=19.10'',''-Wmicrosoft'',''-Wno-invalid-token-paste'',''-Wno-unknown-pragmas'',''-Wno-unused-value'') -tidy ''-*,modernize-avoid-bind,modernize-avoid-c-arrays,modernize-concat-nested-namespaces,modernize-deprecated-headers,modernize-deprecated-ios-base-aliases,modernize-loop-convert,modernize-make-shared,modernize-make-unique,modernize-pass-by-value,modernize-raw-string-literal,modernize-redundant-void-arg,modernize-replace-auto-ptr,modernize-replace-random-shuffle,modernize-return-braced-init-list,modernize-shrink-to-fit,modernize-unary-static-assert,modernize-use-auto,modernize-use-bool-literals,modernize-use-default-member-init,modernize-use-emplace,modernize-use-equals-default,modernize-use-equals-delete,modernize-use-nodiscard,modernize-use-noexcept,modernize-use-nullptr,modernize-use-override,modernize-use-trailing-return-type,modernize-use-transparent-functors,modernize-use-uncaught-exceptions,modernize-use-using,readability-avoid-const-params-in-decls,readability-braces-around-statements,readability-const-return-type,readability-container-size-empty,readability-convert-member-functions-to-static,readability-deleted-default,readability-delete-null-pointer,readability-else-after-return,readability-identifier-naming,readability-implicit-bool-conversion,readability-inconsistent-declaration-parameter-name,readability-isolate-declaration,readability-magic-numbers,readability-misleading-indentation,readability-misplaced-array-index,readability-named-parameter,readability-non-const-parameter,readability-redundant-control-flow,readability-redundant-declaration,readability-redundant-function-ptr-dereference,readability-redundant-member-init,readability-redundant-preprocessor,readability-redundant-smartptr-get,readability-redundant-string-cstr,readability-redundant-string-init,readability-simplify-boolean-expr,readability-simplify-subscript-expr,readability-static-accessed-through-instance,readability-static-definition-in-anonymous-namespace,readability-string-compare,readability-uniqueptr-delete-release,readability-uppercase-literal-suffix'' -header-filter ''.*'' -vs-ver 2019 -vs-sku Professional -dir ''C:\GitRepos\ClangPowerToolsTests\CustomAllocator\CustomAllocator.sln'' '";
    private readonly string tidyFixOnFileExpectedResult = @"PowerShell.exe -ExecutionPolicy Unrestricted -NoProfile -Noninteractive -command '& ''c:\users\enache ionut\appdata\local\microsoft\visualstudio\16.0_ada83f57exp\extensions\caphyon\clang power tools\5.2.1\clang-build.ps1''  -proj ''C:\GitRepos\ClangPowerToolsTests\CustomAllocator\CustomAllocatorTest\CustomAllocatorTest.vcxproj'' -file ''C:\GitRepos\ClangPowerToolsTests\CustomAllocator\CustomAllocatorTest\CustomAllocatorTest.cpp'' -active-config ''Debug|x64'' -clang-flags  (''-Wall'',''-fms-compatibility-version=19.10'',''-Wmicrosoft'',''-Wno-invalid-token-paste'',''-Wno-unknown-pragmas'',''-Wno-unused-value'') -tidy-fix ''-*,modernize-avoid-bind,modernize-avoid-c-arrays,modernize-concat-nested-namespaces,modernize-deprecated-headers,modernize-deprecated-ios-base-aliases,modernize-loop-convert,modernize-make-shared,modernize-make-unique,modernize-pass-by-value,modernize-raw-string-literal,modernize-redundant-void-arg,modernize-replace-auto-ptr,modernize-replace-random-shuffle,modernize-return-braced-init-list,modernize-shrink-to-fit,modernize-unary-static-assert,modernize-use-auto,modernize-use-bool-literals,modernize-use-default-member-init,modernize-use-emplace,modernize-use-equals-default,modernize-use-equals-delete,modernize-use-nodiscard,modernize-use-noexcept,modernize-use-nullptr,modernize-use-override,modernize-use-trailing-return-type,modernize-use-transparent-functors,modernize-use-uncaught-exceptions,modernize-use-using,readability-avoid-const-params-in-decls,readability-braces-around-statements,readability-const-return-type,readability-container-size-empty,readability-convert-member-functions-to-static,readability-deleted-default,readability-delete-null-pointer,readability-else-after-return,readability-identifier-naming,readability-implicit-bool-conversion,readability-inconsistent-declaration-parameter-name,readability-isolate-declaration,readability-magic-numbers,readability-misleading-indentation,readability-misplaced-array-index,readability-named-parameter,readability-non-const-parameter,readability-redundant-control-flow,readability-redundant-declaration,readability-redundant-function-ptr-dereference,readability-redundant-member-init,readability-redundant-preprocessor,readability-redundant-smartptr-get,readability-redundant-string-cstr,readability-redundant-string-init,readability-simplify-boolean-expr,readability-simplify-subscript-expr,readability-static-accessed-through-instance,readability-static-definition-in-anonymous-namespace,readability-string-compare,readability-uniqueptr-delete-release,readability-uppercase-literal-suffix'' -header-filter ''.*'' -vs-ver 2019 -vs-sku Professional -dir ''C:\GitRepos\ClangPowerToolsTests\CustomAllocator\CustomAllocator.sln'' '";

    // Expected results for projects
    private readonly string compileOnProjectExpectedResult = @"PowerShell.exe -ExecutionPolicy Unrestricted -NoProfile -Noninteractive -command '& ''c:\users\enache ionut\appdata\local\microsoft\visualstudio\16.0_ada83f57exp\extensions\caphyon\clang power tools\5.2.1\clang-build.ps1''  -proj ''C:\GitRepos\ClangPowerToolsTests\CustomAllocator\CustomAllocator\CustomAllocator.vcxproj'' -active-config ''Debug|x64'' -clang-flags  (''-Wall'',''-fms-compatibility-version=19.10'',''-Wmicrosoft'',''-Wno-invalid-token-paste'',''-Wno-unknown-pragmas'',''-Wno-unused-value'') -parallel -vs-ver 2019 -vs-sku Professional -dir ''C:\GitRepos\ClangPowerToolsTests\CustomAllocator\CustomAllocator.sln'' '";
    private readonly string tidyOnProjectExpectedResult = @"PowerShell.exe -ExecutionPolicy Unrestricted -NoProfile -Noninteractive -command '& ''c:\users\enache ionut\appdata\local\microsoft\visualstudio\16.0_ada83f57exp\extensions\caphyon\clang power tools\5.2.1\clang-build.ps1''  -proj ''C:\GitRepos\ClangPowerToolsTests\CustomAllocator\CustomAllocator\CustomAllocator.vcxproj'' -active-config ''Debug|x64'' -clang-flags  (''-Wall'',''-fms-compatibility-version=19.10'',''-Wmicrosoft'',''-Wno-invalid-token-paste'',''-Wno-unknown-pragmas'',''-Wno-unused-value'') -tidy ''-*,modernize-avoid-bind,modernize-avoid-c-arrays,modernize-concat-nested-namespaces,modernize-deprecated-headers,modernize-deprecated-ios-base-aliases,modernize-loop-convert,modernize-make-shared,modernize-make-unique,modernize-pass-by-value,modernize-raw-string-literal,modernize-redundant-void-arg,modernize-replace-auto-ptr,modernize-replace-random-shuffle,modernize-return-braced-init-list,modernize-shrink-to-fit,modernize-unary-static-assert,modernize-use-auto,modernize-use-bool-literals,modernize-use-default-member-init,modernize-use-emplace,modernize-use-equals-default,modernize-use-equals-delete,modernize-use-nodiscard,modernize-use-noexcept,modernize-use-nullptr,modernize-use-override,modernize-use-trailing-return-type,modernize-use-transparent-functors,modernize-use-uncaught-exceptions,modernize-use-using,readability-avoid-const-params-in-decls,readability-braces-around-statements,readability-const-return-type,readability-container-size-empty,readability-convert-member-functions-to-static,readability-deleted-default,readability-delete-null-pointer,readability-else-after-return,readability-identifier-naming,readability-implicit-bool-conversion,readability-inconsistent-declaration-parameter-name,readability-isolate-declaration,readability-magic-numbers,readability-misleading-indentation,readability-misplaced-array-index,readability-named-parameter,readability-non-const-parameter,readability-redundant-control-flow,readability-redundant-declaration,readability-redundant-function-ptr-dereference,readability-redundant-member-init,readability-redundant-preprocessor,readability-redundant-smartptr-get,readability-redundant-string-cstr,readability-redundant-string-init,readability-simplify-boolean-expr,readability-simplify-subscript-expr,readability-static-accessed-through-instance,readability-static-definition-in-anonymous-namespace,readability-string-compare,readability-uniqueptr-delete-release,readability-uppercase-literal-suffix'' -header-filter ''.*'' -vs-ver 2019 -vs-sku Professional -dir ''C:\GitRepos\ClangPowerToolsTests\CustomAllocator\CustomAllocator.sln'' '";
    private readonly string tidyFixOnProjectExpectedResult = @"PowerShell.exe -ExecutionPolicy Unrestricted -NoProfile -Noninteractive -command '& ''c:\users\enache ionut\appdata\local\microsoft\visualstudio\16.0_ada83f57exp\extensions\caphyon\clang power tools\5.2.1\clang-build.ps1''  -proj ''C:\GitRepos\ClangPowerToolsTests\CustomAllocator\CustomAllocator\CustomAllocator.vcxproj'' -active-config ''Debug|x64'' -clang-flags  (''-Wall'',''-fms-compatibility-version=19.10'',''-Wmicrosoft'',''-Wno-invalid-token-paste'',''-Wno-unknown-pragmas'',''-Wno-unused-value'') -tidy-fix ''-*,modernize-avoid-bind,modernize-avoid-c-arrays,modernize-concat-nested-namespaces,modernize-deprecated-headers,modernize-deprecated-ios-base-aliases,modernize-loop-convert,modernize-make-shared,modernize-make-unique,modernize-pass-by-value,modernize-raw-string-literal,modernize-redundant-void-arg,modernize-replace-auto-ptr,modernize-replace-random-shuffle,modernize-return-braced-init-list,modernize-shrink-to-fit,modernize-unary-static-assert,modernize-use-auto,modernize-use-bool-literals,modernize-use-default-member-init,modernize-use-emplace,modernize-use-equals-default,modernize-use-equals-delete,modernize-use-nodiscard,modernize-use-noexcept,modernize-use-nullptr,modernize-use-override,modernize-use-trailing-return-type,modernize-use-transparent-functors,modernize-use-uncaught-exceptions,modernize-use-using,readability-avoid-const-params-in-decls,readability-braces-around-statements,readability-const-return-type,readability-container-size-empty,readability-convert-member-functions-to-static,readability-deleted-default,readability-delete-null-pointer,readability-else-after-return,readability-identifier-naming,readability-implicit-bool-conversion,readability-inconsistent-declaration-parameter-name,readability-isolate-declaration,readability-magic-numbers,readability-misleading-indentation,readability-misplaced-array-index,readability-named-parameter,readability-non-const-parameter,readability-redundant-control-flow,readability-redundant-declaration,readability-redundant-function-ptr-dereference,readability-redundant-member-init,readability-redundant-preprocessor,readability-redundant-smartptr-get,readability-redundant-string-cstr,readability-redundant-string-init,readability-simplify-boolean-expr,readability-simplify-subscript-expr,readability-static-accessed-through-instance,readability-static-definition-in-anonymous-namespace,readability-string-compare,readability-uniqueptr-delete-release,readability-uppercase-literal-suffix'' -header-filter ''.*'' -vs-ver 2019 -vs-sku Professional -dir ''C:\GitRepos\ClangPowerToolsTests\CustomAllocator\CustomAllocator.sln'' '";

    private readonly Dictionary<string, string> mVsVersions = new Dictionary<string, string>
    {
      {"11.0", "2010"},
      {"12.0", "2012"},
      {"13.0", "2013"},
      {"14.0", "2015"},
      {"15.0", "2017"},
      {"16.0", "2019"}
    };

    #endregion

    #region Test Methods

    #region On File Tests


    [VsFact(Version = "2019")]
    public async Task CompileOnFile_CreateScript_Async()
    {
      //Arrange
      await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
      var settingsHandler = new SettingsHandler();

      //Act
      LoadSolution();
      settingsHandler.ResetSettings();

      GetVisualStudioInfo(out string edition, out string version, out IItem item, true);
      var compileOnFileScriptTestResult = CreateScript(CommandIds.kCompileId, edition, version, item);

      CloseSolution();

      //Assert
      Assert.Equal(compileOnFileScriptTestResult, compileOnFileExpectedResult);
    }

    [VsFact(Version = "2019")]
    public async Task TidyOnFile_CreateScript_Async()
    {
      //Arrange
      await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
      var settingsHandler = new SettingsHandler();

      //Act
      LoadSolution();
      settingsHandler.ResetSettings();

      GetVisualStudioInfo(out string edition, out string version, out IItem item, true);
      var tidyOnFileScriptTestResult = CreateScript(CommandIds.kTidyId, edition, version, item);

      CloseSolution();

      //Assert
      Assert.Equal(tidyOnFileScriptTestResult, tidyOnFileExpectedResult);
    }

    [VsFact(Version = "2019")]
    public async Task TidyFixOnFile_CreateScript_Async()
    {
      //Arrange

      await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
      var settingsHandler = new SettingsHandler();

      //Act
      LoadSolution();
      settingsHandler.ResetSettings();

      GetVisualStudioInfo(out string edition, out string version, out IItem item, true);
      var tidyFixOnFileScriptTestResult = CreateScript(CommandIds.kTidyFixId, edition, version, item);

      CloseSolution();

      //Assert
      Assert.Equal(tidyFixOnFileScriptTestResult, tidyFixOnFileExpectedResult);
    }

    #endregion

    #region On Project Tests

    [VsFact(Version = "2019")]
    public async Task CompileOnProject_CreateScript_Async()
    {
      //Arrange
      await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
      var settingsHandler = new SettingsHandler();

      //Act
      LoadSolution();
      settingsHandler.ResetSettings();

      GetVisualStudioInfo(out string edition, out string version, out IItem item, false);
      var compileOnProjectScriptTestResult = CreateScript(CommandIds.kCompileId, edition, version, item);

      CloseSolution();

      //Assert
      Assert.Equal(compileOnProjectScriptTestResult, compileOnProjectExpectedResult);
    }

    [VsFact(Version = "2019")]
    public async Task TidyOnProject_CreateScript_Async()
    {
      //Arrange
      await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
      var settingsHandler = new SettingsHandler();

      //Act
      LoadSolution();
      settingsHandler.ResetSettings();

      GetVisualStudioInfo(out string edition, out string version, out IItem item, false);
      var tidyOnProjectScriptTestResult = CreateScript(CommandIds.kTidyId, edition, version, item);

      CloseSolution();

      //Assert
      Assert.Equal(tidyOnProjectScriptTestResult, tidyOnProjectExpectedResult);
    }


    [VsFact(Version = "2019")]
    public async Task TidyFixOnProject_CreateScript_Async()
    {
      //Arrange
      await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
      var settingsHandler = new SettingsHandler();

      //Act
      LoadSolution();
      settingsHandler.ResetSettings();

      GetVisualStudioInfo(out string edition, out string version, out IItem item, false);
      var tidyFixOnProjectScriptTestResult = CreateScript(CommandIds.kTidyFixId, edition, version, item);

      CloseSolution();

      //Assert
      Assert.Equal(tidyFixOnProjectScriptTestResult, tidyFixOnProjectExpectedResult);
    }

    #endregion

    #endregion

    #region Private Methods

    private void LoadSolution()
    {
      ThreadHelper.ThrowIfNotOnUIThread();
      var dte = (DTE2)ServiceProvider.GlobalProvider.GetService(typeof(DTE));
      Assumes.Present(dte);

      dte.Solution.Open(solutionPath);

      var build = dte.Solution.SolutionBuild;
      build.Build(true);
    }

    private void CloseSolution()
    {
      ThreadHelper.ThrowIfNotOnUIThread();
      var dte = (DTE2)ServiceProvider.GlobalProvider.GetService(typeof(DTE));
      Assumes.Present(dte);

      dte.Solution.Close();
    }

    private string CreateScript(int commandId, string edition, string version, IItem item)
    {
      var runModeParameters = ScriptGenerator.GetRunModeParamaters();
      var genericParameters = ScriptGenerator.GetGenericParamaters(commandId, edition, version);

      var itemRelatedParameters = item is CurrentProject ?
        ScriptGenerator.GetItemRelatedParameters(item as CurrentProject) :
        ScriptGenerator.GetItemRelatedParameters(item as CurrentProjectItem);

      return JoinUtility.Join(" ", runModeParameters.Remove(runModeParameters.Length - 1), itemRelatedParameters, genericParameters, "'");
    }

    private void GetVisualStudioInfo(out string edition, out string version, out IItem item, bool onFile)
    {
      ThreadHelper.ThrowIfNotOnUIThread();

      var dte = (DTE2)VsServiceProvider.GetService(typeof(DTE));
      edition = dte.Edition;

      mVsVersions.TryGetValue(dte.Version, out string vsVersion);
      version = vsVersion;

      item = onFile ?
        (IItem) new CurrentProjectItem(dte.Solution.Projects.Item(1).ProjectItems.Item(4)) :
        (IItem) new CurrentProject(dte.Solution.Projects.Item(2));
    }

    #endregion
  }
}
