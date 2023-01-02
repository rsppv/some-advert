using System;
using SomeAdvert.Contracts.Advert;

namespace SomeAdvert.Api.Advert.Services
{
    public static class ModelConverters
    {
        public static AdvertDbModel ToDbModel(this AdvertModel model)
        {
            return new AdvertDbModel
            {
                Id = Guid.NewGuid().ToString("N"),
                Description = model.Description,
                Title = model.Title,
                Price = model.Price,
                CreatedAt = DateTime.UtcNow,
                Status = AdvertStatus.Pending,
            };
        }
    }
}