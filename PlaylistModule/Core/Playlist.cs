namespace PlaylistModule
{
    public class PlaylistSong
    {
        public int Id { get; set; }
        public Song SongData { get; set; }
    }

    public class Playlist
    {
        private readonly LinkedList<PlaylistSong> _songs = new LinkedList<PlaylistSong>();
        private LinkedListNode<PlaylistSong>? _currentSong;
        private CancellationTokenSource? _tokenSource;
        private bool _isPlaying = false;
        private int _currentPosition = 0;
        public int CountSongs => _songs.Count; 
        public bool IsPlaying => _isPlaying;

        public Playlist(List<PlaylistSong> songs)
        {
            _songs = new LinkedList<PlaylistSong>(songs);
            _currentSong = _songs.First;
        }

        public void Play()
        {
            if (_songs.Count == 0)
                return;

            if (_currentSong == null)
                _currentSong = _songs.First;

            if (_isPlaying)
                return;

            _tokenSource = new CancellationTokenSource();
            _isPlaying = true;

            Task.Run(async () =>
            {
                while (_currentSong != null && !_tokenSource.Token.IsCancellationRequested)
                {
                    var song = _currentSong.Value;

                    //await Task.Delay(song.Duration, _tokenSource.Token);
                    while (_isPlaying && _currentPosition < song.SongData.Duration)
                    {
                        await Task.Delay(1000);
                        _currentPosition++;
                    }

                    _currentPosition = 0;

                    if (_currentSong.Next != null)
                    {
                        _currentSong = _currentSong.Next;
                    } else
                    {
                        _currentSong = null;
                        _isPlaying = false;
                    }
                }
            });
        }

        public void Pause()
        {
            if (!_isPlaying)
                return;
            _tokenSource?.Cancel();
            _isPlaying = false;
        }

        public void Stop()
        {
            _tokenSource?.Cancel();
            _currentSong = null;
        }

        public void AddSong(PlaylistSong song)
        {
            lock (_songs)
            {
                _songs.AddLast(song);
            }
        }

        public void UpdateSong(int id, Song newSongData)
        {
            lock(_songs)
            {
                _songs.First(item => item.Id == id).SongData = newSongData;
            }
        }

        public void DeleteSong(int id)
        {
            lock (_songs)
            {
                var song = _songs.First(item => item.Id == id);
                _songs.Remove(song);
            }
        }

        public void Next()
        {
            if (_currentSong == null)
                return;

            if (_currentSong.Next == null)
                _currentSong = _songs.First;
            else
                _currentSong = _currentSong.Next;

            Pause();
            _currentPosition = 0;
            if (_isPlaying)
                Play();
        }

        public void Prev()
        {
            if (_currentSong == null)
                return;

            if (_currentSong.Previous == null)
                _currentSong = _songs.Last;
            else
                _currentSong = _currentSong.Previous;
            Pause();
            _currentPosition = 0;
            if (_isPlaying)
                Play();
        }

        public PlaylistSong? CurrentSong => _currentSong?.Value;

        public IEnumerable<PlaylistSong> Songs
        {
            get
            {
                lock (_songs)
                {
                    return _songs.ToList();
                }
            }
        }
    }

}
