namespace FlawBOT.Models.Music
{
    public struct YouTubeData
    {
        public string Title { get; }
        public string Author { get; }
        public string Id { get; }

        public YouTubeData(string title, string author, string id)
        {
            Title = title;
            Author = author;
            Id = id;
        }
    }
}