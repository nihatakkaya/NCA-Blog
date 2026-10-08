namespace BlogApi.Entities
{
    public class Category
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;    //kategorinin adını tutuyor.

        public List<Post> Posts { get; set; } = new();  //bu kategoriye bağlı birden fazla blog olabilir.
    }
}