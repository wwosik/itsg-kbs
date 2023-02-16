
namespace ITSG.KBS.Migrations;

[Migration(4, "Config")]
public class M_004_Config : Migration
{
    public override void Down()
    {
        Delete.Table("ConfigProperty");
    }

    public override void Up()
    {
        Create.Table("ConfigProperty").WithDescription("Definition einer Eigenschaft vom Benutzer")
            .WithColumn("Name").AsString(100, "Latin1_General_CI_AS").NotNullable().PrimaryKey().WithColumnDescription("Name")
            .WithColumn("IsForFrontend").AsBinary().NotNullable().WithColumnDescription("Wenn 1, wird jedem Frontend-User ausgegeben")
            .WithColumn("Type").AsString(20, "Latin1_General_CI_AS").NotNullable().ForeignKey("FK_Config_PropertyType", "PropertyType", "Type")
            .WithColumn("Description").AsString(int.MaxValue, "Latin1_General_CI_AS").NotNullable().WithColumnDescription("Beschreibung der Eigenschaft")
            .WithColumn("Value").AsString(int.MaxValue, "Latin1_General_CI_AS").NotNullable().WithColumnDescription("Serialisierter Wert");

        Insert.IntoTable("ConfigProperty")
            .Row(new
            {
                Name = "Frontend Log Level Console",
                Type = "text",
                IsForFrontend = true,
                Description = "Loglevel ab dem Nachrichten in der Browser Console ausgegeben werden (debug, info, warn, error)",
                Value = "debug"
            })
            .Row(new
            {
                Name = "Frontend Log Level Backend",
                Type = "text",
                IsForFrontend = true,
                Description = "Loglevel ab dem Nachrichten an Server weitergegeben werden (debug, info, warn, error)",
                Value = "info"
            });
    }
}