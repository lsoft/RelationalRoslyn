using ReRoExtension.Entity;
using ReRoExtension.Helper;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ReRoExtension.RelationalModel
{
    public static class Builder
    {
        public static EntityModel BuildEntityModel(
            Action<string> buildingProgressMadeAction
            )
        {
            var result = new EntityModel();

            var componentModel = (IComponentModel)AsyncPackage.GetGlobalService(typeof(SComponentModel));
            var workspace = (Workspace)componentModel.GetService<VisualStudioWorkspace>();

            if (workspace == null)
            {
                return result;
            }

            var projects = workspace.CurrentSolution.Projects.ToList();
            if (projects.Count == 0)
            {
                return result;
            }

            for (var pi = 0; pi < projects.Count; pi++)
            {
                var project = projects[pi];

                buildingProgressMadeAction(
                    $"{pi + 1} / {projects.Count} ({project.Name})"
                    );

                if (!project.TryGetCompilation(out var compilation))
                {
                    continue;
                }

                var projectEntity = BuildProject(project, compilation);

                result.Projects.Add(projectEntity);
            }

            return result;
        }

        private static ProjectEntity BuildProject(
            Project project,
            Compilation compilation
            )
        {
            var projectEntity = new ProjectEntity
            {
                ProjectGuid = project.Id.Id,
                ProjectName = project.Name
            };

            var allTypesInCompilation = compilation.Assembly.GlobalNamespace.GetAllTypes(a => true);
            foreach (var typeSymbol in allTypesInCompilation)
            {
                if (typeSymbol is not INamedTypeSymbol namedTypeSymbol)
                {
                    continue;
                }

                var namedTypeEntity = BuildNamedType(
                    projectEntity.ProjectGuid,
                    namedTypeSymbol
                    );

                projectEntity.NamedTypes.Add(namedTypeEntity);
            }

            return projectEntity;
        }

        private static NamedTypeEntity BuildNamedType(
            Guid projectGuid,
            INamedTypeSymbol namedTypeSymbol
            )
        {
            var typeEntity = new NamedTypeEntity
            {
                ProjectGuid = projectGuid,
                ContainingNamespace = namedTypeSymbol.ContainingNamespace.ToFullyQualifiedName(),
                GlobalName = namedTypeSymbol.ToGlobalDisplayString(),
                SourceEntityUid = Guid.NewGuid().ToString(),
                FullName = namedTypeSymbol.ToFullDisplayString(),
                Name = namedTypeSymbol.Name,
                IsAbstract = namedTypeSymbol.IsAbstract,
                IsVirtual = namedTypeSymbol.IsVirtual,
                IsSealed = namedTypeSymbol.IsSealed,
                IsStatic = namedTypeSymbol.IsStatic,
                IsValueType = namedTypeSymbol.IsValueType,
            };

            var sfi = BuildSourceInfo(namedTypeSymbol);
            typeEntity.Apply(sfi);

            foreach (var symbol in namedTypeSymbol.GetMembers())
            {
                BuildMemberAllKind(
                    namedTypeSymbol,
                    typeEntity,
                    symbol
                    );
            }

            return typeEntity;
        }

        private static void BuildMemberAllKind(
            INamedTypeSymbol namedTypeSymbol,
            NamedTypeEntity type,
            ISymbol symbol
            )
        {
            {
                var memberEntity = BuildMember(namedTypeSymbol, symbol);
                type.Members.Add(memberEntity);
            }

            if (symbol is IFieldSymbol fieldSymbol)
            {
                //we do not want to process backing property's fields
                if (fieldSymbol.AssociatedSymbol is not null)
                {
                    return;
                }

                var fieldEntity = BuildField(namedTypeSymbol, fieldSymbol);

                type.FieldMembers.Add(fieldEntity);
            }
            else if (symbol is IPropertySymbol propertySymbol)
            {
                //we want to process indexers in special way, as a separate member kind
                if (propertySymbol.IsIndexer)
                {
                    return;
                }

                var propertyEntity = BuildProperty(namedTypeSymbol, propertySymbol);

                type.PropertyMembers.Add(propertyEntity);
            }
            else if (symbol is IMethodSymbol methodSymbol)
            {
                //we do not want to process backing property's methods (get, set)
                if (methodSymbol.MethodKind.In(MethodKind.PropertyGet, MethodKind.PropertySet))
                {
                    return;
                }

                var methodEntity = BuildMethod(namedTypeSymbol, methodSymbol);

                type.MethodMembers.Add(methodEntity);
            }
        }


        private static MemberEntity BuildMember(
            INamedTypeSymbol namedTypeSymbol,
            ISymbol symbol
            )
        {
            var result = new MemberEntity
            {
                TypeGlobalName = namedTypeSymbol.ToGlobalDisplayString(),
                SourceEntityUid = Guid.NewGuid().ToString(),
                Name = symbol.Name,
                IsAbstract = symbol.IsAbstract,
                IsVirtual = symbol.IsVirtual,
                IsOverride = symbol.IsOverride,
            };

            var sfi = BuildSourceInfo(symbol);
            result.Apply(sfi);

            return result;
        }

        private static FieldMemberEntity BuildField(
            INamedTypeSymbol namedTypeSymbol,
            IFieldSymbol fieldSymbol
            )
        {
            var result = new FieldMemberEntity
            {
                TypeGlobalName = namedTypeSymbol.ToGlobalDisplayString(),
                SourceEntityUid = Guid.NewGuid().ToString(),
                Name = fieldSymbol.Name,
                IsAbstract = fieldSymbol.IsAbstract,
                IsVirtual = fieldSymbol.IsVirtual,
                IsOverride = fieldSymbol.IsOverride,
                IsReadOnly = fieldSymbol.IsReadOnly,
                FieldGlobalName = fieldSymbol.Type.ToGlobalDisplayString(),
            };

            var sfi = BuildSourceInfo(fieldSymbol);
            result.Apply(sfi);

            return result;
        }

        private static PropertyMemberEntity BuildProperty(
            INamedTypeSymbol namedTypeSymbol,
            IPropertySymbol propertySymbol
            )
        {
            var result = new PropertyMemberEntity
            {
                TypeGlobalName = namedTypeSymbol.ToGlobalDisplayString(),
                SourceEntityUid = Guid.NewGuid().ToString(),
                Name = propertySymbol.Name,
                IsAbstract = propertySymbol.IsAbstract,
                IsVirtual = propertySymbol.IsVirtual,
                IsOverride = propertySymbol.IsOverride,
                PropertyGlobalName = propertySymbol.Type.ToGlobalDisplayString(),
            };

            var sfi = BuildSourceInfo(propertySymbol);
            result.Apply(sfi);

            return result;
        }

        private static MethodMemberEntity BuildMethod(
            INamedTypeSymbol namedTypeSymbol,
            IMethodSymbol methodSymbol
            )
        {
            var result = new MethodMemberEntity
            {
                TypeGlobalName = namedTypeSymbol.ToGlobalDisplayString(),
                SourceEntityUid = Guid.NewGuid().ToString(),
                Name = methodSymbol.Name,
                MethodKind = methodSymbol.MethodKind.ToString(),
                IsConstructor = methodSymbol.MethodKind == MethodKind.Constructor,
                IsStaticConstructor = methodSymbol.MethodKind == MethodKind.StaticConstructor,
                IsAbstract = methodSymbol.IsAbstract,
                IsVirtual = methodSymbol.IsVirtual,
                IsOverride = methodSymbol.IsOverride,
                IsAsync = methodSymbol.IsAsync,
                ReturnTypeGlobalName = methodSymbol.ReturnType.ToGlobalDisplayString(),
                ParametersCount = methodSymbol.Parameters.Length
            };

            var sfi = BuildSourceInfo(methodSymbol);
            result.Apply(sfi);

            return result;
        }

        private static SourceFullInfo BuildSourceInfo(
            ISymbol symbol
            )
        {
            var result = new SourceFullInfo();

            foreach (var syntaxReference in symbol.DeclaringSyntaxReferences)
            {
                var syntax = syntaxReference.GetSyntax();
                var text = syntax.GetText();

                var spi = new SourcePieceInfo(
                    text.ToString(),
                    syntax.SyntaxTree.FilePath,
                    syntaxReference.Span.Start,
                    syntaxReference.Span.End
                    );

                result.SourcePieces.Add(spi);
            }

            return result;
        }
    }

}
