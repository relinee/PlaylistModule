namespace PlaylistModule
{
    public class Song
    {
        public string Title { get; set; }
        public int Duration { get; set; }

        public Song(string title, int durationInSeconds)
        {
            Title = title;
            Duration = durationInSeconds;
        }
    }

}
