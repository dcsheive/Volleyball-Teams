namespace Volleyball_Teams.Views.Templates
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoadingControl : ContentView
    {
        public static readonly BindableProperty LoadTextProperty = BindableProperty.Create(nameof(LoadText), typeof(string), typeof(LoadingControl), propertyChanged: (bindable, oldValue, newValue) =>
        {
            var control = (LoadingControl)bindable;
            control.LoadText = newValue as string;
            control.myLabel.Text = control.LoadText;
        });
        public string LoadText
        {
            get => GetValue(LoadTextProperty) as string;
            set => SetValue(LoadTextProperty, value);
        }

        public static readonly BindableProperty IsLoadingProperty = BindableProperty.Create(nameof(IsLoading), typeof(string), typeof(LoadingControl), propertyChanged: (bindable, oldValue, newValue) =>
        {
            var control = (LoadingControl)bindable;
            control.IsLoading = newValue as string;
            control.myLoadingControl.IsVisible = bool.Parse(control.IsLoading);
        });
        public string IsLoading
        {
            get => GetValue(IsLoadingProperty) as string;
            set => SetValue(IsLoadingProperty, value);
        }

        public static readonly BindableProperty ShowLoaderProperty = BindableProperty.Create(nameof(ShowLoader), typeof(string), typeof(LoadingControl), "false", propertyChanged: (bindable, oldValue, newValue) =>
        {
            var control = (LoadingControl)bindable;
            control.ShowLoader = newValue as string;
            control.myLoader.IsVisible = bool.Parse(control.ShowLoader);
        });
        public string ShowLoader
        {
            get => GetValue(ShowLoaderProperty) as string;
            set => SetValue(ShowLoaderProperty, value);
        }

        public LoadingControl()
        {
            InitializeComponent();
        }
    }
}