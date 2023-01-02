using System;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using SomeAdvert.Contracts.Advert;

namespace SomeAdvert.Api.Advert.Services
{
    [DynamoDBTable("Adverts")]
    public class AdvertDbModel
    {
        [DynamoDBHashKey]
        public string Id { get; set; }
        
        [DynamoDBProperty]
        public string Title { get; set; }
        
        [DynamoDBProperty]
        public string Description { get; set; }
        
        [DynamoDBProperty]
        public decimal Price { get; set; }
        
        [DynamoDBProperty]
        public DateTime CreatedAt { get; set; }
        
        [DynamoDBProperty]
        public AdvertStatus Status { get; set; }
    }
    
    public interface IAdvertStorage
    {
        Task Add(AdvertModel advert);
        Task Confirm(ConfirmationAdvertModel confirmation);
    }

    public class DynamoDbAdvertStorage : IAdvertStorage
    {
        public Task Add(AdvertModel advert)
        {
            throw new NotImplementedException();
        }

        public Task Confirm(ConfirmationAdvertModel confirmation)
        {
            throw new NotImplementedException();
        }
    }
}