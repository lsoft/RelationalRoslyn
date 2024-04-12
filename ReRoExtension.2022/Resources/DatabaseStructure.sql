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
CREATE TABLE named_types
(
    id_project_guid UNIQUEIDENTIFIER NOT NULL,
    containing_namespace NVARCHAR(2000) NOT NULL,
    global_name NVARCHAR(2000) NOT NULL,

    full_name NVARCHAR(2000) NOT NULL,
    name NVARCHAR(2000) NOT NULL,
    is_abstract bit NOT NULL,
    is_virtual bit NOT NULL,
    is_sealed bit NOT NULL,
    is_static bit NOT NULL,
    is_value_type bit NOT NULL,

    PRIMARY KEY (
        id_project_guid,
        containing_namespace,
        global_name
        )
);
GO
CREATE TABLE members
(
    type_global_name NVARCHAR(2000) NOT NULL,

    name NVARCHAR(2000) NOT NULL,
    is_abstract bit NOT NULL,
    is_virtual bit NOT NULL,
    is_override bit NOT NULL
);
GO
CREATE TABLE member_fields
(
    type_global_name NVARCHAR(2000) NOT NULL,

    name NVARCHAR(2000) NOT NULL,
    is_abstract bit NOT NULL,
    is_virtual bit NOT NULL,
    is_override bit NOT NULL,
    is_read_only bit NOT NULL,
    field_global_name NVARCHAR(2000) NOT NULL
);
GO
CREATE TABLE member_properties
(
    type_global_name NVARCHAR(2000) NOT NULL,

    name NVARCHAR(2000) NOT NULL,
    is_abstract bit NOT NULL,
    is_virtual bit NOT NULL,
    is_override bit NOT NULL,
    property_global_name NVARCHAR(2000) NOT NULL
);
GO
CREATE TABLE member_methods
(
    type_global_name NVARCHAR(2000) NOT NULL,

    name NVARCHAR(2000) NOT NULL,
    method_kind NVARCHAR(2000) NOT NULL,
    is_constructor bit NOT NULL,
    is_static_constructor bit NOT NULL,
    is_abstract bit NOT NULL,
    is_virtual bit NOT NULL,
    is_override bit NOT NULL,
    is_async bit NOT NULL,
    return_type_global_name NVARCHAR(2000) NOT NULL,
    parameters_count INTEGER NOT NULL
);
GO

CREATE TABLE symbol_source
(
    type_global_name NVARCHAR(2000) NOT NULL,

    source NVARCHAR(2000) NOT NULL,
    span_start INTEGER NOT NULL,
    span_length INTEGER NOT NULL
);
GO
