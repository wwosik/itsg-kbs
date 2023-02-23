using System.Data;
using Dapper;

namespace ITSG.KBS.App.Test;

[Collection("Tests with service provider")]
public class UnitTest1
{
    private readonly TestServiceProvider sp;

    public UnitTest1(TestServiceProvider sp)
    {
        this.sp = sp;
    }

    [Fact]
    public void Test1()
    {
        var db = sp.GetRequiredService<IDbConnection>();
        db.QueryFirst("SELECT Name FROM UserPropertyDefinition where Name = 'FirstName'");
    }

    [Fact]
    public void Test2()
    {
        var db = sp.GetRequiredService<IDbConnection>();
        db.QueryFirst("SELECT Name FROM UserPropertyDefinition where Name = 'LastName'");
    }
}