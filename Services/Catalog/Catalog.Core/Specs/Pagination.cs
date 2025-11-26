namespace Catalog.Core.Specs;

public class Pagination<T> where T : class
{
    public Pagination()
    {
    }

    public Pagination(int pageIndex, int pageSize, int count, IReadOnlyList<T> data)
    {
        PageIndex = pageIndex;
        PageSize = pageSize;
        Count = count;
        Data = data;
    }

    public IReadOnlyList<T> Data { get; set; }

    public int Count { get; set; }

    public int PageSize { get; set; }

    public int PageIndex { get; set; }
}