using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace PlaylistModule.Services
{
    public class PlaylistApiService : PlaylistService.PlaylistServiceBase
    {
        private readonly ApplicationContext db;
        private readonly Playlist playlist;

        public PlaylistApiService(ApplicationContext db)
        {
            this.db = db;
            db.Database.EnsureCreated();
            playlist = new Playlist(db.Songs.ToList());
        }

        public override async Task<SongReply> Play(Empty request, ServerCallContext context)
        {
            if (playlist.CountSongs == 0)
                throw new RpcException(new Status(StatusCode.Cancelled, "Playlist is empty!"));

            playlist.Play();
            var song = playlist.CurrentSong;

            if (song == null)
                throw new RpcException(new Status(StatusCode.Internal, "Song is null!"));

            return await Task.FromResult<SongReply>(new SongReply() { Id = song.Id, Title = song.SongData.Title, Duration = song.SongData.Duration });
        }

        public override async Task<SongReply> Pause(Empty request, ServerCallContext context)
        {
            if (playlist.CountSongs == 0)
                throw new RpcException(new Status(StatusCode.Cancelled, "Playlist is empty!"));

            playlist.Pause();
            var song = playlist.CurrentSong;

            if (song == null)
                throw new RpcException(new Status(StatusCode.Internal, "Song is null!"));
            return await Task.FromResult<SongReply>(new SongReply() { Id = song.Id, Title = song.SongData.Title, Duration = song.SongData.Duration });
        }

        public override async Task<SongReply> AddSong(AddSongRequest request, ServerCallContext context)
        {
            if (await db.Songs.FindAsync(request.Title) == null)
                throw new RpcException(new Status(StatusCode.AlreadyExists, "The song has already been added!"));

            var song = new PlaylistSong { SongData = new Song(request.Title, request.Duration)};
            await db.Songs.AddAsync(song);
            await db.SaveChangesAsync();
            playlist.AddSong(song);

            var reply = new SongReply() { Id = song.Id, Title = song.SongData.Title, Duration = song.SongData.Duration };
            return await Task.FromResult(reply);
        }

        public override async Task<SongReply> UpdateSong(UpdateSongRequest request, ServerCallContext context)
        {
            var song = await db.Songs.FindAsync(request.Id);

            if (song == null)
                throw new RpcException(new Status(StatusCode.NotFound, "Song not found"));

            song.SongData.Title = request.Title;
            song.SongData.Duration = request.Duration;
            await db.SaveChangesAsync();
            playlist.UpdateSong(song.Id, song.SongData);

            var reply = new SongReply() { Id = song.Id, Title = song.SongData.Title, Duration = song.SongData.Duration };
            return await Task.FromResult(reply);
        }

        public override async Task<SongReply> GetCurrentSong(GetSongRequest request, ServerCallContext context)
        {
            var song = await db.Songs.FindAsync(request.SongId);
            if (song == null)
                throw new RpcException(new Status(StatusCode.NotFound, "Song not found"));

            var reply = new SongReply() { Id = song.Id, Title = song.SongData.Title, Duration = song.SongData.Duration };
            return await Task.FromResult(reply);
        }

        public override async Task<SongList> GetAllSongs(Empty request, ServerCallContext context)
        {
            if (playlist.CountSongs == 0)
                throw new RpcException(new Status(StatusCode.Cancelled, "Playlist is empty!"));

            var listReply = new SongList();
            var songList = playlist.Songs.Select(item => new SongReply()
                { Id = item.Id, Title = item.SongData.Title, Duration = item.SongData.Duration }).ToList();

            listReply.Songs.AddRange(songList);
            return await Task.FromResult(listReply);
        }

        public override async Task<SongReply> DeleteSong(DeleteSongRequest request, ServerCallContext context)
        {
            var song = await db.Songs.FindAsync(request.SongId);
            if (song == null)
                throw new RpcException(new Status(StatusCode.NotFound, "Song not found"));

            if (song.Id == playlist.CurrentSong.Id)
                throw new RpcException(new Status(StatusCode.Cancelled, "The song is playing now"));

            playlist.DeleteSong(song.Id);
            db.Songs.Remove(song);

            var reply = new SongReply() { Id = song.Id, Title = song.SongData.Title, Duration = song.SongData.Duration };
            return await Task.FromResult(reply);
        }

        public override async Task<SongReply> Next(Empty request, ServerCallContext context)
        {
            if (playlist.CountSongs == 0)
                throw new RpcException(new Status(StatusCode.Cancelled, "Playlist is empty!"));

            playlist.Next();

            var song = playlist.CurrentSong;
            if (song == null)
                throw new RpcException(new Status(StatusCode.Internal, "Current Song is null!"));

            var reply = new SongReply() { Id = song.Id, Title = song.SongData.Title, Duration = song.SongData.Duration };

            return await Task.FromResult(reply);
        }

        public override async Task<SongReply> Prev(Empty request, ServerCallContext context)
        {
            if (playlist.CountSongs == 0)
                throw new RpcException(new Status(StatusCode.Cancelled, "Playlist is empty!"));

            playlist.Prev();

            var song = playlist.CurrentSong;
            if (song == null)
                throw new RpcException(new Status(StatusCode.Internal, "Current Song is null!"));

            var reply = new SongReply() { Id = song.Id, Title = song.SongData.Title, Duration = song.SongData.Duration };
            return await Task.FromResult(reply);
        }
    }
}
