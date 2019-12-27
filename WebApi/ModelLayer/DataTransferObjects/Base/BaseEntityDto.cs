namespace ModelLayer.DataTransferObjects.Base
{
    public abstract class BaseEntityDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}