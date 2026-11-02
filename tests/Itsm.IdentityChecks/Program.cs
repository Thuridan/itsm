using System.Security.Claims;
using Itsm.Web.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

static void Check(bool value,string message) { if(!value) throw new InvalidOperationException(message); }
static ClaimsPrincipal Principal(Guid tenant,Guid id,bool authenticated=true) => new(new ClaimsIdentity(
    new[]{new Claim("tid",tenant.ToString()),new Claim("oid",id.ToString()),new Claim("name","Synthetic user"),new Claim("roles","Administrator")},authenticated?"Test":null));
var tenant=Guid.NewGuid();var objectId=Guid.NewGuid();
Check(IdentityAccess.ReadIdentity(Principal(tenant,objectId),tenant.ToString()) is not null,"Valid identity rejected");
Check(IdentityAccess.ReadIdentity(Principal(Guid.NewGuid(),objectId),tenant.ToString()) is null,"Foreign tenant accepted");
Check(IdentityAccess.ReadIdentity(Principal(tenant,objectId,false),tenant.ToString()) is null,"Anonymous principal accepted");
Check(IdentityAccess.ReadIdentity(Principal(tenant,Guid.Empty),tenant.ToString()) is null,"Empty oid accepted");
new IdentitySettings().Validate(null);
bool rejected=false;try{new IdentitySettings{Enabled=true}.Validate(null);}catch(InvalidOperationException){rejected=true;}
Check(rejected,"Incomplete enabled configuration accepted");
var settings=new IdentitySettings{Enabled=true,TenantId=tenant.ToString(),ClientId=Guid.NewGuid().ToString(),ClientSecret="test-only",PublicOrigin="https://localhost",KeyPath="/tmp/identity-test-keys"};
settings.Validate("test-only");
var raw=Environment.GetEnvironmentVariable("TEST_SQL") ?? throw new InvalidOperationException("TEST_SQL required for disposable SQL integration checks.");
var master=new SqlConnectionStringBuilder(raw);
Check(master.DataSource is "127.0.0.1,1433" or "localhost,1433","Test refuses non-loopback SQL host");
master.InitialCatalog="master";
var dbName="ItsmIdentityTests_"+Guid.NewGuid().ToString("N");
await using var connection=new SqlConnection(master.ConnectionString);
for(var i=0;;i++) { try{await connection.OpenAsync();break;}catch(SqlException)when(i<40){await Task.Delay(1000);} }
await using(var command=connection.CreateCommand()){command.CommandText=$"CREATE DATABASE [{dbName}]";await command.ExecuteNonQueryAsync();}
var sql=new SqlConnectionStringBuilder(master.ConnectionString){InitialCatalog=dbName};
var options=new DbContextOptionsBuilder<IdentityDb>().UseSqlServer(sql.ConnectionString).Options;
try {
 await using(var setup=new SqlConnection(sql.ConnectionString)){
  await setup.OpenAsync();await using var cmd=setup.CreateCommand();cmd.CommandText=await File.ReadAllText("deploy/sql/001_identity.sql");await cmd.ExecuteNonQueryAsync();
 }
 await using(var db=new IdentityDb(options)){
  var service=new IdentityAccess(db,settings);
  var user=await service.RegisterAsync(Principal(tenant,objectId),default);
  Check(user.State=="Pending" && !user.IsAdministrator && user.AreaId is null && user.UnitId is null,"First login elevated privileges");
  Check(!await service.IsAdministratorAsync(Principal(tenant,objectId),default),"Token role elevated local privilege");
  Check(!await service.IsActiveAsync(user,default),"Pending user granted access");
  var again=await service.RegisterAsync(Principal(tenant,objectId),default);Check(again.Id==user.Id,"Repeated login duplicates user");
  Check(await db.Audit.CountAsync()==1,"Repeated login duplicates registration audit");
  var unit=new DirectoryUnit{Id=Guid.NewGuid(),Name="Test unit"};var area=new DirectoryArea{Id=Guid.NewGuid(),Name="Test area"};
  db.AddRange(unit,area);user.UnitId=unit.Id;user.AreaId=area.Id;user.State="Active";user.IsAdministrator=true;await db.SaveChangesAsync();
  Check(await service.IsAdministratorAsync(Principal(tenant,objectId),default),"Explicit admin denied");
  area.Active=false;await db.SaveChangesAsync();Check(!await service.IsAdministratorAsync(Principal(tenant,objectId),default),"Inactive area did not revoke access");
  area.Active=true;user.State="Disabled";await db.SaveChangesAsync();Check(!await service.IsAdministratorAsync(Principal(tenant,objectId),default),"Disabled admin retained access");
  Check(await service.FindAsync(Principal(Guid.NewGuid(),objectId),default) is null,"Cross-tenant profile returned");
 }
 var concurrentId=Guid.NewGuid();
 async Task<Guid> Register(){await using var db=new IdentityDb(options);return (await new IdentityAccess(db,settings).RegisterAsync(Principal(tenant,concurrentId),default)).Id;}
 var ids=await Task.WhenAll(Register(),Register());Check(ids[0]==ids[1],"Concurrent login created duplicates");
 await using(var db=new IdentityDb(options))Check(await db.Users.CountAsync(u=>u.ObjectId==concurrentId)==1,"Unique identity violated");
 Console.WriteLine("PASS: identity boundary, configuration, SQL baseline, pending registration, audit, concurrency, privileges and revocation");
} finally {
 SqlConnection.ClearAllPools();
 await using var cleanup=connection.CreateCommand();cleanup.CommandText=$"ALTER DATABASE [{dbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{dbName}]";await cleanup.ExecuteNonQueryAsync();
}
