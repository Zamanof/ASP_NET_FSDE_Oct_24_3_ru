using System.ComponentModel.DataAnnotations;

namespace ASP_NET_21._TaskFlow.BLL.DTOs;

/// <summary>
/// Represents query parameters for paginating, filtering, searching, and sorting task items.
/// </summary>
public class TaskItemQueryParams
{
    /// <summary>
    /// The page number to retrieve.
    /// </summary>
    /// <example>1</example>
    //[Required]
    //[Range(5, 15)]
    public int Page { get; set; } = 1;

    /// <summary>
    /// The number of items per page.
    /// </summary>
    /// <example>10</example>
    public int Size { get; set; } = 10;

    /// <summary>
    /// The field name to sort by.
    /// </summary>
    /// <example>title</example>
    public string? Sort { get; set; }

    /// <summary>
    /// The direction of sorting: "asc" for ascending or "desc" for descending.
    /// </summary>
    /// <example>asc</example>
    public string? SortDirection { get; set; } = "asc";

    /// <summary>
    /// Filter by task status.
    /// </summary>
    //[Required]
    //[EmailAddress]
    //[MinLength(5)]
    //[StatusValidation]
    public string? Status { get; set; }

    /// <summary>
    /// Filter by task priority.
    /// </summary>

    public string? Priority { get; set; }

    /// <summary>
    /// Search term to filter task items by title or description.
    /// </summary>
    
    public string? Search { get; set; }

    /// <summary>
    /// Filter by the project identifier.
    /// </summary>
   
    public int? ProjectId { get; set; }

    public void Validate()
    {
        if (Page < 1) Page = 1;
        if (Size < 1) Size = 10;
        if (Size > 100) Size = 100;
        if (string.IsNullOrWhiteSpace(SortDirection)) SortDirection = "asc";
        SortDirection = SortDirection.ToLower();
        if (SortDirection != "asc" && SortDirection != "desc") SortDirection = "asc";
    }
}


//public class StatusValidationAttribute: ValidationAttribute
//{
//    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
//    {
//        if (value is not string status)
//        {
//            return new ValidationResult("Status must be a string");
//        }
//        if(status != "ToDo" && status != "InProgress" && status != "Done")
//        {
//            return new ValidationResult("Status must be ToDo, InProgress or Done");
//        }
//        return ValidationResult.Success;
//    }
//}