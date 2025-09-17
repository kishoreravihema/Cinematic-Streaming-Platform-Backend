namespace Netflix_BackendAPI.DTO
{
    public class PlayInfo
    {
        // Tells the front-end what to do: "URL" (use directly) or "STREAM" (fetch from our API)
        public string? PlaybackType { get; set; }

        // The actual URL to use for playback
        public string? Url { get; set; }
        public string? ThumbnailUrl { get; internal set; }
        //  public string? Data { get;  set; }
    }
}
