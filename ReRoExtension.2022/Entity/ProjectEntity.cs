﻿using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReRoExtension.Entity
{
    public sealed class EntityModel
    {
        public List<ProjectEntity> Projects { get; set; }

        public EntityModel()
        {
            Projects = new List<ProjectEntity>();
        }
    }


    [Table("projects")]
    public sealed class ProjectEntity
    {
        [PrimaryKey, Column("project_guid")]
        public Guid ProjectGuid
        {
            get; set;
        }

        [Column("project_name")]
        public string ProjectName
        {
            get; set;
        }

        [NotColumn]
        public List<NamedTypeEntity> NamedTypes
        {
            get; set;
        }

        public ProjectEntity()
        {
            NamedTypes = new List<NamedTypeEntity>();
        }
    }

    [Table("named_types")]
    public sealed class NamedTypeEntity : SourceBasedEntity
    {
        [NotColumn]
        public override string TypeGlobalName
        {
            get => GlobalName;
            set => throw new InvalidOperationException();
        }

        [Column("containing_namespace")]
        public string ContainingNamespace
        {
            get; set;
        }

        [Column("global_name")]
        public string GlobalName
        {
            get; set;
        }

        [Column("full_name")]
        public string FullName
        {
            get; set;
        }

        [Column("name")]
        public string Name
        {
            get; set;
        }

        [Column("is_abstract")]
        public bool IsAbstract
        {
            get; set;
        }

        [Column("is_virtual")]
        public bool IsVirtual
        {
            get; set;
        }

        [Column("is_sealed")]
        public bool IsSealed
        {
            get; set;
        }

        [Column("is_static")]
        public bool IsStatic
        {
            get; set;
        }

        [Column("is_value_type")]
        public bool IsValueType
        {
            get; set;
        }

        [NotColumn]
        public List<MemberEntity> Members
        {
            get; set;
        }

        [NotColumn]
        public List<FieldMemberEntity> FieldMembers
        {
            get; set;
        }

        [NotColumn]
        public List<PropertyMemberEntity> PropertyMembers
        {
            get; set;
        }

        [NotColumn]
        public List<MethodMemberEntity> MethodMembers
        {
            get; set;
        }

        public NamedTypeEntity()
        {
            Members = new List<MemberEntity>();
            FieldMembers = new List<FieldMemberEntity>();
            PropertyMembers = new List<PropertyMemberEntity>();
            MethodMembers = new List<MethodMemberEntity>();
        }
    }

    [Table("members")]
    public sealed class MemberEntity : SourceBasedEntity
    {
        [Column("type_global_name")]
        public override string TypeGlobalName
        {
            get; set;
        }

        [Column("name")]
        public string Name
        {
            get; set;
        }

        [Column("is_abstract")]
        public bool IsAbstract
        {
            get; set;
        }

        [Column("is_virtual")]
        public bool IsVirtual
        {
            get; set;
        }

        [Column("is_override")]
        public bool IsOverride
        {
            get; set;
        }
    }

    [Table("member_fields")]
    public sealed class FieldMemberEntity : SourceBasedEntity
    {
        [Column("type_global_name")]
        public override string TypeGlobalName
        {
            get; set;
        }

        [Column("name")]
        public string Name
        {
            get; set;
        }

        [Column("is_abstract")]
        public bool IsAbstract
        {
            get; set;
        }

        [Column("is_virtual")]
        public bool IsVirtual
        {
            get; set;
        }

        [Column("is_override")]
        public bool IsOverride
        {
            get; set;
        }

        [Column("is_read_only")]
        public bool IsReadOnly
        {
            get; set;
        }

        [Column("field_global_name")]
        public string FieldGlobalName
        {
            get; set;
        }
    }

    [Table("member_properties")]
    public sealed class PropertyMemberEntity : SourceBasedEntity
    {
        [Column("type_global_name")]
        public override string TypeGlobalName
        {
            get; set;
        }

        [Column("name")]
        public string Name
        {
            get; set;
        }

        [Column("is_abstract")]
        public bool IsAbstract
        {
            get; set;
        }

        [Column("is_virtual")]
        public bool IsVirtual
        {
            get; set;
        }

        [Column("is_override")]
        public bool IsOverride
        {
            get; set;
        }

        [Column("property_global_name")]
        public string PropertyGlobalName
        {
            get; set;
        }


    }

    [Table("member_methods")]
    public sealed class MethodMemberEntity : SourceBasedEntity
    {
        [Column("type_global_name")]
        public override string TypeGlobalName
        {
            get; set;
        }

        [Column("name")]
        public string Name
        {
            get; set;
        }

        [Column("method_kind")]
        public string MethodKind
        {
            get; set;
        }

        [Column("is_constructor")]
        public bool IsConstructor
        {
            get; set;
        }

        [Column("is_static_constructor")]
        public bool IsStaticConstructor
        {
            get; set;
        }

        [Column("is_abstract")]
        public bool IsAbstract
        {
            get; set;
        }

        [Column("is_virtual")]
        public bool IsVirtual
        {
            get; set;
        }

        [Column("is_override")]
        public bool IsOverride
        {
            get; set;
        }

        [Column("is_async")]
        public bool IsAsync
        {
            get; set;
        }

        [Column("return_type_global_name")]
        public string ReturnTypeGlobalName
        {
            get; set;
        }

        [Column("parameters_count")]
        public int ParametersCount
        {
            get; set;
        }


    }

    [Table("symbol_source")]
    public sealed class SymbolSourceEntity
    {
        [Column("type_global_name")]
        public string TypeGlobalName
        {
            get; set;
        }

        [Column("source")]
        public string Source
        {
            get; set;
        }

        [Column("span_start")]
        public int SpanStart
        {
            get; set;
        }

        [Column("span_length")]
        public int SpanLength
        {
            get; set;
        }


    }

    public abstract class SourceBasedEntity
    {
        public abstract string TypeGlobalName
        {
            get; set;
        }

        [NotColumn]
        public SourceFullInfo SourceInfo
        {
            get; set;
        }

        protected SourceBasedEntity()
        {
            SourceInfo = new();
        }

        public void Apply(SourceFullInfo source)
        {
            foreach (var spi in source.SourcePieces)
            {
                SourceInfo.SourcePieces.Add(spi);
            }
        }

        public IEnumerable<SymbolSourceEntity> ProduceEntities()
        {
            foreach (var spi in SourceInfo.SourcePieces)
            {
                yield return new SymbolSourceEntity
                {
                    TypeGlobalName = TypeGlobalName,
                    Source = spi.Source,
                    SpanStart = spi.SpanStart,
                    SpanLength = spi.SpanLength
                };
            }
        }

    }

    public sealed class SourceFullInfo
    {
        public List<SourcePieceInfo> SourcePieces
        {
            get;
        }

        public SourceFullInfo()
        {
            SourcePieces = new();
        }

    }

    public sealed class SourcePieceInfo
    {
        public string Source
        {
            get;
        }

        public string FullFilePath
        {
            get;
        }

        public int SpanStart
        {
            get;
        }

        public int SpanLength
        {
            get;
        }

        public SourcePieceInfo(string source, string fullFilePath, int spanStart, int spanLength)
        {
            Source = source;
            FullFilePath = fullFilePath;
            SpanStart = spanStart;
            SpanLength = spanLength;
        }
    }

}
