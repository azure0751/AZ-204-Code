namespace AzureStorageAccountDemo.Models
{
    public class BlobListViewModel
    {
        public List<string> Containers { get; set; }
        public Dictionary<string, List<string>> Blobs { get; set; }
    }
}