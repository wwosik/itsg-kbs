namespace ITSG.KBS.Migrations;

[Migration(1, "Organisations")]
public class M_001_Organisations : Migration
{
    public override void Down()
    {
        this.Delete.Table("OrganisationProperty");
        this.Delete.Table("OrganisationPropertyDefinition");
        this.Delete.Table("Organisation");
        this.Delete.Table("PropertyType");
    }

    public override void Up()
    {
        Create.Table("PropertyType").WithDescription("Erlaubte Liste von Eigenschaftstypen")
            .WithColumn("Type").AsString(20, "Latin1_General_CI_AS").NotNullable().PrimaryKey();

        Insert.IntoTable("PropertyType")
            .Row(new { Type = "text" })
            .Row(new { Type = "date" })
            .Row(new { Type = "datetime" })
            .Row(new { Type = "integer" })
            .Row(new { Type = "decimal" });

        Create.Table("Organisation").WithDescription("Definition von Organisation")
            .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity().WithColumnDescription("Internes Id")
            .WithColumn("Guid").AsGuid().NotNullable().Unique().WithDefault(SystemMethods.NewGuid).WithColumnDescription("Externes Id")
            .WithColumn("Name").AsString(255, "Latin1_General_CI_AS").NotNullable().Unique().WithColumnDescription("Name der Organisation")
            .WithColumn("IK").AsString(9, "Latin1_General_CI_AS").Nullable().Unique().WithColumnDescription("IK der Organisation")
            .WithColumn("CreatedOn").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime).WithColumnDescription("Zeitstempel, wann der Eintrag erstellt ist");

        Create.Table("OrganisationPropertyDefinition").WithDescription("Definition einer Eigenschaft von Organisation")
            .WithColumn("Name").AsString(100, "Latin1_General_CI_AS").NotNullable().PrimaryKey().WithColumnDescription("Name")
            .WithColumn("Type").AsString(20, "Latin1_General_CI_AS").NotNullable().ForeignKey("FK_OrganisationPropertyDefinition_PropertyType", "PropertyType", "Type")
            .WithColumn("Description").AsString(int.MaxValue, "Latin1_General_CI_AS").NotNullable().WithColumnDescription("Beschreibung der Eigenschaft");

        Create.Table("OrganisationProperty").WithDescription("Wert der Eigenschaft von Organisation")
            .WithColumn("OrganisationId").AsInt32().NotNullable().ForeignKey("FK_OrganisationProperty_Organisation", "Organisation", "Id").OnDelete(System.Data.Rule.Cascade)
            .WithColumn("Property").AsString(100, "Latin1_General_CI_AS").NotNullable().ForeignKey("FK_OrganisationProperty_OrganisationPropertyDefinition", "OrganisationPropertyDefinition", "Name").OnDelete(System.Data.Rule.None)
            .WithColumn("Value").AsString(int.MaxValue).NotNullable().WithColumnDescription("Serialisierter Wert")
            .WithColumn("LastChangedOn").AsDateTime2().NotNullable().WithColumnDescription("Zeitstempel, wann der Eintrag geändert ist");

        Create.PrimaryKey("PK_OrganisationProperty").OnTable("OrganisationProperty").Columns("OrganisationId", "Property");

        Insert.IntoTable("OrganisationPropertyDefinition")
            .Row(new { Name = "Street", Type = "text", Description = "Strassenadresse" })
            .Row(new { Name = "Zip", Type = "text", Description = "Postleitzahl" })
            .Row(new { Name = "City", Type = "text", Description = "Stadt" })
            .Row(new { Name = "Country", Type = "text", Description = "Staat" })
            .Row(new { Name = "Phone", Type = "text", Description = "Telefonnummer" })
            .Row(new { Name = "Fax", Type = "text", Description = "Faxnummer" });

    }
}