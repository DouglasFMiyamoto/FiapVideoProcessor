namespace FiapVideoProcessor.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; } = default!;

        public string Password { get; set; } = default!;

        // Navegação opcional, se quiser ver os vídeos do usuário
        public ICollection<Video>? Videos { get; set; }
    }
}
