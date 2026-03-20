namespace FreshNFluffy.ViewModels.Reviews
{
    public class ReviewListItemViewModel
    {
        public int Rating { get; set; }

        public string Comment { get; set; } = null!;

        public string UserName { get; set; } = null!;

        public DateTime CreatedOn { get; set; }
    }
}
