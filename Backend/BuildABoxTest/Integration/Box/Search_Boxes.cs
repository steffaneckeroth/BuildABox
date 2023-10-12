using System.Net.Http.Json;
using Dapper;
using FluentAssertions;
using FluentAssertions.Execution;

namespace BuildABoxTest.Integration.Box;

using Infrastructure.Model;

public class Search_Boxes
{
    /**
     * TO DO:
     *
     * 1. Filtreres boxes væk ud fra søgetekst titel
     * 2. Sendes rigtige mængde resultater tilbage
     * 3. Test at man kan søge på:
     * a) ID
     * b) Titel
     * c) Beskrivelse
     * d) osv. osv. osv...
     * 4. Søg på tomt input og forvent rigtige fejlbeskeder
     *
     */
    
    private HttpClient _httpClient;
    
    [SetUp]
    public void Setup()
    {
        _httpClient = new HttpClient();
        Helper.TriggerRebuild();
    }

    [TestCase("3")]
    public async Task SearchByIdTesting(string searchQuery)
    {
        string[] titles = {
            "Lorem ",
            "ipsum ",
            "dolor ",
            "sit ",
            "amet",
            "consectetur ",
            "adipiscing ",
            "elit",
            "Morbi ",
            "ullamcorper"
        };
        string[] descriptions = {
            "sit amet cursus enim",
            "in vulputate dui nisl ac",
            "ut est blandit, porta",
            ". Aliquam erat volutpat.",
            "is in. Pellentesque bibendum ",
            ", semper dapibus ipsum ultrices vitae. Vestibulum leo felis",
            "s auctor erat, ut accumsan tellus turpis ",
            "tis in. Pellentesque bibendum bibendu",
            "urus laoreet. Quisque ac aliquam dui"
        };
        
        int numBoxes = 10;
        Random random = new Random();

        IEnumerable<Box> boxes;
        
        string insertSql = @"
            insert into buildabox.box (title, description, price, imageurl, width, length, height) 
            values (@title, @description, @price, @imageUrl, @width, @length, @height) 
            returning *;
            ";
        
        for (int i = 0; i < numBoxes; i++)
        {
            Box box = new Box
            {
                Title = titles[random.Next(titles.Length)],
                Description = descriptions[random.Next(descriptions.Length)],
                Price = 20,
                ImageURL = "...",
                Length = 20,
                Width = 20,
                Height = 20
            };
            
            using (var conn = Helper.DataSource.OpenConnection())
            {
                conn.Execute(insertSql, box);
            }
        }
        
        using (var conn = Helper.DataSource.OpenConnection())
        {
            conn.Query<Box>(insertSql, new Box
            {
                Title = titles[0],
                Description = descriptions[0],
                Price = 0,
                ImageURL = "...",
                Length = 0,
                Width = 0,
                Height = 0
            });
        }

        string getSql =
            @"select * from buildabox.box;";
        using (var conn = Helper.DataSource.OpenConnection())
        {
            boxes = conn.Query<Box>(getSql);
        }

        var url = "http://localhost:5000/api/products/filter?searchQuery=" + searchQuery;
        HttpResponseMessage response;
        IEnumerable<Box>? boxesFromSearch;
        
        try
        {
            response = await _httpClient.GetAsync(url);
            TestContext.WriteLine("The full body response: " 
                                  + await response.Content.ReadAsStringAsync());

            boxesFromSearch = response.Content.ReadFromJsonAsync<Box[]>().Result;
        }
        catch (Exception e)
        {
            throw new Exception(Helper.NoResponseMessage, e);
        }
        
        using (new AssertionScope())
        {
            boxesFromSearch.Should().NotBeNull();
            response.IsSuccessStatusCode.Should().BeTrue();

            Box boxFromSearch = boxesFromSearch.FirstOrDefault(box => box.ProductID == int.Parse(searchQuery));
            Box expectedBox = boxes.FirstOrDefault(box => box.ProductID == boxFromSearch.ProductID);

            boxesFromSearch.Count().Should().Be(1);
            boxFromSearch.Should().BeEquivalentTo(expectedBox);
        }
    }
    
    [TestCase("consectetur ")]
    public async Task SearchByTitle(string searchQuery)
    {
        string[] titles = {
            "Lorem ",
            "ipsum ",
            "dolor ",
            "sit ",
            "amet",
            "consectetur ",
            "adipiscing ",
            "elit",
            "Morbi ",
            "ullamcorper"
        };
        string[] descriptions = {
            "sit amet cursus enim",
            "in vulputate dui nisl ac",
            "ut est blandit, porta",
            ". Aliquam erat volutpat.",
            "is in. Pellentesque bibendum ",
            ", semper dapibus ipsum ultrices vitae. Vestibulum leo felis",
            "s auctor erat, ut accumsan tellus turpis ",
            "tis in. Pellentesque bibendum bibendu",
            "urus laoreet. Quisque ac aliquam dui"
        };
        
        Random random = new Random();

        IEnumerable<Box> boxes;
        
        string insertSql = @"
            insert into buildabox.box (title, description, price, imageurl, width, length, height) 
            values (@title, @description, @price, @imageUrl, @width, @length, @height) 
            returning *;
            ";
        
        for (int i = 0; i < titles.Length; i++)
        {
            Box box = new Box
            {
                Title = titles[random.Next(titles.Length)],
                Description = descriptions[random.Next(descriptions.Length)],
                Price = 20,
                ImageURL = "...",
                Length = 20,
                Width = 20,
                Height = 20
            };
            
            using (var conn = Helper.DataSource.OpenConnection())
            {
                conn.Execute(insertSql, box);
            }
        }
        
        using (var conn = Helper.DataSource.OpenConnection())
        {
            conn.Query<Box>(insertSql, new Box
            {
                Title = titles[0],
                Description = descriptions[0],
                Price = 0,
                ImageURL = "...",
                Length = 0,
                Width = 0,
                Height = 0
            });
        }

        string getSql =
            @"select * from buildabox.box;";
        using (var conn = Helper.DataSource.OpenConnection())
        {
            boxes = conn.Query<Box>(getSql);
        }

        var url = "http://localhost:5000/api/products/filter?searchQuery=" + searchQuery;
        HttpResponseMessage response;
        IEnumerable<Box>? boxesFromSearch;
        
        try
        {
            response = await _httpClient.GetAsync(url);
            TestContext.WriteLine("The full body response: " 
                                  + await response.Content.ReadAsStringAsync());

            boxesFromSearch = response.Content.ReadFromJsonAsync<Box[]>().Result;
        }
        catch (Exception e)
        {
            throw new Exception(Helper.NoResponseMessage, e);
        }
        
        using (new AssertionScope())
        {
            boxesFromSearch.Should().NotBeNull();
            response.IsSuccessStatusCode.Should().BeTrue();

            Box? expectedBox = boxes.FirstOrDefault(box => box.Title.Equals(searchQuery));

            expectedBox.Should().NotBeNull();
            
            Box boxFromSearch = boxesFromSearch.FirstOrDefault(box => box.ProductID == expectedBox.ProductID);

            boxFromSearch.Should().BeEquivalentTo(expectedBox);
        }
    }
    
    [TestCase("sit amet cursus enim")]
    public async Task SearchByDescription(string searchQuery)
    {
        string[] titles = {
            "Lorem ",
            "ipsum ",
            "dolor ",
            "sit ",
            "amet",
            "consectetur ",
            "adipiscing ",
            "elit",
            "Morbi ",
            "ullamcorper"
        };
        string[] descriptions = {
            "sit amet cursus enim",
            "in vulputate dui nisl ac",
            "ut est blandit, porta",
            ". Aliquam erat volutpat.",
            "is in. Pellentesque bibendum ",
            ", semper dapibus ipsum ultrices vitae. Vestibulum leo felis",
            "s auctor erat, ut accumsan tellus turpis ",
            "tis in. Pellentesque bibendum bibendu",
            "urus laoreet. Quisque ac aliquam dui"
        };
        
        Random random = new Random();

        IEnumerable<Box> boxes;
        
        string insertSql = @"
            insert into buildabox.box (title, description, price, imageurl, width, length, height) 
            values (@title, @description, @price, @imageUrl, @width, @length, @height) 
            returning *;
            ";
        
        for (int i = 0; i < titles.Length; i++)
        {
            Box box = new Box
            {
                Title = titles[random.Next(titles.Length)],
                Description = descriptions[random.Next(descriptions.Length)],
                Price = 20,
                ImageURL = "...",
                Length = 20,
                Width = 20,
                Height = 20
            };
            
            using (var conn = Helper.DataSource.OpenConnection())
            {
                conn.Execute(insertSql, box);
            }
        }
        
        using (var conn = Helper.DataSource.OpenConnection())
        {
            conn.Query<Box>(insertSql, new Box
            {
                Title = titles[0],
                Description = descriptions[0],
                Price = 0,
                ImageURL = "...",
                Length = 0,
                Width = 0,
                Height = 0
            });
        }

        string getSql =
            @"select * from buildabox.box;";
        using (var conn = Helper.DataSource.OpenConnection())
        {
            boxes = conn.Query<Box>(getSql);
        }

        var url = "http://localhost:5000/api/products/filter?searchQuery=" + searchQuery;
        HttpResponseMessage response;
        IEnumerable<Box>? boxesFromSearch;
        
        try
        {
            response = await _httpClient.GetAsync(url);
            TestContext.WriteLine("The full body response: " 
                                  + await response.Content.ReadAsStringAsync());

            boxesFromSearch = response.Content.ReadFromJsonAsync<Box[]>().Result;
        }
        catch (Exception e)
        {
            throw new Exception(Helper.NoResponseMessage, e);
        }
        
        using (new AssertionScope())
        {
            boxesFromSearch.Should().NotBeNull();
            response.IsSuccessStatusCode.Should().BeTrue();

            Box? expectedBox = boxes.FirstOrDefault(box => box.Description.Equals(searchQuery));

            expectedBox.Should().NotBeNull();
            
            Box boxFromSearch = boxesFromSearch.FirstOrDefault(box => box.ProductID == expectedBox.ProductID);

            boxFromSearch.Should().BeEquivalentTo(expectedBox);
        }
    }
}