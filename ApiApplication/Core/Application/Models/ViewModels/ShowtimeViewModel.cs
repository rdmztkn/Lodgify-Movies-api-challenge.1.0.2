using System;

namespace ApiApplication.Core.Application.Models.ViewModels
{
    public class ShowtimeViewModel
    {
        public int Id { get; set; }
        public MovieWrapperViewModel Movie { get; set; }
        public DateTime SessionDate { get; set; }
        public int AuditoriumId { get; set; }
    }
}