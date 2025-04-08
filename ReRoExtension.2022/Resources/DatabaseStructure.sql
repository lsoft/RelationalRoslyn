CREATE TABLE help
(
    help_text NVARCHAR(2000) NOT NULL
);
GO
CREATE TABLE projects
(
    project_guid UNIQUEIDENTIFIER NOT NULL,
    project_name NVARCHAR(2000) NOT NULL,

    PRIMARY KEY (
        project_guid
        )
);
GO
CREATE TABLE project_references
(
    --a FK into project.project_guid for parent project
    project_guid UNIQUEIDENTIFIER NOT NULL,
    
    --a FK into project.project_guid for referenced project
    referenced_project_guid UNIQUEIDENTIFIER NOT NULL,

    PRIMARY KEY (
        project_guid,
        referenced_project_guid
        )
);
GO
CREATE TABLE named_types
(
    --unique abstract id
    id UNIQUEIDENTIFIER NOT NULL,

    --global type name like global::MyNamespace.MyClassName
    type_global_name NVARCHAR(2000) NOT NULL,
    --namespace of this type
    containing_namespace NVARCHAR(2000) NOT NULL,

    --FK to the projects (parent project)
    id_project_guid UNIQUEIDENTIFIER NOT NULL,

    --FK to symbol_source
    id_source UNIQUEIDENTIFIER NOT NULL,

    full_name NVARCHAR(2000) NOT NULL,
    name NVARCHAR(2000) NOT NULL,
    is_abstract bit NOT NULL,
    is_virtual bit NOT NULL,
    is_sealed bit NOT NULL,
    is_static bit NOT NULL,
    is_value_type bit NOT NULL,

    PRIMARY KEY (
        id
        )
);
GO
CREATE INDEX ix_named_types_type_global_name ON named_types
(
    type_global_name ASC
);
GO
CREATE INDEX ix_named_types_containing_namespace ON named_types
(
    containing_namespace ASC
);
GO
CREATE INDEX ix_named_types_id_project_guid ON named_types
(
    id_project_guid ASC
);
GO
CREATE INDEX ix_named_types_full_name ON named_types
(
    full_name ASC
);

GO
CREATE TABLE members
(
    --unique abstract id
    id UNIQUEIDENTIFIER NOT NULL,

    --FK to named_types (to the parent named type)
    id_named_type UNIQUEIDENTIFIER NOT NULL,
    --global type name for the parent named type (denormalized column)
    type_global_name NVARCHAR(2000) NOT NULL,

    --FK to symbol_source
    id_source UNIQUEIDENTIFIER NOT NULL,

    --member name
    name NVARCHAR(2000) NOT NULL,
    is_abstract bit NOT NULL,
    is_virtual bit NOT NULL,
    is_override bit NOT NULL,

    PRIMARY KEY (
        id
        )
);
GO
CREATE INDEX ix_members_id_named_type ON members
(
    id_named_type ASC
);
GO
CREATE INDEX ix_members_id_type_global_name ON members
(
    type_global_name ASC
);
GO
CREATE INDEX ix_members_name ON members
(
    name ASC
);
GO

CREATE TABLE member_fields
(
    --unique abstract id
    id UNIQUEIDENTIFIER NOT NULL,

    --FK to named_types (to the parent named type)
    id_named_type UNIQUEIDENTIFIER NOT NULL,
    --global type name for the parent named type (denormalized column)
    type_global_name NVARCHAR(2000) NOT NULL,

    --FK to symbol_source
    id_source UNIQUEIDENTIFIER NOT NULL,

    --field name
    name NVARCHAR(2000) NOT NULL,
    is_abstract bit NOT NULL,
    is_virtual bit NOT NULL,
    is_override bit NOT NULL,
    is_read_only bit NOT NULL,
    field_global_name NVARCHAR(2000) NOT NULL,

    PRIMARY KEY (
        id
        )
);
GO
CREATE INDEX ix_member_fields_id_named_type ON member_fields
(
    id_named_type ASC
);
GO
CREATE INDEX ix_member_fields_type_global_name ON member_fields
(
    type_global_name ASC
);
GO
CREATE INDEX ix_member_fields_name ON member_fields
(
    name ASC
);
GO

CREATE TABLE member_properties
(
    --unique abstract id
    id UNIQUEIDENTIFIER NOT NULL,

    --FK to named_types (to the parent named type)
    id_named_type UNIQUEIDENTIFIER NOT NULL,
    --global type name for the parent named type (denormalized column)
    type_global_name NVARCHAR(2000) NOT NULL,

    --FK to symbol_source
    id_source UNIQUEIDENTIFIER NOT NULL,

    --propeerty name
    name NVARCHAR(2000) NOT NULL,
    is_abstract bit NOT NULL,
    is_virtual bit NOT NULL,
    is_override bit NOT NULL,
    property_global_name NVARCHAR(2000) NOT NULL,

    PRIMARY KEY (
        id
        )
);
GO
CREATE INDEX ix_member_properties_id_named_type ON member_properties
(
    id_named_type ASC
);
GO
CREATE INDEX ix_member_properties_type_global_name ON member_properties
(
    type_global_name ASC
);
GO
CREATE INDEX ix_member_properties_name ON member_properties
(
    name ASC
);
GO

CREATE TABLE member_methods
(
    --unique abstract id
    id UNIQUEIDENTIFIER NOT NULL,

    --FK to named_types (to the parent named type)
    id_named_type UNIQUEIDENTIFIER NOT NULL,
    --global type name for the parent named type (denormalized column)
    type_global_name NVARCHAR(2000) NOT NULL,

    --FK to symbol_source
    id_source UNIQUEIDENTIFIER NOT NULL,

    --method name
    name NVARCHAR(2000) NOT NULL,
    method_kind NVARCHAR(2000) NOT NULL,
    is_constructor bit NOT NULL,
    is_static_constructor bit NOT NULL,
    is_abstract bit NOT NULL,
    is_virtual bit NOT NULL,
    is_override bit NOT NULL,
    is_async bit NOT NULL,
    return_type_global_name NVARCHAR(2000) NOT NULL,
    parameters_count INTEGER NOT NULL,

    PRIMARY KEY (
        id
        )
);
GO
CREATE INDEX ix_member_methods_id_named_type ON member_methods
(
    id_named_type ASC
);
GO
CREATE INDEX ix_member_methods_type_global_name ON member_methods
(
    type_global_name ASC
);
GO
CREATE INDEX ix_member_methods_name ON member_methods
(
    name ASC
);
GO

CREATE TABLE symbol_source
(
    --abstract autoincrement PK (not used as FK from other tables!)
    symbol_index INTEGER NOT NULL,

    --id column for FKs from other tables
    id UNIQUEIDENTIFIER NOT NULL,

    --source code of the symbol
    source NVARCHAR(2000) NOT NULL,
    --full path to the source file
    file_path NVARCHAR(2000) NOT NULL,
    --start span index inside the source file
    span_start INTEGER NOT NULL,
    --span length
    span_length INTEGER NOT NULL,

    PRIMARY KEY (
        symbol_index AUTOINCREMENT
        )
);
GO
CREATE INDEX ix_symbol_source_source ON symbol_source
(
    source ASC
);
GO
CREATE INDEX ix_symbol_source_file_path ON symbol_source
(
    file_path ASC
);
GO
