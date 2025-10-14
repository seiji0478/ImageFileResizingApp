using System.ComponentModel;
using ImageFileResizingApp.Models;


namespace ImageFileResizingApp.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {

        private MainWindowModel m_Model;

        public MainWindowViewModel()
        {
            m_Model = new MainWindowModel();
        }

        /// <summary>
        /// Notification Event
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
