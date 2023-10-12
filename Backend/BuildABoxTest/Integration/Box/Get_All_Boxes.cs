using System.Net.Http.Json;
using System.Text.Json;
using Dapper;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualBasic.CompilerServices;

namespace BuildABoxTest.Integration.Box;

public class Get_All_Boxes
{
    /**
     * TO DO:
     *
     * 1. Instantier, send det ind, få objektet igen
     * 2. Get all uden der er instantieret noget
     * 3. Er f.eks. description blevet kortet ned (eller andre limits fra get all metoden)
     *
     */

    private HttpClient _httpClient;
    
    [SetUp]
    public void Setup()
    {
        _httpClient = new HttpClient();
        Helper.TriggerRebuild();
    }

    [Test]
    public async Task GetProductsFeed()
    {
        var expected = new List<Object>();
        for (var i = 1; i < 10; i++)
        {
            var box = new Infrastructure.Model.Box()
            {
                ProductID = i,
                Title = "Test box no. " + i,
                Description = "Test description which is very very very very long " +
                              "and exceeds the maximum amount of characters that we want to show.",
                Price = 10,
                ImageURL = "https://www.packable.sg/wp-content/uploads/2020/07/kraft_box_open.png",
                Length = 25,
                Width = 15,
                Height = 10
            };
            expected.Add(box);
            
            var sql =
                $@"
            insert into buildabox.box (title, description, price, imageurl, width, length, height) 
            values (@title, @description, @price, @imageUrl, @width, @length, @height) 
            returning *;
            ";
            using (var conn = Helper.DataSource.OpenConnection())
            {
                conn.Execute(sql, box);
            }

            var url = "http://localhost:5000/api/products";//todo first part should be a global variable, so we can set it to the domain in future
            HttpResponseMessage response;
            try
            {
                response = await _httpClient.GetAsync(url);
                TestContext.WriteLine("The full body response: " 
                                      + await response.Content.ReadAsStringAsync());
            }
            catch (Exception e)
            {
                throw new Exception(Helper.NoResponseMessage, e);
            }

            using (new AssertionScope())
            {
                response.IsSuccessStatusCode.Should().BeTrue();

            }
        }
    }

    [Test]
    public async Task GetFeedWithoutProducts()
    {
        string url = "http://localhost:5000/api/products";//todo first part should be a global variable, so we can set it to the domain in future
        HttpResponseMessage response;
        Infrastructure.Model.Box[]? products;
        try
        {
            response = await _httpClient.GetAsync(url);
            TestContext.WriteLine("The full body response: " 
                                  + await response.Content.ReadAsStringAsync());
            
            products = response.Content.ReadFromJsonAsync<Infrastructure.Model.Box[]>().Result;
        }
        catch (Exception e)
        {
            throw new Exception(Helper.NoResponseMessage, e);
        }
        using (new AssertionScope())
        {
            products.Should().NotBeNull();
            products.Should().BeEmpty();
        }
    }
}
