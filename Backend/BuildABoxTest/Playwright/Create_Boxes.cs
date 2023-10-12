using System.Text.RegularExpressions;
using BuildABoxTest;
using Dapper;
using FluentAssertions;
using Infrastructure.Model;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class Tests : PageTest
{
    [TestCase("Playwright test kasse", "Denne kasse bruges til at teste playwright", 99, 
        "https://www.celladorales.com/wp-content/uploads/2016/12/ShippingBox_sq.jpg", 40, 50, 70)]
    [TestCase("Kasse nummer 2", "Denne kasse bruges sørme også til at teste playwright lolololol", 150, 
        "https://www.celladorales.com/wp-content/uploads/2016/12/ShippingBox_sq.jpg", 25, 25, 25)]
    public async Task CreateBox(string title, string description, decimal price, string imageUrl, 
        double width, double length, double height)
    {
        //ARRANGE
        Helper.TriggerRebuild();

        //ACT
        await Page.GotoAsync("http://localhost:5000/home");

        await Page.GetByTestId("menu-button").ClickAsync();

        await Page.GetByTestId("create-menu-button").ClickAsync();

        await Page.GetByLabel("title").ClickAsync();

        await Page.GetByLabel("title").FillAsync(title);

        await Page.GetByLabel("title").PressAsync("Tab");
        
        await Page.GetByLabel("description").FillAsync(description);

        await Page.GetByLabel("description").PressAsync("Tab");

        await Page.GetByLabel("Price").FillAsync(price.ToString());

        await Page.GetByLabel("Price").PressAsync("Tab");

        await Page.GetByLabel("imageUrl").ClickAsync();

        await Page.GetByLabel("imageUrl").FillAsync(imageUrl);

        await Page.GetByLabel("width").ClickAsync();

        await Page.GetByLabel("width").FillAsync(width.ToString());

        await Page.GetByLabel("width").PressAsync("Tab");

        await Page.GetByLabel("length").FillAsync(length.ToString());

        await Page.GetByLabel("length").PressAsync("Tab");

        await Page.GetByLabel("height").FillAsync(height.ToString());

        await Page.GetByTestId("create-button").ClickAsync();
        
        //ASSERT
        await Expect(Page.GetByTestId(title)).ToBeVisibleAsync();
        await using (var conn = await Helper.DataSource.OpenConnectionAsync())
        {
            var expected = new Box()
            {
                ProductID = 1, 
                Title = title, 
                Description = description, 
                Price = price, 
                ImageURL = imageUrl, 
                Length = length, 
                Width = width, 
                Height = height
            }; //Box object from test case

            conn.QueryFirst<Box>("SELECT * FROM buildabox.box;").Should()
                .BeEquivalentTo(expected); //Should be equal to box found in DB
        }
    }
}