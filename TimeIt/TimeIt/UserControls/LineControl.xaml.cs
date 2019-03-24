using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TimeIt.UserControls
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LineControl : TemplatedView
	{
        public static readonly BindableProperty LineColorProperty = BindableProperty.Create(nameof(LineColor), typeof(Color), typeof(LineControl), Color.Red);

        public static readonly BindableProperty LineHeightProperty = BindableProperty.Create(nameof(LineHeight), typeof(double), typeof(LineControl), 1.00);

        public Color LineColor
        {
            get { return (Color)GetValue(LineColorProperty); }
            set { SetValue(LineColorProperty, value); }
        }

        public double LineHeight
        {
            get { return (double)GetValue(LineHeightProperty); }
            set { SetValue(LineHeightProperty, value); }
        }

        public LineControl ()
		{
			InitializeComponent ();
		}
	}
}