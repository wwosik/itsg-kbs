
namespace ITSG.KBS.Migrations;

[Migration(5, "Texts")]
public class M_005_Texts : Migration
{
    public override void Down()
    {
        Delete.Table("AppText");
    }

    public override void Up()
    {
        Create.Table("AppText").WithDescription("Texte der Anwendung")
            .WithColumn("Key").AsString(1000, "Latin1_General_CI_AS").NotNullable().PrimaryKey().WithColumnDescription("Schlüssel")
            .WithColumn("IsMarkdown").AsFixedLengthString(1, "Latin1_General_CI_AS").ForeignKey("FK_AppText_IsMarkdown", "Ref_YesNo", "Value").NotNullable()
                .WithColumnDescription("Definiert ob der Wert Markdown ist").WithDefaultValue("N")
            .WithColumn("Description").AsString(int.MaxValue, "Latin1_General_CI_AS").NotNullable().WithColumnDescription("Hilfstext für Übersetzer")
            .WithColumn("Value").AsString(int.MaxValue, "Latin1_General_CI_AS").NotNullable().WithColumnDescription("Text");
    }
}