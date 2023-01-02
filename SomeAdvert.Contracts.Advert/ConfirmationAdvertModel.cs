namespace SomeAdvert.Contracts.Advert
{
    public enum AdvertStatus
    {
        Pending = 1,
        Active
    }

    public class ConfirmationAdvertModel
    {
        public string AdvertId { get; set; } = string.Empty;
        public AdvertStatus Status { get; set; }
    }
}