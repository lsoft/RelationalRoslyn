using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;

namespace ReRoExtension.Wpf.BuildMetadata
{
    [Guid("B0A7D6BE-B19E-4226-8C38-20AA217E9EE8")]
    public class BuildMetadataDatabaseWindow : ToolWindowPane
    {
        public BuildMetadataDatabaseWindow(
            ) : base(null)
        {
            this.Caption = "Building metadata database";

            //var componentModel = (IComponentModel)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SComponentModel));
            //var solutionNameProvider = componentModel.GetService<ISolutionNameProvider>();
            //var configurationProvider = componentModel.GetService<IConfigurationProvider>();
            //var inclusionContainer = componentModel.GetService<InclusionContainer>();

            this.Content = new BuildMetadataDatabaseWindowControl(
                );
        }
    }
}
