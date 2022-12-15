using TBG.Synapse.Repository;
using TBG.Synapse.Services;
using TBG.Synapse.Test;

var connectionString = "Data Source=localhost\\SQLEXPRESS01;Initial Catalog=Synapse;Integrated Security=True;";

#region Test Repo
//RepoTest.TestCreate();
//RepoTest.TestInsert();
//RepoTest.TestUpdate();
//RepoTest.TestDelete();
#endregion

#region Test LogError
try
{
    throwError();
}
catch (Exception ex)
{
    Helper.LogError(ex);
}

void throwError()
{
    throwErrorError();
}

void throwErrorError()
{
    throw new Exception("AAA");
}
#endregion