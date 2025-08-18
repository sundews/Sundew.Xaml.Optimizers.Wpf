using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Sundew.Xaml.Optimizations.TestData.Gallery;

public class PhotoViewModel : INotifyPropertyChanged
{
    private readonly Photo photo;
    private BitmapImage bitmapSource;

    public PhotoViewModel(Photo photo)
    {
        this.photo = photo;
    }

    public string Title => this.photo.Title;

    public string OriginalResolution => $"{this.photo.OriginalWidth}x{this.photo.OriginalHeight}";

    public string LargeUrl => this.photo.LargeUrl;

    public DateTime DateTaken => this.photo.DateTaken;

    public string License => this.photo.License.ToString();

    public string Description => this.photo.OwnerName;

    public string Source => this.photo.SmallUrl;

    public BitmapSource BitmapSource
    {
        get
        {
            if (this.bitmapSource == null)
            {
                Task.Run(() =>
                {
                    var webRequest = WebRequest.CreateDefault(new Uri(this.photo.SmallUrl));
                    webRequest.ContentType = "image/jpeg";
                    WebResponse webResponse = webRequest.GetResponse();

                    using (var response = webRequest.GetResponse())
                    using (var responseStream = response.GetResponseStream())
                    {
                        this.bitmapSource = new BitmapImage
                        {
                            CreateOptions = BitmapCreateOptions.None,
                            CacheOption = BitmapCacheOption.OnLoad
                        };

                        this.bitmapSource.BeginInit();
                        var buffer = new byte[response.ContentLength];
                        responseStream.Read(buffer, 0, buffer.Length);
                        this.bitmapSource.StreamSource = new MemoryStream(buffer);
                        //this.bitmapSource.DecodePixelWidth = 80;
                        this.bitmapSource.EndInit();

                        this.bitmapSource.Freeze();
                    }
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BitmapSource)));
                });
            }

            return this.bitmapSource;
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
}