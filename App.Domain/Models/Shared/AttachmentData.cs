namespace App.Domain.Models.Shared
{
    public class AttachmentData
    {
        public string Name { get; set; }
        public byte[] Files { get; set; }
    }

    public class AttachmentInfo
    {
        public int ParentId { get; set; }
        public int ChildId { get; set; }
    }
}
