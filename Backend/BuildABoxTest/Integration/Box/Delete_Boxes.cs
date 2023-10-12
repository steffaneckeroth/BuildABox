using System.Net.Http.Json;
using Dapper;
using FluentAssertions;
using FluentAssertions.Execution;

namespace BuildABoxTest.Integration.Box;

public class Delete_Boxes
{
    /**
     * TO DO:
     *
     * 1. Instantier, sæt ind, tjek det er der, slet og tjek det er væk ?
     * 2. Prøv at slette noget der ike er der. 
     * 
     */

    private HttpClient? _httpClient;

    [SetUp]
    public void Setup()
    {
        _httpClient = new HttpClient();
        Helper.TriggerRebuild();
    }


    [TestCase("Mock Title", 40, 13, 15, 20, TestName = "ValidBox")]
    [TestCase("Mock Title", 40, 13, 15, 20, TestName = "NotValidBox")]
    public async Task DeleteBox(
        string title,
        object price,
        object length,
        object width,
        object height)
    {
        var box = new
        {
            Title = title,
            Description = "Description",
            Price = price,
            ImageURL = "https://www.celladorales.com/wp-content/uploads/2016/12/ShippingBox_sq.jpg",
            Length = length,
            Width = width,
            Height = height
        };

        var
            url = "http://localhost:5000/api/products/"; //todo first part should be a global variable, so we can set it to the domain in future
        HttpResponseMessage response;
        Results? result;
        int id;
        
        var sql =
            $@"
            insert into buildabox.box (title, description, price, imageurl, width, length, height) 
            values (@title, @description, @price, @imageUrl, @length, @width, @height) 
            returning *;
            ";
        using (var conn = Helper.DataSource.OpenConnection())
        {
            id = conn.Execute(sql, box);
        }

        using (new AssertionScope())
        {
            string testName = TestContext.CurrentContext.Test.Name;

            switch (testName)
            {
                case "ValidBox":
                    response = await _httpClient.DeleteAsync(url + id);
                    result = response.Content.ReadFromJsonAsync<Results>().Result;
                    response.IsSuccessStatusCode.Should().BeTrue();
                    result.Should().NotBeNull();
                    result?.Result.Should().BeTrue();
                    break;
                case "NotValidBox":
                    response = await _httpClient.DeleteAsync(url + "9999");//not a real id
                    result = response.Content.ReadFromJsonAsync<Results>().Result;
                    response.IsSuccessStatusCode.Should().BeTrue();
                    response.Should().NotBeNull();
                    result?.Result.Should().BeFalse();
                    break;
            }
        }
    }
}

public class Results
{
    public bool Result { get; set; }
}