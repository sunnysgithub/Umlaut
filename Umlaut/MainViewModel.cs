using System.Windows.Input;

namespace Umlaut
{
    public class MainViewModel
    {
        private ICommand _cancelCommand;
        public ICommand CancelCommand
        {
            get
            {
                return _cancelCommand ?? (_cancelCommand = new CancelCommand(() =>
                {

                }));
            }
        }

    }
}