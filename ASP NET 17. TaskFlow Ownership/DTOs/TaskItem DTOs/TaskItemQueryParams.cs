using System.ComponentModel.DataAnnotations;

namespace ASP_NET_17._TaskFlow_Ownership.DTOs.TaskItem_DTOs;

public class TaskItemQueryParams
{
    //[Required]
    public int Page { get; set; } = 1;
    //[Range(1, 100)]
    public int PageSize { get; set; } = 10;
    public string? Sort { get; set; }
    public string? SortDirection { get; set; }
    //[MinLength(3)]
    //[EmailAddress]
    //[StatusValidation]
    public string? Status { get; set; }
    public string? Priority { get; set; }
    public string? Search { get; set; }
    public int? ProjectId { get; set; }

    public void Validate()
    {
        if (Page < 1) 
            Page = 1;

        if (PageSize < 1) 
            PageSize = 1;

        if (PageSize > 100) 
            PageSize = 100;

        if (string.IsNullOrWhiteSpace(SortDirection)) 
            SortDirection = "asc";

        SortDirection = SortDirection.ToLower();

        if (SortDirection != "asc" && SortDirection != "desc")
            SortDirection = "asc";
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
//            return new ValidationResult("Status must be a ToDo, InProgress or Done");
//        }
//        return ValidationResult.Success;
//    }
//}