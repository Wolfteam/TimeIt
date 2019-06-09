using System;
using TimeIt.Delegates;
using TimeIt.Interfaces;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TimeIt.Pages.Dialogs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConfirmationDialog : ContentView, IConfirmationDialog<bool>
    {
        public OnConfirmDialogButtonClick<bool> OnOptionSelected { get; set; }

        public ConfirmationDialog(string title, string message, string okButtontext, string cancelButtonText)
        {
            InitializeComponent();
            titleLabel.Text = title;
            messageLabel.Text = message;
            okButton.Text = okButtontext;
            cancelButton.Text = cancelButtonText;
        }

        private void OkButton_Clicked(object sender, EventArgs e)
        {
            OnOptionSelected?.Invoke(true, false);
        }

        private void CancelButton_Clicked(object sender, EventArgs e)
        {
            OnOptionSelected?.Invoke(false, false);
        }
    }
}