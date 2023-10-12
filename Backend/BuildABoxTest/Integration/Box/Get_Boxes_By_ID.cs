using Dapper;
using FluentAssertions;
using Newtonsoft.Json;

namespace BuildABoxTest.Integration.Box;


public class Get_Boxes_By_ID
{
    
    private HttpClient _httpClient;
    
    [SetUp]
    public void Setup()
    {
        _httpClient = new HttpClient();
    }

    [Test]
    public async Task GetBoxById()
    {
        //var retrievedBox = new Infrastructure.Model.Box();

        //Helper.TriggerRebuild();
        
        //insert a box with id 1

        string
            url = "http://localhost:5000/api/products/1"; //todo first part should be a global variable, so we can set it to the domain in future
        HttpResponseMessage response;
        
        response = await _httpClient.GetAsync(url);
        TestContext.WriteLine("The full body response: "
                              + await response.Content.ReadAsStringAsync());
        Infrastructure.Model.Box box =
            JsonConvert.DeserializeObject<Infrastructure.Model.Box>(await response.Content.ReadAsStringAsync());

        var sql = "SELECT * FROM buildabox.box WHERE productid = 1;";

        await using (var conn = Helper.DataSource.OpenConnection())
        {
            conn.QueryFirst<Infrastructure.Model.Box>(sql).Should().BeEquivalentTo(box);
        }
    }
}