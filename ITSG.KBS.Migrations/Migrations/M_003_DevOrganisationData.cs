
namespace ITSG.KBS.Migrations;

[Migration(3, "DevOrganisationData")]
[Tags("dev")]
public class M_003_DevOrganisationData : Migration
{
    public override void Down()
    {
        Delete.FromTable("User").Row(new { Username = "BeispielUser1" });
        Delete.FromTable("Organisation").Row(new { Name = "BeispielOrg1" });
    }

    public override void Up()
    {
        SetupBeispielOrg1();
        SetupBeispielUser1();
    }

    private void SetupBeispielOrg1()
    {
        Insert.IntoTable("Organisation").Row(new { Name = "BeispielOrg1" });

        var values = new List<(string Property, string Value)>{
            ("City", "Städtle"),
            ("Country", "Deutschland"),
            ("Street", "Bahnhofstrasse 12"),
            ("Zip", "12345"),
            ("Phone", "+49123123123"),
            ("Fax", "+49123123123"),
        };

        foreach (var (Property, Value) in values)
        {
            this.Execute.Sql($@"
WITH org AS (SELECT Id as OrganisationId FROM Organisation WHERE Name = 'BeispielOrg1')
INSERT INTO OrganisationProperty (OrganisationId, Property, Value, LastChangedOn)
SELECT OrganisationId, '{Property}', '{Value}', CURRENT_TIMESTAMP
FROM org
        ");
        }
    }


    private void SetupBeispielUser1()
    {
        this.Execute.Sql($@"
WITH org AS (SELECT Id as OrganisationId FROM Organisation WHERE Name = 'BeispielOrg1')
INSERT INTO [User] (OrganisationId, Username)
SELECT OrganisationId, 'BeispielUser1'
FROM org
        ");

        var values = new List<(string Property, string Value)>{
            ("FirstName", "Beispiel"),
            ("LastName", "User1"),
            ("Title", "Doktor"),
            ("Salutation", "Herr"),
            ("Phone", "+49123123123"),
            ("Fax", "+49123123123"),
            ("Email", "beispiel.user1@example.com"),
        };

        foreach (var (Property, Value) in values)
        {
            this.Execute.Sql($@"
WITH u AS (SELECT Id as UserId FROM [User] WHERE Username = 'BeispielUser1')
INSERT INTO UserProperty (UserId, Property, Value, LastChangedOn)
SELECT UserId, '{Property}', '{Value}', CURRENT_TIMESTAMP
FROM u
        ");
        }

    }
}