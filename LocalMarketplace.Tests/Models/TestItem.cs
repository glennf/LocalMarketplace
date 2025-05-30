using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LocalMarketplace.Tests.Models
{
    public class TestItem : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public int SellerId { get; set; }

        private string? _title;
        public string? Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private string? _description;
        public string? Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        private decimal _price;
        public decimal Price
        {
            get => _price;
            set => SetProperty(ref _price, value);
        }

        private string? _category;
        public string? Category
        {
            get => _category;
            set => SetProperty(ref _category, value);
        }

        private string? _condition;
        public string? Condition
        {
            get => _condition;
            set => SetProperty(ref _condition, value);
        }

        private List<string>? _imageUrls;
        public List<string>? ImageUrls
        {
            get => _imageUrls;
            set => SetProperty(ref _imageUrls, value);
        }

        private double _latitude;
        public double Latitude
        {
            get => _latitude;
            set => SetProperty(ref _latitude, value);
        }

        private double _longitude;
        public double Longitude
        {
            get => _longitude;
            set => SetProperty(ref _longitude, value);
        }

        private string? _location;
        public string? Location
        {
            get => _location;
            set => SetProperty(ref _location, value);
        }

        private DateTime _listingDate;
        public DateTime ListingDate
        {
            get => _listingDate;
            set => SetProperty(ref _listingDate, value);
        }

        private bool _isActive;
        public bool IsActive
        {
            get => _isActive;
            set => SetProperty(ref _isActive, value);
        }

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler? PropertyChanged;

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(storage, value))
                return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
