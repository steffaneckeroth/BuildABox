using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using FluentAssertions.Execution;

namespace BuildABoxTest.Integration.Box;

public class Create_Boxes
{
    /**
     * TO DO:
     *
     * 1. Instantier, sæt ind, tjek om den er i db (Test cases med korrekt og forkert input)
     * a) Uden titel
     * b) For lang titel
     * c) bogstaver i talfelt
     * d) korrekt data
     *
     * 2. Lav en duplicate (samme titel bør blocke creation?)
     * 
     */
    
    private HttpClient _httpClient;
    
    [SetUp]
    public void Setup()
    {
        _httpClient = new HttpClient();
        Helper.TriggerRebuild();
    }

    [TestCase("Mock Title", 40, 13, 15, 20, TestName = "ValidBox")]
    [TestCase("", 40, 13, 15, 20, TestName = "EmptyTitle")]
    [TestCase("Mock Title", "hello", 13, 15, 20, TestName = "TextPrice")]
    [TestCase("Mock Title", 40, false, 15, 20, TestName = "BooleanLength")]
    [TestCase("Mock Title", 40, 13,  new[] {1, 2, 3}, 20, TestName = "IntArrayWidth")]
    [TestCase("Mock Title", 40, 13, 15, null, TestName = "NullHeight")]
    public async Task CreateBox(
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

        string url = "http://localhost:5000/api/createBox";//todo first part should be a global variable, so we can set it to the domain in future
        HttpResponseMessage response;
        Infrastructure.Model.Box? responseBox;
        try
        {
            response = await _httpClient.PostAsJsonAsync(url, box);
            TestContext.WriteLine("The full body response: "
                                  + await response.Content.ReadAsStringAsync());
        }
        catch (Exception e)
        {
            throw new Exception(Helper.NoResponseMessage, e);
        }
            
        using (new AssertionScope())
        {
            string testName = TestContext.CurrentContext.Test.Name;

            switch (testName)
            {
                case "ValidBox":
                    responseBox = response.Content.ReadFromJsonAsync<Infrastructure.Model.Box>().Result;
                    response.IsSuccessStatusCode.Should().BeTrue();
                    responseBox.Should().NotBeNull();
                    responseBox?.Title.Equals(box.Title).Should().BeTrue();
                    break;
                case "EmptyTitle":
                    response.IsSuccessStatusCode.Should().BeFalse();
                    break;
                case "MinusPrice":
                    response.IsSuccessStatusCode.Should().BeFalse();
                    break;
                case "BooleanLength":
                    response.IsSuccessStatusCode.Should().BeFalse();
                    break;
                case "IntArrayWidth":
                    response.IsSuccessStatusCode.Should().BeFalse();
                    break;
                case "NullHeight":
                    response.IsSuccessStatusCode.Should().BeFalse();
                    break;
                default:
                    response.IsSuccessStatusCode.Should().BeFalse();
                    break;
            }
        }
    }
}