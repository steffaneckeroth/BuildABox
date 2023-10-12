using Infrastructure;
using Infrastructure.Model;

namespace Service;

public class Service
{
    private readonly Repository _repository;

    public Service(Repository repository)
    {
        _repository = repository;
    }

    public Box CreateBox(Box box)
    {
        return _repository.CreateBox(box);
    }

    public IEnumerable<Box> GetAllProducts()
    {
        return _repository.GetAllProducts();
    }

    public IEnumerable<Box> SearchForProducts(string searchQuery)
    {
        return _repository.SearchForProducts(searchQuery);
    }

    public Box GetBoxById(int id)
    {
        return _repository.GetBoxById(id);
    }
    
    
    public bool DeleteBox(int id)
    {
        return _repository.DeleteBox(id);
    }
    
    public Box UpdateBox(Box box)
    {
        return _repository.UpdateBox(box);
    }
}
