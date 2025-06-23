namespace WebJet.Entertainment.Services
{
    public sealed record MovieSource
    {
        public string Name { get; init; }

        public static readonly MovieSource Filmworld = new MovieSource { Name = nameof(Filmworld) };
        public static readonly MovieSource Cinemaworld = new MovieSource { Name = nameof(Cinemaworld) };
        
        public override string ToString() => Name;
    }
}