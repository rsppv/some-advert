using System;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
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
        Task<string> Add(AdvertModel advert);
        Task Confirm(ConfirmationAdvertModel confirmation);
    }

    public class DynamoDbAdvertStorage : IAdvertStorage
    {
        public async Task<string> Add(AdvertModel advert)
        {
            if (advert == null) throw new ArgumentNullException(nameof(advert));
            
            var dbAdvert = advert.ToDbModel();
            
            using var dbClient = new AmazonDynamoDBClient();
            using var dbContext = new DynamoDBContext(dbClient);
            await dbContext.SaveAsync(dbAdvert);
            
            return dbAdvert.Id;
        }

        public Task Confirm(ConfirmationAdvertModel confirmation)
        {
            throw new NotImplementedException();
        }
    }
}