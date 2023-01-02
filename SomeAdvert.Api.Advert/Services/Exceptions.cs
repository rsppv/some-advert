using System;
using System.Runtime.Serialization;

namespace SomeAdvert.Api.Advert.Services
{
    [Serializable]
    public class AdvertNotFoundException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public AdvertNotFoundException()
        {
        }

        public AdvertNotFoundException(string message) : base(message)
        {
        }

        public AdvertNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }

        protected AdvertNotFoundException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}