namespace BlogApi.Entities
{
    public class Tag
    {
        public int Id { get; set; } //tag ın id si

        public string Name { get; set; } = string.Empty;    //tag ın adı

        public List<Post> Posts { get; set; } = new();  //tag ın bağlı olduğu postları tutacak.
    }
}