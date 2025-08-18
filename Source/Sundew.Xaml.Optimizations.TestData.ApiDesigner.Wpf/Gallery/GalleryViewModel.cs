using FlickrNet;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using System.Reflection;

namespace Sundew.Xaml.Optimizations.TestData.Gallery;

public class GalleryViewModel : INotifyPropertyChanged
{
    private readonly Flickr flickr;
    private ObservableCollection<PhotoViewModel> photos;
    private bool isUpdating = false;

    public GalleryViewModel()
    {
        this.flickr = new Flickr("3a68f22971d8d66b521b362c312c175c");
        this.flickr.InstanceCacheDisabled = true;
    }

    public ObservableCollection<PhotoViewModel> Photos
    {
        get
        {
            if (!this.isUpdating)
            {
                this.isUpdating = true;
                Task.Run(() =>
                {
                    this.photos = new ObservableCollection<PhotoViewModel>();
                    var rawPhotos = JsonConvert.DeserializeObject<List<Photo>>(File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Gallery\photos.json")));
                    foreach (var photo in rawPhotos)
                    {
                        this.photos.Add(new PhotoViewModel(photo));
                    }
                    /*
                    var d = new List<FlickrNet.Photo>();
                    for (var i = 1; i < 4; i++)
                    {
                        var photoSearchOptions = new PhotoSearchOptions
                        {
                            Text = "Landscape",
                            PerPage = 200,
                            Page = i,
                            ColorCodes = new Collection<string>() { "6" },
                            MinTakenDate = new System.DateTime(2000, 1, 1),
                            Extras = PhotoSearchExtras.All | PhotoSearchExtras.AllUrls,
                        };

                        photoSearchOptions.Licenses.Add(LicenseType.AttributionCC);
                        var flickrPhotos = this.flickr.PhotosSearch(photoSearchOptions);
                        var photos2 = flickrPhotos.Where(x => !string.IsNullOrEmpty(x.Title)).ToList();
                        d.AddRange(photos2);


                    }

                    System.IO.File.WriteAllText(@"c:\temp\photos.json", JsonConvert.SerializeObject(d.Take(300).ToList()));*/
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Photos)));
                });
            }

            return this.photos;
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
}