global using Task = System.Threading.Tasks.Task;

using System;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using ReRoExtension.Command;
using ReRoExtension.Wpf.BuildMetadata;

namespace ReRoExtension
{
    [InstalledProductRegistration("Relational Roslyn", "Inspect your project's Roslyn metadata with SQL.", Vsix.Version, IconResourceID = 400)]
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(PackageGuids.ReRoPackageString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(BuildMetadataDatabaseWindow))]
    //[ProvideOptionPage(typeof(OptionsProvider.GeneralOptions), "Relational Roslyn", "General", 0, 0, true)]
    //[ProvideProfile(typeof(OptionsProvider.GeneralOptions), "Relational Roslyn", "General", 0, 0, true)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.NoSolution_string, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExistsAndFullyLoaded_string, PackageAutoLoadFlags.BackgroundLoad)]
    public sealed class RelationalRoslynPackage : AsyncPackage
    {
        public RelationalRoslynPackage()
        {
        }

        #region Package Members

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            //var componentModel = (IComponentModel)await this.GetServiceAsync(typeof(SComponentModel));
            //var ss = componentModel.GetService<StartStopper>();
            //ss.AsyncStart();

            await BuildMetadataDatabaseCommand.InitializeAsync(
                this
                );

        }
        #endregion
    }
}
