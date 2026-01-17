namespace FinalProjectMisa.Core.Dto;

public class PagedResult<T>
{
    public int TotalRecords { get; set; }
    public IEnumerable<T> Data { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}