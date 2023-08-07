namespace Volleyball_Teams.Views.Templates
{
    /// <summary>
    /// This is a view to display a pair of key value with an icon.
    /// +------+--------------+
    /// |      | Name         |
    /// | Icon | -------------+
    /// |      | Description  |
    /// +------+--------------+
    /// This view is a 2x2 grid. The first column is used to display an icon.
    /// The second column is used to display a key value pair.
    /// </summary>
    public partial class Stars : ContentView
    {
        public static readonly BindableProperty NumStarsProperty = BindableProperty.Create(nameof(NumStars), typeof(string), typeof(Stars), "1", propertyChanged: (bindable, oldValue, newValue) =>
        {
            var control = (Stars)bindable;
            control.NumStars = newValue as string;
            int stars = int.Parse(control.NumStars);
            control.Star2.IsVisible = false;
            control.Star3.IsVisible = false;
            control.Star4.IsVisible = false;
            control.Star5.IsVisible = false;
            if (stars > 1)
            {
                control.Star2.IsVisible = true;
            }
            if (stars > 2)
            {
                control.Star3.IsVisible = true;
            }
            if (stars > 3)
            {
                control.Star4.IsVisible = true;
            }
            if (stars > 4)
            {
                control.Star5.IsVisible = true;
            }
        });


        public string NumStars
        {
            get => GetValue(NumStarsProperty) as string;
            set => SetValue(NumStarsProperty, value);
        }

        public Stars()
        {
            InitializeComponent();
        }
    }
}