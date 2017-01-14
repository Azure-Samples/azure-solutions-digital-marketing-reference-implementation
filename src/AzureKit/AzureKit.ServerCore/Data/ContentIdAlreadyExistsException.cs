using System;

namespace AzureKit.Data
{
    public class ContentIdAlreadyExistsException : Exception
    {
        public ContentIdAlreadyExistsException(string id)
            : base($"Cannot create new content with id {id} because an item with that id already exists")
        {
            Id = id;
        }

        public string Id { get; }
    }
}
