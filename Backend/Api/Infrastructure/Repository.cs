using Dapper;
using Infrastructure.Model;
using Npgsql;

namespace Infrastructure;

public class Repository
{
    private readonly NpgsqlDataSource _dataSource;

    public Repository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public Box CreateBox(Box box)
    {
        var sql =
            $@"
            insert into buildabox.box (title, description, price, imageurl, width, length, height) 
            values (@title, @description, @price, @imageUrl, @width, @length, @height) 
            returning *;
            ";

        using (var conn = _dataSource.OpenConnection())
        {
            return conn.QueryFirst<Box>(sql, new { box.Title, box.Description, box.Price, box.ImageURL, box.Width, box.Length, box.Height});
        }
    }

    public IEnumerable<Box> GetAllProducts()
    {
        var sql = $@"select * from buildabox.box;";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.Query<Box>(sql);
        }
    }

    public IEnumerable<Box> SearchForProducts(string searchQuery)
    {
        try
        {
            IEnumerable<Box> products = GetAllProducts();
            IEnumerable<Box> filteredProducts = products.Where(box => box.Search(searchQuery));

            return filteredProducts;
        }
        catch (Exception e)
        {
            throw new Exception("Failed to search for products");
        }
    }

    public Box GetBoxById(int productid)
    {
        var sql = 
            $@"
            select * from buildabox.box
            where productid = @productid;
            ";

        using (var conn = _dataSource.OpenConnection())
        {
            return conn.QueryFirst<Box>(sql, new {productid});
        }
    }
    
    
    public bool DeleteBox(int productid)
    {
        var sql = @"DELETE FROM buildabox.box
                    WHERE productid = @productid";
        using (var conn = _dataSource.OpenConnection())
        {
            var rowsAffected = conn.Execute(sql, new {productid}) == 1;
            return rowsAffected;
        }
    }
    
    
    public Box UpdateBox(Box box)
    {
        var sql = @"UPDATE buildabox.box
                    SET title = @Title,
                        description = @Description,
                        price = @Price,
                        imageurl = @ImageURL,
                        length = @Length,
                        width = @Width,
                        height = @Height
                    WHERE productid = @ProductId
                    RETURNING *;";   
        
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.QueryFirst<Box>(sql, box);
        }
    }
}