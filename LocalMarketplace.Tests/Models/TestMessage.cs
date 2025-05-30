using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LocalMarketplace.Tests.Models
{
    public class TestMessage : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public int? ItemId { get; set; }

        private string? _content;
        public string? Content
        {
            get => _content;
            set => SetProperty(ref _content, value);
        }

        private DateTime _sentDate;
        public DateTime SentDate
        {
            get => _sentDate;
            set => SetProperty(ref _sentDate, value);
        }

        private bool _isRead;
        public bool IsRead
        {
            get => _isRead;
            set => SetProperty(ref _isRead, value);
        }

        private string? _attachmentUrl;
        public string? AttachmentUrl
        {
            get => _attachmentUrl;
            set => SetProperty(ref _attachmentUrl, value);
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
