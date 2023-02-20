namespace ITSG.KBS.Migrations;

[Migration(2, "Users")]
public class M_002_Users : Migration
{
    public override void Down()
    {
        this.Delete.Table("UserProperty");
        this.Delete.Table("UserPropertyDefinition");
        this.Delete.Table("User");
    }

    public override void Up()
    {
        Create.Table("User").WithDescription("Definition von Benutzer")
            .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity().WithColumnDescription("Internes Id")
            .WithColumn("Guid").AsGuid().NotNullable().Unique().WithDefault(SystemMethods.NewGuid).WithColumnDescription("Externes Id")
            .WithColumn("Username").AsString(255, "Latin1_General_CI_AS").NotNullable().Unique().WithColumnDescription("Username mit dem User sich einloggt")
            .WithColumn("CreatedOn").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime).WithColumnDescription("Zeitstempel, wann der Eintrag erstellt ist")
            .WithColumn("OrganisationId").AsInt32().NotNullable().ForeignKey("FK_User_Organisation", "Organisation", "Id").OnDelete(System.Data.Rule.Cascade).WithColumnDescription("Zugehörigkeit zu Organisation");

        Create.Table("UserPropertyDefinition").WithDescription("Definition einer Eigenschaft vom Benutzer")
            .WithColumn("Name").AsString(100, "Latin1_General_CI_AS").NotNullable().PrimaryKey().WithColumnDescription("Name")
            .WithColumn("Type").AsString(20, "Latin1_General_CI_AS").NotNullable().ForeignKey("FK_UserPropertyDefinition_PropertyType", "PropertyType", "Type")
            .WithColumn("Description").AsString(int.MaxValue, "Latin1_General_CI_AS").NotNullable().WithColumnDescription("Beschreibung der Eigenschaft");

        Create.Table("UserProperty").WithDescription("Wert der Eigenschaft von User")
            .WithColumn("UserId").AsInt32().NotNullable().ForeignKey("FK_UserProperty_User", "User", "Id").OnDelete(System.Data.Rule.Cascade)
            .WithColumn("Property").AsString(100, "Latin1_General_CI_AS").NotNullable().ForeignKey("FK_UserProperty_UserPropertyDefinition", "UserPropertyDefinition", "Name").OnDelete(System.Data.Rule.None)
            .WithColumn("Value").AsString(int.MaxValue).NotNullable().WithColumnDescription("Serialisierter Wert")
            .WithColumn("LastChangedOn").AsDateTime2().NotNullable().WithColumnDescription("Zeitstempel, wann der Eintrag geändert ist");

        Create.PrimaryKey("PK_UserProperty").OnTable("UserProperty").Columns("UserId", "Property");

        Insert.IntoTable("UserPropertyDefinition")
           .Row(new { Name = "FirstName", Type = "text", Description = "Vorname" })
           .Row(new { Name = "LastName", Type = "text", Description = "Nachname" })
           .Row(new { Name = "Title", Type = "text", Description = "Akademischer Titel" })
           .Row(new { Name = "Salutation", Type = "text", Description = "Anrede" })
           .Row(new { Name = "Phone", Type = "text", Description = "Telefonnummer" })
           .Row(new { Name = "Fax", Type = "text", Description = "Faxnummer" })
           .Row(new { Name = "Email", Type = "text", Description = "E-mail" });

    }
}