using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace SignalR_Project.ViewModels
{
    public class NewGroupViewModel
    {
        [Required(ErrorMessage = "Group name is required")]
        [StringLength(50, ErrorMessage = "Group name cannot be longer than 50 characters.")]

        public string GroupName { get; set; }

    }
}
