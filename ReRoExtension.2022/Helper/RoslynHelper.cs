using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace ReRoExtension.Helper
{
    public static class RoslynHelper
    {

        public static string GetJoinedParametersNameAndType(
            this IPropertySymbol property
            )
        {
            var parameters = string.Empty;
            if (property.Parameters.Length > 0)
            {
                parameters = string.Join(
                    ",",
                    property.Parameters.Select(p => p.Type.ToGlobalDisplayString() + " " + p.Name)
                    );
            }

            return parameters;
        }

        public static string GetJoinedParametersName(
            this IPropertySymbol property
            )
        {
            var parameters = string.Empty;
            if (property.Parameters.Length > 0)
            {
                parameters = string.Join(
                    ",",
                    property.Parameters.Select(p => p.Name)
                    );
            }

            return parameters;
        }

        public static string ToSource(
            this Accessibility a
            )
        {
            switch (a)
            {
                case Accessibility.Private:
                    return "private";
                case Accessibility.ProtectedAndInternal:
                    return "protected internal";
                case Accessibility.Protected:
                    return "protected";
                case Accessibility.Internal:
                    return "internal";
                case Accessibility.Public:
                    return "public";
                case Accessibility.ProtectedOrInternal:
                case Accessibility.NotApplicable:
                default:
                    throw new ArgumentOutOfRangeException(a.ToString());
            }

        }

        public static T Root<T>(this SyntaxNode node)
            where T : SyntaxNode
        {
            var processed = node;
            while (processed != null)
            {
                if (processed is T t)
                {
                    return t;
                }

                processed = processed.Parent;
            }

            return null;
        }

        public static IEnumerable<INamedTypeSymbol> GetAllTypes(
            this INamespaceSymbol @namespace,
            Func<INamedTypeSymbol, bool> predicate
            )
        {
            if (@namespace == null)
            {
                throw new ArgumentNullException(nameof(@namespace));
            }
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            foreach (var type in @namespace.GetTypeMembers())
            {
                foreach (var nestedType in type.GetNestedTypes(predicate))
                {
                    yield return nestedType;
                }
            }

            foreach (var nestedNamespace in @namespace.GetNamespaceMembers())
            {
                foreach (var type in nestedNamespace.GetAllTypes(predicate))
                {
                    yield return type;
                }
            }
        }

        private static IEnumerable<INamedTypeSymbol> GetNestedTypes(
            this INamedTypeSymbol type,
            Func<INamedTypeSymbol, bool> predicate
            )
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            if (predicate(type))
            {
                yield return type;
            }

            foreach (var nestedType in type.GetTypeMembers().SelectMany(nestedType => nestedType.GetNestedTypes(predicate)))
            {
                yield return nestedType;
            }
        }

        public static bool CanBeCastedTo(
            this ITypeSymbol source,
            ITypeSymbol target
            )
        {
            if (SymbolEqualityComparer.Default.Equals(source, target))
            {
                return true;
            }
            if (source is INamedTypeSymbol ntSource)
            {
                if (SymbolEqualityComparer.Default.Equals(ntSource.OriginalDefinition, target))
                {
                    return true;
                }
            }

            foreach (INamedTypeSymbol @interface in source.AllInterfaces)
            {
                if (SymbolEqualityComparer.Default.Equals(@interface, target))
                {
                    return true;
                }
            }

            if (source.BaseType != null && !SymbolEqualityComparer.Default.Equals(source.BaseType, source))
            {
                if (CanBeCastedTo(source.BaseType, target))
                {
                    return true;
                }
            }

            foreach (INamedTypeSymbol @interface in source.AllInterfaces)
            {
                var r = CanBeCastedTo(
                    @interface,
                    target
                    );

                if (r)
                {
                    return true;
                }
            }

            return false;
        }

    }
}
